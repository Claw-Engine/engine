using System;

namespace Claw.Physics
{
	/// <summary>
	/// Representa um colisor circular.
	/// </summary>
	public sealed class CircleShape : IShape
	{
		public float Area { get; }
		public float Radius { get; }
		public Vector2 Offset { get; }
		internal float radiusInWorld;
		internal Vector2 centerInWorld;

		public CircleShape(float radius, Vector2 offset)
		{
			Area = radius * radius * Mathf.PI;
			Radius = radius;
			Offset = offset;
		}
		public CircleShape(float radius) : this(radius, Vector2.Zero) { }

		public void Update(RigidBody body)
		{
			radiusInWorld = Radius * Math.Max(body.Transform.Scale.X, body.Transform.Scale.Y);
			centerInWorld = body.Transform.Position + Offset * body.Transform.Scale;
		}
	}
}