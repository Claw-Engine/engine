namespace Claw.Graphics;

/// <summary>
/// Representa um atlas de texturas.
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

	/// <summary>
	/// Carrega um atlas de texturas.
	/// </summary>
	public static TextureAtlas Load(string path)
	{
		BinaryReader file = new BinaryReader(new StreamReader(path).BaseStream);

		if (file.ReadString() != "atlas") return null;

		Texture texture = Texture.Load(file);
		Sprite[] sprites = new Sprite[file.ReadInt32()];

		for (int i = 0; i < sprites.Length; i++) sprites[i] = new(texture, file.ReadString(), file.ReadInt32(), file.ReadInt32(), file.ReadInt32(), file.ReadInt32());

		return new TextureAtlas(sprites);
	}
	
	private static void AddSprite(TextureAtlas atlas, Sprite sprite) => atlas.sprites.Add(sprite.Name, sprite);
}
