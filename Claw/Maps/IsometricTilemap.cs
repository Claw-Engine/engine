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

        public override Vector2 PositionToCell(Vector2 position)
        {
            return Vector2.Zero;
        }
        public override Vector2 PositionToGrid(Vector2 position)
        {
            return Vector2.Zero;
        }

        public override void Resize(Vector2 newSize)
        {
            
        }

        public override void Render(TileLayer layer)
        {
            
        }
    }
}