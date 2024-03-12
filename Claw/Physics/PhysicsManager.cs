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
		internal static bool needStep = true;

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

				bodies.Remove(body);
			}
		}
		#endregion

		internal void Step()
		{
			if (!needStep) return;

			int iterations = (int)Math.Min((Time.UnscaledDeltaTime * 1000) / Time.FrameDelay * MaxIterations, MaxIterations);

			for (int i = 0; i < iterations; i++) Iteration();
		}
		private void Iteration()
		{
			needStep = false;

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
						OnCollision();
						bodies[i].UpdateShapes();
						bodies[j].UpdateShapes();
					}
				}
			}
		}
		private void OnCollision()
		{
			RigidBody a = result.Shape.Body, b = result.OtherShape.Body;

			if (a.Type == BodyType.Trigger)
			{
				if (b.Type != BodyType.Trigger) b.Triggering(result);

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
					else if (b.Type == BodyType.Normal) resolve = b.Colliding(result);

					if (resolve)
					{
						ResolveImpulse(a, b, result);

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
		private void ResolveImpulse(RigidBody a, RigidBody b, CollisionResult result)
		{
			float bounciness = (a.Material.Bounciness + b.Material.Bounciness) * .5f;
			float staticFriction = (a.Material.StaticFriction + b.Material.StaticFriction) * .5f;
			float dynamicFriction = (a.Material.DynamicFriction + b.Material.DynamicFriction) * .5f;
			Vector2 aPos = a.Transform.Position, bPos = b.Transform.Position;
			float j = 0;

			Vector2 relativeVelocity = a.MoveSpeed - b.MoveSpeed;
			
			if (Vector2.Dot(relativeVelocity, result.Direction) <= 0)
			{
				j = -(1 + bounciness) * Vector2.Dot(relativeVelocity, result.Direction);
				j /= a.inverseMass + b.inverseMass;

				Vector2 impulse = j * result.Direction;
				a.MoveSpeed += impulse * a.inverseMass;
				b.MoveSpeed -= impulse * b.inverseMass;
			}

			relativeVelocity = a.MoveSpeed - b.MoveSpeed;
			Vector2 tangent = relativeVelocity - Vector2.Dot(relativeVelocity, result.Direction) * result.Direction;

			if (!tangent.Approximately(Vector2.Zero))
			{
				tangent.Normalize();

				float jt = -Vector2.Dot(relativeVelocity, tangent) / (a.inverseMass + b.inverseMass);
				Vector2 frictionImpulse;

				if (Math.Abs(jt) <= j * staticFriction) frictionImpulse = jt * tangent;
				else frictionImpulse = -j * tangent * dynamicFriction;

				a.MoveSpeed += frictionImpulse * a.inverseMass;
				b.MoveSpeed -= frictionImpulse * b.inverseMass;
			}
		}
	}
}