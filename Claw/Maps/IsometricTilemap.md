# IsometricTilemap
```csharp
public class IsometricTilemap : Claw.Maps.Tilemap
```
Representa um mapa isométrico de tiles.<br />
## IsometricTilemap
```csharp
public IsometricTilemap() { }
```
## IsometricTilemap
```csharp
public IsometricTilemap(Claw.Vector2 size, Claw.Vector2 gridSize) { }
```
## PixelSize
```csharp
public virtual Claw.Vector2 PixelSize { get; } 
```
## PositionToCell
```csharp
public virtual Claw.Vector2 PositionToCell(Claw.Vector2 position) { }
```
## PositionToGrid
```csharp
public virtual Claw.Vector2 PositionToGrid(Claw.Vector2 position) { }
```
## Render
```csharp
public virtual void Render(Claw.Maps.TileLayer layer) { }
```
