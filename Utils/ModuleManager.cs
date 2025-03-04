using Claw.Modules;

namespace Claw.Utils;

/// <summary>
/// Gerenciador de <see cref="Module"/>s.
/// </summary>
public sealed class ModuleManager
{
	public readonly ModuleLayer[] StepLayers, RenderLayers;

	/// <summary>
	/// Cria o gerenciador, com camadas separadas para <see cref="Step"/> e <see cref="Render"/>.
	/// </summary>
	public ModuleManager(ModuleLayer[] stepLayers, ModuleLayer[] renderLayers)
	{
		StepLayers = stepLayers;
		RenderLayers = renderLayers;
	}

	/// <summary>
	/// Insere um módulo em <see cref="StepLayers"/> e <see cref="RenderLayers"/>, baseado no nome das camadas.
	/// </summary>
	public void AddTo(Module module, string stepLayer, string renderLayer)
	{
		FindLayer(stepLayer, StepLayers).Add(module);
		FindLayer(renderLayer, RenderLayers).Add(module);
	}
	/// <summary>
	/// Insere um módulo em <see cref="StepLayers"/>, baseado no nome da camada.
	/// </summary>
	public void AddToStep(Module module, string stepLayer) => FindLayer(stepLayer, StepLayers).Add(module);
	/// <summary>
	/// Insere um módulo em <see cref="RenderLayers"/>, baseado no nome da camada.
	/// </summary>
	public void AddToRender(Module module, string renderLayer) => FindLayer(renderLayer, RenderLayers).Add(module);
	private ModuleLayer FindLayer(string name, ModuleLayer[] source)
	{
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i].Name == name) return source[i];
		}

		return null;
	}

	public void Step()
	{
		foreach (ModuleLayer layer in StepLayers)
		{
			for (int i = 0; i < layer.Count; i++) layer[i].Step();
		}
	}

	public void Render()
	{
		foreach (ModuleLayer layer in RenderLayers)
		{
			for (int i = 0; i < layer.Count; i++) layer[i].Render();
		}
	}
}
