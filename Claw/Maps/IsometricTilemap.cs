using System;
using System.Collections.Generic;
using Claw.Graphics;

namespace Claw.Maps
{
    /// <summary>
    /// Representa um mapa isométrico de tiles.
    /// </summary>
    public class IsometricTilemap : Tilemap
    {
        public override Vector2 PixelSize => Size * GridSize;

        public IsometricTilemap() { }
        public IsometricTilemap(Vector2 size, Vector2 gridSize) : base(size, gridSize) { }

        /// <summary>
        /// Obtém o deslocamento necessário para a renderização do ponto (0,0).
        /// </summary>
        private Vector2 GetOffset() => new Vector2(PixelSize.X * .5f - GridSize.X * .5f, -GridSize.Y);

        public override Vector2 PositionToCell(Vector2 position)
        {
            // Corrigir!!!
            position -= GetOffset() - GridSize * .5f;
            Vector2 cell = new Vector2(position.X / GridSize.X, position.Y / GridSize.Y);
            
            return new Vector2((float)Math.Floor(cell.X + cell.Y), (float)Math.Floor(-cell.X + cell.Y));
        }
        public override Vector2 PositionToGrid(Vector2 position) => CellToPosition(PositionToCell(position)) + GetOffset();
        private Vector2 CellToPosition(Vector2 cell) => new Vector2((cell.X - cell.Y) * (GridSize.X * .5f), (cell.X + cell.Y) * (GridSize.Y * .5f));

        public override void Render(TileLayer layer)
        {
            if (tileSets.Count > 0 && GridSize != Vector2.Zero && Size != Vector2.Zero)
            {
                // Realizar o cálculo de corte!!!
                //CameraState camera = Draw.GetCamera()?.State ?? CameraState.Neutral;
                //Vector2 start = Mathf.GetGridPosition(camera.TopLeft, GridSize) / GridSize - new Vector2(OutOfView);
                //Vector2 end = Mathf.GetGridPosition(camera.BottomRight, GridSize) / GridSize + new Vector2(OutOfView * 2);
                //
                //start = Vector2.Clamp(start, Vector2.Zero, Size);
                //end = Vector2.Clamp(end, Vector2.Zero, Size);
                Vector2 start = Vector2.Zero, end = Size;
                TilePalette tileset = null;
                Vector2 middle = GetOffset();

                for (int y = (int)start.Y; y < end.Y; y++)
                {
                    for (int x = (int)start.X; x < end.X; x++)
                    {
                        int tile = layer[x, y];

                        if (tile >= 1)
                        {
                            if (tileset == null || !tileset.Contains(tile)) tileset = GetTileset(tile);

                            GetTileArea(tile, tileset, out Rectangle area);
                            Draw.Sprite(tileset.Texture, CellToPosition(new Vector2(x, y)) + middle, area, layer.Color * layer.Opacity, 0, Vector2.Zero, Vector2.One, Flip.None);
                        }
                    }
                }
            }
        }
    }
}