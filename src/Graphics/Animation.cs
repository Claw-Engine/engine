namespace Claw.Graphics;

/// <summary>
/// Dados de uma animação para o <see cref="Animator"/>.
/// </summary>
public sealed class Animation
{
	public int FramesPerSecond;
	public string Name;
	public Vector2 Origin;
	public List<Frame> Frames;

	public Animation(int framesPerSecond, string name, Vector2 origin, params Frame[] frames)
	{
		FramesPerSecond = framesPerSecond;
		Frames = frames.ToList();
		Name = name;
		Origin = origin;
	}

	/// <summary>
	/// Gera animação com um spritesheet horizontal.
	/// </summary>
	public static Animation[] GenerateHorizontal(Sprite spriteSheet, int amount, int[] frames, int[] animationFPS, string[] names, Vector2[] origins, Vector2 cellSize, Vector2 offset, Vector2 margin)
	{
		Animation[] anim = new Animation[amount];

		for (int i = 0; i < anim.Length; i++)
		{
			List<Frame> framesL = new List<Frame>();

			for (int j = 0; j < frames[i]; j++) framesL.Add(new Frame(spriteSheet, new Rectangle(offset + new Vector2(j * cellSize.X, i * cellSize.Y) + margin, cellSize - margin * 2)));

			anim[i] = new Animation(animationFPS[i], names[i], origins[i], framesL.ToArray());
		}

		return anim.Length > 0 ? anim : null;
	}
	/// <summary>
	/// Gera animação com um spritesheet horizontal.
	/// </summary>
	public static Animation[] GenerateHorizontal(Sprite spriteSheet, int amount, int frames, int animationFPS, Vector2 origin, Vector2 cellSize, Vector2 offset, Vector2 margin, params string[] names)
	{
		Animation[] anim = new Animation[amount];

		for (int i = 0; i < anim.Length; i++)
		{
			List<Frame> framesL = new List<Frame>();

			for (int j = 0; j < frames; j++) framesL.Add(new Frame(spriteSheet, new Rectangle(offset + new Vector2(j * cellSize.X, i * cellSize.Y) + margin, cellSize - margin * 2)));

			anim[i] = new Animation(animationFPS, names[i], origin, framesL.ToArray());
		}

		return anim.Length > 0 ? anim : null;
	}

	/// <summary>
	/// Gera animação com um spritesheet vertical.
	/// </summary>
	public static Animation[] GenerateVertical(Sprite spriteSheet, int amount, int[] frames, int[] animationFPS, string[] names, Vector2[] origins, Vector2 cellSize, Vector2 offset, Vector2 margin)
	{
		Animation[] anim = new Animation[amount];

		for (int i = 0; i < anim.Length; i++)
		{
			List<Frame> framesL = new List<Frame>();

			for (int j = 0; j < frames[i]; j++) framesL.Add(new Frame(spriteSheet, new Rectangle(offset + new Vector2(i * cellSize.X, j * cellSize.Y) + margin, cellSize - margin * 2)));

			anim[i] = new Animation(animationFPS[i], names[i], origins[i], framesL.ToArray());
		}

		return anim.Length > 0 ? anim : null;
	}
	/// <summary>
	/// Gera animação com um spritesheet vertical.
	/// </summary>
	public static Animation[] GenerateVertical(Sprite spriteSheet, int amount, int frames, int animationFPS, Vector2 origin, Vector2 cellSize, Vector2 offset, Vector2 margin, params string[] names)
	{
		Animation[] anim = new Animation[amount];

		for (int i = 0; i < anim.Length; i++)
		{
			List<Frame> framesL = new List<Frame>();

			for (int j = 0; j < frames; j++) framesL.Add(new Frame(spriteSheet, new Rectangle(offset + new Vector2(i * cellSize.X, j * cellSize.Y) + margin, cellSize - margin * 2)));

			anim[i] = new Animation(animationFPS, names[i], origin, framesL.ToArray());
		}

		return anim.Length > 0 ? anim : null;
	}
}
