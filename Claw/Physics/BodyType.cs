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
		/// Os outros corpos detectam a colisão com ele, mas não reagem.
		/// </summary>
		Trigger
	}
}