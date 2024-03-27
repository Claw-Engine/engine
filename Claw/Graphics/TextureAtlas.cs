using System;
using System.IO;
using System.Collections.Generic;

namespace Claw.Graphics
{
    /// <summary>
    /// Representa um texture atlas.
    /// </summary>
    public class TextureAtlas
    {
        public Texture Page { get; private set; }
        /// <summary>
        /// Retorna uma sprite específica.
        /// </summary>
        public Sprite this[string sprite] => sprites[sprite];

		/// <summary>
		/// Qual a última <see cref="Sprite.Texture"/> usada pelo <see cref="Draw"/>.
		/// </summary>
		public static Texture CurrentPage { get; internal set; }
        private Dictionary<string, Sprite> sprites;

        private TextureAtlas() => sprites = new Dictionary<string, Sprite>();
        public TextureAtlas(params Sprite[] sprites) : this()
        {
            if (sprites.Length == 0) return;

            Page = sprites[0].Texture;

            for (int i = 0; i < sprites.Length; i++) AddSprite(this, sprites[i]);
        }
        
        /// <summary>
        /// Carrega um texture atlas e retorna o seu array de sprites.
        /// </summary>
        internal static TextureAtlas ReadAtlas(string filePath)
        {
            StreamReader stream = new StreamReader(filePath);
            BinaryReader reader = new BinaryReader(stream.BaseStream);
			TextureAtlas atlas = new TextureAtlas();
			atlas.Page = Texture.ReadTexture(reader);
            int spriteNumber = reader.ReadInt32();
            
            if (spriteNumber == 0 || atlas.Page == null) return null;

            for (int i = 0; i < spriteNumber; i++) AddSprite(atlas, new Sprite(atlas.Page, reader.ReadString(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()));

			stream.Close();
            reader.Close();

            return atlas;
        }
		private static void AddSprite(TextureAtlas atlas, Sprite sprite) => atlas.sprites.Add(sprite.Name, sprite);
	}
}