using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Claw.Graphics;
using Claw.Audio;
using Claw.Tiled;

namespace Claw
{
    /// <summary>
    /// Classe responsável pelo carregamento de assets.
    /// </summary>
    public static class Asset
    {
        /// <summary>
        /// Diretório base dos assets ("Assets", por padrão).
        /// </summary>
        public static string RootDirectory = "Assets";
        private const string AssetExtension = ".ca";
        private static string currentDirectory;
        private static string fullPath => Path.Combine(currentDirectory, RootDirectory);
        private static Dictionary<Type, Func<string, object>> readers;

        static Asset()
        {
            currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            readers = new Dictionary<Type, Func<string, object>>()
            {
                { typeof(Sprite[]), TextureAtlas.ReadAtlas },
                { typeof(Map), Map.ReadMap },
                { typeof(SpriteFont), SpriteFont.ReadFont },
                { typeof(SoundEffect), SoundEffect.LoadSFX },
                { typeof(Music), Music.LoadMusic }
            };
        }

        /// <summary>
        /// Define uma função que carregará determinado tipo de asset.
        /// </summary>
        /// <param name="type">O tipo de asset.</param>
        /// <param name="reader">A função, que recebe um arquivo e retorna um asset ou nulo.</param>
        public static void AddReader(Type type, Func<string, object> reader)
        {
            if (readers.ContainsKey(type)) throw new ArgumentException("Esse tipo já tem um leitor definido!");

            readers.Add(type, reader);
        }

        /// <summary>
        /// Carrega um asset através de um arquivo.
        /// </summary>
        /// <typeparam name="T">Tipo de asset.</typeparam>
        /// <param name="assetPath">Caminho relativo do arquivo, sem a extensão.</param>
        public static T Load<T>(string assetPath)
        {
            string realPath = Path.Combine(fullPath, assetPath + AssetExtension);

            return (T)readers[typeof(T)](realPath);
        }
    }
}