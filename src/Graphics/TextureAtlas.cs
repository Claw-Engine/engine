namespace Claw.Graphics;

/// <summary>
/// Representa um texture atlas.
/// </summary>
public sealed class TextureAtlas
{
	public Texture Page { get; private set; }
	/// <summary>
	/// Retorna uma sprite específica.
	/// </summary>
	public Sprite this[string sprite] => sprites[sprite];
	private Dictionary<string, Sprite> sprites;

	private TextureAtlas() => sprites = new Dictionary<string, Sprite>();
	public TextureAtlas(params Sprite[] sprites) : this()
	{
		if (sprites.Length == 0) return;

		Page = sprites[0].Texture;

		for (int i = 0; i < sprites.Length; i++) AddSprite(this, sprites[i]);
	}
	
	private static void AddSprite(TextureAtlas atlas, Sprite sprite) => atlas.sprites.Add(sprite.Name, sprite);
}
