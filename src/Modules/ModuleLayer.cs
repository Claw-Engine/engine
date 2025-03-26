using System.Collections.ObjectModel;

namespace Claw.Modules;

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
	public event Action<Module> OnAdded, OnRemoved;
	public event Action OnCleared;

	public ModuleLayer(string name, bool triggersInitialize)
	{
		Name = name;
		TriggersInitialize = triggersInitialize;
	}

	/// <summary>
	/// Insere um <see cref="Module"/> na camada e aciona o evento <see cref="OnAdded"/>.
	/// </summary>
	protected override void InsertItem(int index, Module module)
	{
		if (module.Layer != null) throw new ArgumentException("Esse módulo já faz parte de uma camada!");

		base.InsertItem(index, module);

		if (module != null) HandleAdded(module);
	}
	/// <summary>
	/// Remove um <see cref="Module"/> da camada e aciona o evento <see cref="OnRemoved"/>.
	/// </summary>
	protected override void RemoveItem(int index)
	{
		Module module = this[index];

		base.RemoveItem(index);

		if (module != null) HandleRemoved(module);
	}
	/// <summary>
	/// Remove um <see cref="Module"/> e insere outro no mesmo index.
	/// </summary>
	protected override void SetItem(int index, Module newModule)
	{
		Module oldModule = this[index];

		if (oldModule != null) HandleRemoved(oldModule);

		base.SetItem(index, newModule);
		
		if (newModule != null) HandleAdded(newModule);
	}

	/// <summary>
	/// Limpa a camada e aciona o evento <see cref="OnCleared"/>.
	/// </summary>
	protected override void ClearItems()
	{
		for (int i = 0; i < Count; i++) this[i].Layer = null;

		base.ClearItems();
		OnCleared?.Invoke();
	}

	private void HandleAdded(Module module)
	{
		if (TriggersInitialize) module.Initialize();

		module.Layer = this;

		OnAdded?.Invoke(module);
	}
	private void HandleRemoved(Module module)
	{
		module.Layer = null;

		OnRemoved?.Invoke(module);
	}
}
