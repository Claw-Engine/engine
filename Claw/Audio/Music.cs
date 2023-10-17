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
            get => volume;
            set => volume = Mathf.Clamp(value, 0, 1);
        }
        public long Length => file.BaseStream.Length / 4;
        private const int AudioStart = 9; // BYTE, INT64
        private float volume = 1;
        private BinaryReader file;

        internal Music(Channels channels, BinaryReader file)
        {
            Channels = channels;
            this.file = file;
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
        internal float GetSample()
        {
            float sample = file.ReadSingle();

            if (file.BaseStream.Position >= file.BaseStream.Length) file.BaseStream.Position = AudioStart;

            return sample;
        }
    }
}