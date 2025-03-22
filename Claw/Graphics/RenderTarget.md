# RenderTarget
```csharp
public sealed class RenderTarget : Claw.Graphics.Texture
```
Representa um alvo de renderização para o [Renderer](/api/Claw/Graphics/Renderer.md#Renderer) .<br />
## RenderTarget
```csharp
public RenderTarget(int width, int height) { }
```
## GetData
```csharp
public uint[] GetData() { }
```
Obtém os pixels desta textura.<br />
Aviso: Função lenta e não recomendada de se usar dentro de um loop.<br />
<br />
## SetData
```csharp
public virtual void SetData(uint[] pixels) { }
```
Altera os pixels desta textura.<br />
Aviso: Uso não recomendado (cada pixel será atualizado manualmente, num laço de repetição).<br />
Recomendado: Use o método [Renderer.SetRenderTarget](/api/Claw/Graphics/Renderer.md#SetRenderTarget) no lugar disso.<br />
<br />
