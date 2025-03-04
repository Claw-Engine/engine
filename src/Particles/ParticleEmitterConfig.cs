using Claw.Graphics;

namespace Claw.Particles;

/// <summary>
/// Representa as configurações do <see cref="ParticleEmitter"/>.
/// </summary>
public sealed class ParticleEmitterConfig
{
	public Sprite Sprite;
	public Vector2 RelativeDirection = Vector2.Zero;

	public bool RotationByDirection = false, UseScaledTime = true;
	public float SpawnTime = 0, Rotation = 0, Rotate = 0;
	public Vector2 Range = Vector2.Zero, Offset = Vector2.Zero, LifeTime = Vector2.One;
	public Vector2 Number = new Vector2(1);

	public ParticleValue<float> Direction, Velocity;
	public ParticleValue<Vector2> Scale;
	public ParticleValue<Color> Color;
	public ParticleValue<Rectangle> SpriteArea;

	public ParticleEmitterConfig()
	{
		Direction = new ParticleValue<float>(90);
		Velocity = new ParticleValue<float>(10);
		Scale = new ParticleValue<Vector2>(Vector2.One);
		Color = new ParticleValue<Color>(Claw.Color.White);
		SpriteArea = new ParticleValue<Rectangle>(null);
	}
	public ParticleEmitterConfig(ParticleValue<float> direction, ParticleValue<float> velocity, ParticleValue<Vector2> scale, ParticleValue<Color> color, ParticleValue<Rectangle> spriteArea)
	{
		Direction = direction;
		Velocity = velocity;
		Scale = scale;
		Color = color;
		SpriteArea = spriteArea;
	}
	public ParticleEmitterConfig(float direction, float velocity, Vector2 scale, Color color, Rectangle spriteArea)
	{
		Direction = new ParticleValue<float>(direction);
		Velocity = new ParticleValue<float>(velocity);
		Scale = new ParticleValue<Vector2>(scale);
		Color = new ParticleValue<Color>(color);
		SpriteArea = new ParticleValue<Rectangle>(spriteArea);
	}
}
