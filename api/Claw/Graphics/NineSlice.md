# NineSlice
```csharp
public static class NineSlice
```
Desenha retângulos texturizados.<br />
## AddTexture
```csharp
public static void AddTexture(string index, Claw.Graphics.Sprite sprite) { }
```
Divide uma sprite em 9 partes e adiciona ao [NineSlice](/Claw/Graphics/NineSlice.md#NineSlice) .<br />
**sprite**: Top Left; Top; Top Right; Left; Center; Right; Bottom Left; Bottom; Bottom Right.<br />
## AddTexture
```csharp
public static void AddTexture(string index, Claw.Graphics.Sprite sprite, Claw.Rectangle textureArea) { }
```
Divide uma área de uma sprite em 9 partes e adiciona ao [NineSlice](/Claw/Graphics/NineSlice.md#NineSlice) .<br />
**textureArea**: Top Left; Top; Top Right; Left; Center; Right; Bottom Left; Bottom; Bottom Right.<br />
## AddTexture
```csharp
public static void AddTexture(string index, Claw.Graphics.Sprite sprite, Claw.Rectangle[] parts) { }
```
Adiciona uma textura ao [NineSlice](/Claw/Graphics/NineSlice.md#NineSlice) .<br />
**parts**: Top Left; Top; Top Right; Left; Center; Right; Bottom Left; Bottom; Bottom Right.<br />
## Draw
```csharp
public static Claw.Rectangle Draw(string texture, Claw.Rectangle area, Claw.Color backgroundColor, Claw.Color blendColor, bool scaleCenter) { }
```
Desenha o retângulo texturizado e retorna a área do seu interior.<br />
**scaleCenter**: Define se o centro do [NineSlice](/Claw/Graphics/NineSlice.md#NineSlice) será escalado ou repetido.<br />
