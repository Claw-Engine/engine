# Camera
```csharp
public class Camera
```
Representa uma câmera 2D para operações no [Draw](api/Claw/Graphics/Draw.md#Draw) .<br />
## Camera
```csharp
public Camera() { }
```
## Camera
```csharp
public Camera(Claw.Rectangle viewport) { }
```
**viewport**: Viewport da câmera. Se estiver vazio, ocupará a tela inteira.<br />
## Camera
```csharp
public Camera(Claw.Rectangle viewport, float zoom, float rotation, Claw.Vector2 position, Claw.Vector2 origin, Claw.Vector2 border, Claw.Vector2 minPosition, Claw.Vector2 maxPosition) { }
```
**viewport**: Viewport da câmera. Se estiver vazio, ocupará a tela inteira.<br />
## Camera
```csharp
public Camera(Claw.Rectangle viewport, Claw.Vector2 position, Claw.Vector2 origin, Claw.Vector2 minPosition, Claw.Vector2 maxPosition) { }
```
**viewport**: Viewport da câmera. Se estiver vazio, ocupará a tela inteira.<br />
## Zoom
```csharp
public float Zoom;
```
## Rotation
```csharp
public float Rotation;
```
## Position
```csharp
public Claw.Vector2 Position;
```
## Origin
```csharp
public Claw.Vector2 Origin;
```
## Border
```csharp
public Claw.Vector2 Border;
```
## MinPosition
```csharp
public Claw.Vector2 MinPosition;
```
## MaxPosition
```csharp
public Claw.Vector2 MaxPosition;
```
## State
```csharp
public readonly Claw.Graphics.CameraState State;
```
## Viewport
```csharp
public Claw.Rectangle Viewport { get; set; } 
```
## Follow
```csharp
public virtual void Follow(Claw.Vector2 position, Claw.Graphics.FollowMode mode) { }
```
Segue uma posição específica, sem passar dos limites estipulados.<br />
## CalculateFollow
```csharp
public Claw.Vector2 CalculateFollow(Claw.Vector2 position, Claw.Graphics.FollowMode mode) { }
```
Cálculo de posição usado pelo método [Camera.Follow](api/Claw/Graphics/Camera.md#Follow) .<br />
## ScreenToWorld
```csharp
public Claw.Vector2 ScreenToWorld(Claw.Vector2 point) { }
```
Converte um ponto da tela em um ponto do mundo, com base no [Camera.State](api/Claw/Graphics/Camera.md#State) .<br />
## WorldToScreen
```csharp
public Claw.Vector2 WorldToScreen(Claw.Vector2 point) { }
```
Converte um ponto do mundo em um ponto da tela, com base no [Camera.State](api/Claw/Graphics/Camera.md#State) .<br />
