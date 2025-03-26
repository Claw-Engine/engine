using Claw.Modules;

namespace Claw.Maps;

/// <summary>
/// Representa uma camada dentro do <see cref="Tilemap"/>.
/// </summary>
public sealed class TileLayer : Module
{
	public float Opacity = 1;
	public string Name = string.Empty;
	public Color Color;
	public Tilemap Map => _map;
	internal int index;
	internal Tilemap _map;
	internal List<int> data = new();

	/// <summary>
	/// Retorna/altera um tile da layer.
	/// </summary>
	public int this[int index]
	{
		get => data[index];
		set
		{
			data[index] = value;

			_map.OnTileChange?.Invoke(value, this, Mathf.Get2DIndex(index, (int)_map.Size.X));
		}
	}
	/// <summary>
	/// Retorna/altera um tile da layer.
	/// </summary>
	public int this[Vector2 cell]
	{
		get => data[Mathf.Get1DIndex(cell, _map.Size.X)];
		set
		{
			data[Mathf.Get1DIndex(cell, _map.Size.X)] = value;

			_map.OnTileChange?.Invoke(value, this, cell);
		}
	}
	/// <summary>
	/// Retorna/altera um tile da layer.
	/// </summary>
	public int this[float x, float y]
	{
		get => data[Mathf.Get1DIndex(x, y, _map.Size.X)];
		set
		{
			Vector2 position = new(x, y);
			data[Mathf.Get1DIndex(position, _map.Size.X)] = value;

			_map.OnTileChange?.Invoke(value, this, position);
		}
	}
	/// <summary>
	/// Retorna o número de tiles da layer.
	/// </summary>
	public int Length => data.Count;

	internal TileLayer(int index, string name, Tilemap map, Vector2 size)
	{
		for (int x = 0; x < size.X; x++)
		{
			for (int y = 0; y < size.Y; y++) data.Add(0);
		}

		this.index = index;
		Name = name;
		this._map = map;
	}
	internal TileLayer(int index, string name, Tilemap map) : this(index, name, map, Vector2.Zero){}

	public override void Initialize(){}
	public override void Step(){}

	/// <summary>
	/// Muda vários tiles de uma layer. Este método não chama o <see cref="Tilemap.OnTileChange"/>!
	/// </summary>
	public void SetMultipleTiles(int[] mapData)
	{
		for (int i = 0; i < mapData.Length; i++)
		{
			if (i >= data.Count) break;

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
	/// Muda cada tile da layer para 0 (vazio).
	/// </summary>
	public void Clear()
	{
		for (int i = 0; i < data.Count; i++) data[i] = 0;
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

	public override void Render()
	{
		if (_map != null && Color.A != 0 && Opacity > 0) _map.Render(this);
	}
}
