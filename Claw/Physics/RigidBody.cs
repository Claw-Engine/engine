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
		public float Mass { get; private set; }
		public float RotateSpeed;
		public Vector2 MoveSpeed;
		public BodyType Type = BodyType.Normal;
		public Material Material;
		private int previousCount;
		internal bool needUpdate;
		internal float inverseMass, inverseInertia;

		private float previousRotation;
		private Vector2 previousPosition, previousScale;

		public RigidBody(bool instantlyAdd = true) : base(instantlyAdd) => Material = Material.Default;
		public RigidBody(BodyType type, Material material, bool instantlyAdd = true) : base(instantlyAdd)
		{
			Type = type;
			Material = material;
		}

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
				if (previousCount != shapes.Count || previousRotation != Transform.Rotation || previousPosition != Transform.Position || previousScale != Transform.Scale || needUpdate)
				{
					previousCount = shapes.Count;
					previousRotation = Transform.Rotation;
					previousPosition = Transform.Position;
					previousScale = Transform.Scale;
					needUpdate = false;
					PhysicsManager.needStep = true;
					Mass = 0;
					inverseMass = 0;
					inverseInertia = 0;
					Vector2 min = shapes[0].BoundingBox.Location, max = min + shapes[0].BoundingBox.Size;

					for (int i = 0; i < shapes.Count; i++)
					{
						shapes[i].Update();

						Mass += shapes[i].Area * Material.Density;
						
						if (i > 0)
						{
							min.X = Math.Min(min.X, shapes[i].BoundingBox.X);
							min.Y = Math.Min(min.Y, shapes[i].BoundingBox.Y);
							max.X = Math.Max(max.X, shapes[i].BoundingBox.X + shapes[i].BoundingBox.Width);
							max.Y = Math.Max(max.Y, shapes[i].BoundingBox.Y + shapes[i].BoundingBox.Height);
						}
					}

					float radius = Math.Max(max.X - min.X, max.Y - min.Y) * .5f;

					if (Type != BodyType.Static)
					{
						inverseMass = 1 / Mass;

						if (UseRotation) inverseInertia = 1f / (1f / 12 * Mass * radius * radius);
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
			if (UseGravity) MoveSpeed += PhysicsManager.Gravity * Time.DeltaTime;

			Transform.Position += MoveSpeed * PhysicsManager.Unit * Time.DeltaTime;
			Transform.Rotation += RotateSpeed * PhysicsManager.Unit * Time.DeltaTime;

			UpdateShapes();
		}
	}
}