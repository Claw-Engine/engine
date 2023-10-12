using System;

namespace Claw
{
    /// <summary>
    /// Descreve um vetor 3D.
    /// </summary>
    public struct Vector3
    {
        /// <summary>
        /// X: 0; Y: 0; Z: 0.
        /// </summary>
        public static Vector3 Zero => new Vector3(0);
        /// <summary>
        /// X: 1; Y: 1; Z: 1.
        /// </summary>
        public static Vector3 One => new Vector3(1);

        public float X, Y, Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3(float value)
        {
            X = value;
            Y = value;
            Z = value;
        }
        public Vector3(Vector2 vector2, float z)
        {
            X = vector2.X;
            Y = vector2.Y;
            Z = z;
        }

        /// <summary>
        /// Transforma este <see cref="Vector3"/> em um vetor de unidade com a mesma direção.
        /// </summary>
        public void Normalize()
        {
            float val = 1.0f / (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
            X *= val;
            Y *= val;
            Z *= val;
        }
        
        /// <summary>
        /// Retorna um vetor que respeite os limites mínimo e máximo.
        /// </summary>
        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max) => new Vector3(Mathf.Clamp(value.X, min.X, max.X), Mathf.Clamp(value.Y, min.Y, max.Y), Mathf.Clamp(value.Z, min.Z, max.Z));

        /// <summary>
        /// Retorna a distância entre dois vetores.
        /// </summary>
        public static float Distance(Vector3 a, Vector3 b)
        {
            float v1 = a.X - b.X, v2 = a.Y - b.Y, v3 = a.Z - b.Z;

            return (float)Math.Sqrt((v1 * v1) + (v2 * v2) + (v3 * v3));
        }

        /// <summary>
        /// Transforma um <see cref="Vector3"/> em um vetor de unidade com a mesma direção.
        /// </summary>
        public static Vector2 Normalize(Vector2 value)
        {
            value.Normalize();

            return value;
        }

        /// <summary>
        /// Realiza a interpolação linear entre dois vetores.
        /// </summary>
        /// <param name="a">Valor atual.</param>
        /// <param name="b">Valor alvo.</param>
        /// <param name="amount">Valor de ponderação (entre 0 e 1).</param>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float amount)
        {
            amount = Mathf.Clamp(amount, 0, 1);

            return new Vector3(Mathf.Lerp(a.X, b.X, amount), Mathf.Lerp(a.Y, b.Y, amount), Mathf.Lerp(a.Z, b.Z, amount));
        }
        /// <summary>
        /// Realiza a interpolação linear entre dois vetores, usando delta time.
        /// </summary>
        /// <param name="a">Valor atual.</param>
        /// <param name="b">Valor alvo.</param>
        /// <param name="amount">Valor de ponderação.</param>
        /// <param name="scaled">Se o delta time será <see cref="Time.DeltaTime"/> (true) ou <see cref="Time.UnscaledDeltaTime"/> (false).</param>
        public static Vector3 DeltaLerp(Vector3 a, Vector3 b, float amount, bool scaled = true) => new Vector3(Mathf.DeltaLerp(a.X, b.X, amount, scaled), Mathf.DeltaLerp(a.Y, b.Y, amount, scaled), Mathf.DeltaLerp(a.Z, b.Z, amount, scaled));
        
        /// <summary>
        /// Incrementa um valor por um determinado deslocamento, mas nunca além do valor final.
        /// </summary>
        public static Vector3 Approach(Vector3 value, Vector3 target, float shift) => new Vector3(Mathf.Approach(value.X, target.X, shift), Mathf.Approach(value.Y, target.Y, shift), Mathf.Approach(value.Z, target.Z, shift));
        /// <summary>
        /// Incrementa um valor por um determinado deslocamento, mas nunca além do valor final.
        /// </summary>
        public static Vector3 Approach(Vector3 value, Vector3 target, Vector3 shift) => new Vector3(Mathf.Approach(value.X, target.X, shift.X), Mathf.Approach(value.Y, target.Y, shift.Y), Mathf.Approach(value.Z, target.Z, shift.Z));

        /// <summary>
        /// Retorna uma string representando este vetor 3D no formato:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Z:[<see cref="Z"/>]}
        /// </summary>
        public override string ToString() => '{' + string.Format("X:{0} Y:{1} Z:{2}", X, Y, Z) + '}';
        
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator -(Vector3 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            value.Z = -value.Z;

            return value;
        }
        public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Vector3 operator *(Vector3 a, float b) => new Vector3(a.X * b, a.Y * b, a.Z * b);
        public static Vector3 operator *(float a, Vector3 b) => new Vector3(a * b.X, a * b.Y, a * b.Z);
        public static Vector3 operator /(Vector3 a, Vector3 b) => new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Vector3 operator /(Vector3 a, float b) => new Vector3(a.X / b, a.Y / b, a.Z / b);
        public static Vector3 operator /(float a, Vector3 b) => new Vector3(a / b.X, a / b.Y, a / b.Z);
        public static bool operator ==(Vector3 a, Vector3 b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        public static bool operator !=(Vector3 a, Vector3 b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z;
    }
}