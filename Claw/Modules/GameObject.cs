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
			get => _StepOrder;
			set
			{
				if (_StepOrder != value) StepOrderChanged?.Invoke(this);

				_StepOrder = value;
			}
		}
		public int RenderOrder
		{
			get => _RenderOrder;
			set
			{
				if (_RenderOrder != value) RenderOrderChanged?.Invoke(this);

				_RenderOrder = value;
			}
		}
		private int _StepOrder, _RenderOrder;

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