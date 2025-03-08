namespace Claw.Graphics;

/// <summary>
/// Define os tipos de blend operation.
/// </summary>
public enum BlendFunction
{
	/// <summary>
	/// <para>Operação aditiva.</para>
	/// <para>dst + src.</para>
	/// </summary>
	Add = 0x1,
	/// <summary>
	/// <para>Operação subtrativa.</para>
	/// <para>dst - src.</para>
	/// </summary>
	Subtract = 0x2,
	/// <summary>
	/// <para>Operação subtrativa reversa.</para>
	/// <para>src - dst.</para>
	/// </summary>
	ReverseSubtract = 0x3,
	/// <summary>
	/// <para>Operação mínima.</para>
	/// <para>min(dst, src).</para>
	/// </summary>
	Min = 0x4,
	/// <summary>
	/// <para>Operação máxima.</para>
	/// <para>max(dst, src).</para>
	/// </summary>
	Max = 0x5
}
