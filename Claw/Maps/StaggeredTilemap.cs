using System;
using System.Collections.Generic;
using Claw.Graphics;
using Claw.Utils;

namespace Claw.Maps
{
    /// <summary>
    /// Representa um mapa isométrico escalado de tiles.
    /// </summary>
    public class StaggeredTilemap : Tilemap
    {
        public override Vector2 PixelSize => new Vector2(Size.X * GridSize.X + GridSize.X * .5f, (Size.Y + 1) * (GridSize.Y * .5f));

        public StaggeredTilemap() { }
        public StaggeredTilemap(Vector2 size, Vector2 gridSize) : base(size, gridSize) { }

        public override Vector2 PositionToCell(Vector2 position)
        {
            Vector2 guess1 = Mathf.GetGridPosition(position, GridSize);
            Vector2 guess2 = Mathf.GetGridPosition(position - GridSize * .5f, GridSize) + GridSize * .5f;
            Vector2 result = guess1;

            if (Vector2.Distance(position, guess2 + GridSize * .5f) <= Vector2.Distance(position, guess1 + GridSize * .5f))
            {
                result.X = guess2.X + -GridSize.X * .5f;
                result.Y = (Mathf.ToGrid(guess2.Y + GridSize.Y * .5f, (int)(GridSize.Y * .5f)) / (GridSize.Y * .5f)) * GridSize.Y - 1;
            }
            else if (result.Y != 0) result.Y = (Mathf.ToGrid(result.Y, (int)(GridSize.Y * .5f)) / (GridSize.Y * .5f)) * GridSize.Y;
            
            return Mathf.GetGridPosition(result, GridSize) / GridSize;
        }
        public override Vector2 PositionToGrid(Vector2 position)
        {
            Vector2 cell = PositionToCell(position);
            float addX = (int)cell.Y % 2 != 0 ? GridSize.X * .5f : 0;

            return new Vector2(cell.X * GridSize.X + addX + GridSize.X * .5f, cell.Y * (GridSize.Y * .5f) + GridSize.Y * .5f);
        }

        public override void Render(TileLayer layer)
        {
            if (tileSets.Count > 0 && GridSize != Vector2.Zero && Size != Vector2.Zero)
            {
                CameraState camera = Draw.GetCamera()?.State ?? CameraState.Neutral;
                Vector2 start = Mathf.GetGridPosition(camera.TopLeft, GridSize) / GridSize - new Vector2(OutOfView);
                Vector2 end = Mathf.GetGridPosition(camera.BottomRight, GridSize) / GridSize + new Vector2(OutOfView * 2);

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
                            Draw.Sprite(tileset.Texture, (new Vector2(x * GridSize.X + addX, y * (GridSize.Y * .5f) - (tileset.GridSize.Y - GridSize.Y))), area, layer.Color * layer.Opacity, 0, Vector2.Zero, Vector2.One, Flip.None);
                        }
                    }
                }
            }
        }
    }
}