# Element
```csharp
public abstract class Element
```
Elemento base, para criação de interfaces gráficas.<br />
## Element
```csharp
protected Element() { }
```
## Name
```csharp
public string Name;
```
## Position
```csharp
public Claw.Vector2 Position;
```
Posição relativa do elemento.<br />
### Observações
Objetos, como o [Container](/api/Claw/Graphics/UI/Container.md#Container) , devem alterar os valores deste campo.<br />
## Layout
```csharp
public Claw.Graphics.UI.LayoutMode Layout;
```
Define a disposição deste elemento ( [LayoutMode.InlineBlock](/api/Claw/Graphics/UI/LayoutMode.md#InlineBlock) por padrão).<br />
## Size
```csharp
public abstract Claw.Vector2 Size { get; } 
```
## Contains
```csharp
public virtual bool Contains(Claw.Vector2 relativePoint) { }
```
Verifica se este elemento contém um ponto.<br />
**relativePoint**: Ponto de verificação, relativo à [Element.Position](/api/Claw/Graphics/UI/Element.md#Position) .<br />
## Step
```csharp
public abstract bool Step(Claw.Vector2 relativeCursor) { }
```
**relativeCursor**: Posição do mouse, relativo à [Element.Position](/api/Claw/Graphics/UI/Element.md#Position) .<br />
**Retorna**: Se houve alguma mudança espacial.<br />
## Render
```csharp
public abstract void Render(Claw.Vector2 offset) { }
```
**offset**: Quanto a [Element.Position](/api/Claw/Graphics/UI/Element.md#Position) deve ser deslocada ao desenhar.<br />
