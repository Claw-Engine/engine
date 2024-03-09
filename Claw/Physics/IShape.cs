using System;

namespace Claw.Physics
{
	/// <summary>
	/// Interface para colisores baseados em física.
	/// </summary>
	public interface IShape
	{
		float Area { get; }
		Vector2 Offset { get; }

		/// <summary>
		/// Atualiza o estado deste colisor.
		/// </summary>
		void Update(RigidBody body);
	}
}