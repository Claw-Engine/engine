namespace Claw.Modules;

/// <summary>
/// Cuida do manuseio das animações de um <see cref="IAnimatable"/>.
/// </summary>
public sealed class Animator : IDisposable
{
	public int Animation = -1, Frame = 0;
	public int AnimationCount => animations.Count;
	public bool Stop = false, Reverse = false;
	public IAnimatable Animatable;
	public event Action AnimationEnd;
	private int framesPerSecond = 0;
	private float counter = 0;
	private List<Animation> animations = new List<Animation>();
	private Dictionary<string, int> animationIndexes = new Dictionary<string, int>();

	public Animator(){}
	public Animator(params Animation[] animations)
	{
		for (int i = 0; i < animations.Length; i++) AddAnimation(animations[i]);
	}
	~Animator() => Dispose();

	public void Dispose()
	{
		if (animationIndexes != null)
		{
			animationIndexes.Clear();
			animations.Clear();

			animationIndexes = null;
		}
	}

	/// <summary>
	/// Inicia uma animação pelo index.
	/// </summary>
	/// <param name="frame">Frame que a animação vai começar.</param>
	public void Play(int index, bool playIfNot = true, int frame = 0)
	{
		if (!playIfNot || Animation != index)
		{
			Animation = index;
			Frame = frame;
			framesPerSecond = animations[Animation].FramesPerSecond;
			counter = 0;
			Animatable.Origin = animations[Animation].Origin;

			UpdateFrame();
		}
	}
	/// <summary>
	/// Inicia uma animação pelo nome.
	/// </summary>
	/// <param name="frame">Frame que a animação vai começar.</param>
	public void Play(string name, bool playIfNot = true, int frame = 0) => Play(animationIndexes[name], playIfNot, frame);
	/// <summary>
	/// Atualiza o frame do <see cref="GameObject"/>.
	/// </summary>
	private void UpdateFrame()
	{
		if (Frame < 0 || Frame >= animations[Animation].Frames.Count) return;

		Animatable.Sprite = animations[Animation].Frames[Frame].Sprite;
		Animatable.SpriteArea = animations[Animation].Frames[Frame].Area;
		Animatable.Origin = animations[Animation].Origin;
	}

	/// <summary>
	/// Retorna o index de uma animação pelo nome.
	/// </summary>
	public int GetAnimationIndex(string name) => animationIndexes[name];
	/// <summary>
	/// Retorna o nome de uma animação pelo index.
	/// </summary>
	public string GetAnimationName(int index)
	{
		KeyValuePair<string, int> keyValuePair = animationIndexes.FirstOrDefault(a => a.Value == index);

		return keyValuePair.Key;
	}

	/// <summary>
	/// Retorna o número de frames de uma animação pelo nome.
	/// </summary>
	public int Frames(string name) => animations[animationIndexes[name]].Frames.Count;
	/// <summary>
	/// Retorna o número de frames de uma animação pelo index.
	/// </summary>
	public int Frames(int index) => animations[index].Frames.Count;

	/// <summary>
	/// Adiciona uma animação nova.
	/// </summary>
	public void AddAnimation(Animation animation)
	{
		animationIndexes.Add(animation.Name, animationIndexes.Count);
		animations.Add(animation);
	}
	/// <summary>
	/// Remove uma animação pelo nome.
	/// </summary>
	public void RemoveAnimation(string name)
	{
		if (Animation == animationIndexes[name])
		{
			counter = 0;
			Frame = 0;
			Animation--;
		}

		animations.RemoveAt(animationIndexes[name]);
		animationIndexes.Remove(name);

		if (Animation < 0)
		{
			Animation = 0;
			Stop = true;
		}
		else Play(name);
	}
	/// <summary>
	/// Remove uma animação pelo index.
	/// </summary>
	public void RemoveAnimation(int index) => RemoveAnimation(GetAnimationName(index));
	
	public void Run()
	{
		if (Animatable == null) return;
		
		if (animations.Count > 0 && Animation >= 0 && animations[Animation].Frames.Count > 0 && !Stop)
		{
			counter += Time.DeltaTime * (float)framesPerSecond;
			
			if (counter >= 1)
			{
				if (!Reverse)
				{
					if (Frame < animations[Animation].Frames.Count - 1) Frame++;
					else
					{
						AnimationEnd?.Invoke();

						if (!Stop) Frame = 0;
					}
				}
				else
				{
					if (Frame > 0) Frame--;
					else
					{
						AnimationEnd?.Invoke();

						if (!Stop) Frame = animations[Animation].Frames.Count - 1;
					}
				}

				counter = 0;
				
				UpdateFrame();
			}
		}
	}
}
