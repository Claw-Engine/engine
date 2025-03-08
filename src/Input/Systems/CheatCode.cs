namespace Claw.Input.Systems;

/// <summary>
/// Cria uma sequência de <see cref="InputTag"/>s para fazer um código de trapaça.
/// </summary>
public sealed class CheatCode
{
	/// <summary>
	/// Tempo de tolerância entre um clique e outro.
	/// </summary>
	public static float TimeTolerance = .5f;
	/// <summary>
	/// Diz se a sequência foi completada.
	/// </summary>
	public bool Got { get; private set; } = false;
	/// <summary>
	/// É executado assim que o código é completado.
	/// </summary>
	public Action Completed;
	private int index = 0;
	private float timer = 0;
	private string player;
	private TaggedInput input;
	private string[] tags;

	public CheatCode(string player, TaggedInput input, params string[] tags)
	{
		this.player = player;
		this.input = input;
		this.tags = tags;
	}

	/// <summary>
	/// Reseta o cheatcode para poder ser usado novamente.
	/// </summary>
	public void Reset()
	{
		index = 0;
		timer = 0;
		Got = false;
	}

	/// <summary>
	/// Atualiza o <see cref="Got"/>.
	/// </summary>
	public void Update()
	{
		if (!Got)
		{
			if (index > 0)
			{
				timer += Time.DeltaTime;

				if (timer > TimeTolerance)
				{
					Reset();

					return;
				}
			}

			int downButtons = Input.DownKeys.Count + Input.DownMouseButtons();

			if (input[player].GamePad != -1) downButtons += Input.DownGamePadButtons(input[player].GamePad);

			if (index < tags.Length && downButtons > 0)
			{
				if (downButtons > 1)
				{
					index = 0;
					timer = 0;
				}
				else if (input[player].GetTag<InputTag>(tags[index]).IsPressed)
				{
					index++;
					timer = 0;
				}
			}
			else if (timer > TimeTolerance)
			{
				index = 0;
				timer = 0;
			}

			if (index == tags.Length)
			{
				Got = true;

				Completed?.Invoke();
			}
		}
	}
}
