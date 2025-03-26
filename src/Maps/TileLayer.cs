using Claw.Modules;

namespace Claw.Maps;

/// <summary>
/// Camada de <see cref="Module"/>s, anexado à um <see cref="Tilemap"/>.
/// </summary>
public sealed class TileLayer : ModuleLayer
{
	public float Opacity = 1;
	public Color Color;
	public Tilemap Map
	{
		get => _map;
		set
		{
			if (_map != null) _map.layers.Remove(this);

			_map = value;

			if (_map != null) _map.layers.Add(this);
		}
	}
	private Tilemap _map;
	internal List<int> data = new();

	/// <summary>
	/// Cria uma camada de tiles e anexa ela à um <see cref="Tilemap"/>.
	/// </summary>
	public TileLayer(string name, bool triggersInitialize, Tilemap map, Vector2 size) : base(name, triggersInitialize)
	{
		for (int x = 0; x < size.X; x++)
		{
			for (int y = 0; y < size.Y; y++) data.Add(0);
		}

		Map = map;
	}

	/// <summary>
	/// Retorna um tile da layer.
	/// </summary>
	public int GetTile(int index) => data[index];
	/// <summary>
	/// Altera um tile da layer.
	/// </summary>
	public int SetTile(int index, int tile)
	{
		data[index] = tile;

		_map.OnTileChange?.Invoke(tile, this, Mathf.Get2DIndex(index, (int)_map.Size.X));

		return tile;
	}
	/// <summary>
	/// Retorna um tile da layer.
	/// </summary>
	public int GetTile(Vector2 cell) => data[Mathf.Get1DIndex(cell, _map.Size.X)];
	/// <summary>
	/// Altera um tile da layer.
	/// </summary>
	public int SetTile(Vector2 cell, int tile)
	{
		data[Mathf.Get1DIndex(cell, _map.Size.X)] = tile;

		_map.OnTileChange?.Invoke(tile, this, cell);

		return tile;
	}
	/// <summary>
	/// Retorna um tile da layer.
	/// </summary>
	public int GetTile(float x, float y) => data[Mathf.Get1DIndex(x, y, _map.Size.X)];
	/// <summary>
	/// Altera um tile da layer.
	/// </summary>
	public int SetTile(float x, float y, int tile)
	{
		data[Mathf.Get1DIndex(x, y, _map.Size.X)] = tile;

		_map.OnTileChange?.Invoke(tile, this, new Vector2(x, y));

		return tile;
	}
	/// <summary>
	/// Retorna o número de tiles.
	/// </summary>
	public int CountTiles() => data.Count;

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
	/// Checa se um ponto está dentro de uma célula com tile.
	/// </summary>
	public bool CheckCollision(Vector2 position, out int tile)
	{
		tile = 0;
		Vector2 check = _map.PositionToCell(position);

		if (check.X < 0 || check.Y < 0 || check.X >= _map.Size.X || check.Y >= _map.Size.Y) return false;

		tile = GetTile(check);

		return tile > 0;
	}

	public override void Render()
	{
		if (_map != null && Color.A != 0 && Opacity > 0) _map.Render(this);

		base.Render();
	}
}
