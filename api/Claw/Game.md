# Game
```csharp
public class Game
```
Classe responsável por controlar o jogo.<br />
## Game
```csharp
public Game() { }
```
## OpenWindows
```csharp
public readonly System.Collections.ObjectModel.ReadOnlyCollection<Claw.Window> OpenWindows;
```
## Instance
```csharp
public static Game Instance { get; private set; } 
```
## Window
```csharp
public Claw.Window Window { get; private set; } 
```
Janela em que o jogo está sendo renderizado no momento.<br />
## Renderer
```csharp
public Claw.Graphics.Renderer Renderer { get; } 
```
Atalho para [Window.Renderer](/api/Claw/Window.md#Renderer) .<br />
## Audio
```csharp
public Claw.Audio.AudioManager Audio { get; private set; } 
```
## Modules
```csharp
public Claw.Modules.ModuleManager Modules { get; private set; } 
```
## Finalize
```csharp
protected virtual void Finalize() { }
```
## Dispose
```csharp
public virtual void Dispose() { }
```
## Run
```csharp
public void Run() { }
```
Tenta inicializar o jogo e, se obter sucesso, executa o [Game.Initialize](/api/Claw/Game.md#Initialize) e o game loop.<br />
## Close
```csharp
public void Close() { }
```
## OnClose
```csharp
protected virtual void OnClose() { }
```
## Initialize
```csharp
protected virtual void Initialize() { }
```
## Step
```csharp
protected virtual void Step() { }
```
Roda uma vez para cada janela ativa, antes de [Game.Render](/api/Claw/Game.md#Render) .<br />
## Render
```csharp
protected virtual void Render() { }
```
Roda uma vez para cada janela ativa, depois de [Game.Step](/api/Claw/Game.md#Step) .<br />
