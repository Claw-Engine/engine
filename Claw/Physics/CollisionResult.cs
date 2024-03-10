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
		/// Profundidade da sobreposição.
		/// </summary>
		public float Depth;
		/// <summary>
		/// Direção da sobreposição.
		/// </summary>
		public Vector2 Direction;
		public IShape Shape, OtherShape;

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

		public static implicit operator bool(CollisionResult value) => value.Intersects;
	}
}