using System;
using System.Linq;
using System.Collections.Generic;
using Claw.Physics;

namespace Claw.Maps
{
    /// <summary>
    /// Representa uma camada dentro do <see cref="Tilemap"/>.
    /// </summary>
    public sealed class TileLayer : IGameComponent, IDrawable
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
        public List<int> Data = new List<int>();
        internal int index;
        internal Tilemap map;
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
        public int this[Vector2 cell]
        {
            get => Data[Mathf.Get1DIndex(cell, map.Size)];
            set
            {
                Data[Mathf.Get1DIndex(cell, map.Size)] = value;

                map.OnTileChange?.Invoke(value, this, cell);
            }
        }
        /// <summary>
        /// Retorna/muda um tile da layer.
        /// </summary>
        public int this[int x, int y]
        {
            get => Data[Mathf.Get1DIndex(new Vector2(x, y), map.Size)];
            set
            {
                var position = new Vector2(x, y);
                Data[Mathf.Get1DIndex(position, map.Size)] = value;

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
            if (Game.Instance.Tilemap == map) Game.Instance.Components.Add(this);

            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++) Data.Add(0);
            }
        }

        public void Initialize() { }

        /// <summary>
        /// Muda vários tiles de uma layer. Esse método não chama o <see cref="Tilemap.OnTileChange"/>!
        /// </summary>
        public void SetMultipleTiles(int[] mapData)
        {
            for (int i = 0; i < mapData.Length; i++)
            {
                if (i >= Data.Count) break;

                Data[i] = mapData[i];
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
                    Data[Mathf.Get1DIndex(pos, map.Size)] = chunkData[Mathf.Get1DIndex(pos - chunk.Location, chunk.Size)];
                }
            }
        }
        /// <summary>
        /// Muda cada tile da layer para 0 (vazio).
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Data.Count; i++) Data[i] = 0;
        }

        /// <summary>
        /// Checa se um ponto está dentro de uma célula com tile.
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
        /// Checa se um ponto está dentro de uma célula com tile.
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

            if (origin != Vector2.Clamp(origin, Vector2.Zero, max) || goal != Vector2.Clamp(goal, Vector2.Zero, max)) return new Vector2[0];
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