using System;
using System.Collections.Generic;

namespace Claw
{
    /// <summary>
    /// Descreve uma linha 2D.
    /// </summary>
    public struct Line
    {
        public Vector2 Start, End;

        public Line(float xStart, float yStart, float xEnd, float yEnd)
        {
            Start = new Vector2(xStart, yStart);
            End = new Vector2(xEnd, yEnd);
        }
        public Line(Vector2 start, Vector2 end) : this(start.X, start.Y, end.X, end.Y) { }

        /// <summary>
        /// Transforma os vetores desta linha em vetores unitários com as mesmas direções. 
        /// </summary>
        public void Normalize()
        {
            Start.Normalize();
            End.Normalize();
        }

        /// <summary>
        /// Cria uma nova linha de uma interpolação linear com as linhas especificadas.
        /// </summary>
        /// <param name="value1">Valor atual.</param>
        /// <param name="value2">Valor de destino.</param>
        /// <param name="amount">Valor de ponderação (entre 0 e 1).</param>
        public static Line Lerp(Line value1, Line value2, float amount)
        {
            amount = Mathf.Clamp(0, 1, amount);

            return new Line(Vector2.Lerp(value1.Start, value2.Start, amount), Vector2.Lerp(value1.End, value2.End, amount));
        }

        /// <summary>
        /// Limita o valor especificado.
        /// </summary>
        /// <param name="value">O valor para limitar.</param>
        /// <param name="min">O valor mínimo.</param>
        /// <param name="max">O valor máximo.</param>
        public static Line Clamp(Line value, Line min, Line max) => new Line(Vector2.Clamp(value.Start, min.Start, max.Start), Vector2.Clamp(value.End, min.End, max.End));

        /// <summary>
        /// Retorna uma <see cref="Line"/> rotacionada.
        /// </summary>
        /// <param name="rotation">Graus.</param>
        public static Line Rotate(Line line, Vector2 origin, float rotation) => new Line(Vector2.Rotate(line.Start, origin, rotation), Vector2.Rotate(line.End, origin, rotation));

        /// <summary>
        /// Gera um quadrado.
        /// </summary>
        public static Line[] BoxGenerator(Rectangle rectangle)
        {
            return new Line[]
            {
                new Line(new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Right, rectangle.Top)),
                new Line(new Vector2(rectangle.Right, rectangle.Top), new Vector2(rectangle.Right, rectangle.Bottom)),
                new Line(new Vector2(rectangle.Right, rectangle.Bottom), new Vector2(rectangle.Left, rectangle.Bottom)),
                new Line(new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Left, rectangle.Top))
            };
        }
        /// <summary>
        /// Gera um círculo.
        /// </summary>
        public static Line[] CircleGenerator(float radius, Vector2 center, int segments = 16)
        {
            if (segments <= 0) return new Line[0];

            Line[] lines = new Line[segments];
            Vector2[] points = new Vector2[segments];
            double increment = Math.PI * 2 / segments, theta = 0;

            for (int i = 0; i < segments; i++)
            {
                points[i] = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                theta += increment;
            }

            for (int i = 0; i < points.Length; i++)
            {
                if (i != points.Length - 1) lines[i] = new Line(points[i], points[i + 1]);
                else lines[i] = new Line(points[i], points[0]);
            }

            return lines;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Line)) return false;

            return this == (Line)obj;
        }
        public override int GetHashCode()
        {
            var hashCode = 1075529825;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(Start);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(End);

            return hashCode;
        }

        /// <summary>
        /// Retorna uma string representando esta linha no formato:
        /// {Start:[<see cref="Start"/>] End:[<see cref="End"/>]}
        /// </summary>
        public override string ToString() => "{Start:" + Start.ToString() + " End:" + End.ToString() + "}";
        
        public static implicit operator Line(Rectangle value) => new Line(value.Location, value.End);
        public static Line operator +(Line a, Line b) => new Line(a.Start + b.Start, a.End + b.End);
        public static Line operator +(Line a, Vector2 b) => new Line(a.Start + b, a.End + b);
        public static Line operator +(Line a, float b) => new Line(new Vector2(a.Start.X + b, a.Start.Y + b), new Vector2(a.End.X + b, a.End.Y + b));
        public static Line operator -(Line a) => new Line(-a.Start, -a.End);
        public static Line operator -(Line a, Line b) => new Line(a.Start - b.Start, a.End - b.End);
        public static Line operator -(Line a, Vector2 b) => new Line(a.Start - b, a.End - b);
        public static Line operator -(Line a, float b) => new Line(new Vector2(a.Start.X - b, a.Start.Y - b), new Vector2(a.End.X - b, a.End.Y - b));
        public static Line operator *(Line a, Line b) => new Line(a.Start * b.Start, a.End * b.End);
        public static Line operator *(Line a, Vector2 b) => new Line(a.Start * b, a.End * b);
        public static Line operator *(Line a, float b) => new Line(a.Start * b, a.End * b);
        public static Line operator /(Line a, Line b) => new Line(a.Start / b.Start, a.End / b.End);
        public static Line operator /(Line a, Vector2 b) => new Line(a.Start / b, a.End / b);
        public static Line operator /(Line a, float b) => new Line(a.Start / b, a.End / b);
        public static bool operator ==(Line a, Line b) => a.Start == b.Start && a.End == b.End;
        public static bool operator !=(Line a, Line b) => a.Start != b.Start || a.End != b.End;
    }
}