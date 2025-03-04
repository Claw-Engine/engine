using System.Collections.ObjectModel;
using Claw.Modules;

namespace Claw.Utils;

/// <summary>
/// Camada de <see cref="Module"/>s.
/// </summary>
public sealed class ModuleLayer : Collection<Module>
{
	/// <summary>
	/// Define se o método <see cref="Module.Initialize"/> será acionado ao inserir módulos.
	/// </summary>
	public readonly bool TriggersInitialize;
	public readonly string Name;
	public event Action<Module> ModuleAdded, ModuleRemoved;
	public event Action LayerCleared;

	public ModuleLayer(string name, bool triggersInitialize)
	{
		Name = name;
		TriggersInitialize = triggersInitialize;
	}

	/// <summary>
	/// Insere um <see cref="Module"/> na camada e aciona o evento <see cref="ModuleAdded"/>.
	/// </summary>
	protected override void InsertItem(int index, Module module)
	{
		if (IndexOf(module) != -1) throw new ArgumentException("Não é permitido adicionar o mesmo módulo duas vezes!");

		base.InsertItem(index, module);

		if (module != null) OnModuleAdded(module);
	}
	/// <summary>
	/// Remove um <see cref="Module"/> da camada e aciona o evento <see cref="ModuleRemoved"/>.
	/// </summary>
	protected override void RemoveItem(int index)
	{
		Module module = this[index];

		base.RemoveItem(index);

		if (module != null) OnModuleRemoved(module);
	}
	/// <summary>
	/// Remove um <see cref="Module"/> e insere outro no mesmo index.
	/// </summary>
	protected override void SetItem(int index, Module newModule)
	{
		Module oldModule = this[index];

		if (oldModule != null) OnModuleRemoved(oldModule);

		base.SetItem(index, newModule);
		
		if (newModule != null) OnModuleAdded(newModule);
	}

	/// <summary>
	/// Limpa a camada e aciona o evento <see cref="LayerCleared"/>.
	/// </summary>
	protected override void ClearItems()
	{
		base.ClearItems();
		LayerCleared?.Invoke();
	}

	private void OnModuleAdded(Module module)
	{
		if (TriggersInitialize) module.Initialize();

		ModuleAdded?.Invoke(module);
	}
	private void OnModuleRemoved(Module module) => ModuleRemoved?.Invoke(module);
}
