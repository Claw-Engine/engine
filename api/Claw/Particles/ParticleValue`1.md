# ParticleValue\<T>
```csharp
public struct ParticleValue<T>
```
Lida com gradientes e valores fixos ou aleatórios usados por partículas.<br />
## ParticleValue\<T>
```csharp
public ParticleValue<T>(T? fixedValue) { }
```
## ParticleValue\<T>
```csharp
public ParticleValue<T>(bool isGradient, float lerpValue, T[] valueList) { }
```
## FixedValue
```csharp
public T? FixedValue;
```
## ValueList
```csharp
public T[] ValueList;
```
## IsGradient
```csharp
public bool IsGradient;
```
## LerpValue
```csharp
public float LerpValue;
```
## GradientFunction
```csharp
public static System.Func<T,T,float,bool,T> GradientFunction;
```
Define uma função que para executar o gradiente.<br />
T1: Valor atual.<br />
T2: Valor destino.<br />
T3: Valor para o lerp.<br />
T4: Valor que diz se o [ParticleEmitterConfig.UseScaledTime](/api/Claw/Particles/ParticleEmitterConfig.md#UseScaledTime) é verdadeiro.<br />
TResult: Resultado do cálculo.<br />
<br />
## GetValue
```csharp
public T? GetValue(Claw.Particles.ParticleEmitter emitter, bool updateGradient) { }
```
Retorna um valor fixo, se ! [ParticleValue<T>.IsGradient](/api/Claw/Particles/ParticleValue`1.md#IsGradient) e a [ParticleValue<T>.ValueList](/api/Claw/Particles/ParticleValue`1.md#ValueList) não ter elementos.<br />
Retorna um valor aleatório, se ! [ParticleValue<T>.IsGradient](/api/Claw/Particles/ParticleValue`1.md#IsGradient) e a [ParticleValue<T>.ValueList](/api/Claw/Particles/ParticleValue`1.md#ValueList) ter elementos.<br />
Retorna o estágio do gradiente, se [ParticleValue<T>.IsGradient](/api/Claw/Particles/ParticleValue`1.md#IsGradient) e a [ParticleValue<T>.ValueList](/api/Claw/Particles/ParticleValue`1.md#ValueList) ter elementos.<br />
<br />
**updateGradient**: Define se o gradiente será calculado ou não.<br />
## ResetValue
```csharp
public void ResetValue() { }
```
Reseta o valor atual do gradiente.<br />
## implicit operator
```csharp
public static ParticleValue<T> implicit operator(T fixedValue) { }
```
