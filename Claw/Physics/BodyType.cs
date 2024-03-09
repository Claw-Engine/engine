namespace Claw.Physics
{
	/// <summary>
	/// Descreve os tipos de corpos.
	/// </summary>
	public enum BodyType
	{
		/// <summary>
		/// Reage aos outros corpos e os outros corpos reagem à ele.
		/// </summary>
		Normal,
		/// <summary>
		/// <para>Não reage aos outros corpos, mas os outros corpos reagem à ele.</para>
		/// </summary>
		Static,
		/// <summary>
		/// Detecta a colisão com outros corpos, mas os outros corpos não o detectam.
		/// </summary>
		Trigger
	}
}