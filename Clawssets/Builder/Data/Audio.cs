using System;
using System.IO;
using Claw.Audio;

namespace Clawssets.Builder.Data
{
    /// <summary>
    /// Classe responsável por ser uma ponte entre diferentes tipos de áudio.
    /// </summary>
    public static class Audio
    {
        /// <summary>
        /// Descreve um áudio.
        /// </summary>
        public class Description
        {
            public int SampleRate;
            public byte Channels;
            public float[] Samples;

            /// <summary>
            /// Padroniza o sample rate para <see cref="AudioManager.SampleRate"/>
            /// </summary>
            public void Resample()
            {
                if (SampleRate != AudioManager.SampleRate)
                {
                    float factor = 0;
                    float[] resampled = null;

                    if (AudioManager.SampleRate > SampleRate) // Upsample
                    {
                        factor = (float)AudioManager.SampleRate / SampleRate;
                        resampled = new float[(long)(Samples.LongLength * factor)];

                        for (long i = 0; i < Samples.LongLength; i++)
                        {
                            long index = (long)(i * factor);

                            if (index != resampled.LongLength - 1)
                            {
                                for (int j = 1; j < factor; j++) resampled[index + j] = Samples[i];
                            }

                            resampled[index] = Samples[i];
                        }

                        SampleRate = AudioManager.SampleRate;
                        Samples = resampled;
                    }
                }
            }
        }
        
        /// <summary>
        /// Carrega um áudio.
        /// </summary>
        public static Description LoadAudio(string path)
        {
            string audioExtension = Path.GetExtension(path).ToLower();
            Description result = null;
            StreamReader reader = new StreamReader(path);
            BinaryReader binReader = new BinaryReader(reader.BaseStream);

            switch (audioExtension)
            {
                case ".wav":
                case ".wave":
                    result = WavReader.Load(binReader);
                    break;
                default: Console.WriteLine("Erro: A extensão \"{0}\" não é suportada!\nArquivo: {1}", audioExtension, path); break;
            }

            binReader.Close();
            result?.Resample();

            return result;
        }
    }
}
