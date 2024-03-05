using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Claw.Graphics
{
    /// <summary>
    /// Representa um texture atlas.
    /// </summary>
    public static class TextureAtlas
    {
        /// <summary>
        /// Qual a última <see cref="Sprite.Texture"/> usada pelo <see cref="Draw"/>.
        /// </summary>
        public static Texture CurrentPage { get; internal set; }
        public static ReadOnlyDictionary<string, Sprite> Sprites;
        private static Dictionary<string, Sprite> _sprites;

        static TextureAtlas()
        {
            _sprites = new Dictionary<string, Sprite>();
            Sprites = new ReadOnlyDictionary<string, Sprite>(_sprites);
        }

        /// <summary>
        /// <para>Adiciona uma sprite ao <see cref="Sprites"/>.</para>
        /// <para>Aviso: O primeiro pixel da sua textura deve ser um pixel branco.</para>
        /// </summary>
        public static void AddSprite(Sprite sprite) => _sprites.Add(sprite.Name, sprite);
        /// <summary>
        /// <para>Adiciona sprites ao <see cref="Sprites"/>.</para>
        /// <para>Aviso: O primeiro pixel da sua textura deve ser um pixel branco.</para>
        /// </summary>
        public static void AddSprites(params Sprite[] sprites)
        {
            for (int i = 0; i < sprites.Length; i++) TextureAtlas._sprites.Add(sprites[i].Name, sprites[i]);
        }
        
        /// <summary>
        /// Carrega um texture atlas e retorna o seu array de sprites.
        /// </summary>
        internal static Sprite[] ReadAtlas(string filePath)
        {
            StreamReader stream = new StreamReader(filePath);
            BinaryReader reader = new BinaryReader(stream.BaseStream);
            Texture texture = ReadTexture(reader);
            int spriteNumber = reader.ReadInt32();
            Sprite[] sprites = new Sprite[spriteNumber];
            
            if (spriteNumber == 0 || texture == null) return null;

            for (int i = 0; i < spriteNumber; i++)
            {
                sprites[i] = new Sprite(texture, reader.ReadString(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

                TextureAtlas._sprites.Add(sprites[i].Name, sprites[i]);
            }

            stream.Close();
            reader.Close();

            return sprites;
        }
        /// <summary>
        /// Carrega uma textura, com base num arquivo binário.
        /// </summary>
        internal static Texture ReadTexture(BinaryReader reader)
        {
            bool isLittleEndian = reader.ReadBoolean();
            int width = reader.ReadInt32(), height = reader.ReadInt32();

            if (width <= 0 || height <= 0) return null;

            uint[] pixels = new uint[width * height];
            byte a, b, g, r;

            for (int i = 0; i < pixels.Length; i++)
            {
                if (isLittleEndian)
                {
                    b = reader.ReadByte();
                    g = reader.ReadByte();
                    r = reader.ReadByte();
                    a = reader.ReadByte();
                }
                else
                {
                    a = reader.ReadByte();
                    r = reader.ReadByte();
                    g = reader.ReadByte();
                    b = reader.ReadByte();
                }

                pixels[i] = ((uint)a << 24) | ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
            }

            return new Texture(width, height, pixels);
        }
    }
}