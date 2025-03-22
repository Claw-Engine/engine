# Quaternion
```csharp
public struct Quaternion
```
Descreve a representação de rotações 3D.<br />
## Quaternion
```csharp
public Quaternion(float x, float y, float z, float w) { }
```
## Quaternion
```csharp
public Quaternion(float value) { }
```
## Quaternion
```csharp
public Quaternion(Claw.Vector3 vector, float w) { }
```
## X
```csharp
public float X;
```
## Y
```csharp
public float Y;
```
## Z
```csharp
public float Z;
```
## W
```csharp
public float W;
```
## Identity
```csharp
public static Quaternion Identity { get; } 
```
## Normalize
```csharp
public void Normalize() { }
```
Redimensiona a magnitude deste [Quaternion](/api/Claw/Quaternion.md#Quaternion) para um comprimento em unidade.<br />
## CreateFromAxisAngle
```csharp
public static Quaternion CreateFromAxisAngle(Claw.Vector3 axis, float angle) { }
```
Cria um [Quaternion](/api/Claw/Quaternion.md#Quaternion) que contém a versão conjugada do [Quaternion](/api/Claw/Quaternion.md#Quaternion) especificado.<br />
**angle**: Radianos.<br />
## Normalize
```csharp
public static Quaternion Normalize(Quaternion value) { }
```
Redimensiona a magnitude de um [Quaternion](/api/Claw/Quaternion.md#Quaternion) para um comprimento em unidade.<br />
## ToString
```csharp
public virtual string ToString() { }
```
Retorna uma string representando este quaternion no formato:
            {X:[ [Quaternion.X](/api/Claw/Quaternion.md#X) ] Y:[ [Quaternion.Y](/api/Claw/Quaternion.md#Y) ] Z:[ [Quaternion.Z](/api/Claw/Quaternion.md#Z) ] W:[ [Quaternion.W](/api/Claw/Quaternion.md#W) ]}<br />
## Equals
```csharp
public virtual bool Equals(object obj) { }
```
## GetHashCode
```csharp
public virtual int GetHashCode() { }
```
## operator +
```csharp
public static Quaternion operator +(Quaternion a, Quaternion b) { }
```
## operator -
```csharp
public static Quaternion operator -(Quaternion a, Quaternion b) { }
```
## operator -
```csharp
public static Quaternion operator -(Quaternion value) { }
```
## operator *
```csharp
public static Quaternion operator *(Quaternion a, Quaternion b) { }
```
## operator *
```csharp
public static Quaternion operator *(Quaternion a, float scale) { }
```
## operator *
```csharp
public static Quaternion operator *(float scale, Quaternion a) { }
```
## operator /
```csharp
public static Quaternion operator /(Quaternion a, Quaternion b) { }
```
## operator /
```csharp
public static Quaternion operator /(Quaternion a, float scale) { }
```
## operator /
```csharp
public static Quaternion operator /(float scale, Quaternion a) { }
```
## operator ==
```csharp
public static bool operator ==(Quaternion a, Quaternion b) { }
```
## operator !=
```csharp
public static bool operator !=(Quaternion a, Quaternion b) { }
```
