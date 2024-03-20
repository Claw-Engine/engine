using System;

namespace Claw.Modules
{
	/// <summary>
	/// Módulo pronto, para uso geral.
	/// </summary>
	public class GameObject : BaseModule, IStep, IRender
	{
		public int StepOrder
		{
			get => _stepOrder;
			set
			{
				if (_stepOrder != value) StepOrderChanged?.Invoke(this);

				_stepOrder = value;
			}
		}
		public int RenderOrder
		{
			get => _renderOrder;
			set
			{
				if (_renderOrder != value) RenderOrderChanged?.Invoke(this);

				_renderOrder = value;
			}
		}
		private int _stepOrder, _renderOrder;

		public event Action<IStep> StepOrderChanged;
		public event Action<IRender> RenderOrderChanged;

		public SpriteRenderer SpriteRenderer;

		public GameObject(bool instantlyAdd = true) : base(instantlyAdd) { }

		public override void Initialize()
		{
			SpriteRenderer = new SpriteRenderer(false);
			SpriteRenderer.Transform.Parent = Transform;
		}

		public virtual void Step() { }
		public virtual void Render() => SpriteRenderer.Render();
	}
}