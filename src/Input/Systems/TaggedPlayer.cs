namespace Claw.Input.Systems;

/// <summary>
/// Representa a instância de um player dentro do <see cref="TaggedInput"/>.
/// </summary>
public sealed class TaggedPlayer
{
	public int GamePad = -1;
	private Dictionary<string, IBaseTag> tags = new();

	public IBaseTag this[string tagName] => tags[tagName];
	
	/// <summary>
	/// Cria uma instância de player.
	/// </summary>
	/// <param name="gamePad">-1 para não usar o gamepad.</param>
	public TaggedPlayer(int gamePad = -1) => GamePad = gamePad;

	/// <summary>
	/// Adiciona uma tag com botões configurados.
	/// </summary>
	public TaggedPlayer AddTag(string tagName, IBaseTag tag)
	{
		tags.Add(tagName, tag);

		return this;
	}
	/// <summary>
	/// Retorna uma tag.
	/// </summary>
	public T GetTag<T>(string tagName) where T : IBaseTag => (T)tags[tagName];

	/// <summary>
	/// Realiza o Update das tags.
	/// </summary>
	public void Update()
	{
		foreach (KeyValuePair<string, IBaseTag> tag in tags) tag.Value.Update(this);
	}
}
