# Rectangle
```csharp
public struct Rectangle
```
Descreve um retângulo.<br />
## Rectangle
```csharp
public Rectangle(float x, float y, float width, float height) { }
```
## Rectangle
```csharp
public Rectangle(Claw.Vector2 location, Claw.Vector2 size) { }
```
## X
```csharp
public float X;
```
## Y
```csharp
public float Y;
```
## Width
```csharp
public float Width;
```
## Height
```csharp
public float Height;
```
## Location
```csharp
public Claw.Vector2 Location { get; set; } 
```
## Size
```csharp
public Claw.Vector2 Size { get; set; } 
```
## IsEmpty
```csharp
public bool IsEmpty { get; } 
```
## End
```csharp
public Claw.Vector2 End { get; } 
```
## Left
```csharp
public float Left { get; } 
```
## Right
```csharp
public float Right { get; } 
```
## Top
```csharp
public float Top { get; } 
```
## Bottom
```csharp
public float Bottom { get; } 
```
## Center
```csharp
public Claw.Vector2 Center { get; } 
```
## Lerp
```csharp
public static Rectangle Lerp(Rectangle value1, Rectangle value2, float amount) { }
```
Cria um novo retângulo de uma interpolação linear com os retângulos especificados.<br />
**value1**: Valor atual.<br />
**value2**: Valor de destino.<br />
**amount**: Valor de ponderação (entre 0 e 1).<br />
## DeltaLerp
```csharp
public static Rectangle DeltaLerp(Rectangle value1, Rectangle value2, float amount, bool scaled) { }
```
Cria um novo retângulo de uma interpolação linear com os retângulos especificados, usando delta time.<br />
**value1**: Valor atual.<br />
**value2**: Valor alvo.<br />
**amount**: Valor de ponderação.<br />
**scaled**: Se o delta time será [Time.DeltaTime](/api/Claw/Time.md#DeltaTime) (true) ou [Time.UnscaledDeltaTime](/api/Claw/Time.md#UnscaledDeltaTime) (false).<br />
## Clamp
```csharp
public static Rectangle Clamp(Rectangle value, Rectangle min, Rectangle max) { }
```
Limita o valor especificado.<br />
**value**: O valor para limitar.<br />
**min**: O valor mínimo.<br />
**max**: O valor máximo.<br />
## Positive
```csharp
public static Rectangle Positive(Rectangle rectangle) { }
```
Garante que um [Rectangle](/api/Claw/Rectangle.md#Rectangle) não tenha dimensões negativas, de forma que continue ocupando o mesmo espaço.<br />
## Contains
```csharp
public bool Contains(Claw.Vector2 point) { }
```
Checa se um ponto está dentro do retângulo.<br />
## Contains
```csharp
public bool Contains(float x, float y) { }
```
Checa se um ponto está dentro do retângulo.<br />
## Inflate
```csharp
public void Inflate(float horizontalAmount, float verticalAmount) { }
```
Ajusta as bordas deste retângulo pelos valores horizontais e verticais especificados.<br />
## Intersects
```csharp
public bool Intersects(Rectangle value) { }
```
Checa se tem um retângulo colidindo com este.<br />
## Offset
```csharp
public void Offset(Claw.Vector2 amount) { }
```
Muda a posição deste retângulo.<br />
## ToString
```csharp
public virtual string ToString() { }
```
Retorna uma string representando este retângulo no formato:
            {Location:[ [Rectangle.Location](/api/Claw/Rectangle.md#Location) ] Size:[ [Rectangle.Size](/api/Claw/Rectangle.md#Size) ]}<br />
## Union
```csharp
public static Rectangle Union(Rectangle value1, Rectangle value2) { }
```
Cria um novo [Rectangle](/api/Claw/Rectangle.md#Rectangle) que contem completamente outros dois retângulos.<br />
**Retorna**: A união de dois retângulos.<br />
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
public static Rectangle implicit operator(Claw.Line value) { }
```
## operator +
```csharp
public static Rectangle operator +(Rectangle a, Rectangle b) { }
```
## operator +
```csharp
public static Rectangle operator +(Rectangle a, Claw.Vector2 b) { }
```
## operator +
```csharp
public static Rectangle operator +(Rectangle a, float b) { }
```
## operator -
```csharp
public static Rectangle operator -(Rectangle a) { }
```
## operator -
```csharp
public static Rectangle operator -(Rectangle a, Rectangle b) { }
```
## operator -
```csharp
public static Rectangle operator -(Rectangle a, Claw.Vector2 b) { }
```
## operator -
```csharp
public static Rectangle operator -(Rectangle a, float b) { }
```
## operator *
```csharp
public static Rectangle operator *(Rectangle a, Rectangle b) { }
```
## operator *
```csharp
public static Rectangle operator *(Rectangle a, Claw.Vector2 b) { }
```
## operator *
```csharp
public static Rectangle operator *(Rectangle a, float b) { }
```
## operator /
```csharp
public static Rectangle operator /(Rectangle a, Rectangle b) { }
```
## operator /
```csharp
public static Rectangle operator /(Rectangle a, Claw.Vector2 b) { }
```
## operator /
```csharp
public static Rectangle operator /(Rectangle a, float b) { }
```
## operator ==
```csharp
public static bool operator ==(Rectangle a, Rectangle b) { }
```
## operator !=
```csharp
public static bool operator !=(Rectangle a, Rectangle b) { }
```
