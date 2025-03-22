# Line
```csharp
public struct Line
```
Descreve uma linha 2D.<br />
## Line
```csharp
public Line(float xStart, float yStart, float xEnd, float yEnd) { }
```
## Line
```csharp
public Line(Claw.Vector2 start, Claw.Vector2 end) { }
```
## Start
```csharp
public Claw.Vector2 Start;
```
## End
```csharp
public Claw.Vector2 End;
```
## Normalize
```csharp
public void Normalize() { }
```
Transforma os vetores desta linha em vetores unitários com as mesmas direções.<br />
## Lerp
```csharp
public static Line Lerp(Line value1, Line value2, float amount) { }
```
Cria uma nova linha de uma interpolação linear com as linhas especificadas.<br />
**value1**: Valor atual.<br />
**value2**: Valor de destino.<br />
**amount**: Valor de ponderação (entre 0 e 1).<br />
## Clamp
```csharp
public static Line Clamp(Line value, Line min, Line max) { }
```
Limita o valor especificado.<br />
**value**: O valor para limitar.<br />
**min**: O valor mínimo.<br />
**max**: O valor máximo.<br />
## Rotate
```csharp
public static Line Rotate(Line line, Claw.Vector2 origin, float rotation) { }
```
Retorna uma [Line](api/Claw/Line.md#Line) rotacionada.<br />
**rotation**: Graus.<br />
## BoxGenerator
```csharp
public static Line[] BoxGenerator(Claw.Rectangle rectangle) { }
```
Gera um quadrado.<br />
## CircleGenerator
```csharp
public static Line[] CircleGenerator(float radius, Claw.Vector2 center, int segments) { }
```
Gera um círculo.<br />
## ToString
```csharp
public virtual string ToString() { }
```
Retorna uma string representando esta linha no formato:
            {Start:[ [Line.Start](api/Claw/Line.md#Start) ] End:[ [Line.End](api/Claw/Line.md#End) ]}<br />
## Equals
```csharp
public virtual bool Equals(object obj) { }
```
## GetHashCode
```csharp
public virtual int GetHashCode() { }
```
## implicit operator
```csharp
public static Line implicit operator(Claw.Rectangle value) { }
```
## operator +
```csharp
public static Line operator +(Line a, Line b) { }
```
## operator +
```csharp
public static Line operator +(Line a, Claw.Vector2 b) { }
```
## operator +
```csharp
public static Line operator +(Line a, float b) { }
```
## operator -
```csharp
public static Line operator -(Line a) { }
```
## operator -
```csharp
public static Line operator -(Line a, Line b) { }
```
## operator -
```csharp
public static Line operator -(Line a, Claw.Vector2 b) { }
```
## operator -
```csharp
public static Line operator -(Line a, float b) { }
```
## operator *
```csharp
public static Line operator *(Line a, Line b) { }
```
## operator *
```csharp
public static Line operator *(Line a, Claw.Vector2 b) { }
```
## operator *
```csharp
public static Line operator *(Line a, float b) { }
```
## operator /
```csharp
public static Line operator /(Line a, Line b) { }
```
## operator /
```csharp
public static Line operator /(Line a, Claw.Vector2 b) { }
```
## operator /
```csharp
public static Line operator /(Line a, float b) { }
```
## operator ==
```csharp
public static bool operator ==(Line a, Line b) { }
```
## operator !=
```csharp
public static bool operator !=(Line a, Line b) { }
```
