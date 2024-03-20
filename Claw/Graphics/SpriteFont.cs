using System;
using System.IO;
using System.Collections.Generic;
using Claw.Extensions;

namespace Claw.Graphics
{
    /// <summary>
    /// Representa uma fonte, com base numa <see cref="Graphics.Sprite"/>.
    /// </summary>
    public sealed class SpriteFont
    {
        public readonly Sprite Sprite;
        public Vector2 Spacing;
        public readonly Dictionary<char, Glyph> Glyphs;

        public SpriteFont(Sprite sprite, Vector2 spacing, Dictionary<char, Glyph> glyphs = null)
        {
            Sprite = sprite;
            Spacing = spacing;
            Glyphs = glyphs ?? new Dictionary<char, Glyph>();
        }
        public SpriteFont(Sprite sprite, Vector2 spacing, Vector2 charSize, params char[] chars)
        {
            Sprite = sprite;
            Spacing = spacing;
            Glyphs = new Dictionary<char, Glyph>();
            int rowSize = (int)(Sprite.Width / charSize.X);

            for (int i = 0; i < chars.Length; i++) Glyphs.Add(chars[i], new Glyph(new Rectangle(Mathf.Get2DIndex(i, rowSize) * charSize, charSize)));
        }
        public SpriteFont(Sprite sprite, Vector2 spacing, Vector2 charSize, string chars) : this(sprite, spacing, charSize, chars.ToCharArray()) { }

        /// <summary>
        /// Adiciona um par de kerning para este <see cref="SpriteFont"/>.
        /// </summary>
        public SpriteFont AddKerning(char first, char second, float value)
        {
            Glyphs[second].KerningPair[first] = value;

            return this;
        }

        /// <summary>
        /// Retorna as dimensões que a <see cref="string"/> teria com este <see cref="SpriteFont"/>.
        /// </summary>
        public Vector2 MeasureString(string text)
        {
            Vector2 measure = Vector2.Zero, lineSize = Vector2.Zero;

            for (int i = 0; i < text.Length; i++)
            {
                char glyphChar = text[i];
                bool hasChar = Glyphs.ContainsKey(glyphChar);

                switch (glyphChar)
                {
                    case '\r': continue;
                    case '\n':
                        measure.Y += Spacing.Y + lineSize.Y;
                        measure.X = Math.Max(measure.X, lineSize.X);
                        lineSize = Vector2.Zero;
                        break;
                    case ' ':
                        if (hasChar) goto default;

                        lineSize.X += Spacing.X;
                        break;
                    default:
                        if (hasChar)
                        {
                            Glyph glyph = Glyphs[glyphChar];

                            lineSize.Y = Math.Max(lineSize.Y, glyph.Area.Height);
                            lineSize.X += glyph.Area.Width;

                            if (i != text.Length - 1) lineSize.X += Spacing.X;

                            if (i > 0) lineSize.X += glyph.KerningPair.Get(text[i - 1], 0);
                        }
                        else throw new ArgumentException(string.Format("Este SpriteFont não possui suporte para o caractere \"{0}\"!", glyphChar));
                        break;
                }
            }

            measure.Y += lineSize.Y;
            measure.X = Math.Max(measure.X, lineSize.X);

            return measure;
        }
        /// <summary>
        /// Retorna as dimensões que o tamanho da área de um <see cref="Glyph"/>.
        /// </summary>
        public Vector2 MeasureChar(char character) => Glyphs[character].Area.Size;

        public override string ToString() => string.Format("SpriteFont({0})", Sprite.Name);

        /// <summary>
        /// Carrega um <see cref="SpriteFont"/>.
        /// </summary>
        internal static SpriteFont ReadFont(string filePath)
        {
            StreamReader stream = new StreamReader(filePath);
            BinaryReader reader = new BinaryReader(stream.BaseStream);

            Texture atlas = TextureAtlas.ReadTexture(reader);
            string fontName = reader.ReadString();
            float charSpacing = reader.ReadSingle();
            int charCount = reader.ReadInt32();
            Dictionary<char, Glyph> glyphs = ReadGlyphs(reader, charCount);

            stream.Close();
            reader.Close();

            return new SpriteFont(new Sprite(atlas, fontName, new Rectangle(Vector2.Zero, new Vector2(atlas.Width, atlas.Height))), new Vector2(charSpacing, 0), glyphs);
        }
        /// <summary>
        /// Carrega um dicionário de caracteres.
        /// </summary>
        private static Dictionary<char, Glyph> ReadGlyphs(BinaryReader reader, int charCount)
        {
            Dictionary<char, Glyph> result = new Dictionary<char, Glyph>();

            for (int i = 0; i < charCount; i++)
            {
                char character = reader.ReadChar();
                Rectangle area = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                int pairCount = reader.ReadInt32();
                Dictionary<char, float> pairs = ReadPairs(reader, pairCount);

                result.Add(character, new Glyph(area, pairs));
            }

            return result;
        }
        /// <summary>
        /// Carrega um dicionário de pares kerning.
        /// </summary>
        private static Dictionary<char, float> ReadPairs(BinaryReader reader, int pairCount)
        {
            Dictionary<char, float> result = new Dictionary<char, float>();

            for (int i = 0; i < pairCount; i++) result.Add(reader.ReadChar(), reader.ReadSingle());

            return result;
        }
    }
	/// <summary>
	/// Representa os dados de um único caractere de um <see cref="SpriteFont"/>.
	/// </summary>
	public sealed class Glyph
	{
		public readonly Rectangle Area;
		public readonly Dictionary<char, float> KerningPair;

		public Glyph(Rectangle area, Dictionary<char, float> kerningPair = null)
		{
			Area = area;
			KerningPair = kerningPair ?? new Dictionary<char, float>();
		}
	}
}