namespace Claw.Maps;

/// <summary>
/// Camada tiles de um <see cref="Tilemap"/>.
/// </summary>
public sealed class TileLayer
{
	public float Opacity = 1;
	public Color Color = Color.White;
	public Vector2 Size => _size;
	public Tilemap Map
	{
		get => _map;
		set
		{
			if (_map != null) _map.layers.Remove(this);

			_map = value;

			if (_map != null)
			{
				if (_map.Size != _size) Resize();

				_map.layers.Add(this);
			}
		}
	}
	public int this[int index]
	{
		get => data[index];
		set
		{
			data[index] = value;

			_map.OnTileChange?.Invoke(value, this, Mathf.Get2DIndex(index, (int)_map.Size.X));
		}
	}
	public int this[Vector2 cell]
	{
		get => data[Mathf.Get1DIndex(cell, _map.Size.X)];
		set
		{
			data[Mathf.Get1DIndex(cell, _map.Size.X)] = value;

			_map.OnTileChange?.Invoke(value, this, cell);
		}
	}
	public int this[float x, float y]
	{
		get => data[Mathf.Get1DIndex(x, y, _map.Size.X)];
		set
		{
			data[Mathf.Get1DIndex(x, y, _map.Size.X)] = value;

			_map.OnTileChange?.Invoke(value, this, new Vector2(x, y));
		}
	}
	/// <summary>
	/// Retorna o número de tiles.
	/// </summary>
	public int Length => data.Length;
	private Tilemap _map;
	private Vector2 _size;
	private int[] data;

	/// <summary>
	/// Cria uma camada de tiles e anexa ela à um <see cref="Tilemap"/>.
	/// </summary>
	public TileLayer(Tilemap map)
	{
		if (map != null)
		{
			_map = map;
			_size = map.Size;
			data = new int[(int)(_size.X * _size.Y)];

			_map.layers.Add(this);
		}
		else data = new int[0];
	}

	/// <summary>
	/// Muda vários tiles de uma layer. Este método não chama o <see cref="Tilemap.OnTileChange"/>!
	/// </summary>
	public void SetMultipleTiles(int[] mapData)
	{
		for (int i = 0; i < mapData.Length; i++)
		{
			if (i >= data.Length) break;

			data[i] = mapData[i];
		}
	}
	/// <summary>
	/// Muda vários tiles de um chunk imaginário. Este método não chama o <see cref="Tilemap.OnTileChange"/>!
	/// </summary>
	public void SetChunkTiles(Rectangle chunk, int[] chunkData)
	{
		Vector2 end = chunk.Location + chunk.Size;

		for (int x = (int)chunk.Location.X; x < end.X; x++)
		{
			for (int y = (int)chunk.Location.Y; y < end.Y; y++)
			{
				Vector2 pos = new Vector2(x, y);
				data[Mathf.Get1DIndex(pos, _map.Size.X)] = chunkData[Mathf.Get1DIndex(pos - chunk.Location, chunk.Size.X)];
			}
		}
	}

	/// <summary>
	/// Checa se um ponto está dentro de uma célula com tile.
	/// </summary>
	public bool CheckCollision(Vector2 position, out int tile)
	{
		tile = 0;
		Vector2 check = _map.PositionToCell(position);

		if (check.X < 0 || check.Y < 0 || check.X >= _map.Size.X || check.Y >= _map.Size.Y) return false;

		tile = this[check];

		return tile > 0;
	}

	public void Render()
	{
		if (_map != null && Color.A != 0 && Opacity > 0) _map.Render(this);
	}

	internal void Resize()
	{
		data = Array2DTo1D(Array1DTo2D(data, _size, _map.Size), _map.Size);
		_size = _map.Size;
	}
	private static int[,] Array1DTo2D(int[] array, Vector2 oldSize, Vector2 newSize)
	{
		int[,] result = new int[(int)newSize.Y, (int)newSize.X];
		Vector2 minSize = Vector2.Min(oldSize, newSize);

		for (int y = 0; y < minSize.Y; y++)
		{
			for (int x = 0; x < minSize.X; x++) result[y, x] = array[Mathf.Get1DIndex(x, y, oldSize.X)];
		}

		return result;
	}
	public static int[] Array2DTo1D(int[,] array, Vector2 newSize)
	{
		int[] result = new int[(int)(newSize.Y * newSize.X)];

		for (int y = 0; y < newSize.Y; y++)
		{
			for (int x = 0; x < newSize.X; x++) result[Mathf.Get1DIndex(x, y, newSize.X)] = array[y, x];
		}

		return result;
	}
}
