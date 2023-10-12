using System;
using System.Collections.Generic;

namespace Claw.Input
{
    /// <summary>
    /// Representa o estado do teclado.
    /// </summary>
    internal class KeyboardState
    {
        private static Array keysValues;

        static KeyboardState() => keysValues = Enum.GetValues(typeof(Keys));

        private byte[] keysState = new byte[(int)SDL.SDL_Scancode.SDL_NUM_SCANCODES];

        public KeyboardState() { }
        public KeyboardState(IntPtr sdlState, int keyNumber) => System.Runtime.InteropServices.Marshal.Copy(sdlState, keysState, 0, keyNumber);

        /// <summary>
        /// Diz se uma tecla está pressionada.
        /// </summary>
        public bool IsKeyDown(Keys key) => keysState[(byte)key] == 1;
        /// <summary>
        /// Diz se uma tecla está solta.
        /// </summary>
        public bool IsKeyUp(Keys key) => keysState[(byte)key] == 0;

        /// <summary>
        /// Retorna uma lista com todas as teclas que estão pressionadas.
        /// </summary>
        /// <returns></returns>
        public List<Keys> GetDownKeys()
        {
            List<Keys> down = new List<Keys>();

            foreach (int i in keysValues)
            {
                if (keysState[i] == 1) down.Add((Keys)i);
            }

            return down;
        }
    }
}