using System;

namespace Claw.Input
{
    /// <summary>
    /// Representa o estado do mouse.
    /// </summary>
    internal class MouseState
    {
        public int X, Y;
        public bool Left, Right, Middle, X1, X2;
        
        public static MouseState GetState()
        {
            MouseState mouse = new MouseState();
            uint state = SDL.SDL_GetMouseState(out mouse.X, out mouse.Y);

            mouse.Left = (state & SDL.SDL_BUTTON(SDL.SDL_BUTTON_LEFT)) > 0;
            mouse.Right = (state & SDL.SDL_BUTTON(SDL.SDL_BUTTON_RIGHT)) > 0;
            mouse.Middle = (state & SDL.SDL_BUTTON(SDL.SDL_BUTTON_MIDDLE)) > 0;
            mouse.X1 = (state & SDL.SDL_BUTTON(SDL.SDL_BUTTON_X1)) > 0;
            mouse.X2 = (state & SDL.SDL_BUTTON(SDL.SDL_BUTTON_X2)) > 0;

            return mouse;
        }
    }
}