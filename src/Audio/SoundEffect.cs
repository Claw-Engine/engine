namespace Claw.Audio;

/// <summary>
/// Representa um efeito sonoro no jogo.
/// </summary>
public sealed class SoundEffect
{
	public readonly Channels Channels;
	/// <summary>
	/// Duração deste áudio, em segundos.
	/// </summary>
	public readonly float Duration;
	public long Length => samples.LongLength;
	private readonly float[] samples;
	
	/// <param name="samples">Valores entre -1 e 1.</param>
	public SoundEffect(Channels channels, float[] samples)
	{
		Channels = channels;
		this.samples = samples;
		Duration = AudioManager.CalculateDuration(samples.LongLength, Channels);
	}

	/// <summary>
	/// Cria um <see cref="SoundEffectInstance"/> deste áudio.
	/// </summary>
	public SoundEffectInstance CreateInstance(SoundEffectGroup group) => new SoundEffectInstance(this, group);

	/// <summary>
	/// Carrega um áudio.
	/// </summary>
	public static SoundEffect Load(string path)
	{
		BinaryReader file = new BinaryReader(new StreamReader(path).BaseStream);

		if (file.ReadString() != "audio") throw new Exception("Este não é um arquivo de áudio válido!");

		Channels channels = (Channels)file.ReadByte();
		long size = file.ReadInt64();
		float[] samples = new float[size];

		for (long i = 0; i < size; i++) samples[i] = file.ReadSingle();

		file.Close();

		return new SoundEffect(channels, samples);
	}
	/// <summary>
	/// Retorna um sample específico.
	/// </summary>
	internal float GetSample(long position) => samples[position];
}
