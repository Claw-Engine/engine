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

		public int ShapeCount => shapes.Count;
		/// <summary>
		/// Retorna o colisor específico.
		/// </summary>
		public IShape this[int index] => shapes[index];
		private List<IShape> shapes = new List<IShape>();

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
		private float _density, _bounciness;
		private Vector2 impulse;
		internal bool needUpdate = true;
		internal float inverseMass;

		private float previousRotation;
		private Vector2 previousPosition, previousScale;

		public RigidBody(bool instantlyAdd = true) : base(instantlyAdd) { }
		public RigidBody(float density, float bounciness, BodyType type, bool instantlyAdd = true) : base(instantlyAdd)
		{
			Density = density;
			Bounciness = bounciness;
			Type = type;
		}
		public RigidBody(float density, BodyType type, bool instantlyAdd = true) : this(density, 0, type, instantlyAdd) { }
		public RigidBody(BodyType type, bool instantlyAdd = true) : this(.5f, 0, type, instantlyAdd) { }

		/// <summary>
		/// Adiciona um <see cref="CircleShape"/> ao <see cref="Shapes"/>.
		/// </summary>
		/// <returns>Este <see cref="RigidBody"/>.</returns>
		public RigidBody AddCircle(float radius, Vector2 offset)
		{
			if (radius != 0) shapes.Add(new CircleShape(this, radius, offset));

			return this;
		}
		/// <summary>
		/// Adiciona um <see cref="PolygonShape"/> retangular ao <see cref="Shapes"/>.
		/// </summary>
		/// <returns>Este <see cref="RigidBody"/>.</returns>
		public RigidBody AddBox(Rectangle rectangle)
		{
			Vector2 size = rectangle.Size;

			if (size != Vector2.Zero)
			{
				Vector2 start = size * -.5f;

				shapes.Add(new PolygonShape(this, size.X * size.Y, rectangle.Location, new Vector2[4]
				{
					start,
					new Vector2(start.X + size.X, start.Y),
					new Vector2(start.X + size.X, start.Y + size.Y),
					new Vector2(start.X, start.Y + size.Y),
				}));
			}

			return this;
		}
		/// <summary>
		/// Adiciona um <see cref="PolygonShape"/> ao <see cref="Shapes"/>.
		/// </summary>
		/// <returns>Este <see cref="RigidBody"/>.</returns>
		public RigidBody AddPolygon(Vector2 offset, params Vector2[] vertices)
		{
			if (vertices == null || vertices.Length == 0) shapes.Add(new PolygonShape(this, PolygonShape.CalculateArea(vertices), offset, vertices));

			return this;
		}
		/// <summary>
		/// Remove o colisor específico.
		/// </summary>
		/// <returns>Este <see cref="RigidBody"/>.</returns>
		public RigidBody RemoveShape(int index)
		{
			shapes.RemoveAt(index);

			return this;
		}
		/// <summary>
		/// Atualiza o estado de todos os colisores, caso o Transform tenha sofrido alterações.
		/// </summary>
		internal void UpdateShapes()
		{
			if (shapes.Count > 0)
			{
				if (previousRotation != Transform.Rotation || previousPosition != Transform.Position || previousScale != Transform.Scale || needUpdate)
				{
					previousRotation = Transform.Rotation;
					previousPosition = Transform.Position;
					previousScale = Transform.Scale;
					needUpdate = false;

					for (int i = 0; i < shapes.Count; i++)
					{
						shapes[i].Update();

						Mass += shapes[i].Area * Density;
					}

					if (Type == BodyType.Static) inverseMass = 0;
					else inverseMass = 1 / Mass;
				}
			}
			else Mass = 0;
		}

		/// <summary>
		/// Aplica uma força à velocidade do <see cref="RigidBody"/>.
		/// </summary>
		public void Impulse(Vector2 impulse) => this.impulse += impulse;

		public override void Initialize() { }
		public virtual void Step()
		{
			if (Mass > 0)
			{
				Velocity += (impulse * PhysicsManager.Unit / Mass) * Time.DeltaTime;

				if (UseGravity) Velocity += PhysicsManager.Gravity * Time.DeltaTime;
			}
			
			Transform.Position += Velocity * PhysicsManager.Unit * Time.DeltaTime;
			impulse = Vector2.Zero;

			UpdateShapes();
		}
	}
}