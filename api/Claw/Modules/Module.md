# Module
```csharp
public abstract class Module
```
Descreve um módulo base.<br />
## Module
```csharp
public Module() { }
```
## Enabled
```csharp
public bool Enabled;
```
## Visible
```csharp
public bool Visible;
```
## Transform
```csharp
public readonly Claw.Modules.Transform Transform;
```
## Layer
```csharp
public Claw.Modules.ModuleLayer Layer { get; internal set; } 
```
## Game
```csharp
protected Claw.Game Game { protected get; } 
```
## Initialize
```csharp
public abstract void Initialize() { }
```
## Step
```csharp
public abstract void Step() { }
```
## Render
```csharp
public abstract void Render() { }
```
## Delete
```csharp
public virtual void Delete(bool deleteChildren) { }
```
Remove este módulo de [Module.Layer](/api/Claw/Modules/Module.md#Layer) .<br />
**deleteChildren**: Se verdadeiro, também deleta os filhos de [Module.Transform](/api/Claw/Modules/Module.md#Transform) (o parâmetro também é passado para os filhos).<br />
