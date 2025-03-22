# SpriteFont
```csharp
public sealed class SpriteFont
```
Representa uma fonte, com base numa [Sprite](api/Claw/Graphics/Sprite.md#Sprite) .<br />
## SpriteFont
```csharp
public SpriteFont(Claw.Graphics.Sprite sprite, Claw.Vector2 spacing, System.Collections.Generic.Dictionary<char,Claw.Graphics.Glyph> glyphs) { }
```
## SpriteFont
```csharp
public SpriteFont(Claw.Graphics.Sprite sprite, Claw.Vector2 spacing, Claw.Vector2 charSize, char[] chars) { }
```
## SpriteFont
```csharp
public SpriteFont(Claw.Graphics.Sprite sprite, Claw.Vector2 spacing, Claw.Vector2 charSize, string chars) { }
```
## Sprite
```csharp
public readonly Claw.Graphics.Sprite Sprite;
```
## Spacing
```csharp
public Claw.Vector2 Spacing;
```
## Glyphs
```csharp
public readonly System.Collections.Generic.Dictionary<char,Claw.Graphics.Glyph> Glyphs;
```
## Load
```csharp
public static SpriteFont Load(string path) { }
```
Carrega uma fonte.<br />
**Retorna**: A fonte ou null (se não for um arquivo válido).<br />
## AddKerning
```csharp
public SpriteFont AddKerning(char first, char second, float value) { }
```
Adiciona um par de kerning para este [SpriteFont](api/Claw/Graphics/SpriteFont.md#SpriteFont) .<br />
## MeasureString
```csharp
public Claw.Vector2 MeasureString(string text) { }
```
Retorna as dimensões que a [Não encontrado!] teria com este [SpriteFont](api/Claw/Graphics/SpriteFont.md#SpriteFont) .<br />
## MeasureChar
```csharp
public Claw.Vector2 MeasureChar(char character) { }
```
Retorna as dimensões que o tamanho da área de um [Glyph](api/Claw/Graphics/Glyph.md#Glyph) .<br />
## ToString
```csharp
public virtual string ToString() { }
```
