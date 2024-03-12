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

		public bool UseRotation = true;
		public float GravityScale = 1;
		public float Mass { get; private set; }
		public float RotateSpeed;
		public Vector2 MoveSpeed;
		public BodyType Type = BodyType.Normal;
		public Material Material;
		public IShape Shape;
		internal float inverseMass, inverseInertia;

		private float previousRotation;
		private Vector2 previousPosition, previousScale;

		public RigidBody(bool instantlyAdd = true) : base(instantlyAdd) => Material = Material.Default;
		public RigidBody(BodyType type, Material material, IShape shape, bool instantlyAdd = true) : base(instantlyAdd)
		{
			Type = type;
			Material = material;
			Shape = shape;
		}

		/// <summary>
		/// Atualiza o estado do colisor, caso o Transform tenha sofrido alterações.
		/// </summary>
		internal void UpdateShape()
		{
			if (Shape != null)
			{
				if (previousRotation != Transform.Rotation || previousPosition != Transform.Position || previousScale != Transform.Scale)
				{
					previousRotation = Transform.Rotation;
					previousPosition = Transform.Position;
					previousScale = Transform.Scale;
					PhysicsManager.needStep = true;
					Mass = Shape.Area * Material.Density;
					inverseMass = 0;
					inverseInertia = 0;

					Shape.Update(this);

					if (Type != BodyType.Static)
					{
						if (Mass != 0) inverseMass = 1 / Mass;

						if (UseRotation && Shape.Inertia != 0) inverseInertia = 1f / Shape.Inertia;
					}
				}
			}
			else Mass = 0;
		}

		/// <summary>
		/// Aplica uma força à velocidade do <see cref="RigidBody"/>.
		/// </summary>
		public void Impulse(Vector2 impulse)
		{
			if (Mass > 0) MoveSpeed += (impulse / Mass * PhysicsManager.Unit);
		}

		/// <summary>
		/// Evento executado quando corpos do tipo <see cref="BodyType.Normal"/> ou <see cref="BodyType.Static"/> entram em um <see cref="BodyType.Trigger"/>.
		/// </summary>
		public virtual void Triggering(CollisionResult collision) { }
		/// <summary>
		/// Evento executado quando corpos do tipo <see cref="BodyType.Normal"/> colidem com corpos que NÃO sejam <see cref="BodyType.Trigger"/>.
		/// </summary>
		/// <returns>Se verdadeiro, o sistema deve responder à colisão.</returns>
		public virtual bool Colliding(CollisionResult collisionResult) => true;

		public override void Initialize() { }
		public virtual void Step()
		{
			MoveSpeed += PhysicsManager.Gravity * GravityScale * Time.DeltaTime;
			Transform.Position += MoveSpeed * PhysicsManager.Unit * Time.DeltaTime;
			Transform.Rotation += Mathf.ToDegrees(RotateSpeed * PhysicsManager.Unit) * Time.DeltaTime;

			UpdateShape();
		}
	}
}