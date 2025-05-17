# SpriteRenderer
```csharp
public class SpriteRenderer
```
Comportamento para renderização de sprites animadas.<br />
## SpriteRenderer
```csharp
public SpriteRenderer() { }
```
## Opacity
```csharp
public float Opacity;
```
## Color
```csharp
public Claw.Color Color;
```
## Flip
```csharp
public Claw.Graphics.Flip Flip;
```
## Origin
```csharp
public virtual Claw.Vector2 Origin { get; set; } 
```
## Sprite
```csharp
public virtual Claw.Graphics.Sprite Sprite { get; set; } 
```
## SpriteArea
```csharp
public virtual Claw.Rectangle? SpriteArea { get; set; } 
```
## Animator
```csharp
public Claw.Graphics.Animator Animator { get; set; } 
```
## Run
```csharp
public void Run(Claw.Modules.Module module) { }
```
