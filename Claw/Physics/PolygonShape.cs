using System;

namespace Claw.Physics
{
	/// <summary>
	/// Base de um colisor poligonal.
	/// </summary>
	public sealed class PolygonShape : IShape
	{
		public int VerticeCount => vertices.Length;
		/// <summary>
		/// Retorna/altera o vértice específico.
		/// </summary>
		public Vector2 this[int index]
		{
			get => vertices[index];
			set
			{
				vertices[index] = value;
				Area = CalculateArea(vertices);
				Body.needUpdate = true;
			}
		}

		public float Area { get; private set; }
		public Vector2 Offset { get; set; }
		public RigidBody Body { get; }
		private Vector2[] vertices;
		internal Vector2[] verticesInWorld;

		public Vector2 Center => _center;
		public Rectangle BoundingBox => _boundingBox;
		private Vector2 _center;
		private Rectangle _boundingBox;

		internal PolygonShape(RigidBody body, float area, Vector2 offset, params Vector2[] vertices)
		{
			Body = body;
			Area = area;
			Offset = offset;
			this.vertices = vertices;
			verticesInWorld = new Vector2[vertices.Length];
		}

		public void Update()
		{
			float top = Body.Transform.Position.Y;
			float bottom = Body.Transform.Position.Y;
			float left = Body.Transform.Position.X;
			float right = Body.Transform.Position.X;
			_center = Vector2.Zero;

			for (int i = 0; i < vertices.Length; i++)
			{
				verticesInWorld[i] = Vector2.Rotate((vertices[i] + Offset) * Body.Transform.Scale + Body.Transform.Position, Body.Transform.Position, Body.Transform.Rotation);
				_center.X += verticesInWorld[i].X;
				_center.Y += verticesInWorld[i].Y;

				if (top > verticesInWorld[i].Y) top = verticesInWorld[i].Y;

				if (bottom < verticesInWorld[i].Y) bottom = verticesInWorld[i].Y;

				if (left > verticesInWorld[i].X) left = verticesInWorld[i].X;

				if (right < verticesInWorld[i].X) right = verticesInWorld[i].X;
			}

			_boundingBox.X = left;
			_boundingBox.Y = top;
			_boundingBox.Width = right - left;
			_boundingBox.Height = bottom - top;
			_center /= vertices.Length;
		}

		internal static float CalculateArea(Vector2[] vertices)
		{
			double area = 0;
			int j = vertices.Length - 1;

			for (int i = 0; i < vertices.Length; i++)
			{
				area += (vertices[j].X + vertices[i].X) * (vertices[j].Y - vertices[i].Y);
				j = i;
			}

			return (float)(area * .5d);
		}
	}
}