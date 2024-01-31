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
        public override Vector2 PixelSize => new Vector2(Size.X * GridSize.X + GridSize.X * .5f, Size.Y * (GridSize.Y * .5f) + GridSize.Y * .5f);

        public StaggeredTilemap() { }
        public StaggeredTilemap(Vector2 size, Vector2 gridSize) : base(size, gridSize) { }

        public override Vector2 PositionToGrid(Vector2 position)
        {
            float cellY = (float)Math.Floor((position.Y + GridSize.Y * .5f) / (GridSize.Y * .5f));
            float addX = (int)cellY % 2 == 0 ? GridSize.X * .5f : 0;
            float cellX = (float)Math.Floor((position.X + addX) / GridSize.X);
            
            return new Vector2((cellX + .5f) * GridSize.X - addX, cellY * (GridSize.Y * .5f));
        }

        public override void Resize(Vector2 newSize)
        {
            if (newSize.Y >= 0 && newSize.X >= 0)
            {
                for (int layerIndex = 0; layerIndex < LayerCount; layerIndex++) this[layerIndex].Data = InternalUtils.ResizeList(this[layerIndex].Data, newSize, Size);

                Size = newSize;
            }
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