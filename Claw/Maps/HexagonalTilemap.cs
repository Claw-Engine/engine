using System;
using Claw.Graphics;

namespace Claw.Maps
{
    /// <summary>
    /// Representa um mapa hexagonal escalado de tiles.
    /// </summary>
    public class HexagonalTilemap : StaggeredTilemap
    {
        public override Vector2 PixelSize => new Vector2((float)Math.Floor(Size.X * GridSize.X + GridSize.X * .5f),
            (float)Math.Floor((Size.Y * .75f) * GridSize.Y + GridSize.Y * .25f));
        internal override float VerticalOffset { get; } = .75f;
        internal override Vector2 GuessGridMultiplier { get; } = new Vector2(1, 1.5f);

        public HexagonalTilemap() { }
        public HexagonalTilemap(Vector2 size, Vector2 gridSize) : base(size, gridSize) { }
    }
}