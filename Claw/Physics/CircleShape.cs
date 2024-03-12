using System;

namespace Claw.Physics
{
	/// <summary>
	/// Representa um colisor circular.
	/// </summary>
	public sealed class CircleShape : IShape
	{
		public float Area { get; private set; }
		public float Inertia { get; private set; }
		public float Radius
		{
			get => _radius;
			set
			{
				_radius = value;
				Area = _radius * _radius * Mathf.PI;
			}
		}
		public Vector2 Offset { get; set; }
		private float _radius;
		internal float radiusInWorld;

		public Vector2 Center { get; private set; }
		public Rectangle BoundingBox => _boundingBox;
		private Rectangle _boundingBox;

		public CircleShape(float radius, Vector2 offset)
		{
			Radius = radius;
			Offset = offset;
		}

		public void Update(RigidBody body)
		{
			if (body == null) return;

			radiusInWorld = Radius * Math.Max(body.Transform.Scale.X, body.Transform.Scale.Y);
			Inertia = (.5f * body.Mass * Radius * Radius);
			Center = body.Transform.Position + Offset * body.Transform.Scale;
			_boundingBox.X = Center.X - radiusInWorld;
			_boundingBox.Y = Center.Y - radiusInWorld;
			_boundingBox.Width = radiusInWorld * 2;
			_boundingBox.Height = _boundingBox.Width;
		}
	}
}