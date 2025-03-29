namespace Claw.Modules;

/// <summary>
/// Gerenciador de <see cref="Module"/>s.
/// </summary>
public sealed class ModuleManager
{
	public ModuleLayer this[int index]
	{
		get => Layers[index];
		set => Layers[index] = value;
	}
	public readonly List<ModuleLayer> Layers = new();

	/// <summary>
	/// Procura por uma camada com determinado nome.
	/// </summary>
	public ModuleLayer FindLayer(string name)
	{
		for (int i = 0; i < Layers.Count; i++)
		{
			if (Layers[i].Name == name) return Layers[i];
		}

		return null;
	}

	/// <summary>
	/// Limpa todas as camadas.
	/// </summary>
	public void ClearLayers()
	{
		foreach (ModuleLayer layer in Layers) layer.Clear();
	}

	public void Step()
	{
		foreach (ModuleLayer layer in Layers) layer.Step();
	}

	public void Render()
	{
		foreach (ModuleLayer layer in Layers) layer.Render();
	}
}
