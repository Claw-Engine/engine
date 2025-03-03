namespace Claw.Extensions;

/// <summary>
/// Extensões para a classe <see cref="Random"/>.
/// </summary>
public static class RandomExtensions
{
	/// <summary>
	/// Gera um número quebrado aleatório.
	/// </summary>
	public static float Next(this Random random, float maxValue) => (float)random.NextDouble() * maxValue;
	/// <summary>
	/// Gera um número quebrado aleatório em determinado intervalo.
	/// </summary>
	public static float Next(this Random random, float minValue, float maxValue) => (float)random.NextDouble() * (maxValue - minValue) + minValue;

	/// <summary>
	/// Escholhe um item aleatório de um array.
	/// </summary>
	public static T Choose<T>(this Random random, params T[] array) => array[random.Next(array.Length)];
	/// <summary>
	/// Escholhe um item aleatório de uma lista.
	/// </summary>
	public static T Choose<T>(this Random random, IList<T> list) => list[random.Next(list.Count)];
}
