using Claw.Graphics;

namespace Claw.Maps;

/// <summary>
/// Contém os dados de um tileset.
/// </summary>
public sealed class TilePalette
{
	/// <summary>
	/// Index desta paleta na lista de paletas do <see cref="Tilemap"/>.
	/// </summary>
	public int Index { get; internal set; } = 0;
	/// <summary>
	/// Primeiro tile desta paleta.
	/// </summary>
	public int FirstIndex { get; internal set; } = 1;
	/// <summary>
	/// <para>Quanto deve ser subtraído de um index global para que se chegue ao tile 0 desta paleta.</para>
	/// </summary>
	public int Sub { get; internal set; } = 0;
	/// <summary>
	/// Espaço entre o tile e as bordas da sprite.
	/// </summary>
	public int Margin { get; internal set; } = 0;
	/// <summary>
	/// Espaço entre um tile e outro.
	/// </summary>
	public int Spacing { get; internal set; } = 0;
	/// <summary>
	/// Quantos tiles esta paleta tem.
	/// </summary>
	public int TileCount { get; internal set; }
	/// <summary>
	/// Qual o tamanho de cada tile.
	/// </summary>
	public Vector2 GridSize { get; internal set; }
	/// <summary>
	/// Textura desta paleta.
	/// </summary>
	public Sprite Texture { get; internal set; }

	internal TilePalette() { }

	/// <summary>
	/// Diz se o tile pertence a esta paleta.
	/// </summary>
	public bool Contains(int tile) => tile < FirstIndex || tile > FirstIndex + TileCount - 1;

	/// <summary>
	/// Transforma um index 1D em um index 2D, considerando o tileset.
	/// </summary>
	public Vector2 Get2DIndex(int index) => Mathf.Get2DIndex(index, (int)(Texture.Width / (GridSize.X + Spacing)));
}
