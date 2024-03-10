using Claw.Modules;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

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

		public int BodyCount => bodies?.Count ?? 0;
		private CollisionResult result;
		private List<RigidBody> bodies;

		#region Filtro
		internal void AddTo(ModuleCollection collection)
		{
			if (result == null) result = new CollisionResult();

			bodies = new List<RigidBody>();
			Game.Instance.Modules.ModuleAdded += ModuleAdded;
			Game.Instance.Modules.ModuleRemoved += ModuleRemoved;
		}
		internal void RemoveFrom(ModuleCollection collection)
		{
			bodies = null;
			collection.ModuleAdded -= ModuleAdded;
			collection.ModuleRemoved -= ModuleRemoved;
		}

		private void ModuleAdded(BaseModule module)
		{
			if (module is RigidBody body) bodies.Add(body);
		}
		private void ModuleRemoved(BaseModule module)
		{
			if (module is RigidBody body) bodies.Remove(body);
		}
		#endregion

		internal void Step()
		{
			int iterations = (int)Math.Min((Time.UnscaledDeltaTime * 1000) / Time.FrameDelay * MaxIterations, MaxIterations);

			for (int i = 0; i < iterations; i++) Iteration();
		}
		private void Iteration()
		{
			for (int i = bodies.Count - 1; i >= 0; i--)
			{
				if (!bodies[i].Enabled) continue;

				bodies[i].UpdateShapes();

				for (int j = i - 1; j >= 0; j--)
				{
					if (!bodies[j].Enabled) continue;

					bodies[j].UpdateShapes();
					result.Reset();
					CollisionChecker.Intersects(bodies[i], bodies[j], result);

					if (result.Intersects)
					{
						ResolveCollision();

						bodies[i].UpdateShapes();
						bodies[j].UpdateShapes();
					}
				}
			}
		}
		private void ResolveCollision()
		{
			RigidBody a = result.Shape.Body, b = result.OtherShape.Body;

			if (a.Type == BodyType.Trigger)
			{
				if (b.Type != BodyType.Trigger)
				{
					result.RevertShapes();
					b.Triggering(result);
				}

				return;
			}

			switch (b.Type)
			{
				case BodyType.Trigger: a.Triggering(result); break;
				case BodyType.Static:
					if (a.Type == BodyType.Static) break;
					goto default;
				default:
					bool resolve = true;

					if (a.Type == BodyType.Normal) resolve = a.Colliding(result);
					else if (b.Type == BodyType.Normal)
					{
						result.RevertShapes();

						resolve = b.Colliding(result);
					}

					if (resolve)
					{
						Vector2 relativeVelocity = a.Velocity - b.Velocity;

						if (Vector2.Dot(relativeVelocity, result.Direction) <= 0)
						{
							float j = -(1 + Math.Min(a.Bounciness, b.Bounciness)) * Vector2.Dot(relativeVelocity, result.Direction);
							j /= a.inverseMass + b.inverseMass;

							Vector2 impulse = j * result.Direction;
							a.Velocity += impulse * a.inverseMass;
							b.Velocity -= impulse * b.inverseMass;
						}

						if (b.Type == BodyType.Static) a.Transform.Position += result.Depth * result.Direction;
						else if (a.Type == BodyType.Static) b.Transform.Position -= result.Depth * result.Direction;
						else
						{
							a.Transform.Position += result.Depth * .5f * result.Direction;
							b.Transform.Position -= result.Depth * .5f * result.Direction;
						}
					}
					break;
			}
		}
	}
}