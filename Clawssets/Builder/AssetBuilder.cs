using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Net.NetworkInformation;

namespace Clawssets.Builder
{
    /// <summary>
    /// Classe responsável pelas chamadas de compilação.
    /// </summary>
    public static class AssetBuilder
    {
        public const string AssetExtension = ".ca", ConfigExtension = ".cb";
        public static string BuildDirectory { get; private set; }
        public static string BuildFile { get; private set; }
        private const string BuildDirectoryName = ".bin", CacheFile = ".cache";
        
        private static BuilderCache cache, newCache;
        private static List<string> outputs;
        private static List<AssetGroup> groups;

        /// <summary>
        /// Compila os assets.
        /// </summary>
        /// <param name="configPath">Local do seu .cb.</param>
        public static void BuildAssets(string configPath)
        {
            Setup(configPath);
            Build();
            RemoveDeleted();
            CopyToOutputs();
            Console.WriteLine("\nCompilação finalizada!");
        }

        /// <summary>
        /// Inicializa o compilador.
        /// </summary>
        private static void Setup(string file)
        {
            if (File.Exists(file))
            {
                BuildFile = file;
                BuildDirectory = Path.Combine(Path.GetDirectoryName(file), BuildDirectoryName);
                newCache = new BuilderCache();
                outputs = new List<string>();
                groups = new List<AssetGroup>();
                string cachePath = Path.Combine(BuildDirectory, CacheFile);

                if (File.Exists(cachePath)) cache = JsonConvert.DeserializeObject<BuilderCache>(File.ReadAllText(cachePath));
                else cache = new BuilderCache();

                Interpret(File.ReadAllLines(file));
            }
            else Console.WriteLine("Erro: O caminho {0} não existe!", file);
        }
        /// <summary>
        /// Interpreta as linhas de um arquivo de configuração, gerando a informação para o compilador.
        /// </summary>
        private static void Interpret(string[] lines)
        {
            bool gettingOutputs = true;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > 0 && !lines[i].StartsWith("|"))
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
                        else if (groups.Count > 0)
                        {
                            groups[groups.Count - 1].Files.Add(lines[i]);
                            newCache.AddFile(lines[i], groups[groups.Count - 1].Name, groups[groups.Count - 1].Type);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compila os assets e manda para um diretório temporário (<see cref="BuildDirectory"/>).
        /// </summary>
        /// <returns>Retorna true, caso a compilação tenha tido sucesso.</returns>
        private static void Build()
        {
            if (groups.Count == 0 || outputs.Count == 0) return;

            if (!Directory.Exists(BuildDirectory)) Directory.CreateDirectory(BuildDirectory);
            
            string originalPath = Path.GetDirectoryName(Path.GetFullPath(BuildFile));

            for (int i = groups.Count - 1; i >= 0; i--)
            {
                if (groups[i].Files.Count == 0)
                {
                    switch (groups[i].Type)
                    {
                        case AssetType.Texture: /* Grupo Arquivo */break;
                        default: // Grupo Pasta
                            string directory = Path.Combine(BuildDirectory, groups[i].Name);

                            if (Directory.Exists(directory) && Directory.GetDirectories(directory).Length == 0) Directory.Delete(directory, true);
                            break;
                    }

                    continue;
                }
                else if (!cache.NeedBuild(groups[i]))
                {
                    Console.WriteLine("Grupo \"{0}\" não teve alterações.", groups[i].Name);

                    continue;
                }

                switch (groups[i].Type)
                {
                    case AssetType.Texture: AtlasBuilder.Build(groups[i], originalPath); break;
                    case AssetType.Map: MapBuilder.Build(groups[i], originalPath); break;
                    case AssetType.SpriteFont: FontBuilder.Build(groups[i], originalPath); break;
                    case AssetType.Audio: AudioBuilder.Build(groups[i], originalPath); break;
					case AssetType.Ready: BuildReady(groups[i], originalPath); break;
					default: Console.WriteLine("Erro: O compilador de \"{0}\" ainda não foi implementado!", groups[i].Type); break;
                }
            }

            File.WriteAllText(Path.Combine(BuildDirectory, CacheFile), JsonConvert.SerializeObject(newCache));
        }
        private static void BuildReady(AssetGroup group, string basePath)
        {
			string directory = Path.Combine(AssetBuilder.BuildDirectory, group.Name);

			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			for (int i = 0; i < group.Files.Count; i++)
			{
				string path = Path.GetFullPath(Path.Combine(basePath, group.Files[i])),
                    outputPath = Path.Combine(directory, Path.GetFileNameWithoutExtension(path) + AssetBuilder.AssetExtension);

                File.Copy(path, outputPath);
			}
		}

        /// <summary>
        /// Remove assets que foram tirados da configuração.
        /// </summary>
        private static void RemoveDeleted()
        {
            IEnumerable<BuilderCache.FileData> difference = cache.Files.OrderByDescending((f) => f.Path.Split('/').Length)
                .Where((f) => groups.FirstOrDefault((g) => g.Type == f.Type && g.Name == f.Group && g.Files.Contains(f.Path)) == null);
            string file;

            foreach (BuilderCache.FileData data in difference)
            {
                switch (data.Type)
                {
                    case AssetType.Texture: // Grupo Arquivo
                        file = Path.Combine(BuildDirectory, data.Group + AssetExtension);
                        break;
                    default: // Grupo Pasta
                        file = Path.Combine(BuildDirectory, data.Group, Path.GetFileNameWithoutExtension(data.Path) + AssetExtension);
                        break;
                }

                if (File.Exists(file))
                {
                    File.Delete(file);

                    string directory = Path.GetDirectoryName(file);

                    if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0) Directory.Delete(directory, true);
                }
            }
        }

        /// <summary>
        /// Copia os assets para os diretórios de saída e, em seguida, apaga o diretório temporário (<see cref="BuildDirectory"/>).
        /// </summary>
        private static void CopyToOutputs()
        {
            Console.WriteLine("\nCopiando para...");

            for (int i = 0; i < outputs.Count; i++)
            {
                string dir = Path.GetFullPath(Path.Combine(BuildFile, outputs[i]));

                if (Directory.Exists(dir)) Directory.Delete(dir, true);

                Directory.CreateDirectory(dir);
                Console.WriteLine(" * \"{0}\" - (...)", dir);
                CopyTo(BuildDirectory, dir);
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine(" * \"{0}\" - (Feito!)", dir);
            }
        }
        /// <summary>
        /// Copia, recursivamente, o interior de uma pasta para outra.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="output"></param>
        private static void CopyTo(string directory, string output)
        {
            string[] files = Directory.GetFiles(directory);

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);

                if (fileName == CacheFile) continue;

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