using System;
using System.Collections.Generic;
using Claw.Extensions;

namespace Claw.Audio
{
    /// <summary>
    /// Representa uma instância de <see cref="Audio"/>.
    /// </summary>
    public sealed class SoundEffectInstance
    {
        public bool IsLooped = false;
        /// <summary>
        /// Volume do áudio (entre 0 e 1).
        /// </summary>
        public float Volume
        {
            get => volume;
            set => volume = Mathf.Clamp(value, 0, 1);
        }
        public readonly SoundEffectGroup Group;
        /// <summary>
        /// Evento executado quando o áudio termina, sem loop.
        /// </summary>
        public event Action<SoundEffectInstance> OnEnd;
        private float volume = 1;
        private SoundEffect audio;
        internal ushort offset = 0;

        public SoundEffectInstance(SoundEffect audio, SoundEffectGroup group)
        {
            this.audio = audio;
            Group = group;
        }

        /// <summary>
        /// Retorna um sample e finaliza o efeito sonoro, se preciso.
        /// </summary>
        /// <param name="index">Index do efeito sonoro.</param>
        /// <param name="list">Lista em que o efeito sonoro está.</param>
        internal float GetSample(int index, List<SoundEffectInstance> list)
        {
            bool end = false;

            if (offset >= audio.Length)
            {
                offset = 0;
                end = true;
            }
            
            float value = audio.GetSample(offset);
            offset++;

            if (end && !IsLooped)
            {
                RemoveAt(list, index);
                OnEnd?.Invoke(this);
            }
            
            return value;
        }
        /// <summary>
        /// Remove um item da lista, sem se preocupar com a ordem.
        /// </summary>
        private void RemoveAt<T>(List<T> list, int index)
        {
            list.Swap(index, list.Count - 1);
            list.RemoveAt(list.Count - 1);
        }
    }
}