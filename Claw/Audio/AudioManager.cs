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
        /// Velocidade de transição entre músicas.
        /// </summary>
        public float FadeSpeed = .1f;
        /// <summary>
        /// Volume geral (entre 0 e 1).
        /// </summary>
        public float MasterVolume = 1;

        /// <summary>
        /// Máximo de sons por pilha.
        /// </summary>
        public static int MaxAudioStack = 15;
        private uint device = 0;
        private SDL.SDL_AudioSpec want;
        private float[] groupVolumes;
        private List<AudioInstance>[] groupList;
        internal const ushort AudioFormat = SDL.AUDIO_S32LSB, BufferSize = 1024;

        internal unsafe AudioManager()
        {
            want.freq = 48000;
            want.channels = 2;
            want.samples = BufferSize;
            want.format = AudioFormat;
            want.callback = AudioCallback;

            int groupSize = Enum.GetValues(typeof(AudioGroup)).Length;
            groupVolumes = new float[groupSize];
            groupList = new List<AudioInstance>[groupSize];

            for (int i = 0; i < groupSize; i++)
            {
                groupVolumes[i] = 1;
                groupList[i] = new List<AudioInstance>();
            }

            device = SDL.SDL_OpenAudioDevice(IntPtr.Zero, 0, ref want, IntPtr.Zero, (int)SDL.SDL_AUDIO_ALLOW_ANY_CHANGE);

            if (device == 0) throw new Exception("O sistema não conseguiu iniciar o AudioManager!");

            SDL.SDL_PauseAudioDevice(device, 0);
        }
        ~AudioManager() => Dispose();

        public void Dispose()
        {
            groupVolumes = null;
            groupList = null;

            if (device != 0)
            {
                SDL.SDL_CloseAudioDevice(device);

                device = 0;
            }
        }
        
        /// <summary>
        /// Retorna o volume geral de um grupo.
        /// </summary>
        public float GetVolume(AudioGroup group) => groupVolumes[(int)group];
        /// <summary>
        /// Altera o volume geral de um grupo.
        /// </summary>
        /// <param name="value">Entre 0 e 1.</param>
        public void SetVolume(float value, AudioGroup group) => groupVolumes[(int)group] = Mathf.Clamp(value, 0, 1);

        /// <summary>
        /// Inicia/reinicia um áudio.
        /// </summary>
        public void Play(AudioInstance audio, AudioGroup group)
        {
            int index = (int)group;

            if (groupList[index].Count < MaxAudioStack)
            {
                audio.offset = 0;

                if (!groupList[index].Contains(audio)) groupList[index].Add(audio);
            }
        }
        /// <summary>
        /// Pausa um áudio.
        /// </summary>
        public void Stop(AudioInstance audio, AudioGroup group) => groupList[(int)group].Remove(audio);
        /// <summary>
        /// Pausa um áudio.
        /// </summary>
        public void Stop(AudioInstance audio)
        {
            for (int i = 0; i < groupList.Length; i++)
            {
                if (groupList[i].Contains(audio)) groupList[i].Remove(audio);
            }
        }

        private unsafe void AudioCallback(void* userData, byte* stream, int length)
        {
            
        }
    }
}