# ModuleLayer
```csharp
public sealed class ModuleLayer : System.Collections.ObjectModel.Collection<Claw.Modules.Module>
```
Camada de [Module](api/Claw/Modules/Module.md#Module) s.<br />
## ModuleLayer
```csharp
public ModuleLayer(string name, bool triggersInitialize) { }
```
## TriggersInitialize
```csharp
public readonly bool TriggersInitialize;
```
Define se o método [Module.Initialize](api/Claw/Modules/Module.md#Initialize) será acionado ao inserir módulos.<br />
## Name
```csharp
public readonly string Name;
```
## OnAdded
```csharp
public event System.Action<Claw.Modules.Module> OnAdded;
```
## OnRemoved
```csharp
public event System.Action<Claw.Modules.Module> OnRemoved;
```
## OnCleared
```csharp
public event System.Action OnCleared;
```
## InsertItem
```csharp
protected virtual void InsertItem(int index, Claw.Modules.Module module) { }
```
Insere um [Module](api/Claw/Modules/Module.md#Module) na camada e aciona o evento [ModuleLayer.OnAdded](api/Claw/Utils/ModuleLayer.md#OnAdded) .<br />
## RemoveItem
```csharp
protected virtual void RemoveItem(int index) { }
```
Remove um [Module](api/Claw/Modules/Module.md#Module) da camada e aciona o evento [ModuleLayer.OnRemoved](api/Claw/Utils/ModuleLayer.md#OnRemoved) .<br />
## SetItem
```csharp
protected virtual void SetItem(int index, Claw.Modules.Module newModule) { }
```
Remove um [Module](api/Claw/Modules/Module.md#Module) e insere outro no mesmo index.<br />
## ClearItems
```csharp
protected virtual void ClearItems() { }
```
Limpa a camada e aciona o evento [ModuleLayer.OnCleared](api/Claw/Utils/ModuleLayer.md#OnCleared) .<br />
