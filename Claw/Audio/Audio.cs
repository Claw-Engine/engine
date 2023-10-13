using System;
using System.IO;

namespace Claw.Audio
{
    /// <summary>
    /// Representa um áudio no jogo.
    /// </summary>
    public class Audio
    {
        public int SampleRate => audio.freq;
        public byte Channels => audio.channels;
        public virtual ushort Length => 0;
        internal SDL.SDL_AudioSpec audio;

        internal Audio(int sampleRate, byte channels)
        {
            audio = new SDL.SDL_AudioSpec();
            audio.freq = sampleRate;
            audio.channels = channels;
            audio.format = AudioManager.AudioFormat;
            audio.samples = AudioManager.BufferSize;
        }

        /// <summary>
        /// Cria um <see cref="AudioInstance"/> deste áudio.
        /// </summary>
        public AudioInstance CreateInstance() => new AudioInstance(this);
    }
    /// <summary>
    /// Representa um efeito sonoro no jogo.
    /// </summary>
    public class SoundEffect : Audio
    {
        public override ushort Length => (ushort)samples.Length;
        private readonly int[] samples;

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
    }
    /// <summary>
    /// Representa uma música no jogo.
    /// </summary>
    public class Music : Audio, IDisposable
    {
        public override ushort Length => size;
        private const int AudioStart = 7; // INT32, BYTE, USHORT
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
    }
}