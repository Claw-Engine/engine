# TileLayer
```csharp
public sealed class TileLayer : Claw.Modules.Module
```
Representa uma camada dentro do [Tilemap](/api/Claw/Maps/Tilemap.md#Tilemap) .<br />
## Opacity
```csharp
public float Opacity;
```
## Color
```csharp
public Claw.Color Color;
```
## Name
```csharp
public string Name { get; set; } 
```
## TileLayer[int index]
```csharp
public int TileLayer[int index] { get; set; } 
```
Retorna/altera um tile da layer.<br />
## TileLayer[Claw.Vector2 cell]
```csharp
public int TileLayer[Claw.Vector2 cell] { get; set; } 
```
Retorna/altera um tile da layer.<br />
## TileLayer[int x, int y]
```csharp
public int TileLayer[int x, int y] { get; set; } 
```
Retorna/altera um tile da layer.<br />
## Length
```csharp
public int Length { get; } 
```
Retorna o número de tiles da layer.<br />
## Initialize
```csharp
public virtual void Initialize() { }
```
## Step
```csharp
public virtual void Step() { }
```
## SetMultipleTiles
```csharp
public void SetMultipleTiles(int[] mapData) { }
```
Muda vários tiles de uma layer. Este método não chama o [Tilemap.OnTileChange](/api/Claw/Maps/Tilemap.md#OnTileChange) !<br />
## SetChunkTiles
```csharp
public void SetChunkTiles(Claw.Rectangle chunk, int[] chunkData) { }
```
Muda vários tiles de um chunk imaginário. Este método não chama o [Tilemap.OnTileChange](/api/Claw/Maps/Tilemap.md#OnTileChange) !<br />
## Clear
```csharp
public void Clear() { }
```
Muda cada tile da layer para 0 (vazio).<br />
## CheckCollision
```csharp
public bool CheckCollision(Claw.Vector2 position, out int tile) { }
```
Checa se um ponto está dentro de uma célula com tile.<br />
## CheckCollision
```csharp
public bool CheckCollision(Claw.Vector2 position, int[] filterTiles, out int tile) { }
```
Checa se um ponto está dentro de uma célula com tile.<br />
## Render
```csharp
public virtual void Render() { }
```
