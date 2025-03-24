# Color
```csharp
public struct Color
```
Descreve uma cor.<br />
## Color
```csharp
public Color(uint packedValue) { }
```
**packedValue**: ABGR - 32 bits.<br />
## Color
```csharp
public Color(string hex, HexFormat format) { }
```
## Color
```csharp
public Color(int r, int g, int b, int alpha) { }
```
**r**: De 0 a 255.<br />
**g**: De 0 a 255.<br />
**b**: De 0 a 255.<br />
**alpha**: De 0 a 255.<br />
## Color
```csharp
public Color(float r, float g, float b, float alpha) { }
```
**r**: De 0 a 1.<br />
**g**: De 0 a 1.<br />
**b**: De 0 a 1.<br />
**alpha**: De 0 a 1.<br />
## Color
```csharp
public Color(byte r, byte g, byte b, byte alpha) { }
```
## Transparent
```csharp
public static Color Transparent { get; private set; } 
```
R: 0; G: 0; B: 0; A: 0.<br />
## Red
```csharp
public static Color Red { get; private set; } 
```
R: 255; G: 0; B: 0; A: 255.<br />
## Green
```csharp
public static Color Green { get; private set; } 
```
R: 0; G: 255; B: 0; A: 255.<br />
## Blue
```csharp
public static Color Blue { get; private set; } 
```
R: 0; G: 0; B: 255; A: 255.<br />
## Yellow
```csharp
public static Color Yellow { get; private set; } 
```
R: 255; G: 255; B: 0; A: 255.<br />
## Orange
```csharp
public static Color Orange { get; private set; } 
```
R: 255; G: 127; B: 0; A: 255.<br />
## Cyan
```csharp
public static Color Cyan { get; private set; } 
```
R: 0; G: 255; B: 255; A: 255.<br />
## Purple
```csharp
public static Color Purple { get; private set; } 
```
R: 127; G: 0; B: 255; A: 255.<br />
## Magenta
```csharp
public static Color Magenta { get; private set; } 
```
R: 216; G: 0; B: 72; A: 255.<br />
## Black
```csharp
public static Color Black { get; private set; } 
```
R: 0; G: 0; B: 0; A: 255.<br />
## White
```csharp
public static Color White { get; private set; } 
```
R: 255; G: 255; B: 255; A: 255.<br />
## Gray
```csharp
public static Color Gray { get; private set; } 
```
R: 127; G: 127; B: 127; A: 255.<br />
## CornflowerBlue
```csharp
public static Color CornflowerBlue { get; private set; } 
```
R: 100; G: 149; B: 237; A: 255.<br />
## R
```csharp
public byte R { get; set; } 
```
## G
```csharp
public byte G { get; set; } 
```
## B
```csharp
public byte B { get; set; } 
```
## A
```csharp
public byte A { get; set; } 
```
## PackedValue
```csharp
public uint PackedValue { get; } 
```
ABGR - 32 bits.<br />
## ToString
```csharp
public virtual string ToString() { }
```
Retorna uma string representando esta cor no formato:
            {R:[ [Color.R](/api/Claw/Color.md#R) ] G:[ [Color.G](/api/Claw/Color.md#G) ] B:[ [Color.B](/api/Claw/Color.md#B) ] A:[ [Color.A](/api/Claw/Color.md#A) ]}<br />
## FromHSV
```csharp
public static Color FromHSV(float hue, float saturation, float value, float alpha) { }
```
Cria uma [Color](/api/Claw/Color.md#Color) com HSV.<br />
**hue**: 0 - 359.<br />
**saturation**: 0 - 1.<br />
**value**: 0 - 1.<br />
**alpha**: 0 - 1.<br />
## ToHSV
```csharp
public void ToHSV(out float hue, out float saturation, out float value, out float alpha) { }
```
Desconstrói este [Color](/api/Claw/Color.md#Color) como HSV.<br />
## ToHex
```csharp
public string ToHex(HexFormat format) { }
```
## Lerp
```csharp
public static Color Lerp(Color a, Color b, float amount) { }
```
Realiza a interpolação linear entre duas cores.<br />
**a**: Valor atual.<br />
**b**: Valor alvo.<br />
**amount**: Valor de ponderação (entre 0 e 1).<br />
## DeltaLerp
```csharp
public static Color DeltaLerp(Color a, Color b, float amount, bool scaled) { }
```
Realiza a interpolação linear entre duas cores, usando delta time.<br />
**a**: Valor atual.<br />
**b**: Valor alvo.<br />
**amount**: Valor de ponderação.<br />
**scaled**: Se o delta time será [Time.DeltaTime](/api/Claw/Time.md#DeltaTime) (true) ou [Time.UnscaledDeltaTime](/api/Claw/Time.md#UnscaledDeltaTime) (false).<br />
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
public static uint implicit operator(Color value) { }
```
## implicit operator
```csharp
public static Color implicit operator(uint packedValue) { }
```
## operator *
```csharp
public static Color operator *(Color value, float scale) { }
```
## operator *
```csharp
public static Color operator *(float scale, Color value) { }
```
## operator *
```csharp
public static Color operator *(Color color, Color blendColor) { }
```
## operator +
```csharp
public static Color operator +(Color a, Color b) { }
```
## operator ==
```csharp
public static bool operator ==(Color a, Color b) { }
```
## operator !=
```csharp
public static bool operator !=(Color a, Color b) { }
```
# HexFormat
```csharp
public enum HexFormat
```
Tipos de formato Hex.<br />
|Nome|Valor|Descrição|
|---|---|---|
|RGBA|0||
|BGRA|1||
|ARGB|2||
|ABGR|3||
