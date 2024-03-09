using Claw.Modules;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Claw.Physics
{
	/// <summary>
	/// Classe responsável pelo manuseio da física e suas constantes.
	/// </summary>
	public static class Physics
	{
		public static int BodyCount => bodies?.Count ?? 0;
		/// <summary>
		/// Unidade de medida, em pixels (100 por padrão).
		/// </summary>
		public static float Unit = 100;
		/// <summary>
		/// Gravidade geral dos corpos ({ X: 0, Y: 9.8 } por padrão).
		/// </summary>
		public static Vector2 Gravity = new Vector2(0, 9.8f);
		private static List<RigidBody> bodies;

		internal static void Initialize()
		{
			bodies = new List<RigidBody>();
			Game.Instance.Modules.ModuleAdded += ModuleAdded;
			Game.Instance.Modules.ModuleRemoved += ModuleRemoved;
		}

		internal static void Step()
		{
			for (int it = 0; it < 20; it++)
			{
				for (int i = bodies.Count - 1; i >= 0; i--)
				{
					bodies[i].UpdateShapes();

					if (bodies[i].Type == BodyType.Trigger) continue;

					for (int j = i - 1; j >= 0; j--)
					{
						bodies[j].UpdateShapes();

						bool intersects = Physics.Intersects(bodies[i], bodies[j], out float depth, out Vector2 normal);

						if (intersects)
						{
							ResolveCollision(bodies[i], bodies[j], depth, normal);

							bodies[i].UpdateShapes();
							bodies[j].UpdateShapes();
						}
					}
				}
			}
		}
		private static void ResolveCollision(RigidBody a, RigidBody b, float depth, Vector2 normal)
		{
			switch (b.Type)
			{
				case BodyType.Trigger:

					break;
				case BodyType.Static:
					if (a.Type == b.Type) break;
					
					goto case BodyType.Normal;
				case BodyType.Normal:
					Vector2 relativeVelocity = b.Velocity - a.Velocity;

					if (Vector2.Dot(relativeVelocity, normal) <= 0)
					{
						float j = -(1 + Math.Min(a.Bounciness, b.Bounciness)) * Vector2.Dot(relativeVelocity, normal);
						j /= a.inverseMass + b.inverseMass;

						Vector2 impulse = j * normal;
						a.Velocity -= impulse * a.inverseMass;
						b.Velocity += impulse * b.inverseMass;
					}

					if (b.Type == BodyType.Static) a.Transform.Position -= normal * depth;
					else if (a.Type == BodyType.Static) b.Transform.Position += normal * depth;
					else
					{
						a.Transform.Position -= normal * (depth * .5f);
						b.Transform.Position += normal * (depth * .5f);
					}
					break;
			}
		}

		private static void ModuleAdded(BaseModule module)
		{
			if (module is RigidBody body) bodies.Add(body);
		}
		private static void ModuleRemoved(BaseModule module)
		{
			if (module is RigidBody body) bodies.Remove(body);
		}

		private static bool Intersects(RigidBody a,  RigidBody b, out float depth, out Vector2 normal)
		{
			if (a.Shape is CircleShape)
			{
				if (b.Shape is CircleShape) return Intersects(a, (CircleShape)a.Shape, b, (CircleShape)b.Shape, out depth, out normal);
				else if (b.Shape is PolygonShape) return Intersects(a, (CircleShape)a.Shape, b, (PolygonShape)b.Shape, out depth, out normal);
			}
			else if (a.Shape is PolygonShape)
			{
				if (b.Shape is PolygonShape) return Intersects(a, (PolygonShape)a.Shape, b, (PolygonShape)b.Shape, out depth, out normal);
				else if (b.Shape is CircleShape)
				{
					bool result = Intersects(b, (CircleShape)b.Shape, a, (PolygonShape)a.Shape, out depth, out normal);
					normal = -normal;

					return result;
				}
			}

			depth = 0;
			normal = Vector2.Zero;

			return false;
		}
		private static bool Intersects(RigidBody a, CircleShape aCircle, RigidBody b, CircleShape bCircle, out float depth, out Vector2 normal)
		{
			float distance = Vector2.Distance(aCircle.centerInWorld, bCircle.centerInWorld);
			float radii = aCircle.radiusInWorld + bCircle.radiusInWorld;

			if (distance >= radii)
			{
				depth = 0;
				normal = Vector2.Zero;

				return false;
			}
			else
			{
				depth = radii - distance;
				normal = Vector2.Normalize(bCircle.centerInWorld - aCircle.centerInWorld);

				return true;
			}
		}
		private static bool Intersects(RigidBody a, PolygonShape aPolygon, RigidBody b, PolygonShape bPolygon, out float depth, out Vector2 normal)
		{
			depth = float.MaxValue;
			normal = Vector2.Zero;

			for (int i = 0; i < aPolygon.VerticeCount; i++)
			{
				Vector2 va = aPolygon.verticesInWorld[i],
					vb = aPolygon.verticesInWorld[(i + 1) % aPolygon.VerticeCount];
				Vector2 edge = vb - va;
				Vector2 axis = new Vector2(-edge.Y, edge.X);

				axis.Normalize();
				ProjectVertices(aPolygon.verticesInWorld, axis, out float minA, out float maxA);
				ProjectVertices(bPolygon.verticesInWorld, axis, out float minB, out float maxB);

				if (minA >= maxB || minB >= maxA) return false;

				float axisDepth = Math.Min(maxB - minA, maxA - minB);

				if (axisDepth < depth)
				{
					depth = axisDepth;
					normal = axis;
				}
			}

			for (int i = 0; i < bPolygon.VerticeCount; i++)
			{
				Vector2 va = bPolygon.verticesInWorld[i],
					vb = bPolygon.verticesInWorld[(i + 1) % bPolygon.VerticeCount];
				Vector2 edge = vb - va;
				Vector2 axis = new Vector2(-edge.Y, edge.X);

				axis.Normalize();
				ProjectVertices(aPolygon.verticesInWorld, axis, out float minA, out float maxA);
				ProjectVertices(bPolygon.verticesInWorld, axis, out float minB, out float maxB);

				if (minA >= maxB || minB >= maxA) return false;

				float axisDepth = Math.Min(maxB - minA, maxA - minB);

				if (axisDepth < depth)
				{
					depth = axisDepth;
					normal = axis;
				}
			}

			Vector2 centerA = aPolygon.ArithmeticMean, centerB = bPolygon.ArithmeticMean;

			if (Vector2.Dot(centerB - centerA, normal) < 0) normal = -normal;

			return true;
		}
		private static bool Intersects(RigidBody a, CircleShape aCircle, RigidBody b, PolygonShape bPolygon, out float depth, out Vector2 normal)
		{
			depth = float.MaxValue;
			normal = Vector2.Zero;

			Vector2 closest = Vector2.Zero;
			float minDistance = float.MaxValue;
			float minA, maxA, minB, maxB, axisDepth;
			Vector2 axis;

			for (int i = 0; i < bPolygon.VerticeCount; i++)
			{
				float distance = Vector2.Distance(aCircle.centerInWorld, bPolygon.verticesInWorld[i]);

				if (distance < minDistance)
				{
					minDistance = distance;
					closest = bPolygon.verticesInWorld[i];
				}

				Vector2 va = bPolygon.verticesInWorld[i],
					vb = bPolygon.verticesInWorld[(i + 1) % bPolygon.VerticeCount];
				Vector2 edge = vb - va;
				axis = new Vector2(-edge.Y, edge.X);

				axis.Normalize();
				ProjectCircle(aCircle.radiusInWorld, aCircle.centerInWorld, axis, out minA, out maxA);
				ProjectVertices(bPolygon.verticesInWorld, axis, out minB, out maxB);

				if (minA >= maxB || minB >= maxA) return false;

				axisDepth = Math.Min(maxB - minA, maxA - minB);

				if (axisDepth < depth)
				{
					depth = axisDepth;
					normal = axis;
				}
			}

			axis = closest - aCircle.centerInWorld;

			axis.Normalize();
			ProjectCircle(aCircle.radiusInWorld, aCircle.centerInWorld, axis, out minA, out maxA);
			ProjectVertices(bPolygon.verticesInWorld, axis, out minB, out maxB);

			if (minA >= maxB || minB >= maxA) return false;

			axisDepth = Math.Min(maxB - minA, maxA - minB);

			if (axisDepth < depth)
			{
				depth = axisDepth;
				normal = axis;
			}

			if (Vector2.Dot(bPolygon.ArithmeticMean - aCircle.centerInWorld, normal) < 0) normal = -normal;

			return true;
		}

		private static void ProjectVertices(Vector2[] vertices, Vector2 axis, out float min, out float max)
		{
			min = float.MaxValue;
			max = float.MinValue;

			for (int i = 0; i < vertices.Length; i++)
			{
				float proj = Vector2.Dot(vertices[i], axis);

				if (proj < min) min = proj;
				else if (proj > max) max = proj;
			}
		}
		private static void ProjectCircle(float radius, Vector2 center, Vector2 axis, out float min, out float max)
		{
			Vector2 directionRadius = Vector2.Normalize(axis) * radius;
			Vector2 p1 = center + directionRadius, p2 = center - directionRadius;

			min = Vector2.Dot(p1, axis);
			max = Vector2.Dot(p2, axis);

			if (min > max)
			{
				float temp = max;
				max = min;
				min = temp;
			}
		}
	}
}