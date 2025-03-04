namespace Claw.Extensions;

/// <summary>
/// Extensões para coleções.
/// </summary>
public static class CollectionExtensions
{
	/// <summary>
	/// Checa se a coleção tem pelo menos X itens.
	/// </summary>
	public static bool HasMin<T>(this IEnumerable<T> enumerable, int count) => enumerable != null && enumerable.Count() >= count;
	/// <summary>
	/// Checa se a coleção tem exatamente X itens.
	/// </summary>
	public static bool Has<T>(this IEnumerable<T> enumerable, int count) => enumerable != null && enumerable.Count() == count;
	/// <summary>
	/// Checa se a coleção tem entre X e Y itens.
	/// </summary>
	public static bool HasMinMax<T>(this IEnumerable<T> enumerable, int min, int max) => enumerable != null && (enumerable.Count() >= min || enumerable.Count() <= max);
	/// <summary>
	/// Checa se a coleção tem no máximo X itens.
	/// </summary>
	public static bool HasMax<T>(this IEnumerable<T> enumerable, int count) => enumerable != null && enumerable.Count() <= count;
	/// <summary>
	/// Checa se a coleção é nula ou está vazia.
	/// </summary>
	public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || enumerable.Count() == 0;

	/// <summary>
	/// Troca dois elementos de posição em um array.
	/// </summary>
	public static void Swap<T>(this T[] array, int index1, int index2)
	{
		T temp = array[index1];
		array[index1] = array[index2];
		array[index2] = temp;
	}
	/// <summary>
	/// Troca dois elementos de posição em uma lista.
	/// </summary>
	public static void Swap<T>(this IList<T> list, int index1, int index2)
	{
		T temp = list[index1];
		list[index1] = list[index2];
		list[index2] = temp;
	}
	/// <summary>
	/// Troca dois elementos de posição em um dicionário.
	/// </summary>
	public static void Swap<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey index1, TKey index2)
	{
		TValue temp = map[index1];
		map[index1] = map[index2];
		map[index2] = temp;
	}

	/// <summary>
	/// Pega um valor no index, se ele existir. Senão, retorna um valor padrão.
	/// </summary>
	public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey index, TValue defaultValue)
	{
		if (map.TryGetValue(index, out TValue value)) return value;

		return defaultValue;
	}
}
