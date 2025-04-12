# Animator
```csharp
public sealed class Animator
```
Cuida do manuseio das animações de um [IAnimatable](/api/Claw/Graphics/IAnimatable.md#IAnimatable) .<br />
## Animator
```csharp
public Animator() { }
```
## Animator
```csharp
public Animator(Claw.Graphics.Animation[] animations) { }
```
## Animator
```csharp
public Animator(Claw.Graphics.IAnimatable animatable, Claw.Graphics.Animation[] animations) { }
```
## Animation
```csharp
public int Animation;
```
## Frame
```csharp
public int Frame;
```
## Stop
```csharp
public bool Stop;
```
## Reverse
```csharp
public bool Reverse;
```
## Animatable
```csharp
public Claw.Graphics.IAnimatable Animatable;
```
## AnimationCount
```csharp
public int AnimationCount { get; } 
```
## OnAnimationEnd
```csharp
public event System.Action OnAnimationEnd;
```
## Finalize
```csharp
protected virtual void Finalize() { }
```
## Dispose
```csharp
public virtual void Dispose() { }
```
## Play
```csharp
public void Play(int index, bool playIfNot, int frame) { }
```
Inicia uma animação pelo index.<br />
**frame**: Frame que a animação vai começar.<br />
## Play
```csharp
public void Play(string name, bool playIfNot, int frame) { }
```
Inicia uma animação pelo nome.<br />
**frame**: Frame que a animação vai começar.<br />
## GetAnimationIndex
```csharp
public int GetAnimationIndex(string name) { }
```
Retorna o index de uma animação pelo nome.<br />
## GetAnimationName
```csharp
public string GetAnimationName(int index) { }
```
Retorna o nome de uma animação pelo index.<br />
## Frames
```csharp
public int Frames(string name) { }
```
Retorna o número de frames de uma animação pelo nome.<br />
## Frames
```csharp
public int Frames(int index) { }
```
Retorna o número de frames de uma animação pelo index.<br />
## AddAnimation
```csharp
public void AddAnimation(Claw.Graphics.Animation animation) { }
```
Adiciona uma animação nova.<br />
## RemoveAnimation
```csharp
public void RemoveAnimation(string name) { }
```
Remove uma animação pelo nome.<br />
## RemoveAnimation
```csharp
public void RemoveAnimation(int index) { }
```
Remove uma animação pelo index.<br />
## Run
```csharp
public void Run() { }
```
