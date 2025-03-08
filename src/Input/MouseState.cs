using static Claw.SDL;

namespace Claw.Input;

/// <summary>
/// Representa o estado do mouse.
/// </summary>
internal class MouseState
{
	public float X, Y;
	public bool Left, Right, Middle, X1, X2;
	
	public static MouseState GetState()
	{
		MouseState mouse = new MouseState();
		SDL_MouseButtonFlags state = SDL_GetMouseState(out mouse.X, out mouse.Y);

		mouse.Left = (state & SDL_MouseButtonFlags.SDL_BUTTON_LMASK) > 0;
		mouse.Right = (state & SDL_MouseButtonFlags.SDL_BUTTON_RMASK) > 0;
		mouse.Middle = (state & SDL_MouseButtonFlags.SDL_BUTTON_MMASK) > 0;
		mouse.X1 = (state & SDL_MouseButtonFlags.SDL_BUTTON_X1MASK) > 0;
		mouse.X2 = (state & SDL_MouseButtonFlags.SDL_BUTTON_X2MASK) > 0;

		return mouse;
	}
}
