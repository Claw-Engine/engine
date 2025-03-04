using Claw.Graphics;

namespace Claw.Modules;

/// <summary>
/// Comportamento para renderização de sprites animadas.
/// </summary>
public class SpriteRenderer : IAnimatable, IBehaviour
{
	public float Opacity = 1;
	public Color Color = Color.White;
	public Vector2 Origin { get; set; } = Vector2.Zero;
	public Sprite Sprite { get; set; }
	public Rectangle? SpriteArea { get; set; }
	public Animator Animator
	{
		get => _animator;
		set
		{
			if (_animator != null && _animator != value) _animator.Animatable = null;

			if (value != null) value.Animatable = this;

			_animator = value;
		}
	}
	public Flip Flip = Flip.None;
	private Animator _animator;

	public void Run(Module module)
	{
		_animator?.Run();

		if (Sprite != null) Draw.Sprite(Sprite, module.Transform.Position, SpriteArea, Color * Opacity, module.Transform.Rotation, Origin, module.Transform.Scale, Flip);
	}
}
