using System.Runtime.InteropServices;
using static Claw.SDL;

namespace Claw.Graphics;

/// <summary>
/// Representa o renderizador da <see cref="Window"/>.
/// </summary>
public sealed class Renderer : IDisposable
{
	public bool VSync
	{
		get => _vSync;
		set
		{
			_vSync = value;

			SDL_SetRenderVSync(id, _vSync ? 1 : 0);
		}
	}
	public Color ClearColor
	{
		get
		{
			SDL_GetRenderDrawColor(id, out byte r, out byte g, out byte b, out byte a);

			return new Color(r, g, b, a);
		}
		set => SDL_SetRenderDrawColor(id, value.R, value.G, value.B, value.A);
	}
	private IntPtr id;
	private RenderTarget currentTarget;
	private bool _vSync = true;
	private static readonly SDL_PixelFormat PixelFormat = SDL_PixelFormat.SDL_PIXELFORMAT_ABGR8888;

	internal Renderer(IntPtr renderer) => id = renderer;
	~Renderer() => Dispose();

	public void Dispose()
	{
		SDL_DestroyRenderer(id);

		id = IntPtr.Zero;
	}

	/// <summary>
	/// Retorna o alvo da renderização atual.
	/// </summary>
	public RenderTarget GetRenderTarget() => currentTarget;
	/// <summary>
	/// Altera o alvo da renderização.
	/// </summary>
	/// <param name="renderTarget">Nulo para desenhar na <see cref="Window"/>.</param>
	public void SetRenderTarget(RenderTarget renderTarget)
	{
		if (currentTarget == renderTarget) return;

		currentTarget = renderTarget;

		SDL_SetRenderTarget(id, renderTarget != null ? renderTarget.id : IntPtr.Zero);
	}

	/// <summary>
	/// Desenha uma imagem.
	/// </summary>
	/// <param name="angle">Graus.</param>
	public void DrawTexture(Texture texture, Rectangle source, Rectangle destination, Color color, Vector2 origin, float angle, Flip flip)
	{
		SDL_FRect src = source.ToSDLF();
		SDL_FRect dest = destination.ToSDLF();
		SDL_FPoint center = origin.ToSDLF();

		SDL_SetTextureColorMod(texture.id, color.R, color.G, color.B);
		SDL_SetTextureAlphaMod(texture.id, color.A);
		DrawTexture(texture.id, ref src, ref dest, ref center, (double)angle, (SDL_FlipMode)flip);
		SDL_SetTextureAlphaMod(texture.id, 255);
		SDL_SetTextureColorMod(texture.id, 255, 255, 255);
	}
	/// <summary>
	/// Desenha uma imagem.
	/// </summary>
	/// <param name="angle">Graus.</param>
	private void DrawTexture(IntPtr texture, ref SDL_FRect src, ref SDL_FRect dest, ref SDL_FPoint center, double angle, SDL_FlipMode flip) => SDL_RenderTextureRotated(id, texture, ref src, ref dest, (double)angle, ref center, flip);
	/// <summary>
	/// Desenha um pixel.
	/// </summary>
	internal void DrawPixel(int x, int y) => SDL_RenderPoint(id, x, y);

	/// <summary>
	/// Cria uma textura com as dimensões especificadas.
	/// </summary>
	internal IntPtr CreateTexture(int width, int height, SDL_TextureAccess access) => SDL_CreateTexture(id, PixelFormat, access, width, height);
	/// <summary>
	/// Cria uma textura com as dimensões e pixels especificados.
	/// </summary>
	internal unsafe IntPtr CreateTexture(int width, int height, uint[] data)
	{
		GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
		IntPtr surface = SDL_CreateSurfaceFrom(width, height, PixelFormat, handle.AddrOfPinnedObject(), width * 4);
		IntPtr texture = SDL_CreateTextureFromSurface(id, surface);

		handle.Free();
		SDL_DestroySurface(surface);

		return texture;
	}

	/// <summary>
	/// Preenche um array de pixels com os pixels do <see cref="currentTarget"/>.
	/// </summary>
	internal unsafe uint[] ReadPixels(int width, int height)
	{
		if (currentTarget != null)
		{
			SDL_Rect rect = new();
			rect.w = width;
			rect.h = height;
			SDL_Surface* surface = SDL_RenderReadPixels(id, ref rect);
			uint* pixels = (uint*)(*surface).pixels;
			uint[] result = new uint[width * height];

			for (int i = 0; i < result.Length; i++) result[i] = pixels[i];

			SDL_DestroySurface(surface);

			return result;
		}

		return null;
	}

	/// <summary>
	/// Aplica o viewport da câmera.
	/// </summary>
	internal void SetViewport(Camera camera)
	{
		if (!camera.Viewport.IsEmpty)
		{
			SDL_Rect view = camera.Viewport.ToSDL();

			SDL_SetRenderViewport(id, ref view);
		}
		else SDL_SetRenderViewport(id, IntPtr.Zero);
	}
	/// <summary>
	/// Remove o viewport.
	/// </summary>
	internal void ResetViewport() => SDL_SetRenderViewport(id, IntPtr.Zero);

	/// <summary>
	/// Limpa a tela com o <see cref="ClearColor"/>.
	/// </summary>
	public void Clear() => SDL_RenderClear(id);
	/// <summary>
	/// Limpa a tela com uma cor específica.
	/// </summary>
	public void Clear(Color color)
	{
		SDL_SetRenderDrawColor(id, color.R, color.G, color.B, color.A);
		SDL_RenderClear(id);
		SDL_SetRenderDrawColor(id, ClearColor.R, ClearColor.G, ClearColor.B, ClearColor.A);
	}
	/// <summary>
	/// Desenha o que foi renderizado.
	/// </summary>
	internal void Present() => SDL_RenderPresent(id);
}
