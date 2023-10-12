using System;
using System.IO;
using System.Collections.Generic;

namespace Clawssets.Builder
{
    /// <summary>
    /// Classe responsável pelas chamadas de compilação.
    /// </summary>
    public class AssetBuilder
    {
        public const string AssetExtension = ".ca", ConfigExtension = ".cb", BuildDirectory = ".Temp";
        private string buildFile;
        private List<string> outputs = new List<string>();
        private List<AssetGroup> groups = new List<AssetGroup>();

        public AssetBuilder(string buildFile)
        {
            if (File.Exists(buildFile))
            {
                this.buildFile = buildFile;

                Interpret(File.ReadAllLines(buildFile));
            }
            else Console.WriteLine("Erro: O caminho {0} não existe!", buildFile);
        }

        /// <summary>
        /// Interpreta as linhas de um arquivo de configuração, gerando a informação para o compilador.
        /// </summary>
        private void Interpret(string[] lines)
        {
            bool gettingOutputs = true;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > 0)
                {
                    bool isBegin = lines[i].StartsWith("#");

                    if (gettingOutputs)
                    {
                        if (isBegin) gettingOutputs = false;
                        else outputs.Add(lines[i]);
                    }

                    if (!gettingOutputs)
                    {
                        if (isBegin)
                        {
                            AssetGroup newGroup = new AssetGroup();
                            string[] typeAndName = lines[i].Substring(1).Split(':');
                            newGroup.Type = (AssetType)Enum.Parse(typeof(AssetType), typeAndName[0]);
                            newGroup.Name = typeAndName[1];

                            groups.Add(newGroup);
                        }
                        else if (groups.Count > 0) groups[groups.Count - 1].Files.Add(lines[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Compila os assets e manda para um diretório temporário (<see cref="BuildDirectory"/>).
        /// </summary>
        /// <returns>Retorna true, caso a compilação tenha tido sucesso.</returns>
        public void Build()
        {
            if (groups.Count == 0 || outputs.Count == 0) return;

            if (Directory.Exists(BuildDirectory)) Directory.Delete(BuildDirectory, true);

            Directory.CreateDirectory(BuildDirectory);
            
            string originalPath = Path.GetDirectoryName(Path.GetFullPath(buildFile));

            for (int i = 0; i < groups.Count; i++)
            {
                switch (groups[i].Type)
                {
                    case AssetType.Texture: AtlasBuilder.Build(groups[i], originalPath); break;
                    case AssetType.Map: MapBuilder.Build(groups[i], originalPath); break;
                    case AssetType.SpriteFont: FontBuilder.Build(groups[i], originalPath); break;
                    case AssetType.Audio: AudioBuilder.Build(groups[i], originalPath); break;
                    default: Console.WriteLine("Erro: O compilador de \"{0}\" ainda não foi implementado!", groups[i].Type); break;
                }
            }
        }

        /// <summary>
        /// Copia os assets para os diretórios de saída e, em seguida, apaga o diretório temporário (<see cref="BuildDirectory"/>).
        /// </summary>
        public void CopyToOutputs()
        {
            for (int i = 0; i < outputs.Count; i++)
            {
                string dir = Path.GetFullPath(Path.Combine(buildFile, outputs[i]));

                if (Directory.Exists(dir)) Directory.Delete(dir, true);

                Directory.CreateDirectory(dir);

                CopyTo(BuildDirectory, dir);
            }

            if (Directory.Exists(BuildDirectory)) Directory.Delete(BuildDirectory, true);
        }
        /// <summary>
        /// Copia, recursivamente, o interior de uma pasta para outra.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="output"></param>
        private void CopyTo(string directory, string output)
        {
            string[] files = Directory.GetFiles(directory);

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                string outputPath = Path.Combine(output, fileName);
                string outputDirectory = Path.GetDirectoryName(outputPath);

                if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);

                File.Copy(files[i], outputPath);
            }

            string[] subDirectories = Directory.GetDirectories(directory);

            for (int i = 0; i < subDirectories.Length; i++)
            {
                string dirName = Path.GetFileName(subDirectories[i]);
                string newOutputDir = Path.Combine(output, dirName);

                Directory.CreateDirectory(newOutputDir);

                CopyTo(subDirectories[i], newOutputDir);
            }
        }
    }
}