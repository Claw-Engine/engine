using System;
using Claw.Graphics;

namespace Claw.Modules
{
	/// <summary>
	/// Módulo para renderização de sprites animadas.
	/// </summary>
	public class SpriteRenderer : BaseModule, IAnimatable, IRender
	{
		public int RenderOrder
		{
			get => _RenderOrder;
			set
			{
				if (_RenderOrder != value)
				{
					_RenderOrder = value;

					RenderOrderChanged?.Invoke(this);
				}
			}
		}
		private int _RenderOrder;
		public event Action<IRender> RenderOrderChanged;

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

		public SpriteRenderer(bool instantlyAdd = true) : base(instantlyAdd) { }

		public override void Initialize() { }

		public void Render()
		{
			Animator?.Step();

			if (Sprite != null) Draw.Sprite(Sprite, Transform.Position, SpriteArea, Color * Opacity, Transform.Rotation, Origin, Transform.Scale, Flip);
		}
	}
}