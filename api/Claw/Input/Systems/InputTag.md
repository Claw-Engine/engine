# InputTag
```csharp
public sealed class InputTag
```
Representa um input dentro do [TaggedInput](api/Claw/Input/Systems/TaggedInput.md#TaggedInput) .<br />
## InputTag
```csharp
public InputTag(Claw.Input.Keys key, Claw.Input.Keys altKey, Claw.Input.Buttons? button, Claw.Input.Buttons? altButton, Claw.Input.MouseButtons? mouse, float bufferTime) { }
```
Cria uma instância de input de botões.<br />
**mouse**: -1 para não usar o mouse.<br />
## InputTag
```csharp
public InputTag(Claw.Input.Keys key, Claw.Input.Buttons? button, Claw.Input.MouseButtons? mouse, float bufferTime) { }
```
Cria uma instância de input de botões.<br />
**mouse**: -1 para não usar o mouse.<br />
## InputTag
```csharp
public InputTag(Claw.Input.MouseButtons mouse, float bufferTime) { }
```
Cria uma instância de input de botões.<br />
## MouseButton
```csharp
public Claw.Input.MouseButtons? MouseButton;
```
## IsDown
```csharp
public bool IsDown;
```
## IsPressed
```csharp
public bool IsPressed;
```
## IsReleased
```csharp
public bool IsReleased;
```
## Buffer
```csharp
public float Buffer;
```
## Key
```csharp
public Claw.Input.Keys Key;
```
## AltKey
```csharp
public Claw.Input.Keys AltKey;
```
## Button
```csharp
public Claw.Input.Buttons? Button;
```
## AltButton
```csharp
public Claw.Input.Buttons? AltButton;
```
## Update
```csharp
public virtual void Update(Claw.Input.Systems.TaggedPlayer player) { }
```
