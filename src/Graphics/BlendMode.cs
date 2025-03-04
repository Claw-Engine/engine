namespace Claw.Graphics;

/// <summary>
/// Define os tipos de blend mode usados para renderização.
/// </summary>
public enum BlendMode : uint
{
	/// <summary>
	/// Nenhum.
	/// </summary>
	None = 0x00000000,
	/// <summary>
	/// Mistura no alpha.
	/// </summary>
	Blend = 0x00000001,
	/// <summary>
	/// Mistura aditiva.
	/// </summary>
	Add = 0x00000002,
	/// <summary>
	/// Modulação de cores.
	/// </summary>
	Mod = 0x00000004,
	/// <summary>
	/// Multiplicação de cores.
	/// </summary>
	Mul = 0x00000008
}
