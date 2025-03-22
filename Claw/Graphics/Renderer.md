# Renderer
```csharp
public sealed class Renderer
```
Representa o renderizador da [Window](/api/Claw/Window.md#Window) .<br />
## VSync
```csharp
public bool VSync { get; set; } 
```
## ClearColor
```csharp
public Claw.Color ClearColor { get; set; } 
```
## Finalize
```csharp
protected virtual void Finalize() { }
```
## Dispose
```csharp
public virtual void Dispose() { }
```
## GetRenderTarget
```csharp
public Claw.Graphics.RenderTarget GetRenderTarget() { }
```
Retorna o alvo da renderização atual.<br />
## SetRenderTarget
```csharp
public void SetRenderTarget(Claw.Graphics.RenderTarget renderTarget) { }
```
Altera o alvo da renderização.<br />
**renderTarget**: Nulo para desenhar na [Window](/api/Claw/Window.md#Window) .<br />
## DrawTexture
```csharp
public void DrawTexture(Claw.Graphics.Texture texture, Claw.Rectangle source, Claw.Rectangle destination, Claw.Color color, Claw.Vector2 origin, float angle, Claw.Graphics.Flip flip) { }
```
Desenha uma imagem.<br />
**angle**: Graus.<br />
## Clear
```csharp
public void Clear() { }
```
Limpa a tela com o [Renderer.ClearColor](/api/Claw/Graphics/Renderer.md#ClearColor) .<br />
## Clear
```csharp
public void Clear(Claw.Color color) { }
```
Limpa a tela com uma cor específica.<br />
