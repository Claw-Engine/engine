using System;

namespace Claw.Physics
{
	/// <summary>
	/// Descreve os dados de uma checagem de colisão.
	/// </summary>
	public sealed class CollisionResult
	{
		public bool Intersects;
		/// <summary>
		/// Número de pontos em que houve a colisão.
		/// </summary>
		public int CollisionPoints;
		/// <summary>
		/// Profundidade da sobreposição.
		/// </summary>
		public float Depth;
		/// <summary>
		/// Direção da sobreposição.
		/// </summary>
		public Vector2 Direction;
		/// <summary>
		/// Ponto em que houve a colisão.
		/// </summary>
		public Vector2? CollisionPoint1, CollisionPoint2;
		public RigidBody Body, OtherBody;

		/// <summary>
		/// Se o <paramref name="index"/> for igual a zero, retorna <see cref="CollisionPoint1"/>. Senão, retorna <see cref="CollisionPoint2"/>.
		/// </summary>
		public Vector2 this[int index]
		{
			get
			{
				if (index == 0) return CollisionPoint1.Value;

				return CollisionPoint2.Value;
			}
		}

		/// <summary>
		/// Cria um <see cref="CollisionResult"/> em que não houve colisão.
		/// </summary>
		public CollisionResult() => Intersects = false;
		/// <param name="depth">A profundidade da sobreposição.</param>
		/// <param name="direction">A direção em que a sobreposição está acontecendo.</param>
		public CollisionResult(bool intersects, float depth, Vector2 direction, RigidBody body, RigidBody otherBody)
		{
			Intersects = intersects;
			Depth = depth;
			Direction = direction;
			Body = body;
			OtherBody = otherBody;
		}
		/// <param name="depth">A profundidade da sobreposição.</param>
		/// <param name="direction">A direção em que a sobreposição está acontecendo.</param>
		public CollisionResult(bool intersects, int collisionPoints, float depth, Vector2 direction, Vector2? collisionPoint1, Vector2? collisionPoint2,
			RigidBody body, RigidBody otherBody) : this(intersects, depth, direction, body, otherBody)
		{
			CollisionPoints = collisionPoints;
			CollisionPoint1 = collisionPoint1;
			CollisionPoint2 = collisionPoint2;
		}

		/// <summary>
		/// Clona este resultado.
		/// </summary>
		/// <remarks>
		/// Internamente, o PhysicsManager usa sempre o mesmo <see cref="CollisionResult"/>.
		/// </remarks>
		public CollisionResult Clone() => new CollisionResult(Intersects, CollisionPoints, Depth, Direction, CollisionPoint1, CollisionPoint2, Body, OtherBody);
		/// <summary>
		/// Copia todos os campos de um outro <see cref="CollisionResult"/>.
		/// </summary>
		/// <remarks>
		/// Internamente, o PhysicsManager usa sempre o mesmo <see cref="CollisionResult"/>.
		/// </remarks>
		public void Copy(CollisionResult other)
		{
			Intersects = other.Intersects;
			CollisionPoints = other.CollisionPoints;
			Depth = other.Depth;
			Direction = other.Direction;
			CollisionPoint1 = other.CollisionPoint1;
			CollisionPoint2 = other.CollisionPoint2;
			Body = other.Body;
			OtherBody = other.OtherBody;
		}
		/// <summary>
		/// Reseta as propriedades do resultado.
		/// </summary>
		internal void Reset()
		{
			Intersects = false;
			CollisionPoints = 0;
			Depth = 0;
			CollisionPoint1 = null;
			CollisionPoint2 = null;
			Body = null;
			OtherBody = null;
		}

		public static implicit operator bool(CollisionResult value) => value.Intersects;
	}
}