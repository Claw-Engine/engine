using Claw.Graphics;

namespace Claw.Modules;

/// <summary>
/// Interface para elementos com animação.
/// </summary>
public interface IAnimatable
{
	Vector2 Origin { get; set; }
	Sprite Sprite { get; set; }
	Rectangle? SpriteArea { get; set; }
}
