# TaggedInput
```csharp
public sealed class TaggedInput
```
Gerencia inputs no jogo através do método chave > valor.<br />
## TaggedInput
```csharp
public TaggedInput() { }
```
## DeadAxis
```csharp
public static Claw.Vector2 DeadAxis;
```
## TaggedInput[string playerName]
```csharp
public Claw.Input.Systems.TaggedPlayer TaggedInput[string playerName] { get; } 
```
Retorna um player do input.<br />
## TaggedInput[string playerName, string tagName]
```csharp
public Claw.Input.Systems.IBaseTag TaggedInput[string playerName, string tagName] { get; } 
```
Retorna uma tag do input.<br />
## AddPlayer
```csharp
public Claw.Input.Systems.TaggedPlayer AddPlayer(string playerName, Claw.Input.Systems.TaggedPlayer player) { }
```
Adiciona um player.<br />
## RemovePlayer
```csharp
public void RemovePlayer(string playerName) { }
```
Remove um player.<br />
## Update
```csharp
public void Update() { }
```
Realiza o update dos players.<br />
