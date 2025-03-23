# Container
```csharp
public class Container : Claw.Graphics.UI.Element
```
Contêiner base para carregar outros elementos.<br />
## Container
```csharp
public Container() { }
```
## AllowOverflow
```csharp
public bool AllowOverflow;
```
Define se o conteúdo deste elemento pode ultrapassar as bordas (true por padrão).<br />
### Observações
Ao marcar como false, o elemento usará o [RenderTarget](/api/Claw/Graphics/RenderTarget.md#RenderTarget) como método de corte.<br />
## Gap
```csharp
public Claw.Vector2 Gap;
```
## Size
```csharp
public virtual Claw.Vector2 Size { get; } 
```
## Padding
```csharp
public Claw.Vector2 Padding { get; set; } 
```
## MinSize
```csharp
public Claw.Vector2 MinSize { get; set; } 
```
## MaxSize
```csharp
public Claw.Vector2? MaxSize { get; set; } 
```
## ScrollOffset
```csharp
public Claw.Vector2 ScrollOffset { get; set; } 
```
## MaxScrollOffset
```csharp
public Claw.Vector2 MaxScrollOffset { get; private set; } 
```
## Alignment
```csharp
public Claw.Graphics.UI.LayoutAlignment Alignment { get; set; } 
```
## Count
```csharp
public int Count { get; } 
```
## Container[int index]
```csharp
public Claw.Graphics.UI.Element Container[int index] { get; set; } 
```
## Add
```csharp
public void Add(Claw.Graphics.UI.Element element) { }
```
Insere um elemento no Contêiner.<br />
## Remove
```csharp
public void Remove(Claw.Graphics.UI.Element element) { }
```
Remove um elemento do Contêiner.<br />
## Step
```csharp
public virtual bool Step(Claw.Vector2 relativeCursor) { }
```
## Render
```csharp
public virtual void Render(Claw.Vector2 offset) { }
```
