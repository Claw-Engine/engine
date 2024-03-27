using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Claw.Graphics;

namespace Claw.Tiled
{
    /// <summary>
    /// Configurações para o <see cref="Tiled"/>.
    /// </summary>
    public sealed class Config
    {
        private Assembly assembly;
        private string prefixNamespace;
        private Dictionary<string, Sprite> palettes = new Dictionary<string, Sprite>();

        public Config(string prefixNamespace)
        {
            this.prefixNamespace = prefixNamespace;
            assembly = Game.Instance.GetType().Assembly;
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
        internal object Instantiate(string type) => Activator.CreateInstance(assembly.GetType(string.Format("{0}.{1}", prefixNamespace, type)), new object[] { false });
        
        /// <summary>
        /// Retorna uma paleta com o nome dado caso ela exista. Caso contrário ele retornará a textura padrão.
        /// </summary>
        internal Sprite GetPalette(string tileset)
        {
            if (palettes.TryGetValue(tileset, out Sprite palette)) return palette;

            throw new KeyNotFoundException(string.Format("A paleta \"{0}\" não existe!", tileset));
        }
    }
}