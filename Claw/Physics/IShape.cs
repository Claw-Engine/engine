using System;

namespace Claw.Physics
{
	/// <summary>
	/// Interface para colisores baseados em física.
	/// </summary>
	public interface IShape
	{
		float Area { get; }
		Vector2 Offset { get; set; }
		RigidBody Body { get; }
		/// <summary>
		/// Centro do colisor no mundo.
		/// </summary>
		Vector2 Center { get; }
		/// <summary>
		/// Bordas do colisor no mundo.
		/// </summary>
		Rectangle BoundingBox { get; }

		/// <summary>
		/// Atualiza o estado deste colisor.
		/// </summary>
		void Update();
	}
}