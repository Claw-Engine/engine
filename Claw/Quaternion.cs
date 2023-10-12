using System;

namespace Claw
{
    /// <summary>
    /// Descreve a representação de rotações 3D.
    /// </summary>
    public struct Quaternion
    {
        public static Quaternion Identity => new Quaternion(0, 0, 0, 1);
        public float X, Y, Z, W;

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public Quaternion(Vector3 vector, float w)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = w;
        }

        /// <summary>
        /// Redimensiona a magnitude deste <see cref="Quaternion"/> para um comprimento em unidade.
        /// </summary>
        public void Normalize()
        {
            float val = 1.0f / (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
            X *= val;
            Y *= val;
            Z *= val;
            W *= val;
        }

        /// <summary>
        /// Cria um <see cref="Quaternion"/> que contém a versão conjugada do <see cref="Quaternion"/> especificado.
        /// </summary>
        /// <param name="angle">Radianos.</param>
        public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
        {
            float half = angle * .5f, sin = (float)Math.Sin(half), cos = (float)Math.Cos(half);

            return new Quaternion(axis.X * sin, axis.Y * sin, axis.Z * sin, cos);
        }

        /// <summary>
        /// Redimensiona a magnitude de um <see cref="Quaternion"/> para um comprimento em unidade.
        /// </summary>
        public static Quaternion Normalize(Quaternion value)
        {
            value.Normalize();

            return value;
        }

        /// <summary>
        /// Retorna uma string representando este quaternion no formato:
        /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>] Z:[<see cref="Z"/>] W:[<see cref="W"/>]}
        /// </summary>
        public override string ToString() => '{' + string.Format("X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W) + '}';

        public static Quaternion operator +(Quaternion a, Quaternion b) => new Quaternion(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        public static Quaternion operator -(Quaternion a, Quaternion b) => new Quaternion(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        public static Quaternion operator -(Quaternion value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            value.Z = -value.Z;
            value.W = -value.W;

            return value;
        }
        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            Quaternion result;
            float num = (a.Y * b.Z) - (a.Z * b.Y), num2 = (a.Z * b.X) - (a.X * b.Z), num3 = (a.X * b.Y) - (a.Y * b.X), num4 = ((a.X * b.X) + (a.Y * b.Y)) + (a.Z * b.Z);

            result.X = ((a.X * b.W) + (b.X * a.W)) + num;
            result.Y = ((a.Y * b.W) + (b.Y * a.W)) + num2;
            result.Z = ((b.Z * b.W) + (b.Z * a.W)) + num3;
            result.W = (a.W * b.W) - num4;

            return result;
        }
        public static Quaternion operator *(Quaternion a, float scale) => new Quaternion(a.X * scale, a.Y * scale, a.Z * scale, a.W * scale);
        public static Quaternion operator *(float scale, Quaternion a) => new Quaternion(a.X * scale, a.Y * scale, a.Z * scale, a.W * scale);
        public static Quaternion operator /(Quaternion a, Quaternion b)
        {
            Quaternion result;
            float num14 = (((b.X * b.X) + (b.Y * b.Y)) + (b.Z * b.Z)) + (b.W * b.W);
            float num5 = 1 / num14;
            float num4 = -b.X * num5;
            float num3 = -b.Y * num5;
            float num2 = -b.Z * num5;
            float num = b.W * num5;
            float num13 = (a.Y * num2) - (a.Z * num3);
            float num12 = (a.Z * num4) - (a.X * num2);
            float num11 = (a.X * num3) - (a.Y * num4);
            float num10 = ((a.X * num4) + (a.Y * num3)) + (a.Z * num2);

            result.X = ((a.X * num) + (num4 * a.W)) + num13;
            result.Y = ((a.Y * num) + (num3 * a.W)) + num12;
            result.Z = ((a.Z * num) + (num2 * a.W)) + num11;
            result.W = (a.W * num) - num10;

            return result;
        }
        public static Quaternion operator /(Quaternion a, float scale) => new Quaternion(a.X / scale, a.Y / scale, a.Z / scale, a.W / scale);
        public static Quaternion operator /(float scale, Quaternion a) => new Quaternion(a.X / scale, a.Y / scale, a.Z / scale, a.W / scale);
        public static bool operator ==(Quaternion a, Quaternion b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        public static bool operator !=(Quaternion a, Quaternion b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
    }
}