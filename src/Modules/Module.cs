namespace Claw.Modules;

/// <summary>
/// Descreve um módulo base.
/// </summary>
public abstract class Module
{
	public bool Enabled = true, Visible = true;
	public readonly Transform Transform;
	public ModuleLayer Layer { get; internal set; }

	public Module() => Transform = new(this);

	public abstract void Initialize();
	public abstract void Step();
	public abstract void Render();

	/// <summary>
	/// Remove este módulo de <see cref="Layer" />.
	/// </summary>
	public void Delete() => Layer?.Remove(this);
}
