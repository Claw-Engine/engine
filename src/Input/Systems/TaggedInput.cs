namespace Claw.Input.Systems;

/// <summary>
/// Gerencia inputs no jogo através do método chave > valor.
/// </summary>
public sealed class TaggedInput
{
	public static Vector2 DeadAxis = Vector2.Zero;
	private Dictionary<string, TaggedPlayer> players = new();

	/// <summary>
	/// Retorna um player do input.
	/// </summary>
	public TaggedPlayer this[string playerName] => players[playerName];
	/// <summary>
	/// Retorna uma tag do input.
	/// </summary>
	public IBaseTag this[string playerName, string tagName] => players[playerName][tagName];

	public TaggedInput() { }
	
	/// <summary>
	/// Adiciona um player.
	/// </summary>
	public TaggedPlayer AddPlayer(string playerName, TaggedPlayer player)
	{
		players.Add(playerName, player);

		return player;
	}
	/// <summary>
	/// Remove um player.
	/// </summary>
	public void RemovePlayer(string playerName) => players.Remove(playerName);

	/// <summary>
	/// Realiza o update dos players.
	/// </summary>
	public void Update()
	{
		foreach (KeyValuePair<string, TaggedPlayer> player in players) player.Value.Update();
	}
}
