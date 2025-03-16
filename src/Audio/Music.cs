namespace Claw.Audio;

/// <summary>
/// Representa uma música no jogo.
/// </summary>
public sealed class Music : IDisposable
{
	public readonly Channels Channels;
	/// <summary>
	/// Volume da música (entre 0 e 1).
	/// </summary>
	public float Volume
	{
		get => _volume;
		set => _volume = Mathf.Clamp(value, 0, 1);
	}
	/// <summary>
	/// Duração deste áudio, em segundos.
	/// </summary>
	public readonly float Duration;
	/// <summary>
	/// Momento em que o áudio está, em segundos.
	/// </summary>
	public float Current => AudioManager.CalculateDuration((file.BaseStream.Position - AudioStart) / 4, Channels);
	public readonly long Length, AudioStart;
	private float _volume = 1;
	private BinaryReader file;

	private Music(BinaryReader file)
	{
		Channels = (Channels)file.ReadByte();
		Length = file.ReadInt64();
		Duration = AudioManager.CalculateDuration(Length, Channels);
		AudioStart = file.BaseStream.Position;
		this.file = file;
	}
	~Music() => Dispose();

	public void Dispose()
	{
		file?.Close();

		file = null;
	}

	/// <summary>
	/// Carrega um áudio, em modo Stream.
	/// </summary>
	/// <returns>O áudio ou null (se não for um arquivo válido).</returns>
	public static Music Load(string path)
	{
		BinaryReader file = new BinaryReader(new StreamReader(path).BaseStream);

		if (file.ReadString() != "audio") return null;

		return new Music(file);
	}

	/// <summary>
	/// Reseta a posição da stream.
	/// </summary>
	internal void ResetPosition() => file.BaseStream.Position = AudioStart;
	/// <summary>
	/// Retorna o próximo sample.
	/// </summary>
	internal float GetSample(out bool ended)
	{
		ended = false;

		if (file == null) return 0;

		float sample = file.ReadSingle();

		if (file.BaseStream.Position >= file.BaseStream.Length)
		{
			file.BaseStream.Position = AudioStart;
			ended = true;
		}

		return sample;
	}
}
