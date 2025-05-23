# Window
```csharp
public sealed class Window
```
Representa a janela do jogo.<br />
## Window
```csharp
public Window(string title, Claw.Vector2 size) { }
```
## Enabled
```csharp
public bool Enabled;
```
Define se esta janela faz parte do loop de [Game](/api/Claw/Game.md#Game) .<br />
## OnResize
```csharp
public System.Action OnResize;
```
É executado sempre que o tamanho da janela é alterado.<br />
## OnClose
```csharp
public System.Action OnClose;
```
É executado quando a janela é fechada.<br />
### Observações
Este evento não é chamado caso esta seja a única janela.<br />
## Renderer
```csharp
public readonly Claw.Graphics.Renderer Renderer;
```
Rendereizador ligado à esta janela.<br />
## MouseVisible
```csharp
public bool MouseVisible { get; set; } 
```
## Borderless
```csharp
public bool Borderless { get; set; } 
```
## FullScreen
```csharp
public bool FullScreen { get; set; } 
```
## CanUserResize
```csharp
public bool CanUserResize { get; set; } 
```
## RelativeMouseMode
```csharp
public bool RelativeMouseMode { get; set; } 
```
Se verdadeiro, o cursor ficará escondido e limitado as bordas da janela.<br />
## AlwaysOnTop
```csharp
public bool AlwaysOnTop { get; set; } 
```
## IsActive
```csharp
public bool IsActive { get; } 
```
Diz se a janela está em foco (selecionada).<br />
## IsMouseFocused
```csharp
public bool IsMouseFocused { get; } 
```
Diz se o mouse está dentro da janela.<br />
## IsFocused
```csharp
public bool IsFocused { get; } 
```
Diz se o mouse está dentro da janela e ela está em foco (selecionada).<br />
## Title
```csharp
public string Title { get; set; } 
```
## Location
```csharp
public Claw.Vector2 Location { get; set; } 
```
## Size
```csharp
public Claw.Vector2 Size { get; set; } 
```
## MinSize
```csharp
public Claw.Vector2 MinSize { get; set; } 
```
## Finalize
```csharp
protected virtual void Finalize() { }
```
## Dispose
```csharp
public virtual void Dispose() { }
```
## Close
```csharp
public void Close() { }
```
Fecha esta janela.<br />
### Observações
Se esta for a única janela existente, fecha o jogo.<br />
## Centralize
```csharp
public void Centralize() { }
```
Centraliza a janela.<br />
## Restore
```csharp
public void Restore() { }
```
Restaura o estado da janela.<br />
## Maximize
```csharp
public void Maximize() { }
```
Maximiza a janela.<br />
## SetMousePosition
```csharp
public void SetMousePosition(Claw.Vector2 position) { }
```
Altera a posição do mouse, relativo a janela.<br />
## SetCursor
```csharp
public void SetCursor(Claw.Cursor cursor) { }
```
Altera o cursor atual.<br />
## TurnOnTextInput
```csharp
public void TurnOnTextInput() { }
```
Começa o processamento dos eventos de digitação.<br />
## TurnOffTextInput
```csharp
public void TurnOffTextInput() { }
```
Encera o processamento dos eventos de digitação.<br />
