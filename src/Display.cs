using static Claw.SDL;

namespace Claw;

/// <summary>
/// Representa as telas do dispositivo.
/// </summary>
public class Display
{
	public static List<Display> Instances;
	public static event Action<int> OnAdded, OnRemoved;

	public readonly Rectangle Bounds;
	internal uint id;

	private Display(uint id)
	{
		this.id = id;

		bool success = SDL_GetDisplayBounds(id, out SDL_Rect bounds);

		if (success) Bounds = new(bounds.x, bounds.y, bounds.w, bounds.h);
	}

	/// <summary>
	/// Carrega as telas que já estão conectadas, se ainda não foram carregadas.
	/// </summary>
	internal unsafe static void SetDisplays()
	{
		if (Instances == null)
		{
			Instances = new();
			var displays = (uint*)SDL_GetDisplays(out int count);

			for (int i = 0; i < count; i++) Instances.Add(new(displays[i]));
		}
	}
	/// <summary>
	/// Adiciona uma nova tela na lista.
	/// </summary>
	internal static void AddDisplay(uint id)
	{
		Instances.Add(new(id));

		DisplayAdded?.Invoke(Instances.Count - 1);
	}
	/// <summary>
	/// Remove uma tela da lista.
	/// </summary>
	internal static void RemoveDisplay(uint id)
	{
		for (int i = Instances.Count - 1; i > 0; i--)
		{
			if (Instances[i].id == id)
			{
				Instances.RemoveAt(i);
				DisplayRemoved?.Invoke(i);

				break;
			}
		}
	}
}
