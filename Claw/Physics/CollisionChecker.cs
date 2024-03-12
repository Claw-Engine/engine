using System;
using System.IO;

namespace Claw.Physics
{
	/// <summary>
	/// Realiza as checagens de colisão.
	/// </summary>
	public static class CollisionChecker
	{
		private const float CompareTolerance = .0005f;

		/// <summary>
		/// Checa se dois corpos estão se sobrepondo.
		/// </summary>
		/// <param name="depth">A profundidade da sobreposição.</param>
		/// <param name="direction">A direção em que a sobreposição está acontecendo.</param>
		public static CollisionResult Intersects(RigidBody a, RigidBody b)
		{
			CollisionResult result = new CollisionResult();

			Intersects(a, b, result);

			return result;
		}
		internal static void Intersects(RigidBody a, RigidBody b, CollisionResult result)
		{
			result.Body = a;
			result.OtherBody = b;

			if (!a.Shape.BoundingBox.Intersects(b.Shape.BoundingBox) && !b.Shape.BoundingBox.Intersects(a.Shape.BoundingBox)) return;

			if (a.Shape is CircleShape aCircle)
			{
				if (b.Shape is CircleShape bCircle) Intersects(aCircle.radiusInWorld, aCircle.Center, bCircle.radiusInWorld, bCircle.Center, result);
				else if (b.Shape is PolygonShape bPolygon)
				{
					Intersects(aCircle.radiusInWorld, aCircle.Center, bPolygon.Center, bPolygon.verticesInWorld, result);

					result.Direction = -result.Direction;
				}
			}
			else if (a.Shape is PolygonShape aPolygon)
			{
				if (b.Shape is PolygonShape bPolygon)
				{
					Intersects(aPolygon.verticesInWorld, bPolygon.verticesInWorld, result);

					if (result && Vector2.Dot(aPolygon.Center - bPolygon.Center, result.Direction) < 0) result.Direction = -result.Direction;
				}
				else if (b.Shape is CircleShape bCircle) Intersects(bCircle.radiusInWorld, bCircle.Center, aPolygon.Center, aPolygon.verticesInWorld, result);
			}
		}

		private static void Intersects(float aRadius, Vector2 aCenter, float bRadius, Vector2 bCenter, CollisionResult result)
		{
			float radii = aRadius + bRadius;
			Vector2 difference = aCenter - bCenter;
			float distance = Vector2.Length(difference);

			if (distance < radii)
			{
				result.Depth = radii - distance;
				result.Direction = Vector2.Normalize(difference);
				result.CollisionPoints = 1;
				result.CollisionPoint1 = bCenter + result.Direction * bRadius;
				result.Intersects = true;
			}
		}
		private static void Intersects(Vector2[] aVertices, Vector2[] bVertices, CollisionResult result)
		{
			result.Depth = float.MaxValue;
			result.Direction = Vector2.Zero;
			float minDistance = float.MaxValue;

			if (TestPolygonPolygon(aVertices, aVertices, bVertices, result, ref minDistance)) result.Intersects = TestPolygonPolygon(bVertices, aVertices, bVertices, result, ref minDistance);
		
			if (!result.Intersects)
			{
				result.CollisionPoints = 0;
				result.CollisionPoint1 = null;
				result.CollisionPoint2 = null;
			}
		}
		private static void Intersects(float radius, Vector2 center, Vector2 polygonCenter, Vector2[] vertices, CollisionResult result)
		{
			result.Depth = float.MaxValue;
			result.Direction = Vector2.Zero;

			float minDistance = float.MaxValue, minEdgeDistance = float.MaxValue;
			Vector2 closestVertice = Vector2.Zero;
			Vector2? collisionPoint = null;
			float axisDepth;
			float aMin, aMax, bMin, bMax;
			Vector2 axis;

			for (int i = 0; i < vertices.Length; i++)
			{
				Vector2 vA = vertices[i], vB = vertices[(i + 1) % vertices.Length];
				Vector2 edge = vB - vA;
				axis = new Vector2(-edge.Y, edge.X);

				axis.Normalize();
				ProjectCircle(radius, center, axis, out aMin, out aMax);
				ProjectVertices(vertices, axis, out bMin, out bMax);

				if (aMin >= bMax || bMin >= aMax) return;

				axisDepth = Math.Min(aMax - bMin, bMax - aMin);

				if (axisDepth < result.Depth)
				{
					result.Depth = axisDepth;
					result.Direction = axis;
				}

				float distance = Vector2.Distance(center, vA), edgeDistance = GetPoint(center, vA, vB, out Vector2 cp);

				if (minDistance > distance)
				{
					minDistance = distance;
					closestVertice = vA;
				}

				if (minEdgeDistance > edgeDistance)
				{
					minEdgeDistance = edgeDistance;
					collisionPoint = cp;
				}
			}

			axis = center - closestVertice;

			axis.Normalize();
			ProjectCircle(radius, center, axis, out aMin, out aMax);
			ProjectVertices(vertices, axis, out bMin, out bMax);

			if (aMin >= bMax || bMin >= aMax) return;

			axisDepth = Math.Min(aMax - bMin, bMax - aMin);

			if (axisDepth < result.Depth)
			{
				result.Depth = axisDepth;
				result.Direction = axis;
			}

			if (Vector2.Dot(polygonCenter - center, result.Direction) < 0) result.Direction = -result.Direction;

			result.Intersects = true;
			result.CollisionPoints = 1;
			result.CollisionPoint1 = collisionPoint;
		}

		private static bool TestPolygonPolygon(Vector2[] checkVertices, Vector2[] aVertices, Vector2[] bVertices, CollisionResult result, ref float minDistance)
		{
			float aMin, aMax, bMin, bMax;

			for (int i = 0; i < checkVertices.Length; i++)
			{
				Vector2 vA = checkVertices[i], vB = checkVertices[(i + 1) % checkVertices.Length];
				Vector2 edge = vB - vA;
				Vector2 axis = new Vector2(-edge.Y, edge.X);

				axis.Normalize();
				
				if (checkVertices == aVertices)
				{
					ProjectVertices(aVertices, axis, out aMin, out aMax);
					ProjectVertices(bVertices, axis, out bMin, out bMax, ref minDistance, vA, result);
				}
				else
				{
					ProjectVertices(aVertices, axis, out aMin, out aMax, ref minDistance, vA, result);
					ProjectVertices(bVertices, axis, out bMin, out bMax);
				}
				
				if (aMin >= bMax || bMin >= aMax) return false;
				
				float axisDepth = Math.Min(aMax - bMin, bMax - aMin);

				if (axisDepth < result.Depth)
				{
					result.Depth = axisDepth;
					result.Direction = axis;
				}
			}
			
			return true;
		}
		private static void ProjectVertices(this Vector2[] vertices, Vector2 axis, out float min, out float max, ref float minDistance, Vector2 center, CollisionResult result)
		{
			min = float.MaxValue;
			max = float.MinValue;

			for (int i = 0; i < vertices.Length; i++)
			{
				ProjectPoint(vertices[i], ref axis, ref min, ref max);

				if (result.CollisionPoints < 2)
				{
					float distance = GetPoint(center, vertices[i], vertices[(i + 1) % vertices.Length], out Vector2 cp);
					
					if (distance.Approximately(minDistance, CompareTolerance))
					{
						if (!cp.Approximately(result.CollisionPoint1.Value))
						{
							result.CollisionPoint2 = cp;
							result.CollisionPoints = 2;
						}
					}
					else if (distance < minDistance)
					{
						result.CollisionPoint1 = cp;
						result.CollisionPoints = 1;
						minDistance = distance;
					}
				}
			}
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
		private static float GetPoint(Vector2 center, Vector2 min, Vector2 max, out Vector2 point)
		{
			Vector2 ab = max - min;
			Vector2 ac = center - min;
			float projection = Vector2.Dot(ac, ab);
			float normalized = projection / (ab.X * ab.X + ab.Y * ab.Y);

			if (normalized >= 1) point = max;
			else if (normalized <= 0) point = min;
			else point = min + ab * normalized;

			return DistanceSquared(center, point);
		}

		private static float DistanceSquared(Vector2 a, Vector2 b)
		{
			float v1 = a.X - b.X, v2 = a.Y - b.Y;

			return v1 * v1 + v2 * v2;
		}
		internal static bool Approximately(this Vector2 a, Vector2 b) => DistanceSquared(a, b) <= CompareTolerance * CompareTolerance;
	}
}