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
        public readonly Channels Channels;
        public virtual ushort Length => 0;

        internal Audio(int sampleRate, byte channels)
        {
            SampleRate = sampleRate;
            Channels = channels >= 2 ? Channels.Stereo : Channels.Mono;
        }
    }
    /// <summary>
    /// Representa um efeito sonoro no jogo.
    /// </summary>
    public class SoundEffect : Audio
    {
        public override ushort Length => (ushort)samples.Length;
        private readonly float[] samples;
        
        /// <param name="samples">Valores entre -1 e 1.</param>
        public SoundEffect(int sampleRate, byte channels, float[] samples) : base(sampleRate, channels) => this.samples = samples;

        /// <summary>
        /// Cria um <see cref="SoundEffectInstance"/> deste áudio.
        /// </summary>
        public SoundEffectInstance CreateInstance(SoundEffectGroup group) => new SoundEffectInstance(this, group);

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
            float[] samples = new float[size];

            for (int i = 0; i < size; i++) samples[i] = binReader.ReadSingle();

            binReader.Close();
            reader.Close();

            return new SoundEffect(sampleRate, channels, samples);
        }

        /// <summary>
        /// Retorna um sample específico.
        /// </summary>
        internal float GetSample(ushort position) => samples[position];
    }
    /// <summary>
    /// Representa uma música no jogo.
    /// </summary>
    public class Music : Audio, IDisposable
    {
        /// <summary>
        /// Volume da música (entre 0 e 1).
        /// </summary>
        public float Volume
        {
            get => volume;
            set => volume = Mathf.Clamp(value, 0, 1);
        }
        public override ushort Length => size;
        private const int AudioStart = 7; // INT32, BYTE, USHORT
        private float volume = 1;
        private ushort size;
        private BinaryReader file;

        internal Music(int sampleRate, byte channels, ushort size, BinaryReader file) : base(sampleRate, channels)
        {
            this.size = size;
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
            int sampleRate = reader.ReadInt32();
            byte channels = reader.ReadByte();
            ushort size = reader.ReadUInt16();

            return new Music(sampleRate, channels, size, reader);
        }

        /// <summary>
        /// Retorna o próximo sample.
        /// </summary>
        internal float GetSample()
        {
            float sample = file.ReadSingle();

            if (file.BaseStream.Position >= AudioStart + size) file.BaseStream.Position = AudioStart;

            return sample;
        }
    }
}