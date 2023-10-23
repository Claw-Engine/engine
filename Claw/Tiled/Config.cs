using System;
using System.Collections.Generic;
using System.Linq;
using Claw.Graphics;

namespace Claw.Tiled
{
    /// <summary>
    /// Configurações para o <see cref="Tiled"/>.
    /// </summary>
    public sealed class Config
    {
        private readonly static Sprite NotFoundPalette = new Sprite(Texture.Pixel, "_pixel_", new Rectangle(0, 0, 1, 1));
        private string assemblyName, prefixNamespace;
        private Dictionary<string, Sprite> palettes = new Dictionary<string, Sprite>();

        public Config(string prefixNamespace)
        {
            this.prefixNamespace = prefixNamespace;
            assemblyName = Game.Instance.GetType().Assembly.FullName;
        }

        /// <summary>
        /// Adiciona as paletas do Tiled.
        /// </summary>
        public void AddPalettes(string[] palettes, Sprite[] palettesTexture)
        {
            for (int i = 0; i < palettes.Length; i++) this.palettes.Add(palettes[i], palettesTexture[i]);
        }
        /// <summary>
        /// Adiciona as paletas do Tiled.
        /// </summary>
        public void AddPalettes(params (string name, Sprite texture)[] palettes)
        {
            for (int i = 0; i < palettes.Length; i++) this.palettes.Add(palettes[i].name, palettes[i].texture);
        }

        /// <summary>
        /// Cria uma instância, com base no namespace.
        /// </summary>
        internal object Instantiate(string type) => Activator.CreateInstance(assemblyName, string.Format("{0}.{1}", prefixNamespace, type)).Unwrap();
        
        /// <summary>
        /// Retorna uma paleta com o nome dado caso ela exista. Caso contrário ele retornará a textura padrão.
        /// </summary>
        internal Sprite GetPalette(string tileset)
        {
            if (palettes.TryGetValue(tileset, out Sprite palette)) return palette;

            return NotFoundPalette;
        }
    }
}