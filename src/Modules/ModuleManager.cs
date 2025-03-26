using Claw.Maps;

namespace Claw.Modules;

/// <summary>
/// Gerenciador de <see cref="Module"/>s.
/// </summary>
public sealed class ModuleManager
{
	/// <summary>
	/// Variável de suporte, indicando os mapas presentes neste gerenciador (null por padrão).
	/// </summary>
	/// <remarks>O mapa estar presente nesta lista não significa, necessariamente, que suas camadas estejam em <see cref="StepLayers"/> ou <see cref="RenderLayers"/>.</remarks>
	public List<Tilemap> Tilemaps;
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
	/// Procura por uma camada em <see cref="StepLayers"/> com determinado nome.
	/// </summary>
	public ModuleLayer GetStepLayer(string name) => FindLayer(name, StepLayers);
	/// <summary>
	/// Procura por uma camada em <see cref="RenderLayers"/> com determinado nome.
	/// </summary>
	public ModuleLayer GetRenderLayer(string name) => FindLayer(name, RenderLayers);
	private ModuleLayer FindLayer(string name, ModuleLayer[] source)
	{
		for (int i = 0; i < source.Length; i++)
		{
			if (source[i].Name == name) return source[i];
		}

		return null;
	}

	/// <summary>
	/// Verifica se um módulo existe em pelo menos uma das camadas.
	/// </summary>
	/// <returns>A camada em que o módulo existe.</returns>
	public ModuleLayer Exists(Module module)
	{
		ModuleLayer result = Exists(module, StepLayers);

		if (result != null) return result;

		return Exists(module, RenderLayers);
	}
	/// <summary>
	/// Verifica se um módulo existe em pelo menos uma das camadas em <see cref="StepLayers"/>.
	/// </summary>
	/// <returns>A camada em que o módulo existe.</returns>
	public ModuleLayer ExistsOnStep(Module module) => Exists(module, StepLayers);
	/// <summary>
	/// Verifica se um módulo existe em pelo menos uma das camadas em <see cref="RenderLayers"/>.
	/// </summary>
	/// <returns>A camada em que o módulo existe.</returns>
	public ModuleLayer ExistsOnRender(Module module) => Exists(module, RenderLayers);
	private ModuleLayer Exists(Module module, ModuleLayer[] source)
	{
		foreach (ModuleLayer layer in source)
		{
			if (layer.Contains(module)) return layer;
		}

		return null;
	}

	/// <summary>
	/// Insere um módulo em <see cref="StepLayers"/> e <see cref="RenderLayers"/>, baseado no nome das camadas.
	/// </summary>
	/// <remarks>
	/// Não há nenhuma verificação interna para garantir que o módulo não exista em outras camadas.
	/// Se necessário, você pode utilizar <see cref="Exists(Module)"/>, <see cref="ExistsOnStep"/> ou <see cref="ExistsOnRender"/> para uma verificação genérica.
	/// </remarks>
	public void AddTo(Module module, string stepLayer, string renderLayer)
	{
		FindLayer(stepLayer, StepLayers).Add(module);
		FindLayer(renderLayer, RenderLayers).Add(module);
	}
	/// <summary>
	/// Insere um módulo em <see cref="StepLayers"/>, baseado no nome da camada.
	/// </summary>
	/// <remarks>
	/// Não há nenhuma verificação interna para garantir que o módulo não exista em outras camadas.
	/// Se necessário, você pode utilizar <see cref="Exists(Module)"/>, <see cref="ExistsOnStep"/> ou <see cref="ExistsOnRender"/> para uma verificação genérica.
	/// </remarks>
	public void AddToStep(Module module, string stepLayer) => FindLayer(stepLayer, StepLayers).Add(module);
	/// <summary>
	/// Insere um módulo em <see cref="RenderLayers"/>, baseado no nome da camada.
	/// </summary>
	/// <remarks>
	/// Não há nenhuma verificação interna para garantir que o módulo não exista em outras camadas.
	/// Se necessário, você pode utilizar <see cref="Exists(Module)"/>, <see cref="ExistsOnStep"/> ou <see cref="ExistsOnRender"/> para uma verificação genérica.
	/// </remarks>
	public void AddToRender(Module module, string renderLayer) => FindLayer(renderLayer, RenderLayers).Add(module);

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
