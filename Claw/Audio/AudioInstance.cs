using System;

namespace Claw.Audio
{
    /// <summary>
    /// Representa uma instância de <see cref="Audio"/>.
    /// </summary>
    public sealed class AudioInstance
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
        /// <summary>
        /// Evento executado quando o áudio termina, sem loop.
        /// </summary>
        public event Action OnEnd;
        private float volume = 1;
        private Audio audio;
        internal ushort offset = 0;

        public AudioInstance(Audio audio) => this.audio = audio;
    }
}