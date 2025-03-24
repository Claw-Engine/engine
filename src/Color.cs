using System.Globalization;

namespace Claw;

/// <summary>
/// Descreve uma cor.
/// </summary>
public struct Color
{
	/// <summary>
	/// Tipos de formato Hex.
	/// </summary>
	public enum HexFormat { RGBA, BGRA, ARGB, ABGR }

	#region Banco de cores
	/// <summary>
	/// R: 0; G: 0; B: 0; A: 0.
	/// </summary>
	public static Color Transparent { get; private set; }
	/// <summary>
	/// R: 255; G: 0; B: 0; A: 255.
	/// </summary>
	public static Color Red { get; private set; }
	/// <summary>
	/// R: 0; G: 255; B: 0; A: 255.
	/// </summary>
	public static Color Green { get; private set; }
	/// <summary>
	/// R: 0; G: 0; B: 255; A: 255.
	/// </summary>
	public static Color Blue { get; private set; }
	/// <summary>
	/// R: 255; G: 255; B: 0; A: 255.
	/// </summary>
	public static Color Yellow { get; private set; }
	/// <summary>
	/// R: 255; G: 127; B: 0; A: 255.
	/// </summary>
	public static Color Orange { get; private set; }
	/// <summary>
	/// R: 0; G: 255; B: 255; A: 255.
	/// </summary>
	public static Color Cyan { get; private set; }
	/// <summary>
	/// R: 127; G: 0; B: 255; A: 255.
	/// </summary>
	public static Color Purple { get; private set; }
	/// <summary>
	/// R: 216; G: 0; B: 72; A: 255.
	/// </summary>
	public static Color Magenta { get; private set; }
	/// <summary>
	/// R: 0; G: 0; B: 0; A: 255.
	/// </summary>
	public static Color Black { get; private set; }
	/// <summary>
	/// R: 255; G: 255; B: 255; A: 255.
	/// </summary>
	public static Color White { get; private set; }
	/// <summary>
	/// R: 127; G: 127; B: 127; A: 255.
	/// </summary>
	public static Color Gray { get; private set; }
	/// <summary>
	/// R: 100; G: 149; B: 237; A: 255.
	/// </summary>
	public static Color CornflowerBlue { get; private set; }

	static Color()
	{
		Transparent = new Color(0x00000000);
		Red = new Color(0xff0000ff);
		Green = new Color(0xff00ff00);
		Blue = new Color(0xffff0000);
		Yellow = new Color(0xff00ffff);
		Orange = new Color(0xff007fff);
		Cyan = new Color(0xffffff00);
		Purple = new Color(0xffff007f);
		Magenta = new Color(0xff4800d8);
		Black = new Color(0xff000000);
		White = new Color(0xffffffff);
		Gray = new Color(0xff7f7f7f);
		CornflowerBlue = new Color(0xffed9564);
	}
	#endregion

	public byte R
	{
		get
		{
			unchecked
			{
				return (byte)_packedValue;
			}
		}
		set => _packedValue = (_packedValue & 0xffffff00) | value;
	}
	public byte G
	{
		get
		{
			unchecked
			{
				return (byte)(_packedValue >> 8);
			}
		}
		set => _packedValue = (_packedValue & 0xffff00ff) | ((uint)value << 8);
	}
	public byte B
	{
		get
		{
			unchecked
			{
				return (byte)(_packedValue >> 16);
			}
		}
		set => _packedValue = (_packedValue & 0xff00ffff) | ((uint)value << 16);
	}
	public byte A
	{
		get
		{
			unchecked
			{
				return (byte)(_packedValue >> 24);
			}
		}
		set => _packedValue = (_packedValue & 0x00ffffff) | ((uint)value << 24);
	}
	/// <summary>
	/// ABGR - 32 bits.
	/// </summary>
	public uint PackedValue => _packedValue;

	/// <summary>
	/// ABGR - 32 bits.
	/// </summary>
	private uint _packedValue;
	
	/// <param name="packedValue">ABGR - 32 bits.</param>
	public Color(uint packedValue) => this._packedValue = packedValue;
	public Color(string hex, HexFormat format = HexFormat.RGBA)
	{
		if (hex.IndexOf('#') != -1) hex = hex.Replace("#", "");

		int r = 0, g = 0, b = 0, a = 255;

		switch (format)
		{
			case HexFormat.RGBA:
				r = (byte)int.Parse(hex.Substring(0, 2), NumberStyles.AllowHexSpecifier);
				g = (byte)int.Parse(hex.Substring(2, 2), NumberStyles.AllowHexSpecifier);
				b = (byte)int.Parse(hex.Substring(4, 2), NumberStyles.AllowHexSpecifier);

				if (hex.Length > 6) a = (byte)int.Parse(hex.Substring(6, 2), NumberStyles.AllowHexSpecifier);
				break;
			case HexFormat.BGRA:
				b = (byte)int.Parse(hex.Substring(0, 2), NumberStyles.AllowHexSpecifier);
				g = (byte)int.Parse(hex.Substring(2, 2), NumberStyles.AllowHexSpecifier);
				r = (byte)int.Parse(hex.Substring(4, 2), NumberStyles.AllowHexSpecifier);

				if (hex.Length > 6) a = (byte)int.Parse(hex.Substring(6, 2), NumberStyles.AllowHexSpecifier);
				break;
			case HexFormat.ARGB:
				if (hex.Length <= 6) goto case HexFormat.RGBA;
				else
				{
					a = (byte)int.Parse(hex.Substring(0, 2), NumberStyles.AllowHexSpecifier);
					r = (byte)int.Parse(hex.Substring(2, 2), NumberStyles.AllowHexSpecifier);
					g = (byte)int.Parse(hex.Substring(4, 2), NumberStyles.AllowHexSpecifier);
					b = (byte)int.Parse(hex.Substring(6, 2), NumberStyles.AllowHexSpecifier);
				}
				break;
			case HexFormat.ABGR:
				if (hex.Length <= 6) goto case HexFormat.BGRA;
				else
				{
					a = (byte)int.Parse(hex.Substring(0, 2), NumberStyles.AllowHexSpecifier);
					r = (byte)int.Parse(hex.Substring(2, 2), NumberStyles.AllowHexSpecifier);
					g = (byte)int.Parse(hex.Substring(4, 2), NumberStyles.AllowHexSpecifier);
					b = (byte)int.Parse(hex.Substring(6, 2), NumberStyles.AllowHexSpecifier);
				}
				break;
		}

		_packedValue = ((uint)a << 24) | ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
	}
	/// <param name="r">De 0 a 255.</param>
	/// <param name="g">De 0 a 255.</param>
	/// <param name="b">De 0 a 255.</param>
	/// <param name="alpha">De 0 a 255.</param>
	public Color(int r, int g, int b, int alpha = 255)
	{
		if (((r | g | b | alpha) & 0xffffff00) != 0)
		{
			uint clampedR = (uint)Mathf.Clamp(r, Byte.MinValue, Byte.MaxValue),
				clampedG = (uint)Mathf.Clamp(g, Byte.MinValue, Byte.MaxValue),
				clampedB = (uint)Mathf.Clamp(b, Byte.MinValue, Byte.MaxValue),
				clampedA = (uint)Mathf.Clamp(alpha, Byte.MinValue, Byte.MaxValue);
			_packedValue = (clampedA << 24) | (clampedB << 16) | (clampedG << 8) | (clampedR);
		}
		else _packedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
	}
	/// <param name="r">De 0 a 1.</param>
	/// <param name="g">De 0 a 1.</param>
	/// <param name="b">De 0 a 1.</param>
	/// <param name="alpha">De 0 a 1.</param>
	public Color(float r, float g, float b, float alpha = 1) : this((int)(r * 255), (int)(g * 255), (int)(b * 255), (int)(alpha * 255)) { }
	public Color(byte r, byte g, byte b, byte alpha) => _packedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | (r);

	/// <summary>
	/// Retorna uma string representando esta cor no formato:
	/// {R:[<see cref="R"/>] G:[<see cref="G"/>] B:[<see cref="B"/>] A:[<see cref="A"/>]}
	/// </summary>
	public override string ToString() => '{' + string.Format("R:{0} G:{1} B:{2} A:{3}", R, G, B, A) + '}';

	/// <summary>
	/// Cria uma <see cref="Color"/> com HSV.
	/// </summary>
	/// <param name="hue">0 - 359.</param>
	/// <param name="saturation">0 - 1.</param>
	/// <param name="value">0 - 1.</param>
	/// <param name="alpha">0 - 1.</param>
	public static Color FromHSV(float hue, float saturation, float value, float alpha)
	{
		float r = 0, g = 0, b = 0;

		int hi = (int)Math.Floor(hue / 60.0) % 6;
		float f = (hue / 60) - (float)Math.Floor(hue / 60);

		float p, q, t;
		p = value * (1 - saturation);
		q = value * (1 - (f * saturation));
		t = value * (1 - ((1 - f) * saturation));

		switch (hi)
		{
			case 0:
				r = value;
				g = t;
				b = p;
				break;
			case 1:
				r = q;
				g = value;
				b = p;
				break;
			case 2:
				r = p;
				g = value;
				b = t;
				break;
			case 3:
				r = p;
				g = q;
				b = value;
				break;
			case 4:
				r = t;
				g = p;
				b = value;
				break;
			default:
				r = value;
				g = p;
				b = q;
				break;
		}

		return new Color((int)r * 255, (int)g * 255, (int)b * 255, (int)alpha * 255);
	}
	/// <summary>
	/// Desconstrói este <see cref="Color"/> como HSV.
	/// </summary>
	public void ToHSV(out float hue, out float saturation, out float value, out float alpha)
	{
		double delta, min = Math.Min(Math.Min(R, G), B);
		double h = 0, s = 0, v = Math.Max(Math.Max(R, G), B);
		delta = v - min;


		if (v == 0.0) s = 0;
		else s = delta / v;

		if (s == 0) h = 0.0;
		else
		{
			if (R == v) h = (G - B) / delta;
			else if (G == v) h = 2 + (B - R) / delta;
			else if (B == v) h = 4 + (R - G) / delta;

			h *= 60;

			if (h < 0.0) h = h + 360;
		}

		hue = (float)h;
		saturation = (float)s;
		value = (float)v / 255;
		alpha = A / 255;
	}

	/// <summary>
	/// Transforma essa <see cref="Color"/> para Hex.
	/// </summary>
	public string ToHex(HexFormat format = HexFormat.ARGB)
	{
		switch (format)
		{
			case HexFormat.RGBA: return ToHex(R, G, B, A);
			case HexFormat.ARGB: return ToHex(A, R, G, B);
			case HexFormat.BGRA: return ToHex(B, G, R, A);
			case HexFormat.ABGR: return ToHex(A, B, G, R);
		}

		return ToHex(R, G, B, A);
	}
	private static string ToHex(byte byte1, byte byte2, byte byte3, byte byte4) => string.Format("#{0}{1}{2}{3}", byte1.ToString("X2") + byte2.ToString("X2") + byte3.ToString("X2") + byte4.ToString("X2"));

	/// <summary>
	/// Realiza a interpolação linear entre duas cores.
	/// </summary>
	/// <param name="a">Valor atual.</param>
	/// <param name="b">Valor alvo.</param>
	/// <param name="amount">Valor de ponderação (entre 0 e 1).</param>
	public static Color Lerp(Color a, Color b, float amount)
	{
		amount = Mathf.Clamp(0, 1, amount);

		return new Color((int)Mathf.Lerp(a.R, b.R, amount), (int)Mathf.Lerp(a.G, b.G, amount), (int)Mathf.Lerp(a.B, b.B, amount), (int)Mathf.Lerp(a.A, b.A, amount));
	}
	/// <summary>
	/// Realiza a interpolação linear entre duas cores, usando delta time.
	/// </summary>
	/// <param name="a">Valor atual.</param>
	/// <param name="b">Valor alvo.</param>
	/// <param name="amount">Valor de ponderação.</param>
	/// <param name="scaled">Se o delta time será <see cref="Time.DeltaTime"/> (true) ou <see cref="Time.UnscaledDeltaTime"/> (false).</param>
	public static Color DeltaLerp(Color a, Color b, float amount, bool scaled = true) => new Color((int)Mathf.DeltaLerp(a.R, b.R, amount, scaled), (int)Mathf.DeltaLerp(a.G, b.G, amount, scaled), (int)Mathf.DeltaLerp(a.B, b.B, amount, scaled), (int)Mathf.DeltaLerp(a.A, b.A, amount, scaled));

	public override bool Equals(object obj)
	{
		if (!(obj is Color)) return false;

		return this == (Color)obj;
	}
	public override int GetHashCode()
	{
		var hashCode = 1075529825;
		hashCode = hashCode * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(_packedValue);

		return hashCode;
	}

	public static implicit operator uint(Color value) => value.PackedValue;
	public static implicit operator Color(uint packedValue) => new Color(packedValue);
	public static Color operator *(Color value, float scale) => new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
	public static Color operator *(float scale, Color value) => new Color((int)(value.R * scale), (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));
	public static Color operator *(Color color, Color blendColor) => new Color((byte)(color.R * blendColor.R / 255), (byte)(color.G * blendColor.G / 255), (byte)(color.B * blendColor.B / 255), (byte)(color.A * blendColor.A / 255));
	public static Color operator +(Color a, Color b) => new Color(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);
	public static bool operator ==(Color a, Color b) => a._packedValue == b._packedValue;
	public static bool operator !=(Color a, Color b) => a._packedValue != b._packedValue;
}
