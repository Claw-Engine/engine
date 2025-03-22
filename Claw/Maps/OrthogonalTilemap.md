# OrthogonalTilemap
```csharp
public class OrthogonalTilemap : Claw.Maps.Tilemap
```
Representa um mapa ortogonal de tiles.<br />
## OrthogonalTilemap
```csharp
public OrthogonalTilemap() { }
```
## OrthogonalTilemap
```csharp
public OrthogonalTilemap(Claw.Vector2 size, Claw.Vector2 gridSize) { }
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
