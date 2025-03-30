# TaggedPlayer
```csharp
public sealed class TaggedPlayer
```
Representa a instância de um player dentro do [TaggedInput](/api/Claw/Input/Systems/TaggedInput.md#TaggedInput) .<br />
## TaggedPlayer
```csharp
public TaggedPlayer(int gamePad) { }
```
Cria uma instância de player.<br />
**gamePad**: -1 para não usar o gamepad.<br />
## GamePad
```csharp
public int GamePad;
```
## TaggedPlayer[string tagName]
```csharp
public Claw.Input.Systems.IBaseTag TaggedPlayer[string tagName] { get; } 
```
## AddTag
```csharp
public TaggedPlayer AddTag(string tagName, Claw.Input.Systems.IBaseTag tag) { }
```
Adiciona uma tag com botões configurados.<br />
## GetTag
```csharp
public T GetTag<T>(string tagName) { }
```
Retorna uma tag.<br />
## Update
```csharp
public void Update() { }
```
Executa [IBaseTag.Update](/api/Claw/Input/Systems/IBaseTag.md#Update) em todas as tags.<br />
