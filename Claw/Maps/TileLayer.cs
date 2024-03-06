using System;
using System.Linq;
using System.Collections.Generic;
using Claw.Physics;

namespace Claw.Maps
{
    /// <summary>
    /// Representa uma camada dentro do <see cref="Tilemap"/>.
    /// </summary>
    public sealed class TileLayer : IGameModule, IDrawable
    {
        public float Opacity = 1;
        public string Name
        {
            get => name;
            set
            {
                if (map != null)
                {
                    map.layerIndexes.Add(value, index);
                    map.layerIndexes.Remove(name);
                }

                name = value;
            }
		}
		public Color Color;
        internal int index;
        internal Tilemap map;
        internal List<int> data = new List<int>();
        private string name = string.Empty;

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public int DrawOrder
        {
            get => _drawOrder;
            set
            {
				_drawOrder = value;

                DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public bool Visible
        {
            get => _visible;
            set
            {
				_visible = value;

                VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private int _drawOrder;
        private bool _visible = true;

        /// <summary>
        /// Retorna/muda um tile da layer.
        /// </summary>
        public int this[Vector2 cell]
        {
            get => data[Mathf.Get1DIndex(cell, map.Size)];
            set
            {
                data[Mathf.Get1DIndex(cell, map.Size)] = value;

                map.OnTileChange?.Invoke(value, this, cell);
            }
        }
        /// <summary>
        /// Retorna/muda um tile da layer.
        /// </summary>
        public int this[int x, int y]
        {
            get => data[Mathf.Get1DIndex(new Vector2(x, y), map.Size)];
            set
            {
                var position = new Vector2(x, y);
                data[Mathf.Get1DIndex(position, map.Size)] = value;

                map.OnTileChange?.Invoke(value, this, position);
            }
        }

        internal TileLayer(int index, string name, Tilemap map)
        {
            this.index = index;
            Name = name;
            this.map = map;
        }
        internal TileLayer(int index, string name, Tilemap map, Vector2 size) : this(index, name, map)
        {
            if (Game.Instance.Tilemap == map) Game.Instance.Modules.Add(this);

            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++) data.Add(0);
            }
        }

        public void Initialize() { }

        /// <summary>
        /// Retorna todos os tiles da layer.
        /// </summary>
        public int[] GetData() => data.ToArray();

        /// <summary>
        /// Muda vários tiles de uma layer. Esse método não chama o <see cref="Tilemap.OnTileChange"/>!
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
        /// Muda vários tiles de um chunk imaginário. Esse método não chama o <see cref="Tilemap.OnTileChange"/>!
        /// </summary>
        public void SetChunkTiles(Rectangle chunk, int[] chunkData)
        {
            var end = chunk.Location + chunk.Size;

            for (int x = (int)chunk.Location.X; x < end.X; x++)
            {
                for (int y = (int)chunk.Location.Y; y < end.Y; y++)
                {
                    Vector2 pos = new Vector2(x, y);
                    data[Mathf.Get1DIndex(pos, map.Size)] = chunkData[Mathf.Get1DIndex(pos - chunk.Location, chunk.Size)];
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
            Vector2 check = map.PositionToCell(position);

            if (check.X < 0 || check.Y < 0 || check.X >= map.Size.X || check.Y >= map.Size.Y) return false;

            tile = this[check];

            return tile > 0;
        }
        /// <summary>
        /// Checa se um ponto está dentro de uma célula com tile.
        /// </summary>
        public bool CheckCollision(Vector2 position, int[] filterTiles, out int tile)
        {
            CheckCollision(position, out tile);

            return tile > 0 && filterTiles.Contains(tile);
        }

		public void Render()
        {
            if (map != null && Color.A != 0 && Opacity > 0) map.Render(this);
        }
    }
}