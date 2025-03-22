# TouchInput
```csharp
public static class TouchInput
```
Realiza os gatilhos de input touch.<br />
## OnPressed
```csharp
public static event TouchEvent OnPressed;
```
## OnReleased
```csharp
public static event TouchEvent OnReleased;
```
## OnMoved
```csharp
public static event MotionEvent OnMoved;
```
# TouchEvent
```csharp
public sealed class TouchEvent : System.MulticastDelegate
```
## TouchEvent
```csharp
public TouchEvent(object object, System.IntPtr method) { }
```
## Invoke
```csharp
public virtual void Invoke(int index, float pressure, Claw.Vector2 position) { }
```
## BeginInvoke
```csharp
public virtual System.IAsyncResult BeginInvoke(int index, float pressure, Claw.Vector2 position, System.AsyncCallback callback, object object) { }
```
## EndInvoke
```csharp
public virtual void EndInvoke(System.IAsyncResult result) { }
```
# MotionEvent
```csharp
public sealed class MotionEvent : System.MulticastDelegate
```
## MotionEvent
```csharp
public MotionEvent(object object, System.IntPtr method) { }
```
## Invoke
```csharp
public virtual void Invoke(int index, float pressure, Claw.Vector2 position, Claw.Vector2 motion) { }
```
## BeginInvoke
```csharp
public virtual System.IAsyncResult BeginInvoke(int index, float pressure, Claw.Vector2 position, Claw.Vector2 motion, System.AsyncCallback callback, object object) { }
```
## EndInvoke
```csharp
public virtual void EndInvoke(System.IAsyncResult result) { }
```
