# TextRenderer
```csharp
public sealed class TextRenderer : Claw.Modules.Module
```
Realiza a renderização de textos com diferentes efeitos para cada bloco.<br />
## TextRenderer
```csharp
public TextRenderer() { }
```
## DefaultConfig
```csharp
public Claw.Modules.TextConfig DefaultConfig;
```
Configurações usadas no caso das do bloco serem nulas.<br />
## MaxChar
```csharp
public int MaxChar;
```
Até que caractere ele vai desenhar (negativo para desenhar tudo).<br />
## UseScaledTime
```csharp
public bool UseScaledTime;
```
## RotationAmount
```csharp
public float RotationAmount;
```
## PulsateAmount
```csharp
public float PulsateAmount;
```
## WaveSpeed
```csharp
public float WaveSpeed;
```
## WaveAmplitude
```csharp
public float WaveAmplitude;
```
## PulsateLimit
```csharp
public Claw.Vector2 PulsateLimit;
```
## ScreamOffset
```csharp
public Claw.Vector2 ScreamOffset;
```
## MovingSpeed
```csharp
public Claw.Vector2 MovingSpeed;
```
## MovingAmplitude
```csharp
public Claw.Vector2 MovingAmplitude;
```
## TextConfigs
```csharp
public static System.Collections.Generic.Dictionary<string,Claw.Modules.TextConfig> TextConfigs;
```
Dicionário para armazenar configurações prontas.<br />
## MaxLength
```csharp
public int MaxLength { get; set; } 
```
Máximo de caracteres das linhas para o [TextRenderer.TextWrap](api/Claw/Modules/TextRenderer.md#TextWrap) .<br />
## Text
```csharp
public string Text { get; set; } 
```
## FilteredText
```csharp
public string FilteredText { get; private set; } 
```
Armazena o seu texto, sem as tags.<br />
## TextWrap
```csharp
public Claw.Modules.TextWrap TextWrap { get; set; } 
```
## Initialize
```csharp
public virtual void Initialize() { }
```
## Step
```csharp
public virtual void Step() { }
```
## Render
```csharp
public virtual void Render() { }
```
