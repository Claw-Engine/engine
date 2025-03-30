# RayCaster
```csharp
public sealed class RayCaster
```
Calculador de raycast.<br />
## RayCaster
```csharp
public RayCaster(Claw.Line ray, float maxDistance, System.Func<Claw.Vector2,bool> onMove, Claw.Vector2 cellSize) { }
```
## RayCaster
```csharp
public RayCaster(Claw.Line ray, System.Func<Claw.Vector2,bool> onMove, Claw.Vector2 cellSize) { }
```
## Hit
```csharp
public bool Hit { get; private set; } 
```
## Ended
```csharp
public bool Ended { get; private set; } 
```
## HitPoint
```csharp
public Claw.Vector2? HitPoint { get; private set; } 
```
## OnMove
```csharp
public event System.Func<Claw.Vector2,bool> OnMove;
```
## Cast
```csharp
public static void Cast(Claw.Line ray, System.Func<Claw.Vector2,bool> onMove, out Claw.Vector2? hitPoint, Claw.Vector2 cellSize) { }
```
Realiza a movimentação do raio de um ponto ao outro.<br />
**onMove**: É executado sempre que o raio se move.<br />
Parâmetro Vector2: A posição atual do raio.<br />
Retorno bool: O resultado do seu cálculo de colisão (true para parar o raio, false para prosseguir).<br />
<br />
## Cast
```csharp
public static void Cast(Claw.Line ray, float maxDistance, System.Func<Claw.Vector2,bool> onMove, out Claw.Vector2? hitPoint, Claw.Vector2 cellSize) { }
```
Realiza a movimentação do raio de um ponto ao outro.<br />
**maxDistance**: Distância em células.<br />
**onMove**: É executado sempre que o raio se move.<br />
Parâmetro Vector2: A posição atual do raio.<br />
Retorno bool: O resultado do seu cálculo de colisão (true para parar o raio, false para prosseguir).<br />
<br />
## Move
```csharp
public void Move() { }
```
Move o raio.<br />
