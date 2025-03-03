namespace Claw.Modules;

/// <summary>
/// Descreve um módulo base.
/// </summary>
public abstract class Module
{
	public readonly Transform Transform;

	public Module()
	{
		Transform = new(this);
	}

	public abstract void Initialize();
	public abstract void Step();
	public abstract void Render();
}
