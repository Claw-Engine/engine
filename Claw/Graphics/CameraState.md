# CameraState
```csharp
public sealed class CameraState
```
Estado da câmera para operações que envolvem zoom, rotação, posição e origem da câmera.<br />
## Camera
```csharp
public readonly Claw.Graphics.Camera Camera;
```
## Neutral
```csharp
public static readonly CameraState Neutral;
```
## Zoom
```csharp
public float Zoom { get; private set; } 
```
## Rotation
```csharp
public float Rotation { get; private set; } 
```
## Position
```csharp
public Claw.Vector2 Position { get; private set; } 
```
## Origin
```csharp
public Claw.Vector2 Origin { get; private set; } 
```
## TopLeft
```csharp
public Claw.Vector2 TopLeft { get; private set; } 
```
## BottomRight
```csharp
public Claw.Vector2 BottomRight { get; } 
```
## Update
```csharp
public void Update() { }
```
Atualiza o estado da câmera.<br />
