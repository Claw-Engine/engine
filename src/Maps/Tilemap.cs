using Claw.Graphics;
using Claw.Utils;

namespace Claw.Maps;

/// <summary>
/// Representa um mapa de tiles.
/// </summary>
public abstract class Tilemap
{
	#region Default
	/// <summary>
	/// Define quantos tiles fora da view serão desenhados (1 por padrão).
	/// </summary>
	public static int OutOfView = 1;
	public int LayerCount => layers.Count;
	public Vector2 GridSize;
	public Vector2 Size
	{
		get => _size;
		set
		{
			if (value.Y >= 0 && value.X >= 0)
			{
				for (int layerIndex = 0; layerIndex < LayerCount; layerIndex++) this[layerIndex].data = InternalUtils.ResizeList(this[layerIndex].data, value, _size);

				_size = value;
			}
		}
	}
	/// <summary>
	/// É executado quando um tile é mudado ([novo tile], [layer], [posição do tile]).
	/// </summary>
	public Action<int, TileLayer, Vector2> OnTileChange = null;
	private Vector2 _size = Vector2.Zero;
	private List<TileLayer> layers = new List<TileLayer>();
	internal Dictionary<string, int> layerIndexes = new Dictionary<string, int>();
	internal List<TilePalette> tileSets = new List<TilePalette>();

	/// <summary>
	/// Retorna uma layer.
	/// </summary>
	public TileLayer this[int layerIndex] => layers[layerIndex];
	/// <summary>
	/// Retorna uma layer.
	/// </summary>
	public TileLayer this[string layerName] => layers[layerIndexes[layerName]];

	public Tilemap(){}
	public Tilemap(Vector2 size, Vector2 gridSize)
	{
		Size = size;
		GridSize = gridSize;
	}

	/// <summary>
	/// Transforma um index 2D em um index 1D.
	/// </summary>
	public int GetTileIndex(int palette, Vector2 index) => (int)(index.Y * (tileSets[palette].Texture.Width / GridSize.X) + index.X);

	/// <summary>
	/// Adiciona uma paleta ao <see cref="Tilemap"/>.
	/// </summary>
	public void AddPalette(Sprite palette, int margin = 0, int spacing = 0) => AddPalette(palette, GridSize, margin, spacing);
	/// <summary>
	/// Adiciona uma paleta ao <see cref="Tilemap"/>.
	/// </summary>
	public void AddPalette(Sprite palette, Vector2 gridSize, int margin = 0, int spacing = 0)
	{
		int firstIndex = 1;

		if (tileSets.Count > 0)
		{
			var last = tileSets[tileSets.Count - 1];
			firstIndex = last.FirstIndex + last.TileCount;
		}

		var tileset = new TilePalette() { Index = tileSets.Count, FirstIndex = firstIndex, Texture = palette, GridSize = gridSize, Margin = margin, Spacing = spacing };

		if (palette != null)
		{
			Vector2 grid = GridSize + new Vector2(spacing);
			tileset.TileCount = (int)((palette.Width / grid.X) * (palette.Height / grid.Y));
		}

		if (tileSets.Count != 0)
		{
			var previous = tileSets[tileSets.Count - 1];
			tileset.Sub = previous.Sub + previous.TileCount;
		}

		tileSets.Add(tileset);
	}

	/// <summary>
	/// Adiciona uma layer nova.
	/// </summary>
	/// <returns>O index da layer.</returns>
	public int AddLayer(string name, float opacity, Color color)
	{
		if (!layerIndexes.FirstOrDefault(n => n.Key == name).Equals(default(KeyValuePair<string, int>))) throw new Exception(string.Format("Já existe uma layer \"{0}\" no mapa!", name));

		TileLayer layer = new(layers.Count, name, this, Size) { Color = color, Opacity = opacity };

		layerIndexes.Add(layer.Name, layers.Count);
		layers.Add(layer);

		return layer.index;
	}
	/// <summary>
	/// Adiciona uma layer nova e já insere os tiles dela.
	/// </summary>
	/// <returns>O index da layer.</returns>
	public int AddLayer(string name, bool visible, float opacity, Color color, int[] data)
	{
		if (!layerIndexes.FirstOrDefault(n => n.Key == name).Equals(default(KeyValuePair<string, int>))) throw new Exception(string.Format("Já existe uma layer \"{0}\" no mapa!", name));

		TileLayer layer = new(layers.Count, name, this) { Color = color, Opacity = opacity };
		layer.data = data.ToList();

		layerIndexes.Add(layer.Name, layers.Count);
		layers.Add(layer);

		return layers.Count - 1;
	}
	/// <summary>
	/// Adiciona uma layer.
	/// </summary>
	/// <returns>O index da layer.</returns>
	public int Addlayer(TileLayer layer)
	{
		if (layer.map == null)
		{
			if (!layerIndexes.FirstOrDefault(n => n.Key == layer.Name).Equals(default(KeyValuePair<string, int>))) throw new Exception(string.Format("Já existe uma layer \"{0}\" no mapa!", layer.Name));

			layerIndexes.Add(layer.Name, layers.Count);
			layers.Add(layer);

			layer.index = layers.Count - 1;
			layer.map = this;

			return layer.index;
		}
		else throw new ArgumentException("Essa layer pertence à outro mapa!");
	}
	/// <summary>
	/// Remove uma layer.
	/// </summary>
	public void RemoveLayer(int index)
	{
		layers[index].map = null;

		layerIndexes.Remove(layers[index].Name);
		layers.RemoveAt(index);
	}
	/// <summary>
	/// Remove uma layer.
	/// </summary>
	public void RemoveLayer(string name) => RemoveLayer(layerIndexes[name]);

	/// <summary>
	/// Verifica se a layer existe.
	/// </summary>
	public bool LayerExists(int index) => layers.Count > index;
	/// <summary>
	/// Verifica se a layer existe.
	/// </summary>
	public bool LayerExists(string name) => layerIndexes.Keys.Contains(name);

	/// <summary>
	/// Retorna um tileset.
	/// </summary>
	public TilePalette GetTileset(int tile) => tileSets.Last(t => tile >= t.FirstIndex && tile <= t.FirstIndex + t.TileCount);
	/// <summary>
	/// Retorna a área do tileset que representa o tile.
	/// </summary>
	public void GetTileArea(int tile, TilePalette tileset, out Rectangle area)
	{
		Vector2 index2D = tileset.Get2DIndex(GetRealTile(tile, tileset) - 1);
		Vector2 offset = new Vector2(tileset.Margin) + index2D * new Vector2(tileset.Spacing);

		area = new Rectangle(index2D * tileset.GridSize + offset, tileset.GridSize);
	}
	/// <summary>
	/// Retorna o index real de um tile.
	/// </summary>
	private int GetRealTile(int tileIndex, TilePalette tileset)
	{
		if (tileset.Index == 0) return tileIndex;
		else return tileIndex - tileset.Sub;
	}
	#endregion

	/// <summary>
	/// Tamanho do mapa (em pixels).
	/// </summary>
	public abstract Vector2 PixelSize { get; }

	/// <summary>
	/// Transforma uma posição livre em uma célula.
	/// </summary>
	public abstract Vector2 PositionToCell(Vector2 position);
	/// <summary>
	/// Transforma uma posição livre em uma posição em grid centralizada.
	/// </summary>
	public abstract Vector2 PositionToGrid(Vector2 position);

	/// <summary>
	/// Renderiza uma layer neste mapa.
	/// </summary>
	public abstract void Render(TileLayer layer);
}
