namespace Claw.Graphics.UI;

/// <summary>
/// Define os tipos de disposição de elementos.
/// </summary>
public enum LayoutMode
{
	/// <summary>
	/// O elemento deve pular de linha quando passar do limite.
	/// </summary>
	InlineBlock,
	/// <summary>
	/// O elemento deve sempre pular de linha.
	/// </summary>
	Block,
	/// <summary>
	/// O elemento deve seguir sempre na mesma linha.
	/// </summary>
	Inline
}
