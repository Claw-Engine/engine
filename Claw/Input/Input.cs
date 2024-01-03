using System;
using System.Collections.Generic;

namespace Claw.Input
{
    /// <summary>
    /// Funções de input.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// Indica o número de controles conectados.
        /// </summary>
        public static int ControllersCount => controllers.Count;
        /// <summary>
        /// Guarda o scroll do mouse (0, 1 ou -1).
        /// </summary>
        public static int MouseScroll { get; private set; } = 0;
        public static Vector2 MouseMotion { get; private set; } = Vector2.Zero;
        /// <summary>
        /// Define se o input de botões será considerado somente se <see cref="Window.IsActive"/> (true por padrão).
        /// </summary>
        public static bool ButtonNeedFocus = true;
        /// <summary>
        /// Define se o input do mouse será considerado somente se <see cref="Window.IsMouseFocused"/> (true por padrão).
        /// </summary>
        public static bool MouseNeedFocus = true;
        public static Vector2 MousePosition { get; private set; }
        public static List<Keys> DownKeys = new List<Keys>();
        private static bool canButton => ButtonNeedFocus ? Game.Instance.Window.IsActive : true;
        private static bool canMouse => MouseNeedFocus ? Game.Instance.Window.IsMouseFocused : true;
        private static int previousMouseScroll = 0;

        private static int sdlKeyNumber;
        private static IntPtr sdlKeyState;
        private static KeyboardState keyNewState = new KeyboardState(), keyOldState;
        private static MouseState mouseNewState = new MouseState(), mouseOldState;
        private static List<GameController> controllers;

        internal static void Update()
        {
            if (sdlKeyState == IntPtr.Zero) sdlKeyState = SDL.SDL_GetKeyboardState(out sdlKeyNumber);
            
            keyOldState = keyNewState;
            keyNewState = new KeyboardState(sdlKeyState, sdlKeyNumber);
            DownKeys = keyNewState.GetDownKeys();

            mouseOldState = mouseNewState;
            mouseNewState = MouseState.GetState();
            MousePosition = new Vector2(mouseNewState.X, mouseNewState.Y);

            for (int i = 0; i < controllers.Count; i++) controllers[i].Update();
        }
        internal static void UpdateScroll(SDL.SDL_MouseWheelEvent wheelEvent)
        {
            int wheel = wheelEvent.y;

            if (wheelEvent.direction == (uint)SDL.SDL_MouseWheelDirection.SDL_MOUSEWHEEL_FLIPPED) wheel *= -1;

            if (canMouse && previousMouseScroll != wheel) MouseScroll = Math.Sign(wheel);
            else MouseScroll = 0;

            previousMouseScroll = wheel;
        }
        internal static void UpdateMouseMotion(SDL.SDL_MouseMotionEvent motionEvent) => MouseMotion = new Vector2(motionEvent.xrel, motionEvent.yrel);

        /// <summary>
        /// Checa se uma tecla foi pressionada.
        /// </summary>
        public static bool KeyPressed(Keys key) => keyNewState.IsKeyDown(key) && keyOldState.IsKeyUp(key) && canButton;
        /// <summary>
        /// Checa se uma tecla foi solta.
        /// </summary>
        public static bool KeyReleased(Keys key) => keyNewState.IsKeyUp(key) && keyOldState.IsKeyDown(key) && canButton;

        /// <summary>
        /// Checa se uma tecla está sendo pressionada.
        /// </summary>
        public static bool KeyDown(Keys key) => keyNewState.IsKeyDown(key) && canButton;
        /// <summary>
        /// Checa se uma tecla está solta.
        /// </summary>
        public static bool KeyUp(Keys key) => keyNewState.IsKeyUp(key) && canButton;

        /// <summary>
        /// Checa se um botão do mouse foi pressionado.
        /// </summary>
        public static bool MouseButtonPressed(MouseButtons button)
        {
            if (!canMouse) return false;

            switch (button)
            {
                case MouseButtons.Left: return mouseNewState.Left && !mouseOldState.Left;
                case MouseButtons.Right: return mouseNewState.Right && !mouseOldState.Right;
                case MouseButtons.Middle: return mouseNewState.Middle && !mouseOldState.Middle;
                case MouseButtons.X1: return mouseNewState.X1 && !mouseOldState.X1;
                case MouseButtons.X2: return mouseNewState.X2 && !mouseOldState.X2;
            }

            return false;
        }
        /// <summary>
        /// Checa se um botão do mouse foi solto.
        /// </summary>
        public static bool MouseButtonReleased(MouseButtons button)
        {
            if (!canMouse) return false;

            switch (button)
            {
                case MouseButtons.Left: return !mouseNewState.Left && mouseOldState.Left;
                case MouseButtons.Right: return !mouseNewState.Right && mouseOldState.Right;
                case MouseButtons.Middle: return !mouseNewState.Middle && mouseOldState.Middle;
                case MouseButtons.X1: return !mouseNewState.X1 && mouseOldState.X1;
                case MouseButtons.X2: return !mouseNewState.X2 && mouseOldState.X2;
            }

            return false;
        }

        /// <summary>
        /// Checa se um botão do mouse está sendo pressionado.
        /// </summary>
        public static bool MouseButtonDown(MouseButtons button)
        {
            if (!canMouse) return false;

            switch (button)
            {
                case MouseButtons.Left: return mouseNewState.Left;
                case MouseButtons.Right: return mouseNewState.Right;
                case MouseButtons.Middle: return mouseNewState.Middle;
                case MouseButtons.X1: return mouseNewState.X1;
                case MouseButtons.X2: return mouseNewState.X2;
            }

            return false;
        }
        /// <summary>
        /// Checa se um botão do mouse está solto.
        /// </summary>
        public static bool MouseButtonUp(MouseButtons button)
        {
            if (!canMouse) return false;

            switch (button)
            {
                case MouseButtons.Left: return !mouseNewState.Left;
                case MouseButtons.Right: return !mouseNewState.Right;
                case MouseButtons.Middle: return !mouseNewState.Middle;
                case MouseButtons.X1: return !mouseNewState.X1;
                case MouseButtons.X2: return !mouseNewState.X2;
            }

            return false;
        }

        /// <summary>
        /// Retorna o número de botões do mouse pressionados.
        /// </summary>
        public static int DownMouseButtons()
        {
            int count = 0;

            for (int i = 0; i <= 4; i++)
            {
                if (MouseButtonDown((MouseButtons)i)) count++;
            }

            return count;
        }

        /// <summary>
        /// Retorna o tipo do controle especificado.
        /// </summary>
        public static ControllerTypes GetControllerType(int index) => controllers[index].Type;

        /// <summary>
        /// Checa se um botão do controle foi pressionado.
        /// </summary>
        public static bool GamePadButtonPressed(int index, Buttons button) => controllers[index].IsCurrentButtonDown(button) && !controllers[index].IsOldButtonDown(button) && canButton;
        /// <summary>
        /// Checa se um botão do controle foi solto.
        /// </summary>
        public static bool GamePadButtonReleased(int index, Buttons button) => !controllers[index].IsCurrentButtonDown(button) && controllers[index].IsOldButtonDown(button) && canButton;

        /// <summary>
        /// Checa se um botão do controle está sendo pressionado.
        /// </summary>
        public static bool GamePadButtonDown(int index, Buttons button) => controllers[index].IsCurrentButtonDown(button) && canButton;
        /// <summary>
        /// Checa se um botão do controle está solto.
        /// </summary>
        public static bool GamePadButtonUp(int index, Buttons button) => !controllers[index].IsCurrentButtonDown(button) && canButton;

        /// <summary>
        /// Retorna a quantidade de botões do controle que estão sendo pressionados.
        /// </summary>
        public static int DownGamePadButtons(int index)
        {
            int count = 0;

            foreach (int value in Enum.GetValues(typeof(Buttons)))
            {
                if (controllers[index].IsCurrentButtonDown((Buttons)value)) count++;
            }

            return count;
        }

        /// <summary>
        /// Retorna a posição (de -1 a 1) do analógico esquerdo do controle.
        /// </summary>
        public static Vector2 LeftThumbStick(int index) => controllers[index].LeftThumbStick;
        /// <summary>
        /// Retorna a posição (de -1 a 1) do analógico direito do controle.
        /// </summary>
        public static Vector2 RightThumbStick(int index) => controllers[index].RightThumbStick;

        /// <summary>
        /// Retorna o eixo do gatilho esquerdo.
        /// </summary>
        /// <returns>De 0 a 1.</returns>
        public static float LeftTrigger(int index) => controllers[index].LeftTrigger;
        /// <summary>
        /// Retorna o eixo do gatilho direito.
        /// </summary>
        /// <returns>De 0 a 1.</returns>
        public static float RightTrigger(int index) => controllers[index].RightTrigger;

        /// <summary>
        /// Muda a vibração do controle.
        /// </summary>
        /// <param name="duration">Duração da vibração, em milissegundos.</param>
        /// <param name="leftMotor">Intensidade da vibração do motor esquerdo (de 0 a 1).</param>
        /// <param name="rightMotor">Intensidade da vibração do motor direito (de 0 a 1).</param>
        public static void SetVibration(int index, uint duration, float leftMotor, float rightMotor) => controllers[index].SetVibration(duration, (ushort)(leftMotor * 0xffff), (ushort)(rightMotor * 0xffff));

        /// <summary>
        /// Carrega os controles que já estão conectados, se ainda não foram carregados.
        /// </summary>
        internal static void SetControllers()
        {
            if (controllers == null)
            {
                controllers = new List<GameController>();

                for (int i = 0; i < SDL.SDL_NumJoysticks(); i++)
                {
                    if (SDL.SDL_IsGameController(i) == SDL.SDL_bool.SDL_TRUE) controllers.Add(new GameController(SDL.SDL_GameControllerOpen(i)));
                }
            }
        }
        /// <summary>
        /// Adiciona um novo controle na lista.
        /// </summary>
        internal static void AddController(int id)
        {
            controllers.Add(new GameController(SDL.SDL_GameControllerOpen(id)));

            controllers[controllers.Count - 1].Update();
        }
        /// <summary>
        /// Adiciona um controle na lista.
        /// </summary>
        internal static void RemoveController(int id)
        {
            for (int i = controllers.Count - 1; i > 0; i--)
            {
                if (controllers[i].Compare(id))
                {
                    controllers[i].Dispose();
                    controllers.RemoveAt(i);

                    return;
                }
            }
        }
    }
}