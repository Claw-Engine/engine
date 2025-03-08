namespace Claw.Graphics;

/// <summary>
/// Desenha retângulos texturizados.
/// </summary>
public static class NineSlice
{
	private static Dictionary<string, Piece[]> textures = new Dictionary<string, Piece[]>();

	/// <summary>
	/// Divide uma sprite em 9 partes e adiciona ao <see cref="NineSlice"/>.
	/// </summary>
	/// <param name="sprite">Top Left; Top; Top Right; Left; Center; Right; Bottom Left; Bottom; Bottom Right.</param>
	public static void AddTexture(string index, Sprite sprite) => AddTexture(index, sprite, new Rectangle(0, 0, sprite.Width, sprite.Height));
	/// <summary>
	/// Divide uma área de uma sprite em 9 partes e adiciona ao <see cref="NineSlice"/>.
	/// </summary>
	/// <param name="textureArea">Top Left; Top; Top Right; Left; Center; Right; Bottom Left; Bottom; Bottom Right.</param>
	public static void AddTexture(string index, Sprite sprite, Rectangle textureArea)
	{
		Vector2 pieceSize = new Vector2(textureArea.Width, textureArea.Height) / 3;
		Piece[] pieces = new Piece[9];

		for (int x = 0; x < 3; x++)
		{
			for (int y = 0; y < 3; y++)
			{
				Vector2 pos = new Vector2(x, y);
				pieces[Mathf.Get1DIndex(pos, new Vector2(3))] = new Piece(sprite.Texture, new Rectangle(new Vector2(sprite.X + textureArea.X, sprite.Y + textureArea.Y) + pos * pieceSize, pieceSize));
			}
		}

		textures.Add(index, pieces);
	}
	/// <summary>
	/// Adiciona uma textura ao <see cref="NineSlice"/>.
	/// </summary>
	/// <param name="parts">Top Left; Top; Top Right; Left; Center; Right; Bottom Left; Bottom; Bottom Right.</param>
	public static void AddTexture(string index, Sprite sprite, params Rectangle[] parts)
	{
		if (parts.Length < 9) throw new ArgumentException("\"parts\" precisa ter pelo menos 9 elementos!");

		Piece[] pieces = new Piece[9];
		Vector2 spritePos = new Vector2(sprite.X, sprite.Y);

		for (int i = 0; i < pieces.Length; i++) pieces[i] = new Piece(sprite.Texture, new Rectangle(spritePos + parts[i].Location, parts[i].Size));

		textures.Add(index, pieces);
	}

	/// <summary>
	/// Desenha o retângulo texturizado e retorna a área do seu interior.
	/// </summary>
	/// <param name="scaleCenter">Define se o centro do <see cref="NineSlice"/> será escalado ou repetido.</param>
	public static Rectangle Draw(string texture, Rectangle area, Color backgroundColor, Color blendColor, bool scaleCenter = false) => DrawTexturized(textures[texture], Rectangle.Positive(area), backgroundColor, blendColor, scaleCenter);

	/// <summary>
	/// Retorna um retângulo com o tamanho da distância horizontal e vertical entre dois pontos.
	/// </summary>
	private static Rectangle GetDestination(Vector2 position, Vector2 start, Vector2 stop)
	{
		Vector2 size = stop - start;

		if (size.X < 0 || size.Y < 0) size = Vector2.Zero;

		return new Rectangle(position, size);
	}
	/// <summary>
	/// Desenha uma sprite, preenchendo um área específica.
	/// </summary>
	private static void TiledSprite(Piece piece, Rectangle destinationRectangle, Color color)
	{
		TextureAtlas.CurrentPage = piece.texture;
		Rectangle source = piece.area;
		source.Size = Vector2.Clamp(source.Size, Vector2.Zero, destinationRectangle.Size);

		for (float x = destinationRectangle.X; x < destinationRectangle.End.X; x += source.Width)
		{
			for (float y = destinationRectangle.Y; y < destinationRectangle.End.Y; y += source.Height) Graphics.Draw.Sprite(piece.texture, new Vector2(x, y), source, color, 0, Vector2.Zero, 1, 0);
		}
	}
	/// <summary>
	/// Desenha uma área texturizada e retorna o <see cref="Rectangle"/> de seu interior.
	/// </summary>
	private static Rectangle DrawTexturized(Piece[] pieces, Rectangle area, Color backgroundColor, Color blendColor, bool scaleCenter = false)
	{
		Vector2 Location = area.Location, Size = area.Size;
		Vector2 topLeftOffset = pieces[0].area.Size, topRightOffset = pieces[2].area.Size, bottomLeftOffset = pieces[6].area.Size, bottomRightOffset = pieces[8].area.Size;
		Rectangle rectangle = Rectangle.Positive(new Rectangle(Location + topLeftOffset, Size - new Vector2(bottomRightOffset.X * 2, bottomRightOffset.Y * 2)));

		// Top
		TiledSprite(pieces[1], GetDestination(Location + new Vector2(topLeftOffset.X, 0), Location + new Vector2(topLeftOffset.X, 0),
			new Vector2(area.Right - topRightOffset.X, Location.Y + topLeftOffset.Y)), blendColor);
		// Bottom
		TiledSprite(pieces[7], GetDestination(new Vector2(Location.X + topLeftOffset.X, area.Bottom - bottomLeftOffset.Y), new Vector2(Location.X + topLeftOffset.X, area.Bottom - bottomLeftOffset.Y),
			new Vector2(area.Right - bottomRightOffset.X, area.Bottom)), blendColor);
		// Left
		TiledSprite(pieces[3], GetDestination(Location + new Vector2(0, topLeftOffset.Y), Location + new Vector2(0, topLeftOffset.Y),
			new Vector2(Location.X + topLeftOffset.X, area.Bottom - bottomLeftOffset.Y)), blendColor);
		// Right
		TiledSprite(pieces[5], GetDestination(new Vector2(area.Right - topRightOffset.X, Location.Y + topRightOffset.Y), new Vector2(area.Right - topRightOffset.X, Location.Y + topRightOffset.Y),
			new Vector2(area.Right, area.Bottom - bottomRightOffset.Y)), blendColor);

		// Center
		if (scaleCenter)
		{
			Rectangle repeatSize = GetDestination(Vector2.Zero, Location + topLeftOffset, new Vector2(area.Right - bottomRightOffset.X, area.Bottom - bottomRightOffset.Y));
			Vector2 scale = repeatSize.Size / pieces[4].area.Size;
			
			Graphics.Draw.Sprite(pieces[4].texture, (Location + topLeftOffset), pieces[4].area, blendColor, 0, Vector2.Zero, scale, Flip.None);
		}
		else TiledSprite(pieces[4], GetDestination((Location + topLeftOffset), Location + topLeftOffset, new Vector2(area.Right - bottomRightOffset.X, area.Bottom - bottomRightOffset.Y)), blendColor);

		// TopLeft
		Graphics.Draw.Sprite(pieces[0].texture, Location, pieces[0].area, blendColor);
		// TopRight
		Graphics.Draw.Sprite(pieces[2].texture, new Vector2(area.Right - topRightOffset.X, Location.Y), pieces[2].area, blendColor);
		// BottomLeft
		Graphics.Draw.Sprite(pieces[6].texture, new Vector2(Location.X, area.Bottom - bottomLeftOffset.Y), pieces[6].area, blendColor);
		// BottomRight
		Graphics.Draw.Sprite(pieces[8].texture, new Vector2(area.Right - topRightOffset.X, area.Bottom - bottomLeftOffset.Y), pieces[8].area, blendColor);

		return rectangle;
	}
	
	private class Piece
	{
		public Texture texture;
		public Rectangle area;

		public Piece(Texture texture, Rectangle area)
		{
			this.texture = texture;
			this.area = area;
		}
	}
}
