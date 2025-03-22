# Texture
```csharp
public class Texture
```
Representa uma textura no jogo.<br />
## Texture
```csharp
public Texture(int width, int height) { }
```
## Texture
```csharp
public Texture(int width, int height, uint[] pixels) { }
```
## Width
```csharp
public readonly int Width;
```
## Height
```csharp
public readonly int Height;
```
## Pixel
```csharp
public static Texture Pixel { get; internal set; } 
```
## Size
```csharp
public Claw.Vector2 Size { get; } 
```
## BlendMode
```csharp
public Claw.Graphics.BlendMode BlendMode { get; set; } 
```
## ScaleMode
```csharp
public Claw.Graphics.ScaleMode ScaleMode { get; set; } 
```
## Destroy
```csharp
public void Destroy() { }
```
Destrói esta textura.<br />
## Load
```csharp
public static Texture Load(string path) { }
```
Carrega uma textura.<br />
**Retorna**: A textura ou null (se não for um arquivo válido).<br />
## SetData
```csharp
public virtual void SetData(uint[] pixels) { }
```
Altera os pixels desta textura.<br />
