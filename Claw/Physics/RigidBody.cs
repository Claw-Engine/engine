using System;
using System.Collections.Generic;
using Claw.Modules;

namespace Claw.Physics
{
	/// <summary>
	/// Representa um corpo rígido.
	/// </summary>
	public class RigidBody : BaseModule, IStep
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
		private int _stepOrder;

		public event Action<IStep> StepOrderChanged;

		public bool UseGravity = true, UseRotation = true;
		public float Density
		{
			get => _density;
			set => _density = Math.Max(value, .5f);
		}
		public float Mass { get; private set; }
		public float Bounciness
		{
			get => _bounciness;
			set => _bounciness = Mathf.Clamp(value, 0, 1);
		}
		public Vector2 Velocity;
		public BodyType Type = BodyType.Normal;
		public IShape Shape;
		private float _density, _bounciness;
		private Vector2 impulse;
		internal float inverseMass;

		private float previousRotation;
		private Vector2 previousPosition, previousScale;

		public RigidBody(bool instantlyAdd = true) : base(instantlyAdd) { }
		public RigidBody(float density, float bounciness, BodyType type, IShape shape, bool instantlyAdd = true) : base(instantlyAdd)
		{
			Density = density;
			Bounciness = bounciness;
			Type = type;
			Shape = shape;
		}
		public RigidBody(float density, BodyType type, IShape shape, bool instantlyAdd = true) : this(density, 0, type, shape, instantlyAdd) { }
		public RigidBody(BodyType type, IShape shape, bool instantlyAdd = true) : this(.5f, 0, type, shape, instantlyAdd) { }

		internal void UpdateShapes()
		{
			if (Shape != null)
			{
				if (previousRotation != Transform.Rotation || previousPosition != Transform.Position || previousScale != Transform.Scale)
				{
					previousRotation = Transform.Rotation;
					previousPosition = Transform.Position;
					previousScale = Transform.Scale;

					Shape.Update(this);

					Mass = Shape.Area * Density;

					if (Type == BodyType.Static) inverseMass = 0;
					else inverseMass = 1 / Mass;
				}
			}
			else Mass = 0;
		}

		public void Impulse(Vector2 impulse) => this.impulse += impulse;

		public override void Initialize() { }
		public virtual void Step()
		{
			if (Mass > 0)
			{
				Velocity += (impulse * Physics.Unit / Mass) * Time.DeltaTime;

				if (UseGravity) Velocity += Physics.Gravity * Time.DeltaTime;
			}
			
			Transform.Position += Velocity * Physics.Unit * Time.DeltaTime;
			impulse = Vector2.Zero;

			UpdateShapes();
		}
	}
}