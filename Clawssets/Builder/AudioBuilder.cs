using System;
using System.IO;
using Clawssets.Builder.Data;

namespace Clawssets.Builder
{
    /// <summary>
    /// Classe para compilação de áudios.
    /// </summary>
    public static class AudioBuilder
    {
        /// <summary>
        /// Compila os áudios para a pasta do grupo.
        /// </summary>
        public static void Build(AssetGroup group, string basePath)
        {
            string directory = Path.Combine(AssetBuilder.BuildDirectory, group.Name);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            for (int i = 0; i < group.Files.Count; i++)
            {
                string path = Path.GetFullPath(Path.Combine(basePath, group.Files[i]));
                string outputPath = Path.Combine(directory, Path.GetFileNameWithoutExtension(path) + AssetBuilder.AssetExtension);
                Audio.Description sound = Audio.LoadAudio(path);

                if (sound != null)
                {
                    StreamWriter writer = new StreamWriter(outputPath);
                    BinaryWriter binWriter = new BinaryWriter(writer.BaseStream);

                    binWriter.Write(sound);
                    binWriter.Close();
                }
                else return;
            }

            Console.WriteLine("Grupo {0} compilado com sucesso!", group.Name);
        }

        /// <summary>
        /// Escreve os dados do som num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Audio.Description sound)
        {
            writer.Write(sound.Channels);
            writer.Write(sound.Samples.LongLength);

            for (long i = 0; i < sound.Samples.Length; i++) writer.Write(sound.Samples[i]);
        }
    }
}