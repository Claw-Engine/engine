# Mathf
```csharp
public static class Mathf
```
Conjunto de funções matemáticas.<br />
## PI
```csharp
public const float PI;
```
## Approximately
```csharp
public static bool Approximately(float a, float b, float tolerance) { }
```
Checa se um número é aproximadamente igual a outro.<br />
## ToRadians
```csharp
public static float ToRadians(float degrees) { }
```
Transforma graus em radianos.<br />
## ToDegrees
```csharp
public static float ToDegrees(float radians) { }
```
Transforma radianos em graus.<br />
## Clamp
```csharp
public static int Clamp(int value, int min, int max) { }
```
Retorna um número que respeite os limites mínimo e máximo.<br />
## Clamp
```csharp
public static float Clamp(float value, float min, float max) { }
```
Retorna um número que respeite os limites mínimo e máximo.<br />
## LoopValue
```csharp
public static float LoopValue(float value, float max, float min) { }
```
Limita um valor, em forma de loop.<br />
## Lerp
```csharp
public static float Lerp(float a, float b, float amount) { }
```
Realiza a interpolação linear entre dois valores.<br />
**a**: Valor atual.<br />
**b**: Valor alvo.<br />
**amount**: Valor de ponderação (entre 0 e 1).<br />
## DeltaLerp
```csharp
public static float DeltaLerp(float a, float b, float amount, bool scaled) { }
```
Realiza a interpolação linear entre dois valores, usando delta time.<br />
**a**: Valor atual.<br />
**b**: Valor alvo.<br />
**amount**: Valor de ponderação.<br />
**scaled**: Se o delta time será [Time.DeltaTime](/api/Claw/Time.md#DeltaTime) (true) ou [Time.UnscaledDeltaTime](/api/Claw/Time.md#UnscaledDeltaTime) (false).<br />
## Approach
```csharp
public static float Approach(float value, float target, float shift) { }
```
Incrementa um valor por um determinado deslocamento, mas nunca além do valor final.<br />
## ToGrid
```csharp
public static float ToGrid(float value, int grid) { }
```
Transforma um valor em um valor em grid.<br />
## ToGrid
```csharp
public static Claw.Vector2 ToGrid(Claw.Vector2 position, int grid) { }
```
Transforma uma posição em uma posição em grid.<br />
## ToGrid
```csharp
public static Claw.Vector2 ToGrid(Claw.Vector2 position, Claw.Vector2 grid) { }
```
Transforma uma posição em uma posição em grid.<br />
## Get1DIndex
```csharp
public static int Get1DIndex(Claw.Vector2 index, float width) { }
```
Transforma um index 2D em um index 1D.<br />
## Get2DIndex
```csharp
public static Claw.Vector2 Get2DIndex(int index, float width) { }
```
Transforma um index 1D em um index 2D.<br />
## GetBezierPath
```csharp
public static Claw.Vector2[] GetBezierPath(int segments, Claw.Vector2 point0, Claw.Vector2 point1, Claw.Vector2 point2, Claw.Vector2 point3) { }
```
Retorna uma lista de pontos de uma curva de Bézier.<br />
