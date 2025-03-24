using static Claw.SDL;

namespace Claw;

/// <summary>
/// Representa a janela do jogo.
/// </summary>
public sealed class Window : IDisposable
{
	public bool MouseVisible
	{
		get => SDL_CursorVisible()? true : false;
		set
		{
			if (value) SDL_ShowCursor();
			else SDL_HideCursor();
		}
	}
	public bool Borderless
	{
		get => (SDL_GetWindowFlags(id) & SDL_WindowFlags.SDL_WINDOW_BORDERLESS) == SDL_WindowFlags.SDL_WINDOW_BORDERLESS;
		set => SDL_SetWindowBordered(id, !value);
	}
	public bool FullScreen
	{
		get => (SDL_GetWindowFlags(id) & SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) == SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
		set => SDL_SetWindowFullscreen(id, value);
	}
	public bool CanUserResize
	{
		get => (SDL_GetWindowFlags(id) & SDL_WindowFlags.SDL_WINDOW_RESIZABLE) == SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
		set => SDL_SetWindowResizable(id, value);
	}
	/// <summary>
	/// Se verdadeiro, o cursor ficará escondido e limitado as bordas da janela.
	/// </summary>
	public bool RelativeMouseMode
	{
		get => SDL_GetWindowRelativeMouseMode(id);
		set => SDL_SetWindowRelativeMouseMode(id, value);
	}
	private static Dictionary<Cursor, IntPtr> systemCursors;

	/// <summary>
	/// Diz se a janela está em foco (selecionada).
	/// </summary>
	public bool IsActive => (SDL_GetWindowFlags(id) & SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) == SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS;
	/// <summary>
	/// Diz se o mouse está dentro da janela.
	/// </summary>
	public bool IsMouseFocused => (SDL_GetWindowFlags(id) & SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS) == SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS;
	/// <summary>
	/// Diz se o mouse está dentro da janela e ela está em foco (selecionada).
	/// </summary>
	public bool IsFocused => (SDL_GetWindowFlags(id) & (SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS | SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS)) == (SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS | SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS);

	public string Title
	{
		get => SDL_GetWindowTitle(id);
		set => SDL_SetWindowTitle(id, value);
	}
	public Vector2 Location
	{
		get
		{
			SDL_GetWindowPosition(id, out int x, out int y);

			return new Vector2(x, y);
		}
		set => SDL_SetWindowPosition(id, (int)value.X, (int)value.Y);
	}
	public Vector2 Size
	{
		get
		{
			SDL_GetWindowSize(id, out int w, out int h);

			return new Vector2(w, h);
		}
		set => SDL_SetWindowSize(id, (int)value.X, (int)value.Y);
	}
	public Vector2 MinSize
	{
		get
		{
			SDL_GetWindowMinimumSize(id, out int w, out int h);

			return new Vector2(w, h);
		}
		set => SDL_SetWindowMinimumSize(id, (int)value.X, (int)value.Y);
	}
	/// <summary>
	/// É executado sempre que o tamanho da janela é alterado.
	/// </summary>
	public Action OnResize;
	private IntPtr id;

	internal Window(IntPtr window) => id = window;
	~Window() => Dispose();

	public void Dispose()
	{
		SDL_DestroyWindow(id);

		id = IntPtr.Zero;
	}

	/// <summary>
	/// Centraliza a janela.
	/// </summary>
	public void Centralize()
	{
		uint display = SDL_GetDisplayForWindow(id);

		if (SDL_GetDisplayBounds(display, out SDL_Rect bounds))
		{
			Vector2 size = Size;
			float pointX = bounds.x + bounds.w * .5f - size.X * .5f;
			float pointY = bounds.y + bounds.h * .5f - size.Y * .5f;

			SDL_SetWindowPosition(id, (int)pointX, (int)pointY);
		}
	}
	/// <summary>
	/// Restaura o estado da janela.
	/// </summary>
	public void Restore() => SDL_RestoreWindow(id);
	/// <summary>
	/// Maximiza a janela.
	/// </summary>
	public void Maximize() => SDL_MaximizeWindow(id);

	/// <summary>
	/// Altera a posição do mouse, relativo a janela.
	/// </summary>
	public void SetMousePosition(Vector2 position) => SDL_WarpMouseInWindow(id, (int)position.X, (int)position.Y);
	/// <summary>
	/// Altera o cursor atual.
	/// </summary>
	public void SetCursor(Cursor cursor)
	{
		if (systemCursors == null) systemCursors = new();

		if (!systemCursors.TryGetValue(cursor, out IntPtr sdlCursor)) sdlCursor = systemCursors[cursor] = SDL_CreateSystemCursor((SDL_SystemCursor)cursor);

		SDL_SetCursor(sdlCursor);
	}

	/// <summary>
	/// Começa o processamento dos eventos de digitação.
	/// </summary>
	public void TurnOnTextInput() => SDL_StartTextInput(id);
	/// <summary>
	/// Encera o processamento dos eventos de digitação.
	/// </summary>
	public void TurnOffTextInput() => SDL_StopTextInput(id);
}
