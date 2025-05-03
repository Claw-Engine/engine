namespace Claw.Modules;

/// <summary>
/// Descreve um módulo base.
/// </summary>
public abstract class Module
{
	public bool Enabled = true, Visible = true;
	public readonly Transform Transform;
	public ModuleLayer Layer { get; internal set; }
	protected Game Game => Game.Instance;

	public Module() => Transform = new(this);

	public abstract void Initialize();
	public abstract void Step();
	public abstract void Render();

	/// <summary>
	/// Remove este módulo de <see cref="Layer" />.
	/// </summary>
	/// <param name="deleteChildren">Se verdadeiro, também deleta os filhos de <see cref="Transform" /> (o parâmetro também é passado para os filhos).</param>
	public virtual void Delete(bool deleteChildren = true)
	{
		Layer?.Remove(this);

		if (deleteChildren)
		{
			foreach (Transform child in Transform.children) child.Module.Delete(deleteChildren);
		}
	}
}
