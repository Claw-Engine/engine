using System;
using Claw.Graphics;

namespace Claw.Maps
{
    /// <summary>
    /// Representa um mapa isométrico escalado de tiles.
    /// </summary>
    public class StaggeredTilemap : Tilemap
    {
        public override Vector2 PixelSize => new Vector2((float)Math.Floor(Size.X * GridSize.X + GridSize.X * .5f), (float)Math.Floor((Size.Y + 1) * (GridSize.Y * .5f)));
        /// <summary>
        /// Espaçamento entre cada coluna.
        /// </summary>
        internal virtual float VerticalOffset { get; } = .5f;
        /// <summary>
        /// Multiplicador dos chutes do <see cref="PositionToCell(Vector2)"/>.
        /// </summary>
        internal virtual Vector2 GuessGridMultiplier { get; } = Vector2.One;

        public StaggeredTilemap() { }
        public StaggeredTilemap(Vector2 size, Vector2 gridSize) : base(size, gridSize) { }

        public override Vector2 PositionToCell(Vector2 position)
        {
            Vector2 offset = new Vector2(GridSize.X * .5f, GridSize.Y * VerticalOffset);
            Vector2 guess1 = Mathf.ToGrid(position, GridSize * GuessGridMultiplier);
            Vector2 guess2 = Mathf.ToGrid(position - offset, GridSize * GuessGridMultiplier) + offset;
            Vector2 result = guess1;

            if (Vector2.Distance(position, guess2 + GridSize * .5f) <= Vector2.Distance(position, guess1 + GridSize * .5f))
            {
                result.X = guess2.X + -GridSize.X * .5f;
                result.Y = (Mathf.ToGrid(guess2.Y + GridSize.Y * VerticalOffset, (int)(GridSize.Y * VerticalOffset)) / (GridSize.Y * VerticalOffset)) * GridSize.Y - 1;
            }
            else if (result.Y != 0) result.Y = (Mathf.ToGrid(result.Y, (int)(GridSize.Y * VerticalOffset)) / (GridSize.Y * VerticalOffset)) * GridSize.Y;
            
            return Mathf.ToGrid(result, GridSize) / GridSize;
        }
        public override Vector2 PositionToGrid(Vector2 position)
        {
            Vector2 cell = PositionToCell(position);
            
            float addX = (int)cell.Y % 2 != 0 ? GridSize.X * .5f : 0;

            return new Vector2(cell.X * GridSize.X + addX + GridSize.X * .5f, cell.Y * (GridSize.Y * VerticalOffset) + GridSize.Y * .5f);
        }

        public override void Render(TileLayer layer)
        {
            if (tileSets.Count > 0 && GridSize != Vector2.Zero && Size != Vector2.Zero)
            {
                CameraState camera = Draw.GetCamera()?.State ?? CameraState.Neutral;
                Vector2 start = PositionToCell(camera.TopLeft) - new Vector2(OutOfView);
                Vector2 end = PositionToCell(camera.BottomRight) + new Vector2(OutOfView * 2);

                start = Vector2.Clamp(start, Vector2.Zero, Size);
                end = Vector2.Clamp(end, Vector2.Zero, Size);
                TilePalette tileset = null;
                
                for (int y = (int)start.Y; y < end.Y; y++)
                {
                    float addX = (int)y % 2 != 0 ? GridSize.X * .5f : 0;

                    for (int x = (int)start.X; x < end.X; x++)
                    {
                        int tile = layer[x, y];
                        
                        if (tile >= 1)
                        {
                            if (tileset == null || !tileset.Contains(tile)) tileset = GetTileset(tile);

                            GetTileArea(tile, tileset, out Rectangle area);
                            Draw.Sprite(tileset.Texture, (new Vector2(x * GridSize.X + addX, y * (GridSize.Y * VerticalOffset) - (tileset.GridSize.Y - GridSize.Y))), area, layer.Color * layer.Opacity, 0, Vector2.Zero, Vector2.One, Flip.None);
                        }
                    }
                }
            }
        }
    }
}