# Transform
```csharp
public sealed class Transform
```
Descreve informações espaciais para os módulos.<br />
## Module
```csharp
public readonly Claw.Modules.Module Module;
```
## Rotation
```csharp
public float Rotation { get; set; } 
```
## RelativeRotation
```csharp
public float RelativeRotation { get; set; } 
```
## Position
```csharp
public Claw.Vector2 Position { get; set; } 
```
## RelativePosition
```csharp
public Claw.Vector2 RelativePosition { get; set; } 
```
## Scale
```csharp
public Claw.Vector2 Scale { get; set; } 
```
## RelativeScale
```csharp
public Claw.Vector2 RelativeScale { get; set; } 
```
## Facing
```csharp
public Claw.Vector2 Facing { get; } 
```
## Parent
```csharp
public Transform Parent { get; set; } 
```
## ChildCount
```csharp
public int ChildCount() { }
```
Retorna a quantidade de filhos deste [Transform](api/Claw/Modules/Transform.md#Transform) .<br />
## GetChild
```csharp
public Transform GetChild(int index) { }
```
Retorna um filho deste [Transform](api/Claw/Modules/Transform.md#Transform) .<br />
