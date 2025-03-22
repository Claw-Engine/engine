# SoundEffectInstance
```csharp
public sealed class SoundEffectInstance
```
Representa uma instância de [SoundEffect](api/Claw/Audio/SoundEffect.md#SoundEffect) .<br />
## SoundEffectInstance
```csharp
public SoundEffectInstance(Claw.Audio.SoundEffect audio, Claw.Audio.SoundEffectGroup group) { }
```
## IsLooped
```csharp
public bool IsLooped;
```
## Group
```csharp
public readonly Claw.Audio.SoundEffectGroup Group;
```
## LeftVolume
```csharp
public float LeftVolume { get; set; } 
```
Volume do áudio no lado esquerdo (entre 0 e 1).<br />
## RightVolume
```csharp
public float RightVolume { get; set; } 
```
Volume do áudio no lado direito (entre 0 e 1).<br />
## Duration
```csharp
public float Duration { get; } 
```
Duração do áudio, em segundos.<br />
## Current
```csharp
public float Current { get; } 
```
Momento em que o áudio está, em segundos.<br />
## SetVolume
```csharp
public SoundEffectInstance SetVolume(float volume) { }
```
Altera o [SoundEffectInstance.LeftVolume](api/Claw/Audio/SoundEffectInstance.md#LeftVolume) e [SoundEffectInstance.RightVolume](api/Claw/Audio/SoundEffectInstance.md#RightVolume) para um mesmo volume.<br />
