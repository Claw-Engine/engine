# Sprite
```csharp
public sealed class Sprite
```
Representa uma sprite, dentro de um [TextureAtlas](api/Claw/Graphics/TextureAtlas.md#TextureAtlas) .<br />
## Sprite
```csharp
public Sprite(Claw.Graphics.Texture texture, string name, int x, int y, int width, int height) { }
```
## Sprite
```csharp
public Sprite(Claw.Graphics.Texture texture, string name, Claw.Rectangle area) { }
```
## Sprite
```csharp
public Sprite(Claw.Graphics.Texture texture, string name, Claw.Vector2 location, Claw.Vector2 size) { }
```
## Texture
```csharp
public readonly Claw.Graphics.Texture Texture;
```
## X
```csharp
public readonly int X;
```
## Y
```csharp
public readonly int Y;
```
## Width
```csharp
public readonly int Width;
```
## Height
```csharp
public readonly int Height;
```
## Name
```csharp
public readonly string Name;
```
## ToString
```csharp
public virtual string ToString() { }
```
