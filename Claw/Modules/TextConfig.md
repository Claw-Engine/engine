# TextConfig
```csharp
public sealed class TextConfig
```
Define as configurações para a renderização do [TextRenderer](/api/Claw/Modules/TextRenderer.md#TextRenderer) .<br />
## TextConfig
```csharp
public TextConfig() { }
```
## TextConfig
```csharp
public TextConfig(float? rotation, Claw.Color? color, Claw.Vector2? scale, Claw.Modules.TextOrigin? origin, Claw.Modules.TextEffect? effect, Claw.Graphics.SpriteFont font, Claw.Graphics.Flip? flip) { }
```
## Rotation
```csharp
public float? Rotation;
```
## Color
```csharp
public Claw.Color? Color;
```
## Scale
```csharp
public Claw.Vector2? Scale;
```
## Origin
```csharp
public Claw.Modules.TextOrigin? Origin;
```
## Effect
```csharp
public Claw.Modules.TextEffect? Effect;
```
## Font
```csharp
public Claw.Graphics.SpriteFont Font;
```
## Flip
```csharp
public Claw.Graphics.Flip? Flip;
```
## Copy
```csharp
public void Copy(TextConfig other) { }
```
Copia os valores de um outro [TextConfig](/api/Claw/Modules/TextConfig.md#TextConfig) .<br />
