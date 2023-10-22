using System;
using System.Collections.Generic;
using System.Linq;
using Claw.Graphics;
using Claw.Extensions;
using Claw.Physics;

namespace Claw
{
    /// <summary>
    /// Realiza a rendrização e a organização de um tilemap ortogonal.
    /// </summary>
    public sealed class Tilemap
    {
        /// <summary>
        /// Define quantos tiles fora da view serão desenhados (1 por padrão).
        /// </summary>
        public static int OutOfView = 1;
        public int LayerCount => layers.Count;
        public Vector2 GridSize = Vector2.Zero;
        public Vector2 Size
        {
            get { return size; }
            set
            {
                Vector2 crement = value - size;
                Vector2 previous = size;
                size = value;

                if (size.Y > 0 && size.X > 0)
                {
                    foreach (Layer layer in layers)
                    {
                        var layerData = List1DTo2D(layer.data, previous);
                        
                        if (crement.Y < 0) layerData.RemoveRange(layerData.Count - 1 + (int)crement.Y, (int)Math.Abs(crement.Y));
                        else if (crement.Y > 0)
                        {
                            for (int y = 0; y < crement.Y; y++)
                            {
                                layerData.Add(new List<int>());

                                for (int x = 0; x < size.X; x++) layerData[y].Add(0);
                            }
                        }

                        if (crement.X > 0)
                        {
                            for (int y = 0; y < size.Y; y++)
                            {
                                for (int i = 0; i < crement.X; i++) layerData[y].Add(0);
                            }
                        }
                        else if (crement.X < 0)
                        {
                            for (int y = 0; y < size.Y; y++)
                            {
                                List<int> line = layerData[y];

                                line.RemoveRange(line.Count - 1 + (int)crement.X, (int)Math.Abs(crement.X));
                            }
                        }

                        layer.data = List2DTo1D(layerData);
                    }
                }
            }
        }
        /// <summary>
        /// É executado quando um tile é mudado ([novo tile], [nome da layer], [posição do tile]).
        /// </summary>
        public Action<int, string, Vector2> OnTileChange = null;
        private List<Layer> layers = new List<Layer>();
        private Vector2 size = Vector2.Zero;
        internal Dictionary<string, int> layerIndexes = new Dictionary<string, int>();
        internal List<TileSet> tileSets = new List<TileSet>();

        /// <summary>
        /// Retorna uma layer.
        /// </summary>
        public Layer this[int layerIndex] => layers[layerIndex];
        /// <summary>
        /// Retorna uma layer.
        /// </summary>
        public Layer this[string layerName] => layers[layerIndexes[layerName]];

        public Tilemap() { }
        public Tilemap(Vector2 size, Vector2 gridSize)
        {
            this.size = size;
            GridSize = gridSize;
        }

        /// <summary>
        /// Transforma uma lista 1D em uma lista 2D.
        /// </summary>
        private List<List<T>> List1DTo2D<T>(List<T> list, Vector2 Size)
        {
            List<List<T>> listG = new List<List<T>>();

            for (int y = 0; y < Size.Y; y++)
            {
                listG.Add(new List<T>());

                for (int x = 0; x < Size.X; x++) listG[y].Add(list[Mathf.Get1DIndex(new Vector2(x, y), Size)]);
            }

            return listG;
        }
        /// <summary>
        /// Transforma uma lista 2D em uma lista 1D.
        /// </summary>
        private List<T> List2DTo1D<T>(List<List<T>> list)
        {
            List<T> listG = new List<T>();
            
            for (int y = 0; y < list.Count; y++)
            {
                for (int x = 0; x < list[y].Count; x++) listG.Add(list[y][x]);
            }

            return listG;
        }

        /// <summary>
        /// Transforma um index 2D em um index 1D.
        /// </summary>
        public int GetTileIndex(int palette, Vector2 index) => (int)(index.Y * (tileSets[palette].Texture.Width / GridSize.X) + index.X);
        
        /// <summary>
        /// Adiciona uma paleta ao <see cref="Tilemap"/>.
        /// </summary>
        public void AddPalette(Sprite palette, int margin = 0, int spacing = 0)
        {
            int firstIndex = 1;

            if (tileSets.Count > 0)
            {
                var last = tileSets[tileSets.Count - 1];
                firstIndex = last.FirstIndex + last.TileCount;
            }

            var tileset = new TileSet() { Index = tileSets.Count, FirstIndex = firstIndex, Texture = palette, GridSize = GridSize, Margin = margin, Spacing = spacing };
            
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

            var layer = new Layer(layers.Count, name, this, Size) { DrawOrder = drawOrder, Color = color, Opacity = opacity };

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

            var layer = new Layer(layers.Count, name, this) { DrawOrder = drawOrder, Visible = visible, Color = color, Opacity = opacity };
            layer.data = data.ToList();

            layerIndexes.Add(layer.Name, layers.Count);
            layers.Add(layer);

            return layers.Count - 1;
        }
        /// <summary>
        /// Adiciona uma layer.
        /// </summary>
        public int Addlayer(Layer layer)
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
            foreach (Layer layer in layers) Game.Instance.Components.Add(layer);
        }
        /// <summary>
        /// Remove todas as layers dos componentes do jogo.
        /// </summary>
        internal void RemoveAll()
        {
            foreach (Layer layer in layers) Game.Instance.Components.Remove(layer);

        }

        /// <summary>
        /// Retorna um tileset.
        /// </summary>
        private TileSet GetTileset(int tileIndex) => tileSets.Last(t => tileIndex >= t.FirstIndex && tileIndex <= t.FirstIndex + t.TileCount);
        /// <summary>
        /// Retorna o index real de um tile.
        /// </summary>
        private int GetRealTile(int tileIndex, TileSet tileset)
        {
            if (tileset.Index == 0) return tileIndex;
            else return tileIndex - tileset.Sub;
        }

        internal void Render(Layer layer)
        {
            if (tileSets.Count > 0 && GridSize != Vector2.Zero && Size != Vector2.Zero && layers.Count > 0)
            {
                CameraState camera = Draw.GetCamera()?.State ?? CameraState.Neutral;
                Vector2 topLeft = camera.Position - camera.Origin / camera.Zoom;
                Vector2 bottomRight = Game.Instance.Window.Size;

                if (camera.Camera != null)
                {
                    if (camera.viewport.Size != Vector2.Zero) bottomRight = camera.viewport.End;

                    bottomRight = camera.Camera.ScreenToWorld(bottomRight);
                }

                Vector2 start = Mathf.GetGridPosition(topLeft, GridSize) / GridSize - new Vector2(OutOfView);
                Vector2 end = Mathf.GetGridPosition(bottomRight, GridSize) / GridSize + new Vector2(OutOfView * 2);

                start = Vector2.Clamp(start, Vector2.Zero, Size);
                end = Vector2.Clamp(end, Vector2.Zero, Size);
                
                for (int x = (int)start.X; x < end.X; x++)
                {
                    for (int y = (int)start.Y; y < end.Y; y++)
                    {
                        int tile = layer[x, y];
                        
                        if (tile >= 1)
                        {
                            TileSet tileset = GetTileset(tile);
                            Vector2 index2D = tileset.Get2DIndex(GetRealTile(tile, tileset) - 1);
                            Vector2 offset = new Vector2(tileset.Margin) + index2D * new Vector2(tileset.Spacing);
                            Rectangle area = new Rectangle(index2D * GridSize + offset, GridSize);
                            
                            Draw.Sprite(tileset.Texture, (new Vector2(x, y) * GridSize), area, layer.Color * layer.Opacity, 0, Vector2.Zero, Vector2.One, Flip.None);
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Contém os dados de um tileset.
    /// </summary>
    internal sealed class TileSet
    {
        public int Index = 0, FirstIndex = 1, Sub = 0, Margin = 0, Spacing = 0;
        public int TileCount;
        public Vector2 GridSize;
        public Sprite Texture;

        /// <summary>
        /// Transforma um index 1D em um index 2D, considerando o tileset.
        /// </summary>
        public Vector2 Get2DIndex(int index) => Mathf.Get2DIndex(index, (int)(Texture.Width / (GridSize.X + Spacing)));
    }
    /// <summary>
    /// Representa uma camada dentro do <see cref="Tilemap"/>.
    /// </summary>
    public sealed class Layer : IGameComponent, IDrawable
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
            get => drawOrder;
            set
            {
                drawOrder = value;

                DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;

                VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private int drawOrder;
        private bool visible = true;

        /// <summary>
        /// Retorna/muda um tile da layer.
        /// </summary>
        public int this[Vector2 position]
        {
            get => data[Mathf.Get1DIndex(position, map.Size)];
            set
            {
                data[Mathf.Get1DIndex(position, map.Size)] = value;

                map.OnTileChange?.Invoke(value, Name, position);
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

                map.OnTileChange?.Invoke(value, Name, position);
            }
        }

        internal Layer(int index, string name, Tilemap map)
        {
            this.index = index;
            Name = name;
            this.map = map;
        }
        internal Layer(int index, string name, Tilemap map, Vector2 Size) : this(index, name, map)
        {
            if (Game.Instance.Tilemap == map) Game.Instance.Components.Add(this);
            
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++) data.Add(0);
            }
        }

        public void Initialize() { }

        /// <summary>
        /// Retorna todos os tiles da layer.
        /// </summary>
        public int[] GetData() => data.ToArray();

        /// <summary>
        /// Muda vários tiles de uma layer. Esse método não chama o <see cref="OnTileChange"/>!
        /// </summary>
        /// <param name="map">Sequência de tiles (["0,1,2,0,0,1", "0,1,1,3,0,1"]). Sem espaço.</param>
        public void SetMultipleTiles(string[] map)
        {
            for (var y = 0; y < map.Length; y++)
            {
                var lineTiles = map[y].Split(',');

                for (var x = 0; x < lineTiles.Length; x++) data[Mathf.Get1DIndex(new Vector2(x, y), this.map.Size)] = int.Parse(lineTiles[x]);
            }
        }
        /// <summary>
        /// Muda vários tiles de uma layer. Esse método não chama o <see cref="OnTileChange"/>!
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
        /// Muda vários tiles de um chunk imaginário. Esse método não chama o <see cref="OnTileChange"/>!
        /// </summary>
        /// <param name="chunkTiles">Sequência de tiles (["0,1,2,0,0,1", "0,1,1,3,0,1"]). Sem espaço.</param>
        public void SetChunkTiles(Vector2 chunk, string[] chunkTiles)
        {
            Vector2 chunkSize = new Vector2(chunkTiles[0].Replace(",", "").Length, chunkTiles.Length);
            Vector2 start = chunkSize * chunk;

            for (int y = 0; y < chunkTiles.Length; y++)
            {
                var lineTiles = chunkTiles[y].Split(',');

                for (int x = 0; x < lineTiles.Length; x++) data[Mathf.Get1DIndex(new Vector2(x, y) + start, map.Size)] = int.Parse(lineTiles[x]);
            }
        }
        /// <summary>
        /// Muda vários tiles de um chunk imaginário. Esse método não chama o <see cref="OnTileChange"/>!
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
        /// Limpa a layer.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < data.Count; i++) data[i] = 0;
        }

        /// <summary>
        /// Checa se um ponto está colidindo com um tile.
        /// </summary>
        public bool CheckCollision(Vector2 position, out int tile)
        {
            tile = 0;
            Vector2 check = Mathf.GetGridPosition(position, map.GridSize) / map.GridSize;

            if (check.X < 0 || check.Y < 0 || check.X >= map.Size.X || check.Y >= map.Size.Y) return false;

            tile = this[check];
            
            return tile > 0;
        }
        /// <summary>
        /// Checa se um ponto está colidindo com um tile.
        /// </summary>
        public bool CheckCollision(Vector2 position, int[] filterTiles, out int tile)
        {
            CheckCollision(position, out tile);

            return tile > 0 && filterTiles.Contains(tile);
        }

        /// <summary>
        /// Lança um raio e retorna o tile com que esse raio colidiu e o ponto da colisão.
        /// </summary>
        /// <param name="maxDistance">Distância em tiles.</param>
        public bool Raycast(Line ray, float maxDistance, out int tile, out Vector2? hitPoint, out Vector2 tileIndex)
        {
            int _tile = 0;
            Vector2 _tileIndex = Vector2.Zero;
            bool hasHit = false;

            RayCaster.Cast(ray, maxDistance, (Vector2 check) =>
            {
                _tile = 0;
                _tileIndex = Vector2.Zero;
                bool hit = false;

                if (check.X >= 0 && check.X < map.Size.X && check.Y >= 0 && check.Y < map.Size.Y)
                {
                    _tileIndex = check;
                    _tile = this[_tileIndex];
                    hit = _tile > 0;
                    hasHit = hit;
                }

                return hit;
            }, out hitPoint, map.GridSize);

            tile = _tile;
            tileIndex = _tileIndex;

            return hasHit;
        }

        /// <summary>
        /// Implementa o algoritmo A* para encontrar um caminho entre duas posições.
        /// </summary>
        /// <param name="diagonalMovement">Define se o pathfind terá movimentos na diagonal.</param>
        /// <returns>Lista de indexes do mapa.</returns>
        public Vector2[] Pathfind(Vector2 start, Vector2 end, bool diagonalMovement = true)
        {
            Vector2 origin = Mathf.GetGridPosition(start, map.GridSize) / map.GridSize;
            Vector2 goal = Mathf.GetGridPosition(end, map.GridSize) / map.GridSize;
            Vector2 max = new Vector2(map.Size.X - 1, map.Size.Y - 1);
            
            if (origin != Vector2.Clamp(origin, Vector2.Zero, max) || goal != Vector2.Clamp(goal, Vector2.Zero,  max)) return new Vector2[0];
            else if (this[origin] != 0 || this[goal] != 0) return new Vector2[0];

            List<Vector2> open = new List<Vector2>() { origin };
            Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
            Dictionary<Vector2, float> cheapest = new Dictionary<Vector2, float>();
            Dictionary<Vector2, float> bestGuess = new Dictionary<Vector2, float>();

            cheapest[origin] = 0;
            bestGuess[origin] = PathCost(origin, goal, diagonalMovement);

            while (open.Count > 0)
            {
                Vector2 best = open.OrderBy(o => bestGuess[o]).ElementAt(0);

                if (best == goal)
                {
                    List<Vector2> path = new List<Vector2>() { best };

                    while (cameFrom.ContainsKey(best))
                    {
                        best = cameFrom[best];

                        path.Insert(0, best);
                    }

                    return path.ToArray();
                }

                open.Remove(best);

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if ((diagonalMovement || (x == 0 && y != 0) || (x != 0 && y == 0)) && !(x == 0 && y == 0))
                        {
                            Vector2 neighbor = best + new Vector2(x, y);
                            
                            if (neighbor.X >= 0 && neighbor.X < map.Size.X && neighbor.Y >= 0 && neighbor.Y < map.Size.Y && this[neighbor] == 0)
                            {
                                float tentativeCheapest = cheapest[best] + Vector2.Distance(best, neighbor);

                                if (!cheapest.ContainsKey(neighbor) || tentativeCheapest < cheapest[neighbor])
                                {
                                    cameFrom[neighbor] = best;
                                    cheapest[neighbor] = tentativeCheapest;
                                    bestGuess[neighbor] = tentativeCheapest + PathCost(neighbor, goal, diagonalMovement);

                                    if (!open.Contains(neighbor)) open.Add(neighbor);
                                }
                            }
                        }
                    }
                }
            }

            return new Vector2[0];
        }
        private int PathCost(Vector2 current, Vector2 goal, bool diagonalMovement)
        {
            if (!diagonalMovement) return (int)Math.Abs(current.X - goal.X) + (int)Math.Abs(current.Y - goal.Y);

            Vector2 dist = new Vector2(Math.Abs(current.X - goal.X), Math.Abs(current.Y - goal.Y));

            return (int)(dist.X + dist.Y + (Math.Sqrt(2) - 2) * Math.Min(dist.X, dist.Y));
        }
        
        public void Render()
        {
            if (map != null && Color.A != 0 && Opacity > 0) map.Render(this);
        }
    }
}