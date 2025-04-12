namespace Claw;

/// <summary>
/// Conjunto de funções matemáticas.
/// </summary>
public static class Mathf
{
	public const float PI = 3.14159265358979f;

	/// <summary>
	/// Checa se um número é aproximadamente igual a outro.
	/// </summary>
	public static bool Approximately(this float a, float b, float tolerance = .25f) => Math.Abs(a - b) <= Math.Abs(tolerance);

	/// <summary>
	/// Transforma graus em radianos.
	/// </summary>
	public static float ToRadians(float degrees) => degrees * PI / 180;
	/// <summary>
	/// Transforma radianos em graus.
	/// </summary>
	public static float ToDegrees(float radians) => radians * 180 / PI;

	/// <summary>
	/// Retorna um número que respeite os limites mínimo e máximo.
	/// </summary>
	public static int Clamp(int value, int min, int max)
	{
		if (value > max) value = max;

		if (value < min) value = min;

		return value;
	}
	/// <summary>
	/// Retorna um número que respeite os limites mínimo e máximo.
	/// </summary>
	public static float Clamp(float value, float min, float max)
	{
		if (value > max) value = max;

		if (value < min) value = min;

		return value;
	}

	/// <summary>
	/// Limita um valor, em forma de loop.
	/// </summary>
	public static float LoopValue(float value, float max, float min = 0)
	{
		if (value < 0) return LoopValue(max - LoopValue(Math.Abs(value), min, max), min, max) + min;

		return value % max + min;
	}

	/// <summary>
	/// Realiza a interpolação linear entre dois valores.
	/// </summary>
	/// <param name="a">Valor atual.</param>
	/// <param name="b">Valor alvo.</param>
	/// <param name="amount">Valor de ponderação (entre 0 e 1).</param>
	public static float Lerp(float a, float b, float amount) => a + (b - a) * amount;
	/// <summary>
	/// Realiza a interpolação linear entre dois valores, usando delta time.
	/// </summary>
	/// <param name="a">Valor atual.</param>
	/// <param name="b">Valor alvo.</param>
	/// <param name="amount">Valor de ponderação.</param>
	/// <param name="scaled">Se o delta time será <see cref="Time.DeltaTime"/> (true) ou <see cref="Time.UnscaledDeltaTime"/> (false).</param>
	public static float DeltaLerp(float a, float b, float amount, bool scaled = true) => Lerp(a, b, 1 - (float)Math.Pow(2, -(scaled ? Time.DeltaTime : Time.UnscaledDeltaTime) * amount));

	/// <summary>
	/// Incrementa um valor por um determinado deslocamento, mas nunca além do valor final.
	/// </summary>
	public static float Approach(float value, float target, float shift)
	{
		shift = Math.Abs(shift);

		if (value < target) return Math.Min(value + shift, target);

		return Math.Max(value - shift, target);
	}

	/// <summary>
	/// Transforma um valor em um valor em grid.
	/// </summary>
	public static float ToGrid(float value, int grid) => (float)Math.Floor(value / grid) * grid;
	/// <summary>
	/// Transforma uma posição em uma posição em grid.
	/// </summary>
	public static Vector2 ToGrid(Vector2 position, int grid) => new Vector2(ToGrid(position.X, grid), ToGrid(position.Y, grid));
	/// <summary>
	/// Transforma uma posição em uma posição em grid.
	/// </summary>
	public static Vector2 ToGrid(Vector2 position, Vector2 grid) => new Vector2(ToGrid(position.X, (int)grid.X), ToGrid(position.Y, (int)grid.Y));

	/// <summary>
	/// Transforma um index 2D em um index 1D.
	/// </summary>
	public static int Get1DIndex(Vector2 index, float width) => (int)(index.Y * width + index.X);
	/// <summary>
	/// Transforma um index 2D em um index 1D.
	/// </summary>
	public static int Get1DIndex(float x, float y, float width) => (int)(y * width + x);
	/// <summary>
	/// Transforma um index 1D em um index 2D.
	/// </summary>
	public static Vector2 Get2DIndex(int index, float width) => new Vector2((int)Math.Floor((double)index % width), (int)(index / width));

	/// <summary>
	/// Retorna uma lista de pontos de uma curva de Bézier.
	/// </summary>
	public static Vector2[] GetBezierPath(int segments, Vector2 point0, Vector2 point1, Vector2 point2, Vector2 point3)
	{
		List<Vector2> points = new List<Vector2>();

		for (int i = 0; i < segments; i++)
		{
			float t = (float)i / segments;
			points.Add(Mathf.CalculateBezierPoint(t, point0, point1, point2, point3));
		}

		return points.ToArray();
	}
	/// <summary>
	/// Retorna um ponto numa curva de Bézier.
	/// </summary>
	public static Vector2 CalculateBezierPoint(float theta, Vector2 point0, Vector2 point1, Vector2 point2, Vector2 point3)
	{
		//(1–t)³P0+3(1–t)²tP1+3(1–t)t²P2+t³P3
		float u = 1 - theta, uSquared = u * u, uCubic = uSquared * u,
			tSquared = theta * theta, tCubic = tSquared * theta;
		Vector2 position = uCubic * point0 + 3 * uSquared * theta * point1 + 3 * u * tSquared * point2 + tCubic * point3;

		return position;
	}
}
