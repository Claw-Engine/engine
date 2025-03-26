# TileLayer
```csharp
public sealed class TileLayer : Claw.Modules.ModuleLayer
```
Camada de [Module](/api/Claw/Modules/Module.md#Module) s, anexado à um [Tilemap](/api/Claw/Maps/Tilemap.md#Tilemap) .<br />
## TileLayer
```csharp
public TileLayer(string name, bool triggersInitialize, Claw.Maps.Tilemap map, Claw.Vector2 size) { }
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
## Map
```csharp
public Claw.Maps.Tilemap Map { get; set; } 
```
## GetTile
```csharp
public int GetTile(int index) { }
```
Retorna um tile da layer.<br />
## SetTile
```csharp
public int SetTile(int index, int tile) { }
```
Altera um tile da layer.<br />
## GetTile
```csharp
public int GetTile(Claw.Vector2 cell) { }
```
Retorna um tile da layer.<br />
## SetTile
```csharp
public int SetTile(Claw.Vector2 cell, int tile) { }
```
Altera um tile da layer.<br />
## GetTile
```csharp
public int GetTile(float x, float y) { }
```
Retorna um tile da layer.<br />
## SetTile
```csharp
public int SetTile(float x, float y, int tile) { }
```
Altera um tile da layer.<br />
## CountTiles
```csharp
public int CountTiles() { }
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
public virtual void Render() { }
```
