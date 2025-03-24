namespace Claw.Utils;

/// <summary>
/// Funções úteis internamente.
/// </summary>
internal static class InternalUtils
{
	/// <summary>
	/// Transforma uma lista 1D em uma lista 2D.
	/// </summary>
	public static List<List<T>> List1DTo2D<T>(List<T> list, Vector2 size)
	{
		List<List<T>> listG = new List<List<T>>();

		for (int y = 0; y < size.Y; y++)
		{
			listG.Add(new List<T>());

			for (int x = 0; x < size.X; x++) listG[y].Add(list[Mathf.Get1DIndex(new Vector2(x, y), size.X)]);
		}

		return listG;
	}
	/// <summary>
	/// Transforma uma lista 2D em uma lista 1D.
	/// </summary>
	public static List<T> List2DTo1D<T>(List<List<T>> list)
	{
		List<T> listG = new List<T>();

		for (int y = 0; y < list.Count; y++)
		{
			for (int x = 0; x < list[y].Count; x++) listG.Add(list[y][x]);
		}

		return listG;
	}

	/// <summary>
	/// Altera o tamanho de uma lista unidimensional, como se fosse bidimensional.
	/// </summary>
	public static List<T> ResizeList<T>(List<T> list, Vector2 newSize, Vector2 oldSize)
	{
		Vector2 add = newSize - oldSize;
		var result = InternalUtils.List1DTo2D(list, oldSize);
		int start = result.Count;

		if (add.Y < 0) result.RemoveRange(result.Count + (int)add.Y, (int)Math.Abs(add.Y));
		else if (add.Y > 0)
		{
			for (int y = 0; y < add.Y; y++)
			{
				result.Add(new List<T>());

				for (int x = 0; x < newSize.X; x++) result[start + y].Add(default);
			}
		}

		if (add.X > 0)
		{
			for (int y = 0; y < newSize.Y; y++)
			{
				for (int i = 0; i < add.X; i++) result[y].Add(default);
			}
		}
		else if (add.X < 0)
		{
			for (int y = 0; y < newSize.Y; y++)
			{
				List<T> line = result[y];

				line.RemoveRange(line.Count + (int)add.X, (int)Math.Abs(add.X));
			}
		}

		return InternalUtils.List2DTo1D(result);
	}
}
