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
		internal List<Phygrid> grids = new List<Phygrid>(4);

		private float? previousRotation;
		private Vector2? previousPosition, previousScale;
		internal Vector2? previousTopLeft, previousTopRight, previousBottomLeft, previousBottomRight;

		public RigidBody(bool instantlyAdd = true) : base(instantlyAdd) => Material = Material.Default;
		public RigidBody(BodyType type, Material material, IShape shape, bool instantlyAdd = true) : base(instantlyAdd)
		{
			Type = type;
			Material = material;
			Shape = shape;
		}

		internal void UpdateBody()
		{
			if (Shape != null)
			{
				if (previousRotation != Transform.Rotation || previousPosition != Transform.Position || previousScale != Transform.Scale)
				{
					previousRotation = Transform.Rotation;
					previousPosition = Transform.Position;
					previousScale = Transform.Scale;
					Mass = Shape.Area * Material.Density / Math.Max(Transform.Scale.X, Transform.Scale.Y);
					inverseMass = 0;
					inverseInertia = 0;

					Shape.Update(this);

					if (Type != BodyType.Static)
					{
						if (Mass != 0) inverseMass = 1 / Mass;

						if (UseRotation && Shape.Inertia != 0) inverseInertia = 1f / Shape.Inertia / Math.Max(Transform.Scale.X, Transform.Scale.Y);
					}

					if (Game.Physics != null)
					{
						Game.Physics.needStep = true;

						Game.Physics.UpdateGrid(this);
					}
				}
			}
			else
			{
				Mass = 0;

				Game.Physics?.RemoveFromGrid(this);
			}
		}

		/// <summary>
		/// Aplica uma força à velocidade do <see cref="RigidBody"/>.
		/// </summary>
		public void Impulse(Vector2 impulse)
		{
			if (Mass > 0) MoveSpeed += impulse / (Mass * Math.Max(Transform.Scale.X, Transform.Scale.Y)) * PhysicsManager.Unit;
		}

		/// <summary>
		/// Evento executado quando corpos do tipo <see cref="BodyType.Normal"/> entram em um <see cref="BodyType.Trigger"/>.
		/// </summary>
		/// <remarks>
		/// Este evento é executado nos dois corpos.
		/// </remarks>
		public virtual void Triggering(CollisionResult collision) { }
		/// <summary>
		/// Evento executado quando corpos do tipo <see cref="BodyType.Normal"/> colidem com <see cref="BodyType.Static"/>.
		/// </summary>
		/// <remarks>
		/// Este evento é executado nos dois corpos.
		/// </remarks>
		/// <returns>Se verdadeiro, o sistema deve responder à colisão.</returns>
		public virtual bool Colliding(CollisionResult collision) => true;

		public override void Initialize() { }
		public virtual void Step()
		{
			MoveSpeed += PhysicsManager.Gravity * GravityScale * Time.DeltaTime;

			Transform.Position += MoveSpeed * PhysicsManager.Unit * Time.DeltaTime;

			if (UseRotation) Transform.Rotation += Mathf.ToDegrees(RotateSpeed * PhysicsManager.Unit) * Time.DeltaTime;

			UpdateBody();
		}
	}
}