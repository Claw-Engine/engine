using System;
using System.Linq;
using System.Collections.Generic;
using Claw.Graphics;

namespace Claw.Maps
{
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
        public Vector2 GridSize = Vector2.Zero;
        public Vector2 Size { get; protected set; } = Vector2.Zero;
        /// <summary>
        /// É executado quando um tile é mudado ([novo tile], [layer], [posição do tile]).
        /// </summary>
        public Action<int, TileLayer, Vector2> OnTileChange = null;
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

        public Tilemap() { }
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
        public int AddLayer(int drawOrder, string name, float priority, float opacity, Color color)
        {
            if (!layerIndexes.FirstOrDefault(n => n.Key == name).Equals(default(KeyValuePair<string, int>))) throw new Exception(string.Format("Já existe uma layer \"{0}\" no mapa!", name));

            var layer = new TileLayer(layers.Count, name, this, Size) { DrawOrder = drawOrder, Color = color, Opacity = opacity };

            layerIndexes.Add(layer.Name, layers.Count);
            layers.Add(layer);

            return layer.index;
        }
        /// <summary>
        /// Adiciona uma layer nova e já insere os tiles dela.
        /// </summary>
        public int AddLayer(int drawOrder, string name, bool visible, float priority, float opacity, Color color, int[] data)
        {
            if (!layerIndexes.FirstOrDefault(n => n.Key == name).Equals(default(KeyValuePair<string, int>))) throw new Exception(string.Format("Já existe uma layer \"{0}\" no mapa!", name));

            var layer = new TileLayer(layers.Count, name, this) { DrawOrder = drawOrder, Visible = visible, Color = color, Opacity = opacity };
            layer.Data = data.ToList();

            layerIndexes.Add(layer.Name, layers.Count);
            layers.Add(layer);

            return layers.Count - 1;
        }
        /// <summary>
        /// Adiciona uma layer.
        /// </summary>
        public int Addlayer(TileLayer layer)
        {
            if (layer.map == null)
            {
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

            Game.Instance.Components.Remove(layers[index]);
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
        /// Adiciona todas as layers nos componentes do jogo.
        /// </summary>
        internal void AddAll()
        {
            foreach (TileLayer layer in layers) Game.Instance.Components.Add(layer);
        }
        /// <summary>
        /// Remove todas as layers dos componentes do jogo.
        /// </summary>
        internal void RemoveAll()
        {
            foreach (TileLayer layer in layers) Game.Instance.Components.Remove(layer);

        }

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
        /// Transforma uma posição livre em uma posição em grid.
        /// </summary>
        public abstract Vector2 PositionToGrid(Vector2 position);

        /// <summary>
        /// Altera as dimensões do mapa.
        /// </summary>
        public abstract void Resize(Vector2 newSize);

        /// <summary>
        /// Renderiza uma layer neste mapa.
        /// </summary>
        public abstract void Render(TileLayer layer);
    }
}