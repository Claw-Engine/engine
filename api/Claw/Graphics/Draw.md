# Draw
```csharp
public static class Draw
```
Desenha formas, textos e sprites de forma simplificada.<br />
## IgnoreCamera
```csharp
public static bool IgnoreCamera;
```
Diz se o [Draw](/Claw/Graphics/Draw.md#Draw) deverá ignorar a câmera (falso por padrão).<br />
## Sprite
```csharp
public static void Sprite(Claw.Graphics.Sprite sprite, Claw.Vector2 position, Claw.Rectangle? sourceRectangle, Claw.Color color, float rotation, Claw.Vector2 origin, Claw.Vector2 scale, Claw.Graphics.Flip flip) { }
```
Desenha uma sprite.<br />
**rotation**: Graus.<br />
**origin**: De 0 a 1.<br />
## Sprite
```csharp
public static void Sprite(Claw.Graphics.Sprite sprite, Claw.Vector2 position, Claw.Rectangle? sourceRectangle, Claw.Color color, float rotation, Claw.Vector2 origin, float scale, Claw.Graphics.Flip flip) { }
```
Desenha uma sprite.<br />
**rotation**: Graus.<br />
**origin**: De 0 a 1.<br />
## Sprite
```csharp
public static void Sprite(Claw.Graphics.Sprite sprite, Claw.Vector2 position, Claw.Rectangle? sourceRectangle, Claw.Color color) { }
```
Desenha uma sprite.<br />
## Sprite
```csharp
public static void Sprite(Claw.Graphics.Sprite sprite, Claw.Vector2 position, Claw.Color color) { }
```
Desenha uma sprite.<br />
## Sprite
```csharp
public static void Sprite(Claw.Graphics.Texture texture, Claw.Vector2 position, Claw.Rectangle? sourceRectangle, Claw.Color color, float rotation, Claw.Vector2 origin, Claw.Vector2 scale, Claw.Graphics.Flip flip) { }
```
Desenha uma sprite.<br />
**rotation**: Graus.<br />
**origin**: De 0 a 1.<br />
## Sprite
```csharp
public static void Sprite(Claw.Graphics.Texture texture, Claw.Vector2 position, Claw.Rectangle? sourceRectangle, Claw.Color color, float rotation, Claw.Vector2 origin, float scale, Claw.Graphics.Flip flip) { }
```
Desenha uma sprite.<br />
**rotation**: Graus.<br />
**origin**: De 0 a 1.<br />
## Sprite
```csharp
public static void Sprite(Claw.Graphics.Texture texture, Claw.Vector2 position, Claw.Rectangle? sourceRectangle, Claw.Color color) { }
```
Desenha uma sprite.<br />
## Sprite
```csharp
public static void Sprite(Claw.Graphics.Texture texture, Claw.Vector2 position, Claw.Color color) { }
```
Desenha uma sprite.<br />
## Text
```csharp
public static void Text(Claw.Graphics.SpriteFont font, string text, Claw.Vector2 position, Claw.Color color, float rotation, Claw.Vector2 origin, Claw.Vector2 scale, Claw.Graphics.Flip flip) { }
```
Desenha um texto.<br />
**rotation**: Graus.<br />
**origin**: De 0 a 1.<br />
## Text
```csharp
public static void Text(Claw.Graphics.SpriteFont font, string text, Claw.Vector2 position, Claw.Color color, float rotation, Claw.Vector2 origin, float scale, Claw.Graphics.Flip flip) { }
```
Desenha um texto.<br />
**rotation**: Graus.<br />
**origin**: De 0 a 1.<br />
## Text
```csharp
public static void Text(Claw.Graphics.SpriteFont font, string text, Claw.Vector2 position, Claw.Color color) { }
```
Desenha um texto.<br />
## Pixel
```csharp
public static void Pixel(int scale, Claw.Vector2 position, Claw.Color color) { }
```
Desenha um pixel.<br />
## Pixel
```csharp
public static void Pixel(Claw.Vector2 scale, Claw.Vector2 position, Claw.Color color) { }
```
Desenha um pixel.<br />
## Line
```csharp
public static void Line(float lineWidth, Claw.Vector2 start, Claw.Vector2 end, Claw.Color color) { }
```
Desenha uma linha.<br />
## Line
```csharp
public static void Line(float lineWidth, Claw.Line line, Claw.Color color) { }
```
Desenha uma linha.<br />
## Polygon
```csharp
public static void Polygon(float lineWidth, Claw.Color color, Claw.Vector2[] points) { }
```
Desenha um polígono.<br />
## Polygon
```csharp
public static void Polygon(float lineWidth, Claw.Color color, Claw.Line[] lines) { }
```
Desenha um polígono.<br />
## Rectangle
```csharp
public static void Rectangle(float lineWidth, Claw.Rectangle rectangle, Claw.Color color) { }
```
Desenha um retângulo.<br />
## FilledRectangle
```csharp
public static void FilledRectangle(float lineWidth, Claw.Rectangle rectangle, Claw.Color outline, Claw.Color fill) { }
```
Desenha um retângulo cheio.<br />
## Circle
```csharp
public static void Circle(float lineWidth, float radius, Claw.Vector2 center, Claw.Color color, int segments) { }
```
Desenha um círculo.<br />
## FilledCircle
```csharp
public static void FilledCircle(float lineWidth, float radius, Claw.Vector2 center, Claw.Color outline, Claw.Color fill, int segments) { }
```
Desenha um círculo cheio.<br />
## Oval
```csharp
public static void Oval(float lineWidth, Claw.Vector2 radius, Claw.Vector2 center, Claw.Color color, int segments) { }
```
Desenha um formato oval.<br />
## FilledOval
```csharp
public static void FilledOval(float lineWidth, Claw.Vector2 radius, Claw.Vector2 center, Claw.Color outline, Claw.Color fill, int segments) { }
```
Desenha um formato oval cheio.<br />
## CircleHealthBar
```csharp
public static void CircleHealthBar(float lineWidth, float radius, Claw.Vector2 center, Claw.Color color, float life, float maxLife, int segments) { }
```
Desenha uma barra de vida circular.<br />
## FilledCircleHealthBar
```csharp
public static void FilledCircleHealthBar(float lineWidth, float radius, Claw.Vector2 center, Claw.Color outline, Claw.Color fill, float life, float maxLife, int segments) { }
```
Desenha uma barra de vida circular cheia.<br />
## OvalHealthBar
```csharp
public static void OvalHealthBar(float lineWidth, Claw.Vector2 radius, Claw.Vector2 center, Claw.Color color, float life, float maxLife, int segments) { }
```
Desenha uma barra de vida oval.<br />
## FilledOvalHealthBar
```csharp
public static void FilledOvalHealthBar(float lineWidth, Claw.Vector2 radius, Claw.Vector2 center, Claw.Color outline, Claw.Color fill, float life, float maxLife, int segments) { }
```
Desenha uma barra de vida oval cheia.<br />
## BezierCurve
```csharp
public static void BezierCurve(float lineWidth, int segments, Claw.Color color, Claw.Vector2 point0, Claw.Vector2 point1, Claw.Vector2 point2, Claw.Vector2 point3) { }
```
Desenha uma curva de Bézier.<br />
## BezierCurve
```csharp
public static void BezierCurve(float lineWidth, int segments, Claw.Color color, Claw.Vector2 point0, Claw.Vector2 point1, Claw.Vector2 point2) { }
```
Desenha uma curva de Bézier.<br />
## BezierCurve
```csharp
public static void BezierCurve(float lineWidth, int segments, Claw.Color color, Claw.Vector2 point0, Claw.Vector2 point1) { }
```
Desenha uma curva de Bézier.<br />
## ForceBlendMode
```csharp
public static void ForceBlendMode(Claw.Graphics.BlendMode? blendMode) { }
```
Se for diferente de nulo, o [Draw](/Claw/Graphics/Draw.md#Draw) forçará todas as texturas a terem esse [BlendMode](/Claw/Graphics/BlendMode.md#BlendMode) .<br />
## GetCamera
```csharp
public static Claw.Graphics.Camera GetCamera() { }
```
Retorna a câmera atual.<br />
## SetCamera
```csharp
public static void SetCamera(Claw.Graphics.Camera camera) { }
```
Aplica a câmera e o viewport.<br />
**camera**: Se nulo, reseta o viewport e remove a câmera.<br />
