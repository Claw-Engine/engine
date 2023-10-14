using System;
using System.Collections.Generic;
using Claw.Extensions;

namespace Claw.Audio
{
    /// <summary>
    /// Controla os áudios.
    /// </summary>
    public sealed class AudioManager : IDisposable
    {
        /// <summary>
        /// Máximo de efeitos sonoros simultâneos.
        /// </summary>
        public static int MaxConcurrent = 15;
        /// <summary>
        /// Velocidade de transição entre músicas.
        /// </summary>
        public float FadeSpeed = .1f;
        /// <summary>
        /// Volume geral (entre 0 e 1).
        /// </summary>
        public float MasterVolume = 1;
        /// <summary>
        /// Volume geral das músicas (entre 0 e 1).
        /// </summary>
        public float MusicVolume = 1;
        /// <summary>
        /// Evento executado quando um efeito sonoro termina, sem loop.
        /// </summary>
        public event Action<SoundEffectInstance> OnSoundEffectEnd;

        private float fadeMultipliyer = 1;
        internal ushort musicOffset = 0;
        private Music music, nextMusic;
        private float[] groupVolumes;
        private List<SoundEffectInstance> soundEffects;

        private uint device = 0;
        private SDL.SDL_AudioSpec want;
        internal const ushort AudioFormat = SDL.AUDIO_F32, BufferSize = 4096;

        internal unsafe AudioManager()
        {
            want.freq = 48000;
            want.channels = 2;
            want.samples = BufferSize;
            want.format = AudioFormat;
            want.callback = AudioCallback;

            device = SDL.SDL_OpenAudioDevice(IntPtr.Zero, 0, ref want, IntPtr.Zero, (int)SDL.SDL_AUDIO_ALLOW_ANY_CHANGE);

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
            soundEffects?.Clear();

            groupVolumes = null;
            soundEffects = null;
            music = null;

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
            if (soundEffects.Count < MaxConcurrent)
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
        }

        private unsafe void AudioCallback(void* userData, byte* stream, int length)
        {
            if (music != nextMusic) fadeMultipliyer = Math.Max(fadeMultipliyer - Math.Abs(FadeSpeed), 0);

            length /= 4;
            float* buffer = (float*)stream;
            bool finished;
            float sample;
            SoundEffectInstance current;

            for (int i = 0; i < length; i += 2)
            {
                buffer[i] = 0;
                buffer[i + 1] = 0;

                if (music != null)
                {
                    sample = MasterVolume * (MusicVolume * fadeMultipliyer) * music.GetSample();

                    if (music.Channels == Channels.Stereo)
                    {
                        buffer[i] = sample * .5f;
                        buffer[i + 1] = sample * .5f;
                    }
                    else
                    {
                        buffer[i] = sample;
                        buffer[i + 1] = sample;
                    }
                }

                if (soundEffects.Count != 0)
                {
                    for (int j = 0; j < soundEffects.Count; j++)
                    {
                        current = soundEffects[j];
                        sample = MasterVolume * groupVolumes[(int)current.Group] * current.GetSample(out finished);
                        sample = Mathf.Clamp(buffer[i] + sample, -1, 1);

                        if (current.audio.Channels == Channels.Stereo)
                        {
                            sample *= .5f;
                            buffer[i] = sample;
                            buffer[i + 1] = sample;
                        }
                        else
                        {
                            buffer[i] = sample;
                            buffer[i + 1] = sample;
                        }

                        if (finished &&  !current.IsLooped)
                        {
                            OnSoundEffectEnd?.Invoke(current);
                            RemoveAt(soundEffects, j);
                        }
                    }
                }
            }

            if (fadeMultipliyer == 0)
            {
                music = nextMusic;
                fadeMultipliyer = 1;
                musicOffset = 0;
            }
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