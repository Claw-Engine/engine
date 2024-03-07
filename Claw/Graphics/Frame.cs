using System;

namespace Claw.Graphics
{
	/// <summary>
	/// Dados de um frame para o <see cref="Animator"/>.
	/// </summary>
	public sealed class Frame
	{
		public Sprite Sprite;
		public Rectangle? Area;

		public Frame(Sprite sprite, Rectangle? area)
		{
			Sprite = sprite;
			Area = area;
		}
	}
}