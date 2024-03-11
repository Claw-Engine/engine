using System;

namespace Claw.Physics
{
	/// <summary>
	/// Descreve as propriedades físicas de um <see cref="RigidBody"/>.
	/// </summary>
	public struct Material
	{
		/// <summary>
		/// Densidade do material (no mínimo 0.5).
		/// </summary>
		public float Density
		{
			get => Math.Max(_density, .5f);
			set => _density = value;
		}
		/// <summary>
		/// O quanto o material quica (de 0 a 1).
		/// </summary>
		public float Bounciness
		{
			get => Mathf.Clamp(_bounciness, 0, 1);
			set => _bounciness = value;
		}
		/// <summary>
		/// Fricção usada quando o objeto está em repouso (de 0 a 1).
		/// </summary>
		public float StaticFriction
		{
			get => Mathf.Clamp(_staticFriction, 0, 1);
			set => _staticFriction = value;
		}
		/// <summary>
		/// Fricção usada quando o objeto já está em movimento (de 0 a 1).
		/// </summary>
		public float DynamicFriction
		{
			get => Mathf.Clamp(_dynamicFriction, 0, 1);
			set => _dynamicFriction = value;
		}
		private float _density, _bounciness, _staticFriction, _dynamicFriction;

		/// <summary>
		/// Density: 0,5; Bounciness: 0,5; StaticFriction: 0,6; DynamicFriction: 0,4.
		/// </summary>
		public static readonly Material Default = new Material(.5f, .5f, .6f, .4f);

		public Material(float density, float bounciness, float staticFriction, float dynamicFriction)
		{
			_density = density;
			_bounciness = bounciness;
			_staticFriction = staticFriction;
			_dynamicFriction = dynamicFriction;
		}
	}
}