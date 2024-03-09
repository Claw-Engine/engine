using System;

namespace Claw.Physics
{
	/// <summary>
	/// Representa um colisor circular.
	/// </summary>
	public sealed class CircleShape : IShape
	{
		public float Area { get; private set; }
		public float Radius
		{
			get => _radius;
			set
			{
				_radius = value;
				Area = _radius * _radius * Mathf.PI;
				Body.needUpdate = true;
			}
		}
		public Vector2 Offset { get; set; }
		public RigidBody Body { get; }
		private float _radius;
		internal float radiusInWorld;

		public Vector2 Center { get; private set; }
		public Rectangle BoundingBox => _boundingBox;
		private Rectangle _boundingBox;

		internal CircleShape(RigidBody body, float radius, Vector2 offset)
		{
			Body = body;
			Radius = radius;
			Offset = offset;
		}

		public void Update()
		{
			radiusInWorld = Radius * Math.Max(Body.Transform.Scale.X, Body.Transform.Scale.Y);
			Center = Body.Transform.Position + Offset * Body.Transform.Scale;
			_boundingBox.X = Center.X - radiusInWorld;
			_boundingBox.Y = Center.Y - radiusInWorld;
			_boundingBox.Width = radiusInWorld * 2;
			_boundingBox.Height = _boundingBox.Width;
		}
	}
}