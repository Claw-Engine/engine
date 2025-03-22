# Parallax
```csharp
public sealed class Parallax : Claw.Modules.Module
```
Representa uma sequência de backgrounds para efeito de parallax.<br />
## Parallax
```csharp
public Parallax() { }
```
## UseDeltaTime
```csharp
public bool UseDeltaTime;
```
## UseScaledDeltaTime
```csharp
public bool UseScaledDeltaTime;
```
## Color
```csharp
public Claw.Color Color;
```
## Backgrounds
```csharp
public System.Collections.Generic.List<Claw.Modules.Background> Backgrounds;
```
## Initialize
```csharp
public virtual void Initialize() { }
```
## Step
```csharp
public virtual void Step() { }
```
## ChangeAllSpeed
```csharp
public void ChangeAllSpeed(float value) { }
```
Muda a velocidade de todos os backgrounds.<br />
## ChangeAllZoom
```csharp
public void ChangeAllZoom(float value) { }
```
Muda o zoom de todos os backgrounds.<br />
## ChangeAllDirection
```csharp
public void ChangeAllDirection(Claw.Vector2 value) { }
```
Muda a direção de todos os backgrounds.<br />
## Render
```csharp
public virtual void Render() { }
```
# Axis
```csharp
public enum Axis
```
Define os eixos em que o background poderá se repetir durante o parallax.<br />
|Nome|Valor|Descrição|
|---|---|---|
|Horizontal|0||
|Vertical|1||
