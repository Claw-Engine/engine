# StaggeredTilemap
```csharp
public class StaggeredTilemap : Claw.Maps.Tilemap
```
Representa um mapa isométrico escalado de tiles.<br />
## StaggeredTilemap
```csharp
public StaggeredTilemap() { }
```
## StaggeredTilemap
```csharp
public StaggeredTilemap(Claw.Vector2 size, Claw.Vector2 gridSize) { }
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
