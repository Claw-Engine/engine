namespace Claw.Graphics.UI;

/// <summary>
/// Elemento base, para criação de interfaces gráficas.
/// </summary>
public abstract class Element
{
	public string Name = string.Empty;
	/// <summary>
	/// Posição relativa do elemento, para fins de renderização.
	/// </summary>
	/// <remarks>Objetos, como o <see cref="Container"/>, devem alterar os valores deste campo.</remarks> 
	public Vector2 Position;
	public abstract Vector2 Size { get; }
	/// <summary>
	/// Define a disposição deste elemento (<see cref="LayoutMode.InlineBlock"/> por padrão).
	/// </summary>
	public LayoutMode Layout = LayoutMode.InlineBlock;

	/// <summary>
	/// Verifica se este elemento contém um ponto.
	/// </summary>
	/// <param name="relativePoint">Ponto de verificação, relativo à <see cref="Position"/>.</param>
	public virtual bool Contains(Vector2 relativePoint) => relativePoint.X >= 0 && relativePoint.Y >= 0 && relativePoint.X < Size.X && relativePoint.Y < Size.Y;

	/// <param name="relativeCursor">Posição do mouse, relativo à <see cref="Position"/>.</param>
	/// <returns>Se houve alguma mudança espacial.</returns>
	public abstract bool Step(Vector2 relativeCursor);
	public abstract void Render();
}
