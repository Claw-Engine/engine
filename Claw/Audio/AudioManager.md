# AudioManager
```csharp
public sealed class AudioManager
```
Representa o controle de áudios do jogo.<br />
## PauseMusic
```csharp
public bool PauseMusic;
```
## MaxConcurrent
```csharp
public static int MaxConcurrent;
```
Máximo de efeitos sonoros simultâneos.<br />
## FadeSpeed
```csharp
public static float FadeSpeed;
```
O quanto o volume deve aumentar/diminuir a cada sample do fade.<br />
## TrackDistance
```csharp
public static float TrackDistance;
```
A distância do fim da música e do começo do fade, em segundos.<br />
## SampleRate
```csharp
public const int SampleRate;
```
## MasterVolume
```csharp
public float MasterVolume { get; set; } 
```
Volume geral (entre 0 e 1).<br />
## MusicVolume
```csharp
public float MusicVolume { get; set; } 
```
Volume geral das músicas (entre 0 e 1).<br />
## OnMusicChange
```csharp
public event System.Action OnMusicChange;
```
Evento executado quando a música é trocada.<br />
## OnSoundEffectEnd
```csharp
public event System.Action<Claw.Audio.SoundEffectInstance> OnSoundEffectEnd;
```
Evento executado quando um efeito sonoro termina, sem loop.<br />
## Finalize
```csharp
protected virtual void Finalize() { }
```
## Dispose
```csharp
public virtual void Dispose() { }
```
## CalculateDuration
```csharp
public static float CalculateDuration(long sampleLength, Claw.Audio.Channels channels) { }
```
Calcula duração de um áudio.<br />
## GetVolume
```csharp
public float GetVolume(Claw.Audio.SoundEffectGroup group) { }
```
Retorna o volume geral de um grupo.<br />
## SetVolume
```csharp
public void SetVolume(float value, Claw.Audio.SoundEffectGroup group) { }
```
Altera o volume geral de um grupo.<br />
**value**: Entre 0 e 1.<br />
## Play
```csharp
public void Play(Claw.Audio.SoundEffectInstance soundEffect) { }
```
Inicia/reinicia um sonoro.<br />
## Stop
```csharp
public void Stop(Claw.Audio.SoundEffectInstance soundEffect) { }
```
Pausa um efeito sonoro.<br />
## AddMusic
```csharp
public void AddMusic(Claw.Audio.Music music) { }
```
Adiciona uma música na lista de faixas.<br />
## JumpMusic
```csharp
public void JumpMusic() { }
```
Pula para a próxima música.<br />
## BackMusic
```csharp
public void BackMusic() { }
```
Volta para a música anterior.<br />
## ResetMusic
```csharp
public void ResetMusic() { }
```
Reseta a música atual.<br />
## ClearTrack
```csharp
public void ClearTrack() { }
```
Limpa a lista de faixas.<br />
## CurrentMusic
```csharp
public Claw.Audio.Music CurrentMusic() { }
```
Retorna a música atual.<br />
