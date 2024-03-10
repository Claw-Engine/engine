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
		public IShape Shape, OtherShape;

		/// <summary>
		/// Cria um <see cref="CollisionResult"/> em que não houve colisão.
		/// </summary>
		public CollisionResult() => Intersects = false;
		/// <param name="depth">A profundidade da sobreposição.</param>
		/// <param name="direction">A direção em que a sobreposição está acontecendo.</param>
		public CollisionResult(bool intersects, float depth, Vector2 direction, IShape shape, IShape otherShape)
		{
			Intersects = intersects;
			Depth = depth;
			Direction = direction;
			Shape = shape;
			OtherShape = otherShape;
		}
		/// <param name="depth">A profundidade da sobreposição.</param>
		/// <param name="direction">A direção em que a sobreposição está acontecendo.</param>
		public CollisionResult(bool intersects, int collisionPoints, float depth, Vector2 direction, Vector2? collisionPoint1, Vector2? collisionPoint2,
			IShape shape, IShape otherShape) : this(intersects, depth, direction, shape, otherShape)
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
		public CollisionResult Clone() => new CollisionResult(Intersects, CollisionPoints, Depth, Direction, CollisionPoint1, CollisionPoint2, Shape, OtherShape);
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
			Shape = null;
			OtherShape = null;
		}
		/// <summary>
		/// Troca <see cref="Shape"/> por <see cref="OtherShape"/> e vice-versa.
		/// </summary>
		internal void RevertShapes()
		{
			IShape temp = Shape;
			Shape = OtherShape;
			OtherShape = temp;
		}

		public static implicit operator bool(CollisionResult value) => value.Intersects;
	}
}