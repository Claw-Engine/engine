using System;
using System.Text;
using Claw.Graphics;
using Claw.Graphics.UI;
using Claw.Audio;
using Claw.Maps;
using Claw.Physics;

namespace Claw
{
    /// <summary>
    /// Classe central do jogo.
    /// </summary>
    public class Game : IDisposable
    {
        public static Game Instance { get; private set; }
        public bool ConsoleOnly { get; private set; }
        public Window Window { get; private set; }
        public Renderer Renderer { get; private set; }
        public AudioManager Audio { get; private set; }
        public PhysicsManager Physics
        {
            get => _physics;
            set
            {
                if (_physics != value)
                {
					if (_physics != null) _physics.RemoveFrom(Modules);

					if (value != null) value.AddTo(Modules);

					_physics = value;
				}
            }
        }
        public UI UI;
		public Tilemap Tilemap
		{
			get => _tilemap;
			set
			{
				if (value != _tilemap)
				{
					if (_tilemap != null) _tilemap.RemoveAll();

					if (value != null) value.AddAll();

					_tilemap = value;
				}
			}
		}
		public ModuleCollection Modules => _modules;
        private bool isRunning;
        private PhysicsManager _physics;
		private Tilemap _tilemap;
		private ModuleCollection _modules;

		public Game() { }
        ~Game() => Dispose();

        public void Dispose()
        {
            Window?.Dispose();
            Renderer?.Dispose();
            Audio?.Dispose();

            if (_modules != null)
            {
                for (int i = 0; i < _modules.Count; i++)
                {
                    if (_modules[i] is IDisposable dispose) dispose.Dispose();
                }

				_modules = null;
            }

            Window = null;
            Renderer = null;
            Audio = null;
            isRunning = false;
        }

		/// <summary>
		/// Tenta inicializar o jogo e, se obter sucesso, executa o <see cref="Initialize"/> e o game loop.
		/// </summary>
		/// <param name="consoleOnly">
		/// <para>Se verdadeiro, o jogo será aberto apenas pelo console.</para>
		/// <para>Se falso, o jogo abrirá com <see cref="Claw.Window"/> + <see cref="Claw.Graphics.Renderer"/> + <see cref="Claw.Audio.AudioManager"/>.</para>
		/// </param>
		public void Run(bool consoleOnly = false)
        {
            if (isRunning) return;
            else if (Instance != null) throw new Exception("Não é possível rodar duas instâncias de jogo ao mesmo tempo!");

            ConsoleOnly = consoleOnly;

			if (!ConsoleOnly)
            {
				if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) == 0)
				{
					int result = SDL.SDL_CreateWindowAndRenderer(800, 600, 0, out IntPtr window, out IntPtr renderer);

					if (result == 0)
					{
						isRunning = true;
						Instance = this;
						Window = new Window(window);
						Renderer = new Renderer(renderer);
						Audio = new AudioManager();
						Renderer.ClearColor = Color.CornflowerBlue;
						_modules = new ModuleCollection();

                        Input.Input.TurnOffTextInput();
					}
				}
			}
            else if (SDL.SDL_Init(SDL.SDL_INIT_TIMER | SDL.SDL_INIT_EVENTS) == 0)
            {
				isRunning = true;
				Instance = this;
				_modules = new ModuleCollection();
			}

            if (isRunning)
            {
                Input.Input.SetControllers();
                Initialize();

                if (!ConsoleOnly && Texture.Pixel == null)
                {
                    Texture.Pixel = new Texture(1, 1, 0xffffffff);

                    Draw.Initialize();
                }
                
                GameLoop();
            }
        }
        /// <summary>
        /// Fecha o jogo.
        /// </summary>
        public void Close()
        {
            isRunning = false;

            OnClose();
        }

        protected virtual void OnClose() { }
        protected virtual void Initialize() { }
        protected virtual void Step() { }
        protected virtual void Render() { }
        
        private void GameLoop()
        {
            uint frameStart;
            int frameTime = 0;

            while (isRunning)
            {
                frameStart = SDL.SDL_GetTicks();

                if (!ConsoleOnly) Input.Input.Update();
                
                Time.Update(frameTime);
                Physics?.Step();
                Step();

                if (!ConsoleOnly)
                {
					Renderer.Clear();
					Draw.UpdateCamera();
					Render();
					UI?.Render();
					Renderer.Present();
				}

                frameTime = (int)(SDL.SDL_GetTicks() - frameStart);

                if (Time.FrameDelay > frameTime) SDL.SDL_Delay((uint)(Time.FrameDelay - frameTime));

                HandleEvents();
            }

            Clear();
        }
        private void HandleEvents()
        {
            SDL.SDL_Event sdlEvent;
            bool scroll = false;

            while(SDL.SDL_PollEvent(out sdlEvent) != 0)
            {
                switch (sdlEvent.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        isRunning = false;
                        Instance = null;

                        return;
                    case SDL.SDL_EventType.SDL_WINDOWEVENT:
                        switch (sdlEvent.window.windowEvent)
                        {
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED: Window.ClientResized?.Invoke(); break;
                        }
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                        Input.Input.UpdateScroll(sdlEvent.wheel);

                        scroll = true;
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEMOTION: Input.Input.UpdateMouseMotion(sdlEvent.motion); break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED: Input.Input.AddController(sdlEvent.cdevice.which); break;
                    case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED: Input.Input.RemoveController(sdlEvent.cdevice.which); break;
                    case SDL.SDL_EventType.SDL_FINGERDOWN:
                        Input.TouchInput.DownFinger(sdlEvent.tfinger.touchId, sdlEvent.tfinger.fingerId, sdlEvent.tfinger.pressure, new Vector2(sdlEvent.tfinger.x, sdlEvent.tfinger.y));
                        break;
                    case SDL.SDL_EventType.SDL_FINGERUP: 
                        Input.TouchInput.UpFinger(sdlEvent.tfinger.touchId, sdlEvent.tfinger.fingerId, sdlEvent.tfinger.pressure, new Vector2(sdlEvent.tfinger.x, sdlEvent.tfinger.y));
                        break;
                    case SDL.SDL_EventType.SDL_FINGERMOTION:
						Input.TouchInput.MotionFinger(sdlEvent.tfinger.touchId, sdlEvent.tfinger.fingerId, sdlEvent.tfinger.pressure,
                            new Vector2(sdlEvent.tfinger.x, sdlEvent.tfinger.y), new Vector2(sdlEvent.tfinger.dx, sdlEvent.tfinger.dy));
						break;
					case SDL.SDL_EventType.SDL_TEXTINPUT:
                        unsafe
                        {
                            Input.Input.TriggerText(Encoding.UTF8.GetString(sdlEvent.text.text, 32)[0]);
						}
                        break;
                }

                SDL.SDL_PumpEvents();
            }

            if (!scroll) Input.Input.UpdateScroll(new SDL.SDL_MouseWheelEvent());
        }
        private void Clear()
		{
			Dispose();
			SDL.SDL_Quit();
        }
    }
}