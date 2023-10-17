using System;
using System.IO;

namespace Claw.Audio
{
    /// <summary>
    /// Representa um efeito sonoro no jogo.
    /// </summary>
    public class SoundEffect
    {
        public readonly Channels Channels;
        public long Length => samples.LongLength;
        private readonly float[] samples;
        
        /// <param name="samples">Valores entre -1 e 1.</param>
        public SoundEffect(Channels channels, float[] samples)
        {
            Channels = channels;
            this.samples = samples;
        }

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
            Channels channels = (Channels)binReader.ReadByte();
            long size = binReader.ReadInt64();
            float[] samples = new float[size];

            for (long i = 0; i < size; i++) samples[i] = binReader.ReadSingle();

            binReader.Close();
            reader.Close();

            return new SoundEffect(channels, samples);
        }

        /// <summary>
        /// Retorna um sample específico.
        /// </summary>
        internal float GetSample(long position) => samples[position];
    }
}