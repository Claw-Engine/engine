# Input
```csharp
public static class Input
```
Funções de input.<br />
## ButtonNeedFocus
```csharp
public static bool ButtonNeedFocus;
```
Define se o input de botões será considerado somente se [Window.IsActive](api/Claw/Window.md#IsActive) (true por padrão).<br />
## MouseNeedFocus
```csharp
public static bool MouseNeedFocus;
```
Define se o input do mouse será considerado somente se [Window.IsMouseFocused](api/Claw/Window.md#IsMouseFocused) (true por padrão).<br />
## DownKeys
```csharp
public static System.Collections.Generic.List<Claw.Input.Keys> DownKeys;
```
## ControllersCount
```csharp
public static int ControllersCount { get; } 
```
Indica o número de controles conectados.<br />
## MouseScroll
```csharp
public static int MouseScroll { get; private set; } 
```
Guarda o scroll do mouse (0, 1 ou -1).<br />
## MouseMotion
```csharp
public static Claw.Vector2 MouseMotion { get; private set; } 
```
## MousePosition
```csharp
public static Claw.Vector2 MousePosition { get; private set; } 
```
## OnControllerAdded
```csharp
public static event System.Action<int> OnControllerAdded;
```
## OnControllerRemoved
```csharp
public static event System.Action<int> OnControllerRemoved;
```
## OnTextInput
```csharp
public static event System.Action<char> OnTextInput;
```
## KeyPressed
```csharp
public static bool KeyPressed(Claw.Input.Keys key) { }
```
Checa se uma tecla foi pressionada.<br />
## KeyReleased
```csharp
public static bool KeyReleased(Claw.Input.Keys key) { }
```
Checa se uma tecla foi solta.<br />
## KeyDown
```csharp
public static bool KeyDown(Claw.Input.Keys key) { }
```
Checa se uma tecla está sendo pressionada.<br />
## KeyUp
```csharp
public static bool KeyUp(Claw.Input.Keys key) { }
```
Checa se uma tecla está solta.<br />
## MouseButtonPressed
```csharp
public static bool MouseButtonPressed(Claw.Input.MouseButtons button) { }
```
Checa se um botão do mouse foi pressionado.<br />
## MouseButtonReleased
```csharp
public static bool MouseButtonReleased(Claw.Input.MouseButtons button) { }
```
Checa se um botão do mouse foi solto.<br />
## MouseButtonDown
```csharp
public static bool MouseButtonDown(Claw.Input.MouseButtons button) { }
```
Checa se um botão do mouse está sendo pressionado.<br />
## MouseButtonUp
```csharp
public static bool MouseButtonUp(Claw.Input.MouseButtons button) { }
```
Checa se um botão do mouse está solto.<br />
## DownMouseButtons
```csharp
public static int DownMouseButtons() { }
```
Retorna o número de botões do mouse pressionados.<br />
## GamePadExists
```csharp
public static bool GamePadExists(int index) { }
```
Checa se existe um gamepad com determinado index.<br />
## GetControllerType
```csharp
public static Claw.Input.ControllerTypes GetControllerType(int index) { }
```
Retorna o tipo do controle especificado.<br />
## GamePadButtonPressed
```csharp
public static bool GamePadButtonPressed(int index, Claw.Input.Buttons button) { }
```
Checa se um botão do controle foi pressionado.<br />
## GamePadButtonReleased
```csharp
public static bool GamePadButtonReleased(int index, Claw.Input.Buttons button) { }
```
Checa se um botão do controle foi solto.<br />
## GamePadButtonDown
```csharp
public static bool GamePadButtonDown(int index, Claw.Input.Buttons button) { }
```
Checa se um botão do controle está sendo pressionado.<br />
## GamePadButtonUp
```csharp
public static bool GamePadButtonUp(int index, Claw.Input.Buttons button) { }
```
Checa se um botão do controle está solto.<br />
## DownGamePadButtons
```csharp
public static int DownGamePadButtons(int index) { }
```
Retorna a quantidade de botões do controle que estão sendo pressionados.<br />
## LeftThumbStick
```csharp
public static Claw.Vector2 LeftThumbStick(int index) { }
```
Retorna a posição (de -1 a 1) do analógico esquerdo do controle.<br />
## RightThumbStick
```csharp
public static Claw.Vector2 RightThumbStick(int index) { }
```
Retorna a posição (de -1 a 1) do analógico direito do controle.<br />
## LeftTrigger
```csharp
public static float LeftTrigger(int index) { }
```
Retorna o eixo do gatilho esquerdo.<br />
**Retorna**: De 0 a 1.<br />
## RightTrigger
```csharp
public static float RightTrigger(int index) { }
```
Retorna o eixo do gatilho direito.<br />
**Retorna**: De 0 a 1.<br />
## SetVibration
```csharp
public static void SetVibration(int index, uint duration, float leftMotor, float rightMotor) { }
```
Muda a vibração do controle.<br />
**duration**: Duração da vibração, em milissegundos.<br />
**leftMotor**: Intensidade da vibração do motor esquerdo (de 0 a 1).<br />
**rightMotor**: Intensidade da vibração do motor direito (de 0 a 1).<br />
