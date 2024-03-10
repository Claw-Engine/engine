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
		/// Quantas vezes o processo de física pode rodar por frame (10 por padrão).
		/// </summary>
		public static int MaxIterations = 10;
		/// <summary>
		/// Unidade de medida, em pixels (16 por padrão).
		/// </summary>
		public static float Unit = 16;
		/// <summary>
		/// Gravidade geral dos corpos ({ X: 0, Y: 9.8 } por padrão).
		/// </summary>
		public static Vector2 Gravity = new Vector2(0, 9.8f);

		public int BodyCount => bodies?.Count ?? 0;
		private List<RigidBody> bodies;

		#region Filtro
		internal void AddTo(ModuleCollection collection)
		{
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

				if (bodies[i].Type == BodyType.Trigger) continue;

				for (int j = i - 1; j >= 0; j--)
				{
					if (!bodies[j].Enabled) continue;

					bodies[j].UpdateShapes();

					if (CollisionChecker.Intersects(bodies[i], bodies[j], out float depth, out Vector2 direction, out IShape a, out IShape b))
					{
						ResolveCollision(a, b, depth, direction);

						bodies[i].UpdateShapes();
						bodies[j].UpdateShapes();
					}
				}
			}
		}
		private void ResolveCollision(IShape a, IShape b, float depth, Vector2 direction)
		{
			switch (b.Body.Type)
			{
				case BodyType.Trigger: a.Body.Triggering(new CollisionResult(true, depth, direction, a, b)); break;
				case BodyType.Static:
					if (a.Body.Type == BodyType.Static) break;
					goto default;
				default:
					bool resolve = true;

					if (a.Body.Type == BodyType.Normal) resolve = a.Body.Colliding(new CollisionResult(true, depth, direction, a, b));
					else if (b.Body.Type == BodyType.Normal) resolve = b.Body.Colliding(new CollisionResult(true, depth, direction, b, a));

					if (resolve)
					{
						Vector2 relativeVelocity = a.Body.Velocity - b.Body.Velocity;

						if (Vector2.Dot(relativeVelocity, direction) <= 0)
						{
							float j = -(1 + Math.Min(a.Body.Bounciness, b.Body.Bounciness)) * Vector2.Dot(relativeVelocity, direction);
							j /= a.Body.inverseMass + b.Body.inverseMass;

							Vector2 impulse = j * direction;
							a.Body.Velocity += impulse * a.Body.inverseMass;
							b.Body.Velocity -= impulse * b.Body.inverseMass;
						}

						if (b.Body.Type == BodyType.Static) a.Body.Transform.Position += depth * direction;
						else if (a.Body.Type == BodyType.Static) b.Body.Transform.Position -= depth * direction;
						else
						{
							a.Body.Transform.Position += depth * .5f * direction;
							b.Body.Transform.Position -= depth * .5f * direction;
						}
					}
					break;
			}
		}
	}
}