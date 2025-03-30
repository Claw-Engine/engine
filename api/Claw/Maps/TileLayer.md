# TileLayer
```csharp
public sealed class TileLayer
```
Camada tiles de um [Tilemap](/api/Claw/Maps/Tilemap.md#Tilemap) .<br />
## TileLayer
```csharp
public TileLayer(Claw.Maps.Tilemap map) { }
```
Cria uma camada de tiles e anexa ela à um [Tilemap](/api/Claw/Maps/Tilemap.md#Tilemap) .<br />
## Opacity
```csharp
public float Opacity;
```
## Color
```csharp
public Claw.Color Color;
```
## Size
```csharp
public Claw.Vector2 Size { get; } 
```
## Map
```csharp
public Claw.Maps.Tilemap Map { get; set; } 
```
## TileLayer[int index]
```csharp
public int TileLayer[int index] { get; set; } 
```
## TileLayer[Claw.Vector2 cell]
```csharp
public int TileLayer[Claw.Vector2 cell] { get; set; } 
```
## TileLayer[float x, float y]
```csharp
public int TileLayer[float x, float y] { get; set; } 
```
## Length
```csharp
public int Length { get; } 
```
Retorna o número de tiles.<br />
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
## CheckCollision
```csharp
public bool CheckCollision(Claw.Vector2 position, out int tile) { }
```
Checa se um ponto está dentro de uma célula com tile.<br />
## Render
```csharp
public void Render() { }
```
## Array2DTo1D
```csharp
public static int[] Array2DTo1D(int[,] array, Claw.Vector2 newSize) { }
```
