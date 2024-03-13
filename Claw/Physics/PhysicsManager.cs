using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Claw.Modules;

namespace Claw.Physics
{
	/// <summary>
	/// Representa a física do jogo.
	/// </summary>
	public sealed class PhysicsManager
	{
		/// <summary>
		/// Quantas vezes o processo de resolução de colisão pode rodar por frame (5 por padrão).
		/// </summary>
		public static int MaxIterations = 5;
		/// <summary>
		/// Unidade de medida, em pixels (16 por padrão).
		/// </summary>
		public static float Unit = 16;
		/// <summary>
		/// Gravidade geral dos corpos ({ X: 0, Y: 9.8 } por padrão).
		/// </summary>
		public static Vector2 Gravity = new Vector2(0, 9.8f);

		public bool Enabled = true;
		public int BodyCount => bodies?.Count ?? 0;
		public readonly int GridSize;
		private CollisionResult result;
		private List<RigidBody> bodies;
		private Dictionary<Vector2, Phygrid> grids;
		internal bool needStep = true;

		#region Filtro
		internal void AddTo(ModuleCollection collection)
		{
			Game.Instance.Modules.ModuleAdded += ModuleAdded;
			Game.Instance.Modules.ModuleRemoved += ModuleRemoved;
		}
		internal void RemoveFrom(ModuleCollection collection)
		{
			bodies.Clear();
			grids.Clear();

			collection.ModuleAdded -= ModuleAdded;
			collection.ModuleRemoved -= ModuleRemoved;
		}

		private void ModuleAdded(BaseModule module)
		{
			if (module is RigidBody body)
			{
				needStep = true;

				bodies.Add(body);
			}
		}
		private void ModuleRemoved(BaseModule module)
		{
			if (module is RigidBody body)
			{
				needStep = true;

				for (int i = body.grids.Count - 1; i >= 0; i--) body.grids[i].bodies.Remove(body);

				body.grids.Clear();
				bodies.Remove(body);
			}
		}
		internal void UpdateGrid(RigidBody body)
		{
			Vector2 start = body.Shape.BoundingBox.Location, size = body.Shape.BoundingBox.Size;
			Vector2 topLeft = Mathf.ToGrid(start, GridSize), topRight = Mathf.ToGrid(start + new Vector2(size.X - 1, 0), GridSize),
				bottomLeft = Mathf.ToGrid(start + new Vector2(0, size.Y - 1), GridSize), bottomRight = Mathf.ToGrid(start + new Vector2(size.X - 1, size.Y - 1), GridSize);

			if (topLeft != body.previousTopLeft || topRight != body.previousTopRight || bottomLeft != body.previousBottomLeft || bottomRight != body.previousBottomRight)
			{
				body.previousTopLeft = topLeft;
				body.previousTopRight = topRight;
				body.previousBottomLeft = bottomLeft;
				body.previousBottomRight = bottomRight;

				RemoveFromGrid(body);
				UpdateGrid(body, topLeft);

				if (topRight != topLeft) UpdateGrid(body, topRight);

				if (bottomLeft != topLeft && bottomLeft != topRight) UpdateGrid(body, bottomLeft);

				if (bottomRight != topLeft && bottomRight != topRight && bottomRight != bottomLeft) UpdateGrid(body, bottomRight);
			}
		}
		private void UpdateGrid(RigidBody body, Vector2 gridIndex)
		{
			if (grids.TryGetValue(gridIndex, out Phygrid grid))
			{
				grid.bodies.Add(body);
				body.grids.Add(grid);
			}
			else
			{
				grid = new Phygrid() { index = gridIndex };

				grid.bodies.Add(body);
				body.grids.Add(grid);
				grids.Add(gridIndex, grid);
			}
		}
		internal void RemoveFromGrid(RigidBody body)
		{
			for (int i = 0; i < body.grids.Count; i++) body.grids[i].bodies.Remove(body);

			body.grids.Clear();
		}
		#endregion

		public PhysicsManager(int gridSize)
		{
			GridSize = gridSize > 0 ? gridSize : 400;
			result = new CollisionResult();
			bodies = new List<RigidBody>();
			grids = new Dictionary<Vector2, Phygrid>();
		}

		internal void Step()
		{
			if (!needStep || !Enabled) return;

			int iterations = (int)Math.Min((Time.UnscaledDeltaTime * 1000) / Time.FrameDelay * MaxIterations, MaxIterations);

			for (int i = 0; i < iterations; i++) Iteration();
		}
		private void Iteration()
		{
			needStep = false;

			for (int i = bodies.Count - 1; i >= 0; i--)
			{
				if (bodies[i].Enabled && bodies[i].Type == BodyType.Normal)
				{
					bodies[i].UpdateBody();

					for (int grid = bodies[i].grids.Count - 1; grid >= 0; grid--) BodyAgainstGrid(bodies[i], bodies[i].grids[grid]);
				}
			}
		}
		private void BodyAgainstGrid(RigidBody body, Phygrid grid)
		{
			for (int i = grid.bodies.Count - 1; i >= 0; i--)
			{
				RigidBody other = grid.bodies[i];

				if (other.Enabled && body != other)
				{
					result.Reset();
					CollisionChecker.Intersects(body, other, result);

					if (result.Intersects) OnCollision();
				}
			}
		}
		private void OnCollision()
		{
			RigidBody a = result.Body, b = result.OtherBody;

			switch (b.Type)
			{
				case BodyType.Trigger: a.Triggering(result); break;
				default:
					bool resolve = a.Colliding(result);

					if (resolve)
					{
						ResolveImpulse(a, b, result);

						if (b.Type == BodyType.Static) a.Transform.Position += result.Depth * result.Direction;
						else
						{
							a.Transform.Position += result.Depth * .5f * result.Direction;
							b.Transform.Position -= result.Depth * .5f * result.Direction;
						}
					}
					break;
			}
		}
		private void ResolveImpulse(RigidBody a, RigidBody b, CollisionResult result)
		{
			float bounciness = (a.Material.Bounciness + b.Material.Bounciness) * .5f;
			float staticFriction = (a.Material.StaticFriction + b.Material.StaticFriction) * .5f;
			float dynamicFriction = (a.Material.DynamicFriction + b.Material.DynamicFriction) * .5f;
			Vector2 aPos = a.Shape.Center, bPos = b.Shape.Center;
			
			for (int i = 0; i < result.CollisionPoints; i++)
			{
				float j = 0;
				Vector2 closest = result[i];
				Vector2 ra = closest - aPos, rb = closest - bPos;
				Vector2 raPerp = new Vector2(-ra.Y, ra.X), rbPerp = new Vector2(-rb.Y, rb.X);

				Vector2 relativeVelocity = a.MoveSpeed + (raPerp * a.RotateSpeed) - b.MoveSpeed + (rbPerp * -b.RotateSpeed);

				if (Vector2.Dot(relativeVelocity, result.Direction) <= 0)
				{
					float raPerpDot = Vector2.Dot(raPerp, result.Direction), rbPerpDot = Vector2.Dot(rbPerp, result.Direction);
					j = -(1 + bounciness) * Vector2.Dot(relativeVelocity, result.Direction);
					j /= a.inverseMass + b.inverseMass + (raPerpDot * raPerpDot * a.inverseInertia) + (rbPerpDot * rbPerpDot * b.inverseInertia);

					Vector2 impulse = j * result.Direction;
					a.MoveSpeed += impulse * a.inverseMass;
					b.MoveSpeed -= impulse * b.inverseMass;
					a.RotateSpeed += Vector2.Cross(ra, impulse) * a.inverseInertia;
					b.RotateSpeed -= Vector2.Cross(rb, impulse) * b.inverseInertia;
				}

				relativeVelocity = a.MoveSpeed + (raPerp * a.RotateSpeed) - b.MoveSpeed + (rbPerp * -b.RotateSpeed);
				Vector2 tangent = relativeVelocity - Vector2.Dot(relativeVelocity, result.Direction) * result.Direction;

				if (!tangent.Approximately(Vector2.Zero))
				{
					tangent.Normalize();

					float raPerpDot = Vector2.Dot(raPerp, tangent), rbPerpDot = Vector2.Dot(rbPerp, tangent);
					float jt = -Vector2.Dot(relativeVelocity, tangent);
					jt /= a.inverseMass + b.inverseMass + (raPerpDot * raPerpDot * a.inverseInertia) + (rbPerpDot * rbPerpDot * b.inverseInertia);
					Vector2 frictionImpulse;

					if (Math.Abs(jt) <= j * staticFriction) frictionImpulse = jt * tangent;
					else frictionImpulse = -j * tangent * dynamicFriction;

					a.MoveSpeed += frictionImpulse * a.inverseMass;
					b.MoveSpeed -= frictionImpulse * b.inverseMass;
					a.RotateSpeed += Vector2.Cross(ra, frictionImpulse) * a.inverseInertia;
					b.RotateSpeed -= Vector2.Cross(rb, frictionImpulse) * b.inverseInertia;
				}
			}
		}
	}
	internal class Phygrid
	{
		public Vector2 index;
		public List<RigidBody> bodies = new List<RigidBody>();
	}
}