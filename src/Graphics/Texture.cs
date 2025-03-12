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
	private BlendMode _blendMode = BlendMode.None;
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
