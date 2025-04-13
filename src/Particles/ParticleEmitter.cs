using Claw.Extensions;
using Claw.Modules;

namespace Claw.Particles;

/// <summary>
/// Classe de emissão e manuseio de partículas.
/// </summary>
public sealed class ParticleEmitter : Module
{
	public Vector2 DrawOffset = Vector2.Zero, ParticleOriginDistortion = Vector2.Zero;

	public bool Stopped = false, PauseParticles = false;
	/// <summary>
	/// Configuração da emissão de partículas.
	/// </summary>
	public ParticleEmitterConfig Config;
	/// <summary>
	/// Retorna a quantidade de partículas deste <see cref="ParticleEmitter"/>.
	/// </summary>
	public int Count => particles.Count;
	private float spawnCounter = 0;
	internal Random random = new Random();
	internal List<Particle> particles = new List<Particle>();

	public ParticleEmitter() : this(new ParticleEmitterConfig()){}
	public ParticleEmitter(ParticleEmitterConfig config) => Config = config;

	public override void Initialize() { }

	/// <summary>
	/// Retorna a quantidade de partículas que estão esperando na pool.
	/// </summary>
	public static int PoolCount() => Particle.Pool.Count();
	/// <summary>
	/// Limpa a lista de partículas que estão esperando na pool.
	/// </summary>
	public static void ClearPool() => Particle.Pool.Clear();

	/// <summary>
	/// Elimina todas as partículas.
	/// </summary>
	public void Clear() => particles.Clear();

	/// <summary>
	/// Emite partículas em uma direção.
	/// </summary>
	/// <param name="direction">Null para deixar o <see cref="ParticleValue{T}"/> como padrão. A direção deve ser em graus.</param>
	public void Emit(Vector2 basePosition, float? direction = null)
	{
		int min = (int)Math.Min(Config.Number.X, Config.Number.Y), max = (int)Math.Max(Config.Number.X, Config.Number.Y);
		int particleNumber = random.Next(min, max);

		for (int i = 0; i < particleNumber; i++)
		{
			Rectangle area = new Rectangle((basePosition + Config.Offset - Config.Range), (basePosition + Config.Offset + Config.Range));
			Vector2 maxPos = new Vector2(area.Width > area.Left ? area.Width + 1 : area.Width, area.Height > area.Top ? area.Height + 1 : area.Height);
			Vector2 position = new Vector2(random.Next(area.Left, (int)maxPos.X), random.Next(area.Top, (int)maxPos.Y));

			Particle particle = Particle.Instantiate(this, Config.Sprite, random.Next(Config.LifeTime.X, Config.LifeTime.Y), Config.Rotation, Config.Rotate, direction, position, basePosition);

			particles.Add(particle);
		}
	}
	
	public override void Step()
	{
		if (Config != null && Config.SpawnTime > 0 && !Stopped)
		{
			spawnCounter += Time.DeltaTime;

			if (spawnCounter >= Config.SpawnTime)
			{
				spawnCounter = 0;
				
				Emit(Transform.Position);
			}
		}

		if (!PauseParticles)
		{
			for (int i = particles.Count - 1; i >= 0; i--) particles[i].Step();
		}
	}
	public override void Render()
	{
		for (int i = particles.Count - 1; i >= 0; i--) particles[i].Render();
	}
}
