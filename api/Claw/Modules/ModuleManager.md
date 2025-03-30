# ModuleManager
```csharp
public sealed class ModuleManager
```
Gerenciador de [Module](/api/Claw/Modules/Module.md#Module) s.<br />
## ModuleManager
```csharp
public ModuleManager() { }
```
## Layers
```csharp
public readonly System.Collections.Generic.List<Claw.Modules.ModuleLayer> Layers;
```
## ModuleManager[int index]
```csharp
public Claw.Modules.ModuleLayer ModuleManager[int index] { get; set; } 
```
## FindLayer
```csharp
public Claw.Modules.ModuleLayer FindLayer(string name) { }
```
Procura por uma camada com determinado nome.<br />
## ClearLayers
```csharp
public void ClearLayers() { }
```
Limpa todas as camadas.<br />
## Step
```csharp
public void Step() { }
```
## Render
```csharp
public void Render() { }
```
