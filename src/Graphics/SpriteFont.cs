using Claw.Extensions;

namespace Claw.Graphics;

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
	/// Carrega uma fonte.
	/// </summary>
	/// <returns>A fonte ou null (se não for um arquivo válido).</returns>
	public static SpriteFont Load(string path)
	{
		BinaryReader file = new BinaryReader(new StreamReader(path).BaseStream);

		if (file.ReadString() != "font") return null;

		Texture texture = Texture.Load(file);
		Vector2 spacing = new(file.ReadSingle(), file.ReadSingle());
		SpriteFont font = new SpriteFont(new Sprite(texture, Path.GetFileNameWithoutExtension(path), 0, 0, texture.Width, texture.Height), spacing, new Dictionary<char, Glyph>());
		int length = file.ReadInt32(), pairLength;
		char currentChar;
		Glyph currentGlyph;

		for (int i = 0; i < length; i++)
		{
			currentChar = (char)file.ReadInt32();
			currentGlyph = new(new Rectangle(file.ReadInt32(), file.ReadInt32(), file.ReadInt32(), file.ReadInt32()));
			pairLength = file.ReadInt32();

			for (int j = 0; j < pairLength; j++) currentGlyph.KerningPair.Add(file.ReadChar(), file.ReadSingle());

			font.Glyphs.Add(currentChar, currentGlyph);
		}

		return font;
	}

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
				case '\n':
					measure.Y += Spacing.Y + lineSize.Y;
					measure.X = Math.Max(measure.X, lineSize.X);
					lineSize = Vector2.Zero;
					break;
				case '\r':
					measure.X = Math.Max(measure.X, lineSize.X);
					lineSize.X = 0;
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
