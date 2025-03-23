# Tilemap
```csharp
public abstract class Tilemap
```
Representa um mapa de tiles.<br />
## Tilemap
```csharp
public Tilemap() { }
```
## Tilemap
```csharp
public Tilemap(Claw.Vector2 size, Claw.Vector2 gridSize) { }
```
## GridSize
```csharp
public Claw.Vector2 GridSize;
```
## OnTileChange
```csharp
public System.Action<int,Claw.Maps.TileLayer,Claw.Vector2> OnTileChange;
```
É executado quando um tile é mudado ([novo tile], [layer], [posição do tile]).<br />
## OutOfView
```csharp
public static int OutOfView;
```
Define quantos tiles fora da view serão desenhados (1 por padrão).<br />
## LayerCount
```csharp
public int LayerCount { get; } 
```
## Size
```csharp
public Claw.Vector2 Size { get; set; } 
```
## Tilemap[int layerIndex]
```csharp
public Claw.Maps.TileLayer Tilemap[int layerIndex] { get; } 
```
Retorna uma layer.<br />
## Tilemap[string layerName]
```csharp
public Claw.Maps.TileLayer Tilemap[string layerName] { get; } 
```
Retorna uma layer.<br />
## PixelSize
```csharp
public abstract Claw.Vector2 PixelSize { get; } 
```
Tamanho do mapa (em pixels).<br />
## GetTileIndex
```csharp
public int GetTileIndex(int palette, Claw.Vector2 index) { }
```
Transforma um index 2D em um index 1D.<br />
## AddPalette
```csharp
public void AddPalette(Claw.Graphics.Sprite palette, int margin, int spacing) { }
```
Adiciona uma paleta ao [Tilemap](/api/Claw/Maps/Tilemap.md#Tilemap) .<br />
## AddPalette
```csharp
public void AddPalette(Claw.Graphics.Sprite palette, Claw.Vector2 gridSize, int margin, int spacing) { }
```
Adiciona uma paleta ao [Tilemap](/api/Claw/Maps/Tilemap.md#Tilemap) .<br />
## AddLayer
```csharp
public int AddLayer(string name, float opacity, Claw.Color color) { }
```
Adiciona uma layer nova.<br />
**Retorna**: O index da layer.<br />
## AddLayer
```csharp
public int AddLayer(string name, bool visible, float opacity, Claw.Color color, int[] data) { }
```
Adiciona uma layer nova e já insere os tiles dela.<br />
**Retorna**: O index da layer.<br />
## Addlayer
```csharp
public int Addlayer(Claw.Maps.TileLayer layer) { }
```
Adiciona uma layer.<br />
**Retorna**: O index da layer.<br />
## RemoveLayer
```csharp
public void RemoveLayer(int index) { }
```
Remove uma layer.<br />
## RemoveLayer
```csharp
public void RemoveLayer(string name) { }
```
Remove uma layer.<br />
## LayerExists
```csharp
public bool LayerExists(int index) { }
```
Verifica se a layer existe.<br />
## LayerExists
```csharp
public bool LayerExists(string name) { }
```
Verifica se a layer existe.<br />
## GetTileset
```csharp
public Claw.Maps.TilePalette GetTileset(int tile) { }
```
Retorna um tileset.<br />
## GetTileArea
```csharp
public void GetTileArea(int tile, Claw.Maps.TilePalette tileset, out Claw.Rectangle area) { }
```
Retorna a área do tileset que representa o tile.<br />
## PositionToCell
```csharp
public abstract Claw.Vector2 PositionToCell(Claw.Vector2 position) { }
```
Transforma uma posição livre em uma célula.<br />
## PositionToGrid
```csharp
public abstract Claw.Vector2 PositionToGrid(Claw.Vector2 position) { }
```
Transforma uma posição livre em uma posição em grid centralizada.<br />
## Render
```csharp
public abstract void Render(Claw.Maps.TileLayer layer) { }
```
Renderiza uma layer neste mapa.<br />
