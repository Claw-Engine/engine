using System.Text;
using Claw.Graphics;
using Claw.Audio;
using Claw.Modules;
using static Claw.SDL;

namespace Claw;

/// <summary>
/// Classe responsável por controlar o jogo.
/// </summary>
public class Game : IDisposable
{
	public static Game Instance { get; private set; }
	public Window Window { get; private set; }
	public Renderer Renderer { get; private set; }
	public AudioManager Audio { get; private set; }
	public ModuleManager Modules { get; private set; }
	private bool isRunning;

	public Game(){}
	~Game() => Dispose();

	public void Dispose()
	{
		Window?.Dispose();
        Renderer?.Dispose();
		Audio?.Dispose();

		Window = null;
		Renderer = null;
		Audio = null;
		Modules = null;
		Instance = null;
		isRunning = false;
	}

	/// <summary>
	/// Tenta inicializar o jogo e, se obter sucesso, executa o <see cref="Initialize"/> e o game loop.
	/// </summary>
	public void Run()
	{
		if (isRunning) return;
		else if (Instance != null) throw new Exception("Não é possível rodar duas instâncias de jogo ao mesmo tempo!");

		if (SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_AUDIO | SDL_InitFlags.SDL_INIT_TIMER | SDL_InitFlags.SDL_INIT_EVENTS| SDL_InitFlags.SDL_INIT_GAMEPAD))
		{
			bool result = SDL_CreateWindowAndRenderer("Game", 800, 600, 0, out IntPtr window, out IntPtr renderer);

			if (result)
			{
				isRunning = true;
				Instance = this;
				Window = new(window);
				Renderer = new(renderer);
				Audio = new();
				Modules = new();
				Renderer.ClearColor = Color.CornflowerBlue;
			}
		}

		if (isRunning)
		{
			Input.Input.SetControllers();

			if (Texture.Pixel == null) Texture.Pixel = new Texture(1, 1, 0xffffffff);

			Initialize();
			GameLoop();
		}
	}

	public void Close()
	{
		isRunning = false;

		OnClose();
	}

	protected virtual void OnClose(){}
	protected virtual void Initialize(){}
	protected virtual void Step() => Modules.Step();
	protected virtual void Render() => Modules.Render();

	private void GameLoop()
	{
		ulong frameStart, frameTime = 0;

		while (isRunning)
		{
			frameStart = SDL_GetTicks();

			Input.Input.Update();
			Step();
			Renderer.Clear();
			Draw.UpdateCamera();
			Render();
			Renderer.Present();

			frameTime = (uint)(SDL_GetTicks() - frameStart);

			if (Time.FrameDelay > frameTime)
			{
				SDL_Delay((uint)(Time.FrameDelay - frameTime));
				Time.Update(Time.FrameDelay);
			}
			else Time.Update(frameTime);

			HandleEvents();
		}

		Clear();
	}
	private void HandleEvents()
	{
		SDL_Event sdlEvent;
		bool scroll = false;

		while(SDL_PollEvent(out sdlEvent))
		{
			switch (sdlEvent.type)
			{
				case (uint)SDL_EventType.SDL_EVENT_QUIT:
					isRunning = false;
					Instance = null;

					return;
				case (uint)SDL_EventType.SDL_EVENT_WINDOW_RESIZED: Window.OnResize?.Invoke(); break;
				case (uint)SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
					Input.Input.UpdateScroll(sdlEvent.wheel);

					scroll = true;
					break;
				case (uint)SDL_EventType.SDL_EVENT_MOUSE_MOTION: Input.Input.UpdateMouseMotion(sdlEvent.motion); break;
				case (uint)SDL_EventType.SDL_EVENT_GAMEPAD_ADDED: Input.Input.AddController(sdlEvent.cdevice.which); break;
				case (uint)SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED: Input.Input.RemoveController(sdlEvent.cdevice.which); break;
				case (uint)SDL_EventType.SDL_EVENT_FINGER_DOWN:
					Input.TouchInput.DownFinger(sdlEvent.tfinger.touchID, sdlEvent.tfinger.fingerID, sdlEvent.tfinger.pressure, new Vector2(sdlEvent.tfinger.x, sdlEvent.tfinger.y));
					break;
				case (uint)SDL_EventType.SDL_EVENT_FINGER_UP: 
					Input.TouchInput.UpFinger(sdlEvent.tfinger.touchID, sdlEvent.tfinger.fingerID, sdlEvent.tfinger.pressure, new Vector2(sdlEvent.tfinger.x, sdlEvent.tfinger.y));
					break;
				case (uint)SDL_EventType.SDL_EVENT_FINGER_MOTION:
					Input.TouchInput.MotionFinger(sdlEvent.tfinger.touchID, sdlEvent.tfinger.fingerID, sdlEvent.tfinger.pressure,
						new Vector2(sdlEvent.tfinger.x, sdlEvent.tfinger.y), new Vector2(sdlEvent.tfinger.dx, sdlEvent.tfinger.dy));
					break;
				case (uint)SDL_EventType.SDL_EVENT_TEXT_INPUT:
					unsafe
					{
						Input.Input.TriggerText(Encoding.UTF8.GetString(sdlEvent.text.text, 32)[0]);
					}
					break;
			}

			SDL_PumpEvents();
		}

		if (!scroll) Input.Input.UpdateScroll(new SDL_MouseWheelEvent());
	}
	private void Clear()
	{
		Dispose();
		SDL_Quit();
	}
}
