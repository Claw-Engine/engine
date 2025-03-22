# Music
```csharp
public sealed class Music
```
Representa uma música no jogo.<br />
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
public readonly long Length;
```
## AudioStart
```csharp
public readonly long AudioStart;
```
## Volume
```csharp
public float Volume { get; set; } 
```
Volume da música (entre 0 e 1).<br />
## Current
```csharp
public float Current { get; } 
```
Momento em que o áudio está, em segundos.<br />
## Finalize
```csharp
protected virtual void Finalize() { }
```
## Dispose
```csharp
public virtual void Dispose() { }
```
## Load
```csharp
public static Music Load(string path) { }
```
Carrega um áudio, em modo Stream.<br />
**Retorna**: O áudio ou null (se não for um arquivo válido).<br />
