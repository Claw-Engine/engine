using System;

namespace Claw.Physics
{
    /// <summary>
    /// Calculador de raycast.
    /// </summary>
    public sealed class RayCaster
    {
        public bool Hit { get; private set; }
        public bool Ended { get; private set; }
        public Vector2? HitPoint { get; private set; }
        public event Func<Vector2, bool> OnMove;
        private float distance, maxDistance;
        private Vector2 cellSize, check, direction, step, stepSize, rayLength;
        private Line ray;

		public RayCaster(Line ray, float maxDistance, Func<Vector2, bool> onMove, Vector2 cellSize)
		{
            this.maxDistance = maxDistance;
            OnMove = onMove;
            this.cellSize = cellSize;

			this.ray = ray / cellSize;
			check = new Vector2((int)ray.Start.X, (int)ray.Start.Y);
			direction = Vector2.LengthDir(1, Vector2.GetAngle(ray.Start, ray.End));

			step = SignOne(direction);
            stepSize = Vector2.Abs(1 / direction);
			rayLength = Vector2.Zero;

			if (direction.X >= 0) rayLength.X = (check.X + 1 - ray.Start.X) * stepSize.X;
			else rayLength.X = (ray.Start.X - check.X) * stepSize.X;

			if (direction.Y >= 0) rayLength.Y = (check.Y + 1 - ray.Start.Y) * stepSize.Y;
			else rayLength.Y = (ray.Start.Y - check.Y) * stepSize.Y;
		}
        public RayCaster(Line ray, Func<Vector2, bool> onMove, Vector2 cellSize) : this(ray, Vector2.Distance(ray.Start, ray.End), onMove, cellSize) { }

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
            RayCaster caster = new RayCaster(ray, maxDistance, onMove, cellSize);

            while (!caster.Ended) caster.Move();

            hitPoint = caster.HitPoint;
        }

        /// <summary>
        /// Move o raio.
        /// </summary>
        public void Move()
        {
            if (Ended) return;

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

			Hit = OnMove?.Invoke(check) ?? false;

			if (Hit) HitPoint = (ray.Start + direction * distance) * cellSize;
			else HitPoint = null;

            Ended = Hit || distance >= maxDistance;
		}

        private static float SignOne(float value)
        {
            if (value < 0) return -1;

            return 1;
        }
        private static Vector2 SignOne(Vector2 value) => new Vector2(SignOne(value.X), SignOne(value.Y));
    }
}