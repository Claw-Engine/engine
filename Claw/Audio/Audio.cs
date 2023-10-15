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
        public virtual long Length => 0;

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
        public override long Length => samples.LongLength;
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
            long size = binReader.ReadInt64();
            float[] samples = new float[size];

            for (long i = 0; i < size; i++) samples[i] = binReader.ReadSingle();

            binReader.Close();
            reader.Close();

            return new SoundEffect(sampleRate, channels, samples);
        }

        /// <summary>
        /// Retorna um sample específico.
        /// </summary>
        internal float GetSample(long position) => samples[position];
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
        public override long Length => file.BaseStream.Length / 4;
        private const int AudioStart = 13; // INT32, BYTE, INT64
        private float volume = 1;
        private BinaryReader file;

        internal Music(int sampleRate, byte channels, BinaryReader file) : base(sampleRate, channels) => this.file = file;
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

            return new Music(sampleRate, channels, reader);
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