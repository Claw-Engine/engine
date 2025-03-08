using Claw.Graphics;

namespace Claw.Maps;

/// <summary>
/// Representa um mapa ortogonal de tiles.
/// </summary>
public class OrthogonalTilemap : Tilemap
{
	public override Vector2 PixelSize => Size * GridSize;

	public OrthogonalTilemap(){}
	public OrthogonalTilemap(Vector2 size, Vector2 gridSize) : base(size, gridSize){}

	public override Vector2 PositionToCell(Vector2 position) => Mathf.ToGrid(position, GridSize) / GridSize;
	public override Vector2 PositionToGrid(Vector2 position) => Mathf.ToGrid(position, GridSize) + GridSize * .5f;

	public override void Render(TileLayer layer)
	{
		if (tileSets.Count > 0 && GridSize != Vector2.Zero && Size != Vector2.Zero)
		{
			CameraState camera = Draw.GetCamera()?.State ?? CameraState.Neutral;
			Vector2 start = Mathf.ToGrid(camera.TopLeft, GridSize) / GridSize - new Vector2(OutOfView);
			Vector2 end = Mathf.ToGrid(camera.BottomRight, GridSize) / GridSize + new Vector2(OutOfView * 2);

			start = Vector2.Clamp(start, Vector2.Zero, Size);
			end = Vector2.Clamp(end, Vector2.Zero, Size);
			TilePalette tileset = null;

			for (int y = (int)start.Y; y < end.Y; y++)
			{
				for (int x = (int)start.X; x < end.X; x++)
				{
					int tile = layer[x, y];

					if (tile >= 1)
					{
						if (tileset == null || !tileset.Contains(tile)) tileset = GetTileset(tile);

						GetTileArea(tile, tileset, out Rectangle area);
						Draw.Sprite(tileset.Texture, (new Vector2(x, y) * GridSize) - new Vector2(0, tileset.GridSize.Y - GridSize.Y), area, layer.Color * layer.Opacity, 0, Vector2.Zero, Vector2.One, Flip.None);
					}
				}
			}
		}
	}
}
