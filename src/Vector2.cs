using static Claw.SDL;

namespace Claw;

/// <summary>
/// Descreve um vetor 2D.
/// </summary>
public struct Vector2
{
	/// <summary>
	/// X: 0; Y: 0.
	/// </summary>
	public static readonly Vector2 Zero = new Vector2(0);
	/// <summary>
	/// X: 1; Y: 1.
	/// </summary>
	public static readonly Vector2 One = new Vector2(1);
	/// <summary>
	/// X: 1; Y: 0.
	/// </summary>
	public static readonly Vector2 UnitX = new Vector2(1, 0);
	/// <summary>
	/// X: 0; Y: 1.
	/// </summary>
	public static readonly Vector2 UnitY = new Vector2(0, 1);

	public float X, Y;

	public Vector2(float x, float y)
	{
		X = x;
		Y = y;
	}
	public Vector2(float value)
	{
		X = value;
		Y = value;
	}

	/// <summary>
	/// Troca o X pelo Y e vice-versa.
	/// </summary>
	public void Invert()
	{
		float x = X;
		X = Y;
		Y = x;
	}

	/// <summary>
	/// Transforma este <see cref="Vector2"/> em um vetor de magnitude 1 com a mesma direção.
	/// </summary>
	public void Normalize()
	{
		float value = 1.0f / (float)Math.Sqrt((X * X) + (Y * Y));
		X *= value;
		Y *= value;
	}

	/// <summary>
	/// Retorna um vetor com o X e Y absolutos.
	/// </summary>
	public static Vector2 Abs(Vector2 value) => new Vector2(Math.Abs(value.X), Math.Abs(value.Y));
	
	/// <summary>
	/// Retorna um vetor que respeite os limites mínimo e máximo.
	/// </summary>
	public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max) => new Vector2(Mathf.Clamp(value.X, min.X, max.X), Mathf.Clamp(value.Y, min.Y, max.Y));
	
	/// <summary>
	/// Retorna um vetor com o menor valor de X e o menor valor de Y.
	/// </summary>
	public static Vector2 Min(Vector2 a, Vector2 b) => new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
	
	/// <summary>
	/// Retorna um vetor com o maior valor de X e o maior valor de Y.
	/// </summary>
	public static Vector2 Max(Vector2 a, Vector2 b) => new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

	/// <summary>
	/// Retorna o ângulo entre duas posições.
	/// </summary>
	/// <returns>Graus.</returns>
	public static float GetAngle(Vector2 position, Vector2 direction)
	{
		double rotation = 0;
		Vector2 angle = direction - position;

		if (angle != Vector2.Zero)
		{
			angle.Normalize();

			rotation = Math.Atan2(angle.Y, angle.X);
		}

		return Mathf.ToDegrees((float)rotation);
	}

	/// <summary>
	/// Retorna a distância entre dois vetores.
	/// </summary>
	public static float Distance(Vector2 a, Vector2 b)
	{
		float v1 = a.X - b.X, v2 = a.Y - b.Y;

		return (float)Math.Sqrt(v1 * v1 + v2 * v2);
	}

	/// <summary>
	/// Retorna a distância entre um vetor e o ponto 0,0.
	/// </summary>
	public static float Length(Vector2 value) => (float)Math.Sqrt(value.X * value.X + value.Y * value.Y);

	/// <summary>
	/// Retorna o produto escalar entre dois vetores.
	/// </summary>
	public static float Dot(Vector2 a, Vector2 b) => a.X * b.X + a.Y * b.Y;

	/// <summary>
	/// Retorna o produto cruzado entre dois vetores.
	/// </summary>
	public static float Cross(Vector2 a, Vector2 b) => a.X * b.Y - a.Y * b.X;
	/// <summary>
	/// Retorna o produto cruzado entre um vetor e um escalar.
	/// </summary>
	public static Vector2 Cross(Vector2 a, float s) => new Vector2(s * a.Y, -s * a.X);

	/// <summary>
	/// Transforma um <see cref="Vector2"/> em um vetor de magnitude 1 com a mesma direção.
	/// </summary>
	public static Vector2 Normalize(Vector2 value)
	{
		value.Normalize();

		return value;
	}
	
	/// <summary>
	/// Retorna um <see cref="Vector2"/> rotacionado.
	/// </summary>
	/// <param name="rotation">Graus.</param>
	public static Vector2 Rotate(Vector2 point, Vector2 origin, float rotation)
	{
		float r = Mathf.ToRadians(rotation);
		Vector2 translated = point - origin;
		Vector2 rotated = new Vector2((float)(translated.X * Math.Cos(r) - translated.Y * Math.Sin(r)),
			(float)(translated.X * Math.Sin(r) + translated.Y * Math.Cos(r)));

		return rotated + origin;
	}

	/// <summary>
	/// Realiza a interpolação linear entre dois vetores.
	/// </summary>
	/// <param name="a">Valor atual.</param>
	/// <param name="b">Valor alvo.</param>
	/// <param name="amount">Valor de ponderação (entre 0 e 1).</param>
	public static Vector2 Lerp(Vector2 a, Vector2 b, float amount)
	{
		amount = Mathf.Clamp(amount, 0, 1);

		return new Vector2(Mathf.Lerp(a.X, b.X, amount), Mathf.Lerp(a.Y, b.Y, amount));
	}
	/// <summary>
	/// Realiza a interpolação linear entre dois vetores, usando delta time.
	/// </summary>
	/// <param name="a">Valor atual.</param>
	/// <param name="b">Valor alvo.</param>
	/// <param name="amount">Valor de ponderação.</param>
	/// <param name="scaled">Se o delta time será <see cref="Time.DeltaTime"/> (true) ou <see cref="Time.UnscaledDeltaTime"/> (false).</param>
	public static Vector2 DeltaLerp(Vector2 a, Vector2 b, float amount, bool scaled = true) => new Vector2(Mathf.DeltaLerp(a.X, b.X, amount, scaled), Mathf.DeltaLerp(a.Y, b.Y, amount, scaled));
	
	/// <summary>
	/// Incrementa um valor por um determinado deslocamento, mas nunca além do valor final.
	/// </summary>
	public static Vector2 Approach(Vector2 value, Vector2 target, float shift) => new Vector2(Mathf.Approach(value.X, target.X, shift), Mathf.Approach(value.Y, target.Y, shift));
	/// <summary>
	/// Incrementa um valor por um determinado deslocamento, mas nunca além do valor final.
	/// </summary>
	public static Vector2 Approach(Vector2 value, Vector2 target, Vector2 shift) => new Vector2(Mathf.Approach(value.X, target.X, shift.X), Mathf.Approach(value.Y, target.Y, shift.Y));

	/// <summary>
	/// Retorna o módulo horizontal do vetor determinado pelo comprimento e direção indicados.
	/// </summary>
	/// <param name="angle">Graus.</param>
	public static Vector2 LengthDir(Vector2 distance, float angle)
	{
		angle = Mathf.ToRadians(angle);

		return new Vector2(distance.X * (float)Math.Cos(angle), distance.Y * (float)Math.Sin(angle));
	}
	/// <summary>
	/// Retorna o módulo horizontal do vetor determinado pelo comprimento e direção indicados.
	/// </summary>
	/// <param name="angle">Graus.</param>
	public static Vector2 LengthDir(float distance, float angle) => LengthDir(new Vector2(distance), angle);

	/// <summary>
	/// Cria um novo <see cref="Vector2"/> que contém a transformação do vetor 2d pelo <see cref="Quaternion"/> especificado, representando a rotação.
	/// </summary>
	public static Vector2 Transform(Vector2 value, Quaternion rotation)
	{
		Vector2 result;
		Vector3 r1 = new Vector3(rotation.X + rotation.X, rotation.Y + rotation.Y, rotation.Z + rotation.Z),
			r2 = new Vector3(rotation.X, rotation.X, rotation.W),
			r3 = new Vector3(1, rotation.Y, rotation.Z),
			r4 = r1 * r2,
			r5 = r1 * r3;

		result.X = (float)((double)value.X * (1.0 - (double)r5.Y - (double)r5.Z) + (double)value.Y * ((double)r4.Y - (double)r4.Z));
		result.Y = (float)((double)value.X * ((double)r4.Y + (double)r4.Z) + (double)value.Y * (1.0 - (double)r4.X - (double)r5.Z));

		return result;
	}

	/// <summary>
	/// Transforma um ângulo em um <see cref="Vector2"/>.
	/// </summary>
	/// <param name="angle">Graus.</param>
	public static Vector2 FindFacing(float angle)
	{
		float r = Mathf.ToRadians(angle) + (float)Math.PI / 2;
		Vector2 v = new Vector2(0, -1);
		Quaternion q = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), r);

		return Vector2.Transform(v, q);
	}

	/// <summary>
	/// Retorna uma string representando este vetor 2D no formato:
	/// {X:[<see cref="X"/>] Y:[<see cref="Y"/>]}
	/// </summary>
	public override string ToString() => '{' + string.Format("X:{0} Y:{1}", X, Y) + '}';

	public override bool Equals(object obj)
	{
		if (!(obj is Vector2)) return false;

		return this == (Vector2)obj;
	}
	public override int GetHashCode()
	{
		var hashCode = 1075529825;
		hashCode = hashCode * -1521134295 + EqualityComparer<float>.Default.GetHashCode(X);
		hashCode = hashCode * -1521134295 + EqualityComparer<float>.Default.GetHashCode(Y);

		return hashCode;
	}

	internal SDL_Point ToSDL() => new SDL_Point() { x = (int)X, y = (int)Y };
	internal SDL_FPoint ToSDLF() => new SDL_FPoint() { x = X, y = Y };
	
	public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);
	public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
	public static Vector2 operator -(Vector2 value)
	{
		value.X = -value.X;
		value.Y = -value.Y;

		return value;
	}
	public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.X * b.X, a.Y * b.Y);
	public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.X * b, a.Y * b);
	public static Vector2 operator *(float a, Vector2 b) => new Vector2(a * b.X, a * b.Y);
	public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.X / b.X, a.Y / b.Y);
	public static Vector2 operator /(Vector2 a, float b) => new Vector2(a.X / b, a.Y / b);
	public static Vector2 operator /(float a, Vector2 b) => new Vector2(a / b.X, a / b.Y);
	public static bool operator ==(Vector2 a, Vector2 b) => a.X == b.X && a.Y == b.Y;
	public static bool operator !=(Vector2 a, Vector2 b) => a.X != b.X || a.Y != b.Y;
}
