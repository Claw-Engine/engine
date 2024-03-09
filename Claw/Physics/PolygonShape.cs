using System;
using System.Reflection;

namespace Claw.Physics
{
	/// <summary>
	/// Base de um colisor poligonal.
	/// </summary>
	public sealed class PolygonShape : IShape
	{
		public int VerticeCount => vertices.Length;
		/// <summary>
		/// Retorna o vértice específico.
		/// </summary>
		public Vector2 this[int index, bool inWorld = false]
		{
			get
			{
				if (inWorld) return verticesInWorld[index];
				
				return vertices[index];
			}
		}

		public float Area { get; }
		public Vector2 Offset { get; }
		private Vector2[] vertices;
		internal Vector2[] verticesInWorld;

		public float Top { get; private set; }
		public float Bottom { get; private set; }
		public float Left { get; private set; }
		public float Right { get; private set; }
		public Vector2 Center { get; private set; }
		public Vector2 ArithmeticMean => _arithmeticMean;
		public Rectangle BoundingBox => _boundingBox;
		private Vector2 _arithmeticMean;
		private Rectangle _boundingBox;

		private PolygonShape(float area, Vector2 offset, params Vector2[] vertices)
		{
			Area = area;
			Offset = offset;
			this.vertices = vertices;
			verticesInWorld = new Vector2[vertices.Length];
		}

		public static PolygonShape CreateBox(Vector2 size, Vector2 offset)
		{
			Vector2 start = size * -.5f + offset;

			PolygonShape result = new PolygonShape(size.X * size.Y, offset, new Vector2[4]
			{
				start,
				new Vector2(start.X + size.X, start.Y),
				new Vector2(start.X + size.X, start.Y + size.Y),
				new Vector2(start.X, start.Y + size.Y),
			});

			return result;
		}
		public static PolygonShape CreatePolygon(Vector2 offset, params Vector2[] vertices)
		{
			if (vertices == null) return null;

			double area = 0;
			int j = vertices.Length - 1;

			for (int i = 0; i < vertices.Length; i++)
			{
				area += (vertices[j].X + vertices[i].X) * (vertices[j].Y - vertices[i].Y);
				j = i;
			}

			return new PolygonShape((float)(area * .5f), offset, vertices);
		}

		public void Update(RigidBody body)
		{
			Center = body.Transform.Position;
			Top = body.Transform.Position.Y;
			Bottom = body.Transform.Position.Y;
			Left = body.Transform.Position.X;
			Right = body.Transform.Position.X;
			_arithmeticMean = Vector2.Zero;

			if (vertices == null)
			{
				_boundingBox.X = body.Transform.Position.X;
				_boundingBox.Y = body.Transform.Position.Y;
				_boundingBox.Width = 0;
				_boundingBox.Height = 0;

				return;
			}

			for (int i = 0; i < vertices.Length; i++)
			{
				verticesInWorld[i] = Vector2.Rotate((vertices[i] + Offset) * body.Transform.Scale + body.Transform.Position, body.Transform.Position, body.Transform.Rotation);
				_arithmeticMean.X += verticesInWorld[i].X;
				_arithmeticMean.Y += verticesInWorld[i].Y;

				if (Top > verticesInWorld[i].Y) Top = verticesInWorld[i].Y;

				if (Bottom < verticesInWorld[i].Y) Bottom = verticesInWorld[i].Y;

				if (Left > verticesInWorld[i].X) Left = verticesInWorld[i].X;

				if (Right < verticesInWorld[i].X) Right = verticesInWorld[i].X;
			}

			Center = new Vector2(Left + Math.Abs(Right - Left) * .5f, Top + Math.Abs(Bottom - Top) * .5f);

			_boundingBox.X = Left;
			_boundingBox.Y = Top;
			_boundingBox.Width = Right - Left;
			_boundingBox.Height = Bottom - Top;
			_arithmeticMean /= vertices.Length;
		}
	}
}