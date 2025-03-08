namespace Claw;

/// <summary>
/// Guarda as informações de tempo do jogo.
/// </summary>
public static class Time
{
	/// <summary>
	/// FPS desejável (60 por padrão).
	/// </summary>
	public static uint TargetFPS = 60;
	public static uint FrameDelay => 1000 / TargetFPS;

	public static float Scale = 1;
	public static float DeltaTime { get; private set; } = 0;
	public static float UnscaledDeltaTime { get; private set; } = 0;
	public static float FPS { get; private set; } = 0;

	internal static void Update(ulong frameTime)
	{
		UnscaledDeltaTime = (float)(0.001d * frameTime);
		DeltaTime = UnscaledDeltaTime * Scale;
		FPS = (int)Math.Round(1 / UnscaledDeltaTime);
	}
}
