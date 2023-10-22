using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Clawssets.Builder
{
    /// <summary>
    /// Representa o cache de um projeto.
    /// </summary>
    public class BuilderCache
    {
        public List<FileData> Files = new List<FileData>();

        /// <summary>
        /// Adiciona um arquivo ao cachê.
        /// </summary>
        public void AddFile(string file, string group, AssetType type)
        {
            FileData fileData = new FileData() { Path = file, Group = group, Type = type, LastModified = File.GetLastWriteTimeUtc(GetPath(file)) };

            Files.Add(fileData);
        }

        /// <summary>
        /// Diz se o <see cref="AssetGroup"/> precisa ser compilado.
        /// </summary>
        public bool NeedBuild(AssetGroup group)
        {
            IEnumerable<FileData> filtered = Files.Where((f) => f.Group == group.Name && f.Type == group.Type);
            int count = 0;

            foreach (FileData data in filtered)
            {
                if (!group.Files.Contains(data.Path)) return true;
                else if (File.GetLastWriteTimeUtc(GetPath(data.Path)) != data.LastModified) return true;

                count++;
            }

            return count != group.Files.Count;
        }

        /// <summary>
        /// Retorna o caminho completo.
        /// </summary>
        private static string GetPath(string path) => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(AssetBuilder.BuildFile), path));

        /// <summary>
        /// Representa informações dos arquivos compilados.
        /// </summary>
        public struct FileData
        {
            public string Group, Path;
            public AssetType Type;
            public DateTime LastModified;

            /// <summary>
            /// Atualiza a informação de data de modificação.
            /// </summary>
            public void UpdateDate() => LastModified = File.GetLastWriteTimeUtc(GetPath(Path));
        }
    }
}