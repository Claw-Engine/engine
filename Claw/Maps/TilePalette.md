# TilePalette
```csharp
public sealed class TilePalette
```
Contém os dados de um tileset.<br />
## Index
```csharp
public int Index { get; internal set; } 
```
Index desta paleta na lista de paletas do [Tilemap](/api/Claw/Maps/Tilemap.md#Tilemap) .<br />
## FirstIndex
```csharp
public int FirstIndex { get; internal set; } 
```
Primeiro tile desta paleta.<br />
## Sub
```csharp
public int Sub { get; internal set; } 
```
Quanto deve ser subtraído de um index global para que se chegue ao tile 0 desta paleta.<br />
<br />
## Margin
```csharp
public int Margin { get; internal set; } 
```
Espaço entre o tile e as bordas da sprite.<br />
## Spacing
```csharp
public int Spacing { get; internal set; } 
```
Espaço entre um tile e outro.<br />
## TileCount
```csharp
public int TileCount { get; internal set; } 
```
Quantos tiles esta paleta tem.<br />
## GridSize
```csharp
public Claw.Vector2 GridSize { get; internal set; } 
```
Qual o tamanho de cada tile.<br />
## Texture
```csharp
public Claw.Graphics.Sprite Texture { get; internal set; } 
```
Textura desta paleta.<br />
## Contains
```csharp
public bool Contains(int tile) { }
```
Diz se o tile pertence a esta paleta.<br />
## Get2DIndex
```csharp
public Claw.Vector2 Get2DIndex(int index) { }
```
Transforma um index 1D em um index 2D, considerando o tileset.<br />
