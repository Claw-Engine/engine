# Animation
```csharp
public sealed class Animation
```
Dados de uma animação para o [Animator](/Claw/Graphics/Animator.md#Animator) .<br />
## Animation
```csharp
public Animation(int framesPerSecond, string name, Claw.Vector2 origin, Claw.Graphics.Frame[] frames) { }
```
## FramesPerSecond
```csharp
public int FramesPerSecond;
```
## Name
```csharp
public string Name;
```
## Origin
```csharp
public Claw.Vector2 Origin;
```
## Frames
```csharp
public System.Collections.Generic.List<Claw.Graphics.Frame> Frames;
```
## GenerateHorizontal
```csharp
public static Animation[] GenerateHorizontal(Claw.Graphics.Sprite spriteSheet, int amount, int[] frames, int[] animationFPS, string[] names, Claw.Vector2[] origins, Claw.Vector2 cellSize, Claw.Vector2 offset) { }
```
Gera animação com um spritesheet horizontal.<br />
## GenerateHorizontal
```csharp
public static Animation[] GenerateHorizontal(Claw.Graphics.Sprite spriteSheet, int amount, int frames, int animationFPS, Claw.Vector2 origin, Claw.Vector2 cellSize, Claw.Vector2 offset, string[] names) { }
```
Gera animação com um spritesheet horizontal.<br />
## GenerateVertical
```csharp
public static Animation[] GenerateVertical(Claw.Graphics.Sprite spriteSheet, int amount, int[] frames, int[] animationFPS, string[] names, Claw.Vector2[] origins, Claw.Vector2 cellSize, Claw.Vector2 offset) { }
```
Gera animação com um spritesheet vertical.<br />
## GenerateVertical
```csharp
public static Animation[] GenerateVertical(Claw.Graphics.Sprite spriteSheet, int amount, int frames, int animationFPS, Claw.Vector2 origin, Claw.Vector2 cellSize, Claw.Vector2 offset, string[] names) { }
```
Gera animação com um spritesheet vertical.<br />
