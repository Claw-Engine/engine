using System;
using System.IO;

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
			}
		}

		public float Area { get; private set; }
		public float Inertia { get; private set; }
		public Vector2 Offset { get; set; }
		private Vector2[] vertices;
		internal Vector2[] verticesInWorld;

		public Vector2 Center => _center;
		public Rectangle BoundingBox => _boundingBox;
		private Vector2 _center;
		private Rectangle _boundingBox;

		/// <summary>
		/// Cria um polígono.
		/// </summary>
		public PolygonShape(Vector2 offset, params Vector2[] vertices)
		{
			Area = CalculateArea(vertices);
			Offset = offset;
			this.vertices = vertices;
			this.verticesInWorld = new Vector2[vertices.Length];
		}
		/// <summary>
		/// Cria um polígono retangular.
		/// </summary>
		public PolygonShape(Vector2 offset, Vector2 size)
		{
			Area = size.X * size.Y;
			Offset = offset;

			vertices = new Vector2[4]
			{
				Vector2.Zero,
				new Vector2(size.X, 0),
				new Vector2(size.X, size.Y),
				new Vector2(0, size.Y),
			};
			this.verticesInWorld = new Vector2[4];
		}

		public void Update(RigidBody body)
		{
			if (body == null) return;

			float top = body.Transform.Position.Y;
			float bottom = body.Transform.Position.Y;
			float left = body.Transform.Position.X;
			float right = body.Transform.Position.X;
			_center = Vector2.Zero;

			int previous = vertices.Length - 1;
			Inertia = 0;

			for (int i = 0; i < vertices.Length; i++)
			{
				verticesInWorld[i] = Vector2.Rotate((vertices[i] + Offset) * body.Transform.Scale + body.Transform.Position, body.Transform.Position, body.Transform.Rotation);
				_center.X += verticesInWorld[i].X;
				_center.Y += verticesInWorld[i].Y;

				if (top > verticesInWorld[i].Y) top = verticesInWorld[i].Y;

				if (bottom < verticesInWorld[i].Y) bottom = verticesInWorld[i].Y;

				if (left > verticesInWorld[i].X) left = verticesInWorld[i].X;

				if (right < verticesInWorld[i].X) right = verticesInWorld[i].X;

				Vector2 a = vertices[previous], b = vertices[i];
				float massTri = body.Material.Density * .5f * Math.Abs(Vector2.Cross(a, b));
				float inertiaTri = massTri * ((a.X * a.X + a.Y * a.Y) + (b.X * b.X + b.Y * b.Y) + Vector2.Dot(a, b)) / 6;
				Inertia += inertiaTri;
				previous = i;
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