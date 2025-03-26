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
public TileChangeEvent OnTileChange;
```
É executado quando um tile é mudado ([novo tile], [layer], [posição do tile]).<br />
## OutOfView
```csharp
public static int OutOfView;
```
Define quantos tiles fora da view serão desenhados (1 por padrão).<br />
## Size
```csharp
public Claw.Vector2 Size { get; set; } 
```
## Count
```csharp
public int Count { get; } 
```
## Tilemap[int layerIndex]
```csharp
public Claw.Maps.TileLayer Tilemap[int layerIndex] { get; } 
```
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
public Claw.Maps.TilePalette AddPalette(Claw.Graphics.Sprite palette, int margin, int spacing) { }
```
Adiciona uma paleta ao [Tilemap](/api/Claw/Maps/Tilemap.md#Tilemap) .<br />
## AddPalette
```csharp
public Claw.Maps.TilePalette AddPalette(Claw.Graphics.Sprite palette, Claw.Vector2 gridSize, int margin, int spacing) { }
```
Adiciona uma paleta ao [Tilemap](/api/Claw/Maps/Tilemap.md#Tilemap) .<br />
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
# TileChangeEvent
```csharp
public sealed class TileChangeEvent : System.MulticastDelegate
```
## TileChangeEvent
```csharp
public TileChangeEvent(object object, System.IntPtr method) { }
```
## Invoke
```csharp
public virtual void Invoke(int newTile, Claw.Maps.TileLayer layer, Claw.Vector2 position) { }
```
## BeginInvoke
```csharp
public virtual System.IAsyncResult BeginInvoke(int newTile, Claw.Maps.TileLayer layer, Claw.Vector2 position, System.AsyncCallback callback, object object) { }
```
## EndInvoke
```csharp
public virtual void EndInvoke(System.IAsyncResult result) { }
```
