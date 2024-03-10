using System;

namespace Claw.Physics
{
	/// <summary>
	/// Realiza as checagens de colisão.
	/// </summary>
	public static class CollisionChecker
	{
		/// <summary>
		/// Checa se dois corpos estão se sobrepondo.
		/// </summary>
		/// <param name="depth">A profundidade da sobreposição.</param>
		/// <param name="direction">A direção em que a sobreposição está acontecendo.</param>
		public static bool Intersects(RigidBody a, RigidBody b, out float depth, out Vector2 direction, out IShape aShape, out IShape bShape)
		{
			for (int aIndex = 0; aIndex < a.ShapeCount; aIndex++)
			{
				for (int bIndex = 0; bIndex < b.ShapeCount; bIndex++)
				{
					aShape = a[aIndex];
					bShape = b[bIndex];

					if (Intersects(aShape, bShape, out depth, out direction)) return true;
				}
			}

			depth = 0;
			direction = Vector2.Zero;
			aShape = null;
			bShape = null;

			return false;
		}
		/// <summary>
		/// Checa se dois colisores estão se sobrepondo.
		/// </summary>
		/// <param name="depth">A profundidade da sobreposição.</param>
		/// <param name="direction">A direção em que a sobreposição está acontecendo.</param>
		public static bool Intersects(IShape a, IShape b, out float depth, out Vector2 direction)
		{
			if (a is CircleShape aCircle)
			{
				if (b is CircleShape bCircle) return Intersects(aCircle.radiusInWorld, aCircle.Center, bCircle.radiusInWorld, bCircle.Center, out depth, out direction);
				else if (b is PolygonShape bPolygon)
				{
					bool result = Intersects(aCircle.radiusInWorld, aCircle.Center, bPolygon.Center, bPolygon.verticesInWorld, out depth, out direction);
					direction = -direction;

					return result;
				}
			}
			else if (a is PolygonShape aPolygon)
			{
				if (b is PolygonShape bPolygon)
				{
					bool result = Intersects(aPolygon.verticesInWorld, bPolygon.verticesInWorld, out depth, out direction);
					
					if (result && Vector2.Dot(aPolygon.Center - bPolygon.Center, direction) < 0) direction = -direction;

					return result;
				}
				else if (b is CircleShape bCircle) return Intersects(bCircle.radiusInWorld, bCircle.Center, aPolygon.Center, aPolygon.verticesInWorld, out depth, out direction);
			}

			depth = 0;
			direction = Vector2.Zero;

			return false;
		}

		/// <summary>
		/// Checa se dois corpos estão se sobrepondo.
		/// </summary>
		public static CollisionResult Intersects(RigidBody a, RigidBody b)
		{
			bool intersects = Intersects(a, b, out float depth, out Vector2 direction, out IShape aShape, out IShape bShape);

			if (!intersects)
			{
				depth = 0;
				direction = Vector2.Zero;
				aShape = null;
				bShape = null;
			}

			return new CollisionResult(intersects, depth, direction, aShape, bShape);
		}
		/// <summary>
		/// Checa se dois colisores estão se sobrepondo.
		/// </summary>
		public static CollisionResult Intersect(IShape a, IShape b)
		{
			bool intersects = Intersects(a, b, out float depth, out Vector2 direction);

			return new CollisionResult(intersects, depth, direction, a, b);
		}

		private static bool Intersects(float aRadius, Vector2 aCenter, float bRadius, Vector2 bCenter, out float depth, out Vector2 direction)
		{
			float radii = aRadius + bRadius;
			Vector2 difference = aCenter - bCenter;
			float distance = Vector2.Length(difference);

			if (distance < radii)
			{
				depth = radii - distance;
				direction = Vector2.Normalize(difference);

				return true;
			}

			depth = 0;
			direction = Vector2.Zero;

			return false;
		}
		private static bool Intersects(Vector2[] aVertices, Vector2[] bVertices, out float depth, out Vector2 direction)
		{
			depth = float.MaxValue;
			direction = Vector2.Zero;

			return TestPolygonPolygon(aVertices, aVertices, bVertices, ref depth, ref direction) && TestPolygonPolygon(bVertices, aVertices, bVertices, ref depth, ref direction);
		}
		private static bool Intersects(float radius, Vector2 center, Vector2 polygonCenter, Vector2[] vertices, out float depth, out Vector2 direction)
		{
			depth = float.MaxValue;
			direction = Vector2.Zero;

			float minDistance = float.MaxValue;
			Vector2 closestVertice =Vector2.Zero;
			float axisDepth;
			float aMin, aMax, bMin, bMax;
			Vector2 axis;

			for (int i = 0; i < vertices.Length; i++)
			{
				float distance = Vector2.Distance(center, vertices[i]);

				if (minDistance > distance)
				{
					distance = minDistance;
					closestVertice = vertices[i];
				}

				Vector2 vA = vertices[i], vB = vertices[(i + 1) % vertices.Length];
				Vector2 edge = vB - vA;
				axis = new Vector2(-edge.Y, edge.X);

				axis.Normalize();
				ProjectCircle(radius, center, axis, out aMin, out aMax);
				ProjectVertices(vertices, axis, out bMin, out bMax);

				if (aMin >= bMax || bMin >= aMax) return false;

				axisDepth = Math.Min(aMax - bMin, bMax - aMin);

				if (axisDepth < depth)
				{
					depth = axisDepth;
					direction = axis;
				}
			}

			axis = closestVertice - center;

			axis.Normalize();
			ProjectCircle(radius, center, axis, out aMin, out aMax);
			ProjectVertices(vertices, axis, out bMin, out bMax);

			if (aMin >= bMax || bMin >= aMax) return false;

			axisDepth = Math.Min(aMax - bMin, bMax - aMin);

			if (axisDepth < depth)
			{
				depth = axisDepth;
				direction = axis;
			}

			if (Vector2.Dot(polygonCenter - center, direction) < 0) direction = -direction;

			return true;
		}

		private static bool TestPolygonPolygon(Vector2[] checkVertices, Vector2[] aVertices, Vector2[] bVertices, ref float depth, ref Vector2 direction)
		{
			for (int i = 0; i < checkVertices.Length; i++)
			{
				Vector2 vA = checkVertices[i], vB = checkVertices[(i + 1) % checkVertices.Length];
				Vector2 edge = vB - vA;
				Vector2 axis = new Vector2(-edge.Y, edge.X);

				axis.Normalize();
				ProjectVertices(aVertices, axis, out float aMin, out float aMax);
				ProjectVertices(bVertices, axis, out float bMin, out float bMax);
				
				if (aMin >= bMax || bMin >= aMax) return false;
				
				float axisDepth = Math.Min(aMax - bMin, bMax - aMin);

				if (axisDepth < depth)
				{
					depth = axisDepth;
					direction = axis;
				}
			}
			
			return true;
		}
		private static void ProjectVertices(this Vector2[] vertices, Vector2 axis, out float min, out float max)
		{
			min = float.MaxValue;
			max = float.MinValue;

			for (int i = 0; i < vertices.Length; i++) ProjectPoint(vertices[i], ref axis, ref min, ref max);
		}
		private static void ProjectCircle(float radius, Vector2 center, Vector2 axis, out float min, out float max)
		{
			min = float.MaxValue;
			max = float.MinValue;
			Vector2 directionRadius = Vector2.Normalize(axis) * radius;

			ProjectPoint(center + directionRadius, ref axis, ref min, ref max);
			ProjectPoint(center - directionRadius, ref axis, ref min, ref max);

			if (min > max)
			{
				float temp = min;
				min = max;
				max = min;
			}
		}
		private static void ProjectPoint(Vector2 point, ref Vector2 axis, ref float min, ref float max)
		{
			float projection = Vector2.Dot(point, axis);

			if (projection < min) min = projection;

			if (projection > max) max = projection;
		}
	}
}