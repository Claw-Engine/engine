namespace Claw.Input;

/// <summary>
/// Realiza os gatilhos de input touch.
/// </summary>
public static class TouchInput
{
	public static event TouchEvent OnPressed, OnReleased;
	public static event MotionEvent OnMoved;
	private static Dictionary<ulong, Dictionary<ulong, int>> ids;
	private static int currentIndex = 0;

	public delegate void TouchEvent(int index, float pressure, Vector2 position);
	public delegate void MotionEvent(int index, float pressure, Vector2 position, Vector2 motion);

	internal static void DownFinger(ulong deviceId, ulong fingerId, float pressure, Vector2 position)
	{
		int index = GetIndex(deviceId, fingerId);

		OnPressed?.Invoke(index, pressure, position * Game.Instance.Window.Size);
	}
	internal static void UpFinger(ulong deviceId, ulong fingerId, float pressure, Vector2 position)
	{
		int index = GetIndex(deviceId, fingerId);

		OnReleased?.Invoke(index, pressure, position * Game.Instance.Window.Size);
	}
	internal static void MotionFinger(ulong deviceId, ulong fingerId, float pressure, Vector2 position, Vector2 motion)
	{
		int index = GetIndex(deviceId, fingerId);

		OnMoved?.Invoke(index, pressure, position * Game.Instance.Window.Size, motion * Game.Instance.Window.Size);
	}
	private static int GetIndex(ulong deviceId, ulong fingerId)
	{
		int index = 0;

		if (ids == null) ids = new();
		else if (!ids.TryGetValue(deviceId, out Dictionary<ulong, int> device))
		{
			index = currentIndex++;

			ids.Add(deviceId, new() { { fingerId, index } });
		}
		else if (!device.TryGetValue(fingerId, out int id))
		{
			index = currentIndex++;

			device.Add(fingerId, index);
		}
		else index = id;

		return index;
	}
}
