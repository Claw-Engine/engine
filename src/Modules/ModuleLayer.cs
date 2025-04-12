using System.Collections.ObjectModel;
using Claw.Maps;

namespace Claw.Modules;

/// <summary>
/// Camada de <see cref="Module"/>s.
/// </summary>
public class ModuleLayer : Collection<Module>
{
	/// <summary>
	/// Define se o método <see cref="Module.Initialize"/> será acionado ao inserir módulos.
	/// </summary>
	public readonly bool TriggersInitialize;
	public readonly string Name;
	public TileLayer Tiles;
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
	/// <remarks>
	/// Este método não realiza a chamada de nenhum evento. Para isso, use <see cref="Set"/>.
	/// </remarks>
	protected override void SetItem(int index, Module newModule) => base.SetItem(index, newModule);
	/// <summary>
	/// Remove um <see cref="Module"/> e insere outro no mesmo index, acionando os eventos adequados.
	/// </summary>
	public virtual void Set(int index, Module newModule)
	{
		Module oldModule = this[index];

		if (oldModule != null) HandleRemoved(oldModule);

		this.SetItem(index, newModule);
		
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
		module.Layer = this;

		if (TriggersInitialize) module.Initialize();

		OnAdded?.Invoke(module);
	}
	private void HandleRemoved(Module module)
	{
		module.Layer = null;

		OnRemoved?.Invoke(module);
	}

	public virtual void Step()
	{
		for (int i = 0; i < Count; i++)
		{
			if (this[i].Enabled) this[i].Step();
		}
	}

	public virtual void Render()
	{
		Tiles?.Render();

		for (int i = 0; i < Count; i++)
		{
			if (this[i].Visible) this[i].Render();
		}
	}
}
