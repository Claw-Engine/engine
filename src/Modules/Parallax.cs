using Claw.Graphics;

namespace Claw.Modules;

/// <summary>
/// Representa uma sequência de backgrounds para efeito de parallax.
/// </summary>
public sealed class Parallax : Module
{
	/// <summary>
	/// Define os eixos em que o background poderá se repetir durante o parallax.
	/// </summary>
	public enum Axis { Horizontal, Vertical }

	public bool UseDeltaTime = true, UseScaledDeltaTime = true;
	public Color Color = Color.White;
	public List<Background> Backgrounds = new List<Background>();

	public override void Initialize(){}
	public override void Step(){}

	/// <summary>
	/// Muda a velocidade de todos os backgrounds.
	/// </summary>
	public void ChangeAllSpeed(float value)
	{
		foreach (Background back in Backgrounds) back.Speed = value;
	}
	/// <summary>
	/// Muda o zoom de todos os backgrounds.
	/// </summary>
	public void ChangeAllZoom(float value)
	{
		foreach (Background back in Backgrounds) back.Zoom = value;
	}
	/// <summary>
	/// Muda a direção de todos os backgrounds.
	/// </summary>
	public void ChangeAllDirection(Vector2 value)
	{
		foreach (Background back in Backgrounds) back.Direction = value;
	}

	public override void Render()
	{
		foreach (Background background in Backgrounds) background.Run(this);
	}
}
/// <summary>
/// Representa um background do <see cref="Parallax"/>.
/// </summary>
public sealed class Background
{
	public float Zoom = 1, Speed = 0;
	public Sprite Sprite;
	public Vector2 Direction = Vector2.Zero, ParallaxEffect = Vector2.Zero;
	public Parallax.Axis Axis = Parallax.Axis.Horizontal;
	private Vector2 offset = Vector2.Zero;

	/// <summary>
	/// Desenha o background.
	/// </summary>
	private void DrawBackground(Parallax parallax, CameraState camera)
	{
		Vector2 basePos = (parallax.Transform.Position + camera.Position + camera.Origin) / camera.Zoom;

		if (offset != Vector2.Zero)
		{
			switch (Axis)
			{
				case Parallax.Axis.Horizontal: DrawScroll(Sprite, basePos + new Vector2(0, offset.Y), new Rectangle((int)offset.X, 0, Sprite.Width, Sprite.Height), parallax.Color, Zoom); break;
				case Parallax.Axis.Vertical: DrawScroll(Sprite, basePos + new Vector2(offset.X, 0), new Rectangle(0, (int)offset.Y, Sprite.Width, Sprite.Height), parallax.Color, Zoom); break;
			}
		}
		else Draw.Sprite(Sprite, basePos, null, parallax.Color, 0, Vector2.Zero, Zoom, 0);
	}
	/// <summary>
	/// Desenha o parallax, repetindo a textura na horizontal ou vertical.
	/// </summary>
	private void DrawScroll(Sprite sprite, Vector2 position, Rectangle area, Color color, float scale)
	{
		position = new Vector2((int)position.X, (int)position.Y);
		Rectangle first = area;
		first.Location = new Vector2(Mathf.LoopValue(area.X, sprite.Width), Mathf.LoopValue(area.Y, sprite.Height));
		first.Size -= first.Location;

		Draw.Sprite(sprite, position, first, color, 0, Vector2.Zero, scale, 0);

		Vector2 secondPos = Vector2.Zero;
		Rectangle secondRect = new Rectangle();

		switch (Axis)
		{
			case Parallax.Axis.Horizontal:
				secondPos = position + new Vector2((float)Math.Round(first.Width * scale), 0);
				secondRect.Size = new Vector2(area.Width - first.Width, area.Height);
				break;
			case Parallax.Axis.Vertical:
				secondPos = position + new Vector2(0, (float)Math.Round(first.Height * scale));
				secondRect.Size = new Vector2(area.Width, area.Height - first.Height);
				break;
		}

		Draw.Sprite(sprite, secondPos, secondRect, color, 0, Vector2.Zero, scale, 0);
	}

	internal void Run(Parallax parallax)
	{
		if (Sprite == null) return;

		CameraState camera = Draw.GetCamera()?.State ?? CameraState.Neutral;

		if (Direction != Vector2.Zero && Speed != 0)
		{
			float d = 1;

			if (parallax.UseDeltaTime) d = parallax.UseScaledDeltaTime ? Time.DeltaTime : Time.UnscaledDeltaTime;

			offset += Direction * (Speed * d);
		}
		else offset = camera.Position * ParallaxEffect;

		DrawBackground(parallax, camera);
	}
}
