# Vector2
```csharp
public struct Vector2
```
Descreve um vetor 2D.<br />
## Vector2
```csharp
public Vector2(float x, float y) { }
```
## Vector2
```csharp
public Vector2(float value) { }
```
## X
```csharp
public float X;
```
## Y
```csharp
public float Y;
```
## Zero
```csharp
public static readonly Vector2 Zero;
```
X: 0; Y: 0.<br />
## One
```csharp
public static readonly Vector2 One;
```
X: 1; Y: 1.<br />
## UnitX
```csharp
public static readonly Vector2 UnitX;
```
X: 1; Y: 0.<br />
## UnitY
```csharp
public static readonly Vector2 UnitY;
```
X: 0; Y: 1.<br />
## Invert
```csharp
public void Invert() { }
```
Troca o X pelo Y e vice-versa.<br />
## Normalize
```csharp
public void Normalize() { }
```
Transforma este [Vector2](/api/Claw/Vector2.md#Vector2) em um vetor de magnitude 1 com a mesma direção.<br />
## Abs
```csharp
public static Vector2 Abs(Vector2 value) { }
```
Retorna um vetor com o X e Y absolutos.<br />
## Clamp
```csharp
public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) { }
```
Retorna um vetor que respeite os limites mínimo e máximo.<br />
## Min
```csharp
public static Vector2 Min(Vector2 a, Vector2 b) { }
```
Retorna um vetor com o menor valor de X e o menor valor de Y.<br />
## Max
```csharp
public static Vector2 Max(Vector2 a, Vector2 b) { }
```
Retorna um vetor com o maior valor de X e o maior valor de Y.<br />
## GetAngle
```csharp
public static float GetAngle(Vector2 position, Vector2 direction) { }
```
Retorna o ângulo entre duas posições.<br />
**Retorna**: Graus.<br />
## Distance
```csharp
public static float Distance(Vector2 a, Vector2 b) { }
```
Retorna a distância entre dois vetores.<br />
## Length
```csharp
public static float Length(Vector2 value) { }
```
Retorna a distância entre um vetor e o ponto 0,0.<br />
## Dot
```csharp
public static float Dot(Vector2 a, Vector2 b) { }
```
Retorna o produto escalar entre dois vetores.<br />
## Cross
```csharp
public static float Cross(Vector2 a, Vector2 b) { }
```
Retorna o produto cruzado entre dois vetores.<br />
## Cross
```csharp
public static Vector2 Cross(Vector2 a, float s) { }
```
Retorna o produto cruzado entre um vetor e um escalar.<br />
## Normalize
```csharp
public static Vector2 Normalize(Vector2 value) { }
```
Transforma um [Vector2](/api/Claw/Vector2.md#Vector2) em um vetor de magnitude 1 com a mesma direção.<br />
## Rotate
```csharp
public static Vector2 Rotate(Vector2 point, Vector2 origin, float rotation) { }
```
Retorna um [Vector2](/api/Claw/Vector2.md#Vector2) rotacionado.<br />
**rotation**: Graus.<br />
## Lerp
```csharp
public static Vector2 Lerp(Vector2 a, Vector2 b, float amount) { }
```
Realiza a interpolação linear entre dois vetores.<br />
**a**: Valor atual.<br />
**b**: Valor alvo.<br />
**amount**: Valor de ponderação (entre 0 e 1).<br />
## DeltaLerp
```csharp
public static Vector2 DeltaLerp(Vector2 a, Vector2 b, float amount, bool scaled) { }
```
Realiza a interpolação linear entre dois vetores, usando delta time.<br />
**a**: Valor atual.<br />
**b**: Valor alvo.<br />
**amount**: Valor de ponderação.<br />
**scaled**: Se o delta time será [Time.DeltaTime](/api/Claw/Time.md#DeltaTime) (true) ou [Time.UnscaledDeltaTime](/api/Claw/Time.md#UnscaledDeltaTime) (false).<br />
## Approach
```csharp
public static Vector2 Approach(Vector2 value, Vector2 target, float shift) { }
```
Incrementa um valor por um determinado deslocamento, mas nunca além do valor final.<br />
## Approach
```csharp
public static Vector2 Approach(Vector2 value, Vector2 target, Vector2 shift) { }
```
Incrementa um valor por um determinado deslocamento, mas nunca além do valor final.<br />
## LengthDir
```csharp
public static Vector2 LengthDir(Vector2 distance, float angle) { }
```
Retorna o módulo horizontal do vetor determinado pelo comprimento e direção indicados.<br />
**angle**: Graus.<br />
## LengthDir
```csharp
public static Vector2 LengthDir(float distance, float angle) { }
```
Retorna o módulo horizontal do vetor determinado pelo comprimento e direção indicados.<br />
**angle**: Graus.<br />
## Transform
```csharp
public static Vector2 Transform(Vector2 value, Claw.Quaternion rotation) { }
```
Cria um novo [Vector2](/api/Claw/Vector2.md#Vector2) que contém a transformação do vetor 2d pelo [Quaternion](/api/Claw/Quaternion.md#Quaternion) especificado, representando a rotação.<br />
## FindFacing
```csharp
public static Vector2 FindFacing(float angle) { }
```
Transforma um ângulo em um [Vector2](/api/Claw/Vector2.md#Vector2) .<br />
**angle**: Graus.<br />
## ToString
```csharp
public virtual string ToString() { }
```
Retorna uma string representando este vetor 2D no formato:
            {X:[ [Vector2.X](/api/Claw/Vector2.md#X) ] Y:[ [Vector2.Y](/api/Claw/Vector2.md#Y) ]}<br />
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
public static Vector2 operator +(Vector2 a, Vector2 b) { }
```
## operator -
```csharp
public static Vector2 operator -(Vector2 a, Vector2 b) { }
```
## operator -
```csharp
public static Vector2 operator -(Vector2 value) { }
```
## operator *
```csharp
public static Vector2 operator *(Vector2 a, Vector2 b) { }
```
## operator *
```csharp
public static Vector2 operator *(Vector2 a, float b) { }
```
## operator *
```csharp
public static Vector2 operator *(float a, Vector2 b) { }
```
## operator /
```csharp
public static Vector2 operator /(Vector2 a, Vector2 b) { }
```
## operator /
```csharp
public static Vector2 operator /(Vector2 a, float b) { }
```
## operator /
```csharp
public static Vector2 operator /(float a, Vector2 b) { }
```
## operator ==
```csharp
public static bool operator ==(Vector2 a, Vector2 b) { }
```
## operator !=
```csharp
public static bool operator !=(Vector2 a, Vector2 b) { }
```
