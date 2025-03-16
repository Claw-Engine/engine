using System.Runtime.InteropServices;
using static Claw.SDL;

namespace Claw.Graphics;

/// <summary>
/// Representa uma textura no jogo.
/// </summary>
public class Texture
{
	public static Texture Pixel { get; internal set; }
	public readonly int Width, Height;
	public Vector2 Size => new Vector2(Width, Height);
	public BlendMode BlendMode
	{
		get => _blendMode;
		set
		{
			_blendMode = value;

			SDL_SetTextureBlendMode(id, (uint)value);
		}
	}
	public ScaleMode ScaleMode
	{
		get => _scaleMode;
		set
		{
			_scaleMode = value;

			SDL_SetTextureScaleMode(id, (SDL_ScaleMode)value);
		}
	}
	private BlendMode _blendMode = BlendMode.None;
	private ScaleMode _scaleMode = ScaleMode.Linear;
	internal readonly IntPtr id;

	internal Texture(int width, int height, IntPtr texture)
	{
		Width = width;
		Height = height;
		id = texture;
	}
	public Texture(int width, int height) : this(width, height, Game.Instance.Renderer.CreateTexture(width, height, SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING)) { }
	public Texture(int width, int height, params uint[] pixels) : this(width, height, Game.Instance.Renderer.CreateTexture(width, height, pixels)) { }

	/// <summary>
	/// Destrói esta textura.
	/// </summary>
	public void Destroy() => SDL_DestroyTexture(id);

	/// <summary>
	/// Carrega uma textura.
	/// </summary>
	/// <returns>A textura ou null (se não for um arquivo válido).</returns>
	public static Texture Load(string path)
	{
		BinaryReader file = new BinaryReader(new StreamReader(path).BaseStream);

		if (file.ReadString() != "texture") return null;

		return Load(file);
	}
	internal static Texture Load(BinaryReader reader)
	{
		int width = reader.ReadInt32(), height = reader.ReadInt32();
		uint[] pixels = new uint[width * height];

		for (int i = 0; i < pixels.Length; i++) pixels[i] = reader.ReadUInt32();

		return new Texture(width, height, pixels);
	}
	
	/// <summary>
	/// Altera os pixels desta textura.
	/// </summary>
	public virtual void SetData(params uint[] pixels)
	{
		GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
		SDL_Rect rect = new SDL_Rect();
		rect.w = Width;
		rect.h = Height;

		SDL_UpdateTexture(id, ref rect, handle.AddrOfPinnedObject(), Width * sizeof(UInt32));
		handle.Free();
	}
}
