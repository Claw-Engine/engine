using System;
using System.IO;

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

            return result;
        }
    }
}
