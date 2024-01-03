using System;
using System.Collections.Generic;

namespace Claw
{
    /// <summary>
    /// Representa os cursores possíveis.
    /// </summary>
    internal class Cursor
    {
        private static Dictionary<SystemCursor, IntPtr> systemCursors;

        /// <summary>
        /// Obtém um cursor do sistema.
        /// </summary>
        internal static IntPtr GetSystemCursor(SystemCursor cursor)
        {
            if (systemCursors == null) systemCursors = new Dictionary<SystemCursor, IntPtr>();

            if (!systemCursors.TryGetValue(cursor, out IntPtr sdlCursor)) sdlCursor = systemCursors[cursor] = SDL.SDL_CreateSystemCursor((SDL.SDL_SystemCursor)cursor);

            return sdlCursor;
        }
    }
}