using System;

namespace Claw.Physics
{
    /// <summary>
    /// Calculador de raycast.
    /// </summary>
    public static class RayCaster
    {
        /// <summary>
        /// Realiza a movimentação do raio de um ponto ao outro.
        /// </summary>
        /// <param name="onMove">É executado sempre que o raio se move.
        /// <para>Parâmetro Vector2: A posição atual do raio.</para>
        /// <para>Retorno bool: O resultado do seu cálculo de colisão (true para parar o raio, false para prosseguir).</para>
        /// </param>
        public static void Cast(Line ray, Func<Vector2, bool> onMove, out Vector2? hitPoint, Vector2 cellSize) => Cast(ray, Vector2.Distance(ray.Start, ray.End), onMove, out hitPoint, cellSize);
        /// <summary>
        /// Realiza a movimentação do raio de um ponto ao outro.
        /// </summary>
        /// <param name="maxDistance">Distância em células.</param>
        /// <param name="onMove">É executado sempre que o raio se move.
        /// <para>Parâmetro Vector2: A posição atual do raio.</para>
        /// <para>Retorno bool: O resultado do seu cálculo de colisão (true para parar o raio, false para prosseguir).</para>
        /// </param>
        public static void Cast(Line ray, float maxDistance, Func<Vector2, bool> onMove, out Vector2? hitPoint, Vector2 cellSize)
        {
            Vector2 grid = cellSize;
            ray = ray / grid;
            Vector2 check = new Vector2((int)ray.Start.X, (int)ray.Start.Y);

            Vector2 dir = Vector2.LengthDir(1, Vector2.GetAngle(ray.Start, ray.End));
            Vector2 delta = ray.End - ray.Start;

            Vector2 step = new Vector2(dir.X < 0 ? -1 : 1, dir.Y < 0 ? -1 : 1);
            Vector2 stepSize = new Vector2(Math.Abs(1 / dir.X), Math.Abs(1 / dir.Y));
            Vector2 rayLength = Vector2.Zero;

            if (dir.X >= 0) rayLength.X = (check.X + 1 - ray.Start.X) * stepSize.X;
            else rayLength.X = (ray.Start.X - check.X) * stepSize.X;

            if (dir.Y >= 0) rayLength.Y = (check.Y + 1 - ray.Start.Y) * stepSize.Y;
            else rayLength.Y = (ray.Start.Y - check.Y) * stepSize.Y;

            bool hit = false;
            float distance = 0;

            while (!hit && distance < maxDistance)
            {
                if (rayLength.X < rayLength.Y)
                {
                    distance = rayLength.X;
                    check.X += step.X;
                    rayLength.X += stepSize.X;
                }
                else
                {
                    distance = rayLength.Y;
                    check.Y += step.Y;
                    rayLength.Y += stepSize.Y;
                }

                hit = onMove?.Invoke(check) ?? false;
            }

            if (hit) hitPoint = (ray.Start + dir * distance) * grid;
            else hitPoint = null;
        }
    }
}