using System;
using System.Collections.Generic;

namespace Claw.Modules
{
	/// <summary>
	/// Descreve informações espaciais para os módulos.
	/// </summary>
	public sealed class Transform
	{
		public float Rotation
		{
			get
			{
				if (Parent != null) return _rotation + Parent.Rotation;

				return _rotation;
			}
			set
			{
				if (Parent != null) _rotation = Parent.Rotation - value;
				else _rotation = value;
			}
		}
		public float RelativeRotation
		{
			get => _rotation;
			set => _rotation = value;
		}
		public Vector2 Position
		{
			get
			{
				if (Parent != null) return Vector2.Rotate(_position * Parent.Scale + Parent.Position, Parent.Position, Parent.Rotation);

				return _position;
			}
			set
			{
				if (Parent != null) _position = Vector2.Rotate((value - Parent.Position) / Parent.Scale, Vector2.Zero, -Parent.Rotation);
				else _position = value;
			}
		}
		public Vector2 RelativePosition
		{
			get => _position;
			set => _position = value;
		}
		public Vector2 Scale
		{
			get
			{
				if (Parent != null) return _scale * Parent.Scale;

				return _scale;
			}
			set
			{
				if (Parent != null) _scale = value / Parent.Scale;
				else _scale = value;
			}
		}
		public Vector2 RelativeScale
		{
			get => _scale;
			set => _scale = value;
		}

		public Vector2 Facing => Vector2.FindFacing(Rotation);
		public readonly BaseModule Module;
		private float _rotation = 0;
		private Vector2 _position = Vector2.Zero, _scale = new Vector2(1);

		public Transform Parent
		{
			get => _parent;
			set
			{
				if (_parent != null) _parent.children.Remove(this);

				if (value == null)
				{
					_position = Position;
					_scale = Scale;
					_rotation = Rotation;
					_parent = null;
				}
				else if (value != this && value._parent != this)
				{
					_parent = value;
					_rotation = _rotation - Parent.Rotation;
					_scale = Parent.Scale / _scale;
					_position = _position - Parent.Position;

					value.children.Add(this);
				}
			}
		}
		private Transform _parent;
		internal List<Transform> children = new List<Transform>();

		internal Transform(BaseModule module) => Module = module;

		/// <summary>
		/// Retorna a quantidade de filhos deste <see cref="Transform"/>.
		/// </summary>
		public int ChildCount() => children.Count;
		/// <summary>
		/// Retorna um filho deste <see cref="Transform"/>.
		/// </summary>
		public Transform GetChild(int index) => children[index];
	}
}