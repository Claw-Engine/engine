# MovementTag
```csharp
public sealed class MovementTag
```
Representa um input de movimento dentro do [TaggedInput](/Claw/Input/Systems/TaggedInput.md#TaggedInput) .<br />
## MovementTag
```csharp
public MovementTag(bool leftThumb, bool rightThumb, System.ValueTuple<Claw.Input.Keys,Claw.Input.Keys,Claw.Input.Keys,Claw.Input.Keys> keys, System.ValueTuple<Claw.Input.Keys,Claw.Input.Keys,Claw.Input.Keys,Claw.Input.Keys> altKeys, System.ValueTuple<Claw.Input.Buttons,Claw.Input.Buttons,Claw.Input.Buttons,Claw.Input.Buttons>? buttons) { }
```
Cria uma instância de uma tag de movimento.<br />
**keys**: Up, Down, Right, Left.<br />
**altKeys**: Up, Down, Right, Left.<br />
**buttons**: Up, Down, Right, Left.<br />
## MovementTag
```csharp
public MovementTag(bool leftThumb, bool rightThumb, System.ValueTuple<Claw.Input.Keys,Claw.Input.Keys,Claw.Input.Keys,Claw.Input.Keys> keys, System.ValueTuple<Claw.Input.Buttons,Claw.Input.Buttons,Claw.Input.Buttons,Claw.Input.Buttons>? buttons) { }
```
Cria uma instância de uma tag de movimento.<br />
**keys**: Up, Down, Right, Left.<br />
**buttons**: Up, Down, Right, Left.<br />
## UseLeftThumb
```csharp
public bool UseLeftThumb;
```
## UseRightThumb
```csharp
public bool UseRightThumb;
```
## CurrentAxis
```csharp
public Claw.Vector2 CurrentAxis;
```
## Keys
```csharp
public System.ValueTuple<Claw.Input.Keys,Claw.Input.Keys,Claw.Input.Keys,Claw.Input.Keys> Keys;
```
## AltKeys
```csharp
public System.ValueTuple<Claw.Input.Keys,Claw.Input.Keys,Claw.Input.Keys,Claw.Input.Keys> AltKeys;
```
## Buttons
```csharp
public System.ValueTuple<Claw.Input.Buttons,Claw.Input.Buttons,Claw.Input.Buttons,Claw.Input.Buttons>? Buttons;
```
## Update
```csharp
public virtual void Update(Claw.Input.Systems.TaggedPlayer player) { }
```
