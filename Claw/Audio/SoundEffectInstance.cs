using System;
using System.Collections.Generic;

namespace Claw.Audio
{
    /// <summary>
    /// Representa uma instância de <see cref="SoundEffect"/>.
    /// </summary>
    public sealed class SoundEffectInstance
    {
        public bool IsLooped = false;
		/// <summary>
		/// Volume do áudio no lado esquerdo (entre 0 e 1).
		/// </summary>
		public float LeftVolume
		{
			get => _leftVolume;
			set => _leftVolume = Mathf.Clamp(value, 0, 1);
		}
		/// <summary>
		/// Volume do áudio no lado direito (entre 0 e 1).
		/// </summary>
		public float RightVolume
		{
			get => _rightVolume;
			set => _rightVolume = Mathf.Clamp(value, 0, 1);
		}
		/// <summary>
		/// Duração do áudio, em segundos.
		/// </summary>
		public float Duration => audio.Duration;
        /// <summary>
        /// Momento em que o áudio está, em segundos.
        /// </summary>
        public float Current => AudioManager.CalculateDuration(audio.Length, audio.Channels);
        public readonly SoundEffectGroup Group;
        private float _leftVolume = 1, _rightVolume = 1;
        internal long offset = 0;
        internal SoundEffect audio;

        public SoundEffectInstance(SoundEffect audio, SoundEffectGroup group)
        {
            this.audio = audio;
            Group = group;
        }

        /// <summary>
        /// Altera o <see cref="LeftVolume"/> e <see cref="RightVolume"/> para um mesmo volume.
        /// </summary>
        public SoundEffectInstance SetVolume(float volume)
        {
            LeftVolume = volume;
            RightVolume = volume;

            return this;
        }

        /// <summary>
        /// Retorna um sample e finaliza o efeito sonoro, se preciso.
        /// </summary>
        /// <param name="index">Index do efeito sonoro.</param>
        /// <param name="list">Lista em que o efeito sonoro está.</param>
        internal float GetSample(out bool finished)
        {
            finished = false;

            if (offset >= audio.Length)
            {
                offset = 0;
                finished = true;
            }

            float value = audio.GetSample(offset);
            offset++;

            return value;
        }
    }
}