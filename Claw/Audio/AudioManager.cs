using System;
using System.Collections.Generic;
using Claw.Extensions;

namespace Claw.Audio
{
    /// <summary>
    /// Representa o controle de áudios do jogo.
    /// </summary>
    public sealed class AudioManager : IDisposable
    {
        public const int SampleRate = 48000;
        /// <summary>
        /// Máximo de efeitos sonoros simultâneos.
        /// </summary>
        public static int MaxConcurrent = 15;
        /// <summary>
        /// Velocidade de transição entre músicas.
        /// </summary>
        public static float FadeSpeed = .01f;

        public bool PauseMusic = false;
        /// <summary>
        /// Volume geral (entre 0 e 1).
        /// </summary>
        public float MasterVolume
        {
            get => masterVolume;
            set => masterVolume = Mathf.Clamp(value, 0, 1);
        }
        /// <summary>
        /// Volume geral das músicas (entre 0 e 1).
        /// </summary>
        public float MusicVolume
        {
            get => musicVolume;
            set => musicVolume = Mathf.Clamp(value, 0, 1);
        }
        /// <summary>
        /// Evento executado quando um efeito sonoro termina, sem loop.
        /// </summary>
        public event Action<SoundEffectInstance> OnSoundEffectEnd;

        private float fadeMultipliyer = 1, masterVolume = 1, musicVolume = 1;
        private Music music, nextMusic;
        private float[] groupVolumes;
        private List<SoundEffectInstance> soundEffects;

        private uint device = 0;
        private SDL.SDL_AudioSpec want;
        internal const ushort AudioFormat = SDL.AUDIO_F32, BufferSize = 4096;

        internal unsafe AudioManager()
        {
            want.freq = SampleRate * 2;
            want.channels = 2;
            want.samples = BufferSize;
            want.format = AudioFormat;
            want.callback = AudioCallback;

            device = SDL.SDL_OpenAudioDevice(IntPtr.Zero, 0, ref want, IntPtr.Zero, 0);

            if (device == 0) throw new Exception("O sistema não conseguiu iniciar o AudioManager!");

            SDL.SDL_PauseAudioDevice(device, 0);

            int groupSize = Enum.GetValues(typeof(SoundEffectGroup)).Length;
            groupVolumes = new float[groupSize];
            soundEffects = new List<SoundEffectInstance>();

            for (int i = 0; i < groupSize; i++) groupVolumes[i] = 1;
        }
        ~AudioManager() => Dispose();

        public void Dispose()
        {
            soundEffects.Clear();

            groupVolumes = null;
            nextMusic = null;
            want.callback = null;

            if (device != 0)
            {
                SDL.SDL_CloseAudioDevice(device);

                device = 0;
            }
        }
        
        /// <summary>
        /// Retorna o volume geral de um grupo.
        /// </summary>
        public float GetVolume(SoundEffectGroup group) => groupVolumes[(int)group];
        /// <summary>
        /// Altera o volume geral de um grupo.
        /// </summary>
        /// <param name="value">Entre 0 e 1.</param>
        public void SetVolume(float value, SoundEffectGroup group) => groupVolumes[(int)group] = Mathf.Clamp(value, 0, 1);

        /// <summary>
        /// Inicia/reinicia um sonoro.
        /// </summary>
        public void Play(SoundEffectInstance soundEffect)
        {
            if (soundEffect != null && soundEffects.Count < MaxConcurrent)
            {
                soundEffect.offset = 0;

                if (!soundEffects.Contains(soundEffect)) soundEffects.Add(soundEffect);
            }
        }
        /// <summary>
        /// Pausa um efeito sonoro.
        /// </summary>
        public void Stop(SoundEffectInstance soundEffect) => soundEffects.Remove(soundEffect);

        /// <summary>
        /// Inicia a troca de música.
        /// </summary>
        public void SetMusic(Music music)
        {
            if (this.music == null) this.music = music;
            
            nextMusic = music;

            if (music != null) music.ResetPosition();
        }

        /// <summary>
        /// Callback de manuseio do buffer de áudio.
        /// </summary>
        private unsafe void AudioCallback(void* userData, byte* stream, int length)
        {
            if (!PauseMusic)
            {
                if (music != nextMusic && fadeMultipliyer != 0) fadeMultipliyer = Math.Max(fadeMultipliyer - Math.Abs(FadeSpeed), 0);
                else if (music == nextMusic && fadeMultipliyer != 1) fadeMultipliyer = Math.Min(fadeMultipliyer + Math.Abs(FadeSpeed), 1);
            }

            length /= 4;
            float* buffer = (float*)stream;
            bool finished;
            float sample;
            SoundEffectInstance current;

            for (int i = 0; i < length; i += 2)
            {
                buffer[i] = 0;
                buffer[i + 1] = 0;

                if (!PauseMusic && music != null)
                {
                    sample = music.GetSample();

                    SetSample(buffer, i, sample, (musicVolume * fadeMultipliyer), music.Channels);
                }

                for (int j = soundEffects.Count - 1; j >= 0; j--)
                {
                    current = soundEffects[j];
                    
                    if (current != null)
                    {
                        sample = current.GetSample(out finished);

                        SetSample(buffer, i, sample, groupVolumes[(int)current.Group], current.audio.Channels);

                        if (finished && !current.IsLooped)
                        {
                            OnSoundEffectEnd?.Invoke(current);
                            RemoveAt(soundEffects, j);
                        }
                    }
                }
            }

            if (fadeMultipliyer == 0) music = nextMusic;
        }
        /// <summary>
        /// Seta um sample em um índice e no seguinte.
        /// </summary>
        private unsafe void SetSample(float* buffer, int index, float sample, float volume, Channels channels)
        {
            if (sample == 0) return;

            sample *= masterVolume * volume;

            if (channels == Channels.Stereo) sample *= .5f;

            float sample1 = Mathf.Clamp(buffer[index] + sample, -1, 1),
                sample2 = Mathf.Clamp(buffer[index + 1] + sample, -1, 1);

            buffer[index] = sample1;
            buffer[index + 1] = sample2;
        }
        /// <summary>
        /// Remove um item da lista, sem se preocupar com a ordem.
        /// </summary>
        private static void RemoveAt<T>(List<T> list, int index)
        {
            list.Swap(index, list.Count - 1);
            list.RemoveAt(list.Count - 1);
        }
    }
}