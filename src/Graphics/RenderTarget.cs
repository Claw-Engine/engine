using static Claw.SDL;

namespace Claw.Graphics;

/// <summary>
/// Representa um alvo de renderização para o <see cref="Renderer"/>.
/// </summary>
public sealed class RenderTarget : Texture
{
	public RenderTarget(int width, int height) : base(width, height, Game.Instance.Renderer.CreateTexture(width, height, SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET)) { }

	/// <summary>
	/// Destrói este alvo.
	/// </summary>
	public void Destroy() => SDL_DestroyTexture(id);

	/// <summary>
	/// <para>Obtém os pixels desta textura.</para>
	/// <para>Aviso: Função lenta e não recomendada de se usar dentro de um loop.</para>
	/// </summary>
	public uint[] GetData()
	{
		RenderTarget target = Game.Instance.Renderer.GetRenderTarget();

		Game.Instance.Renderer.SetRenderTarget(this);

		uint[] pixels = Game.Instance.Renderer.ReadPixels(Width, Height);

		Game.Instance.Renderer.SetRenderTarget(target);

		return pixels;
	}
	/// <summary>
	/// <para>Altera os pixels desta textura.</para>
	/// <para>Aviso: Uso não recomendado (cada pixel será atualizado manualmente, num laço de repetição).</para>
	/// <para>Recomendado: Use o método <see cref="Renderer.SetRenderTarget(RenderTarget)"/> no lugar disso.</para>
	/// </summary>
	public override void SetData(params uint[] pixels)
	{
		int length = Math.Min(pixels.Length, Width * Height);
		RenderTarget original = Game.Instance.Renderer.GetRenderTarget();
		Color originalColor = Game.Instance.Renderer.ClearColor;

		Game.Instance.Renderer.SetRenderTarget(this);

		int x = 0, y = 0;

		for (int i = 0; i < length; i++)
		{
			Game.Instance.Renderer.ClearColor = pixels[i];

			Game.Instance.Renderer.DrawPixel(x, y);

			x++;

			if (x == Width)
			{
				x = 0;
				y++;
			}
		}

		Game.Instance.Renderer.SetRenderTarget(original);

		Game.Instance.Renderer.ClearColor = originalColor;
	}
}
