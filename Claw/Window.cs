using System;
using System.Collections.Generic;
using Claw.Graphics;

namespace Claw
{
    /// <summary>
    /// Representa a janela do jogo.
    /// </summary>
    public sealed class Window : IDisposable
    {
        public bool MouseVisible
        {
            get => SDL.SDL_ShowCursor(SDL.SDL_QUERY) == SDL.SDL_ENABLE ? true : false;
            set => SDL.SDL_ShowCursor(value ? SDL.SDL_ENABLE : SDL.SDL_DISABLE);
        }
        public bool Borderless
        {
            get => (SDL.SDL_GetWindowFlags(sdlWindow) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS) == (uint)SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS;
            set => SDL.SDL_SetWindowBordered(sdlWindow, !value ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
        }
        public bool FullScreen
        {
            get => (SDL.SDL_GetWindowFlags(sdlWindow) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) == (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
            set => SDL.SDL_SetWindowFullscreen(sdlWindow, value ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : 0);
        }
        public bool CanUserResize
        {
            get => (SDL.SDL_GetWindowFlags(sdlWindow) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE) == (uint)SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
            set => SDL.SDL_SetWindowResizable(sdlWindow, value ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
        }
        /// <summary>
        /// Se verdadeiro, o cursor ficará escondido e limitado as bordas da janela.
        /// </summary>
        public bool RelativeMouseMode
        {
            get => SDL.SDL_GetRelativeMouseMode() == SDL.SDL_bool.SDL_TRUE;
            set => SDL.SDL_SetRelativeMouseMode(value ? SDL.SDL_bool.SDL_TRUE : SDL.SDL_bool.SDL_FALSE);
        }
        private static Dictionary<Cursor, IntPtr> systemCursors;

        /// <summary>
        /// Diz se a janela está em foco (selecionada).
        /// </summary>
        public bool IsActive => (SDL.SDL_GetWindowFlags(sdlWindow) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) == (uint)SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS;
        /// <summary>
        /// Diz se o mouse está dentro da janela.
        /// </summary>
        public bool IsMouseFocused => (SDL.SDL_GetWindowFlags(sdlWindow) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS) == (uint)SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS;
        /// <summary>
        /// Diz se o mouse está dentro da janela e ela está em foco (selecionada).
        /// </summary>
        public bool IsFocused => (SDL.SDL_GetWindowFlags(sdlWindow) & ((uint)SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS | (uint)SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS)) == ((uint)SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS | (uint)SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS);

        public string Title
        {
            get => SDL.SDL_GetWindowTitle(sdlWindow);
            set => SDL.SDL_SetWindowTitle(sdlWindow, value);
        }
        public Vector2 Location
        {
            get
            {
                SDL.SDL_GetWindowPosition(sdlWindow, out int x, out int y);

                return new Vector2(x, y);
            }
            set => SDL.SDL_SetWindowPosition(sdlWindow, (int)value.X, (int)value.Y);
        }
        public Vector2 Size
        {
            get
            {
                SDL.SDL_GetWindowSize(sdlWindow, out int w, out int h);

                return new Vector2(w, h);
            }
            set => SDL.SDL_SetWindowSize(sdlWindow, (int)value.X, (int)value.Y);
        }
        /// <summary>
        /// É executado sempre que o tamanho da janela é alterado, independentemente de quem causou essa mudança.
        /// </summary>
        public Action ClientResized;
        private IntPtr sdlWindow;

        internal Window(IntPtr window) => sdlWindow = window;
        ~Window() => Dispose();
        
        public void Dispose()
        {
            SDL.SDL_DestroyWindow(sdlWindow);

            sdlWindow = IntPtr.Zero;
        }

        /// <summary>
        /// Centraliza a janela.
        /// </summary>
        public void Centralize() => SDL.SDL_SetWindowPosition(sdlWindow, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED);
        /// <summary>
        /// Restaura o estado da janela.
        /// </summary>
        public void Restore() => SDL.SDL_RestoreWindow(sdlWindow);
        /// <summary>
        /// Maximiza a janela.
        /// </summary>
        public void Maximize() => SDL.SDL_MaximizeWindow(sdlWindow);

        /// <summary>
        /// Altera a posição do mouse, relativo a janela.
        /// </summary>
        public void SetMousePosition(Vector2 position) => SDL.SDL_WarpMouseInWindow(sdlWindow, (int)position.X, (int)position.Y);
        /// <summary>
        /// Altera o cursor atual.
        /// </summary>
        public void SetCursor(Cursor cursor)
        {
            if (systemCursors == null) systemCursors = new Dictionary<Cursor, IntPtr>();

            if (!systemCursors.TryGetValue(cursor, out IntPtr sdlCursor)) sdlCursor = systemCursors[cursor] = SDL.SDL_CreateSystemCursor((SDL.SDL_SystemCursor)cursor);

            SDL.SDL_SetCursor(sdlCursor);
        }
    }
}