using System;
using System.IO;

namespace Claw.Audio
{
    /// <summary>
    /// Representa uma música no jogo.
    /// </summary>
    public class Music : IDisposable
    {
        public readonly Channels Channels;
        /// <summary>
        /// Volume da música (entre 0 e 1).
        /// </summary>
        public float Volume
        {
            get => _volume;
            set => _volume = Mathf.Clamp(value, 0, 1);
        }
        /// <summary>
        /// Duração deste áudio, em segundos.
        /// </summary>
        public readonly float Duration;
        /// <summary>
        /// Momento em que o áudio está, em segundos.
        /// </summary>
        public float Current => AudioManager.CalculateDuration((file.BaseStream.Position - AudioStart) / 4, Channels);
        public readonly long Length;
        private const int AudioStart = 9; // BYTE, INT64
        private float _volume = 1;
        private BinaryReader file;

        internal Music(Channels channels, BinaryReader file)
        {
            Channels = channels;
            this.file = file;
            Length = file.BaseStream.Length / 4;
            Duration = AudioManager.CalculateDuration(Length, Channels);
        }
        ~Music() => Dispose();

        public void Dispose()
        {
            file?.Close();

            file = null;
        }

        /// <summary>
        /// Carrega uma música.
        /// </summary>
        internal static Music LoadMusic(string filePath)
        {
            BinaryReader reader = new BinaryReader(new StreamReader(filePath).BaseStream);
            Channels channels = (Channels)reader.ReadByte();

            return new Music(channels, reader);
        }

        /// <summary>
        /// Reseta a posição da stream.
        /// </summary>
        internal void ResetPosition() => file.BaseStream.Position = AudioStart;
        /// <summary>
        /// Retorna o próximo sample.
        /// </summary>
        internal float GetSample(out bool ended)
        {
            ended = false;

            if (file == null) return 0;

            float sample = file.ReadSingle();

            if (file.BaseStream.Position >= file.BaseStream.Length)
            {
                file.BaseStream.Position = AudioStart;
                ended = true;
            }

            return sample;
        }
    }
}