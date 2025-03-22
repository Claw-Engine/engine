# Game
```csharp
public class Game
```
Classe responsável por controlar o jogo.<br />
## Game
```csharp
public Game() { }
```
## Instance
```csharp
public static Game Instance { get; private set; } 
```
## Window
```csharp
public Claw.Window Window { get; private set; } 
```
## Renderer
```csharp
public Claw.Graphics.Renderer Renderer { get; private set; } 
```
## Audio
```csharp
public Claw.Audio.AudioManager Audio { get; private set; } 
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
## Render
```csharp
protected virtual void Render() { }
```
