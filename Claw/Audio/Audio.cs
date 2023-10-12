using System;
using System.IO;

namespace Claw.Audio
{
    /// <summary>
    /// Representa um áudio no jogo.
    /// </summary>
    public class Audio
    {
        public readonly int SampleRate;
        public readonly byte Channels;
        /// <summary>
        /// Volume do áudio (entre 0 a 1).
        /// </summary>
        public float Volume
        {
            get => volume;
            set => volume = Mathf.Clamp(value, 0, 1);
        }
        private float volume = 1;

        internal Audio(int sampleRate, byte channels)
        {
            SampleRate = sampleRate;
            Channels = channels;
        }

        /// <summary>
        /// Recebe o tamanho do som em samples.
        /// </summary>
        public virtual ushort Length() => 0;
    }
    /// <summary>
    /// Representa um efeito sonoro no jogo.
    /// </summary>
    public class SoundEffect : Audio
    {
        private int[] samples;

        public SoundEffect(int sampleRate, byte channels, int[] samples) : base(sampleRate, channels) => this.samples = samples;

        /// <summary>
        /// Carrega um efeito sonoro.
        /// </summary>
        internal static SoundEffect LoadSFX(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            BinaryReader binReader = new BinaryReader(reader.BaseStream);
            int sampleRate = binReader.ReadInt32();
            byte channels = binReader.ReadByte();
            ushort size = binReader.ReadUInt16();
            int[] samples = new int[size];

            for (int i = 0; i < size; i++) samples[i] = binReader.ReadInt32();

            binReader.Close();
            reader.Close();

            return new SoundEffect(sampleRate, channels, samples);
        }

        /// <summary>
        /// Recebe o tamanho do som em samples.
        /// </summary>
        public override ushort Length() => (ushort)samples.Length;
    }
    /// <summary>
    /// Representa uma música no jogo.
    /// </summary>
    public class Music : Audio
    {
        private const int AudioStart = 7; // INT32, BYTE, USHORT
        private ushort size;
        private BinaryReader reader;

        internal Music(int sampleRate, byte channels) : base(sampleRate, channels) { }

        /// <summary>
        /// Carrega uma música.
        /// </summary>
        internal static Music LoadMusic(string filePath)
        {
            BinaryReader reader = new BinaryReader(new StreamReader(filePath).BaseStream);
            int sampleRate = reader.ReadInt32();
            byte channels = reader.ReadByte();
            ushort size = reader.ReadUInt16();

            return new Music(sampleRate, channels) { size = size, reader = reader };
        }

        /// <summary>
        /// Recebe o tamanho do som em samples.
        /// </summary>
        public override ushort Length() => size;
    }
}