using System;
using System.Collections.Generic;

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
            
            float* buffer = (float*)stream;
            float sample;

            for (int i = 0; i < length / 4; i++)
            {
                buffer[i] = 0;

                if (music != null) buffer[i] = MasterVolume * (MusicVolume * fadeMultipliyer) * music.GetSample();

                if (soundEffects.Count != 0)
                {
                    for (int j = 0; j < soundEffects.Count; j++)
                    {
                        sample = MasterVolume * groupVolumes[(int)soundEffects[j].Group] * soundEffects[j].GetSample(j, soundEffects);
                        buffer[i] = Mathf.Clamp(buffer[i] + sample, -1, 1);
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
    }
}