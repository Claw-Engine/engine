using static Claw.SDL;

namespace Claw;

/// <summary>
/// Descreve um retângulo.
/// </summary>
public struct Rectangle
{
	public float X, Y, Width, Height;
	public Vector2 Location
	{
		get => new Vector2(X, Y);
		set
		{
			X = value.X;
			Y = value.Y;
		}
	}
	public Vector2 Size
	{
		get => new Vector2(Width, Height);
		set
		{
			Width = value.X;
			Height = value.Y;
		}
	}

	public bool IsEmpty => ((((this.Width == 0) && (this.Height == 0)) && (this.X == 0)) && (this.Y == 0));

	public Vector2 End => new Vector2(X + Width, Y + Height);
	public float Left => X;
	public float Right => X + Width;
	public float Top => Y;
	public float Bottom => Y + Height;
	public Vector2 Center => new Vector2(X + Width / 2, Y + Height / 2);

	public Rectangle(float x, float y, float width, float height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}
	public Rectangle(Vector2 location, Vector2 size) : this(location.X, location.Y, size.X, size.Y) { }

	/// <summary>
	/// Cria um novo retângulo de uma interpolação linear com os retângulos especificados.
	/// </summary>
	/// <param name="value1">Valor atual.</param>
	/// <param name="value2">Valor de destino.</param>
	/// <param name="amount">Valor de ponderação (entre 0 e 1).</param>
	public static Rectangle Lerp(Rectangle value1, Rectangle value2, float amount)
	{
		amount = Mathf.Clamp(0, 1, amount);

		return new Rectangle(Vector2.Lerp(value1.Location, value2.Location, amount), Vector2.Lerp(value1.Size, value2.Size, amount));
	}
	/// <summary>
	/// Cria um novo retângulo de uma interpolação linear com os retângulos especificados, usando delta time.
	/// </summary>
	/// <param name="a">Valor atual.</param>
	/// <param name="b">Valor alvo.</param>
	/// <param name="amount">Valor de ponderação.</param>
	/// <param name="scaled">Se o delta time será <see cref="Time.DeltaTime"/> (true) ou <see cref="Time.UnscaledDeltaTime"/> (false).</param>
	public static Rectangle DeltaLerp(Rectangle value1, Rectangle value2, float amount, bool scaled = true) => new Rectangle(Vector2.DeltaLerp(value1.Location, value2.Location, amount, scaled), Vector2.DeltaLerp(value1.Size, value2.Size, amount, scaled));

	/// <summary>
	/// Limita o valor especificado.
	/// </summary>
	/// <param name="value">O valor para limitar.</param>
	/// <param name="min">O valor mínimo.</param>
	/// <param name="max">O valor máximo.</param>
	public static Rectangle Clamp(Rectangle value, Rectangle min, Rectangle max) => new Rectangle(Vector2.Clamp(value.Location, min.Location, max.Location), Vector2.Clamp(value.Size, min.Size, max.Size));

	/// <summary>
	/// Garante que um <see cref="Rectangle"/> não tenha dimensões negativas, de forma que continue ocupando o mesmo espaço.
	/// </summary>
	public static Rectangle Positive(Rectangle rectangle)
	{
		if (rectangle.Width < 0)
		{
			rectangle.X += rectangle.Width;
			rectangle.Width = Math.Abs(rectangle.Width);
		}

		if (rectangle.Height < 0)
		{
			rectangle.Y += rectangle.Height;
			rectangle.Height = Math.Abs(rectangle.Height);
		}

		return rectangle;
	}

	/// <summary>
	/// Checa se um ponto está dentro do retângulo.
	/// </summary>
	public bool Contains(Vector2 point) => X <= point.X && point.X < X + Width && Y <= point.Y && point.Y < Y + Height;
	/// <summary>
	/// Checa se um ponto está dentro do retângulo.
	/// </summary>
	public bool Contains(float x, float y) => Contains(new Vector2(x, y));
	
	/// <summary>
	/// Ajusta as bordas deste retângulo pelos valores horizontais e verticais especificados.
	/// </summary>
	public void Inflate(float horizontalAmount, float verticalAmount)
	{
		X -= horizontalAmount;
		Y -= verticalAmount;
		Width += horizontalAmount * 2;
		Height += verticalAmount * 2;
	}

	/// <summary>
	/// Checa se tem um retângulo colidindo com este.
	/// </summary>
	public bool Intersects(Rectangle value)
	{
		return value.Left < Right &&
			   Left < value.Right &&
			   value.Top < Bottom &&
			   Top < value.Bottom;
	}

	/// <summary>
	/// Muda a posição deste retângulo.
	/// </summary>
	public void Offset(Vector2 amount)
	{
		X += amount.X;
		Y += amount.Y;
	}

	/// <summary>
	/// Retorna uma string representando este retângulo no formato:
	/// {Location:[<see cref="Location"/>] Size:[<see cref="Size"/>]}
	/// </summary>
	public override string ToString() => '{' + string.Format("Location:{0} Size:{1}", Location, Size) + '}';

	/// <summary>
	/// Cria um novo <see cref="Rectangle"/> que contem completamente outros dois retângulos.
	/// </summary>
	/// <returns>A união de dois retângulos.</returns>
	public static Rectangle Union(Rectangle value1, Rectangle value2)
	{
		float x = Math.Min(value1.X, value2.X),
			y = Math.Min(value1.Y, value2.Y);

		return new Rectangle(x, y, Math.Max(value1.Right, value2.Right) - x, Math.Max(value1.Bottom, value2.Bottom) - y);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Rectangle)) return false;

		return this == (Rectangle)obj;
	}
	public override int GetHashCode()
	{
		var hashCode = 1075529825;
		hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(Location);
		hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(Size);

		return hashCode;
	}

	internal SDL_Rect ToSDL() => new SDL_Rect() { x = (int)X, y = (int)Y, w = (int)Width, h = (int)Height };
	internal SDL_FRect ToSDLF() => new SDL_FRect() { x = X, y = Y, w = Width, h = Height };
	
	public static implicit operator Rectangle(Line value) => new Rectangle(value.Start, value.End - value.Start);
	public static Rectangle operator +(Rectangle a, Rectangle b) => new Rectangle(a.Location + b.Location, a.Size + b.Size);
	public static Rectangle operator +(Rectangle a, Vector2 b) => new Rectangle(a.Location + b, a.Size + b);
	public static Rectangle operator +(Rectangle a, float b) => new Rectangle(new Vector2(a.Location.X + b, a.Location.Y + b), new Vector2(a.Size.X + b, a.Size.Y + b));
	public static Rectangle operator -(Rectangle a) => new Rectangle(-a.Location, -a.Size);
	public static Rectangle operator -(Rectangle a, Rectangle b) => new Rectangle(a.Location - b.Location, a.Size - b.Size);
	public static Rectangle operator -(Rectangle a, Vector2 b) => new Rectangle(a.Location - b, a.Size - b);
	public static Rectangle operator -(Rectangle a, float b) => new Rectangle(new Vector2(a.Location.X - b, a.Location.Y - b), new Vector2(a.Size.X - b, a.Size.Y - b));
	public static Rectangle operator *(Rectangle a, Rectangle b) => new Rectangle(a.Location * b.Location, a.Size * b.Size);
	public static Rectangle operator *(Rectangle a, Vector2 b) => new Rectangle(a.Location * b, a.Size * b);
	public static Rectangle operator *(Rectangle a, float b) => new Rectangle(a.Location * b, a.Size * b);
	public static Rectangle operator /(Rectangle a, Rectangle b) => new Rectangle(a.Location / b.Location, a.Size / b.Size);
	public static Rectangle operator /(Rectangle a, Vector2 b) => new Rectangle(a.Location / b, a.Size / b);
	public static Rectangle operator /(Rectangle a, float b) => new Rectangle(a.Location / b, a.Size / b);
	public static bool operator ==(Rectangle a, Rectangle b) => a.Location == b.Location && a.Size == b.Size;
	public static bool operator !=(Rectangle a, Rectangle b) => a.Location != b.Location || a.Size != b.Size;
}
