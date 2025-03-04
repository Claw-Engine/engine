using Claw.Graphics;

namespace Claw.Particles;

/// <summary>
/// Representa uma partícula no <see cref="ParticleEmitter"/>.
/// </summary>
internal class Particle
{
	public static List<Particle> Pool = new List<Particle>();
	private static readonly Vector2 origin = new Vector2(.5f);

	private float lifeTime, rotation, rotate, counter = 0;
	private Vector2 position;
	
	private ParticleValue<float> direction, velocity;
	private ParticleValue<Vector2> scale;
	private ParticleValue<Color> color;
	private ParticleValue<Rectangle> spriteArea;

	private Sprite sprite;
	private ParticleEmitter emitter;

	private float? partDir, partVel;
	private Vector2? partScale;
	private Color? partColor;
	private Rectangle? partArea;

	static Particle()
	{
		float tolerance = .25f;

		ParticleValue<float>.GradientFunction += (value1, value2, multiply, useDelta) =>
		{
			float val = Mathf.DeltaLerp(value1, value2, multiply, useDelta);

			if (Math.Abs(val - value2) <= tolerance) val = value2;

			return val;
		};

		ParticleValue<Vector2>.GradientFunction += (value1, value2, multiply, useDelta) =>
		{
			Vector2 val = Vector2.DeltaLerp(value1, value2, multiply, useDelta);

			if (Vector2.Distance(val, value2) <= tolerance) val = value2;

			return val;
		};

		ParticleValue<Color>.GradientFunction += (value1, value2, multiply, useDelta) =>
		{
			Color val = Color.DeltaLerp(value1, value2, multiply, useDelta);

			return val;
		};

		Func<Rectangle, Rectangle, float, bool, Rectangle> rectGrad = (value1, value2, multiply, useDelta) =>
		{
			Rectangle val = Rectangle.DeltaLerp(value1, value2, multiply, useDelta);

			if (Vector2.Distance(val.Location, value2.Location) <= tolerance) val.Location = value2.Location;

			if (Vector2.Distance(val.Size, value2.Size) <= tolerance) val.Size = value2.Size;

			return val;
		};
		ParticleValue<Rectangle>.GradientFunction += rectGrad;
	}

	public Particle() { }

	public static Particle Instantiate(ParticleEmitter emitter, Sprite sprite, float lifeTime, float rotation, float rotate, float? direction, Vector2 position, Vector2 basePosition)
	{
		Particle part;

		if (Pool.Count > 0)
		{
			part = Pool[0];

			Pool.RemoveAt(0);
		}
		else part = new Particle();

		part.emitter = emitter;
		part.sprite = sprite;
		part.lifeTime = lifeTime;
		part.rotation = emitter.Config.RotationByDirection && direction.HasValue ? direction.Value : rotation;
		part.rotate = rotate;
		part.position = position;
		part.counter = 0;

		if (!direction.HasValue)
		{
			if (emitter.Config.RelativeDirection == Vector2.Zero) part.direction = emitter.Config.Direction;
			else part.direction = new ParticleValue<float>(Mathf.ToDegrees(Vector2.GetAngle(basePosition, basePosition + new Vector2(emitter.Config.RelativeDirection.X, -emitter.Config.RelativeDirection.Y))));
		}
		else part.direction = new ParticleValue<float>(direction.Value);

		part.velocity = emitter.Config.Velocity;
		part.scale = emitter.Config.Scale;
		part.color = emitter.Config.Color;
		part.spriteArea = emitter.Config.SpriteArea;

		part.SetValues();

		return part;
	}

	private void SetValues()
	{
		partDir = direction.GetValue(emitter, false);
		partVel = velocity.GetValue(emitter, false);
		partScale = scale.GetValue(emitter, false);
		partColor = color.GetValue(emitter, false);
		partArea = spriteArea.GetValue(emitter, false);
	}
	private void UpdateValues()
	{
		if (direction.IsGradient) partDir = direction.GetValue(emitter, true);
		
		if (velocity.IsGradient) partVel = velocity.GetValue(emitter, true);

		if (scale.IsGradient) partScale = scale.GetValue(emitter, true);

		if (color.IsGradient) partColor = color.GetValue(emitter, true);

		if (spriteArea.IsGradient) partArea = spriteArea.GetValue(emitter, true);
	}

	public void Step()
	{
		UpdateValues();

		if (partVel.HasValue)
		{
			Vector2 dir = Vector2.FindFacing(partDir ?? 0);
			dir.Y = -dir.Y;

			position += dir * partVel.Value * Time.DeltaTime;
		}

		rotation += rotate * Time.DeltaTime;
		counter += Time.DeltaTime;

		if (counter >= lifeTime)
		{
			emitter.particles.Remove(this);
			Pool.Add(this);
		}
	}
	public void Render() => Draw.Sprite(sprite, position + emitter.DrawOffset, partArea, partColor ?? Color.White, rotation, origin + emitter.ParticleOriginDistortion, partScale ?? Vector2.One, 0);
}
