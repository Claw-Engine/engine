# TextureAtlas
```csharp
public sealed class TextureAtlas
```
Representa um atlas de texturas.<br />
## TextureAtlas
```csharp
public TextureAtlas(Claw.Graphics.Sprite[] sprites) { }
```
## Page
```csharp
public Claw.Graphics.Texture Page { get; private set; } 
```
## TextureAtlas[string sprite]
```csharp
public Claw.Graphics.Sprite TextureAtlas[string sprite] { get; } 
```
Retorna uma sprite específica.<br />
## Load
```csharp
public static TextureAtlas Load(string path) { }
```
Carrega um atlas de texturas.<br />
**Retorna**: O atlas ou null (se não for um arquivo válido).<br />
