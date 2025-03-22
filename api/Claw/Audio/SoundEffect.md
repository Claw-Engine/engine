# SoundEffect
```csharp
public sealed class SoundEffect
```
Representa um efeito sonoro no jogo.<br />
## SoundEffect
```csharp
public SoundEffect(Claw.Audio.Channels channels, float[] samples) { }
```
**samples**: Valores entre -1 e 1.<br />
## Channels
```csharp
public readonly Claw.Audio.Channels Channels;
```
## Duration
```csharp
public readonly float Duration;
```
Duração deste áudio, em segundos.<br />
## Length
```csharp
public long Length { get; } 
```
## CreateInstance
```csharp
public Claw.Audio.SoundEffectInstance CreateInstance(Claw.Audio.SoundEffectGroup group) { }
```
Cria um [SoundEffectInstance](/Claw/Audio/SoundEffectInstance.md#SoundEffectInstance) deste áudio.<br />
## Load
```csharp
public static SoundEffect Load(string path) { }
```
Carrega um áudio.<br />
**Retorna**: O áudio ou null (se não for um arquivo válido).<br />
