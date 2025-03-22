# CheatCode
```csharp
public sealed class CheatCode
```
Cria uma sequência de [InputTag](api/Claw/Input/Systems/InputTag.md#InputTag) s para fazer um código de trapaça.<br />
## CheatCode
```csharp
public CheatCode(string player, Claw.Input.Systems.TaggedInput input, string[] tags) { }
```
## Completed
```csharp
public System.Action Completed;
```
É executado assim que o código é completado.<br />
## TimeTolerance
```csharp
public static float TimeTolerance;
```
Tempo de tolerância entre um clique e outro.<br />
## Got
```csharp
public bool Got { get; private set; } 
```
Diz se a sequência foi completada.<br />
## Reset
```csharp
public void Reset() { }
```
Reseta o cheatcode para poder ser usado novamente.<br />
## Update
```csharp
public void Update() { }
```
Atualiza o [CheatCode.Got](api/Claw/Input/Systems/CheatCode.md#Got) .<br />
