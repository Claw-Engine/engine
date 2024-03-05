using System;

namespace Claw.Graphics.UI
{
    /// <summary>
    /// Elemento base para a <see cref="UI"/>.
    /// </summary>
    public abstract class Element
    {
        public string Name = string.Empty;
        public Vector2 RealSize { get; private set; }
        public UI UI => Game.Instance.UI;
        public Style Style
        {
            get => _style;
            set
            {
                if (value == null) Style = new Style();
                else _style = value;
            }
        }
        private Style _style = new Style();

        /// <summary>
        /// Checa se este elemento contém um ponto.
        /// </summary>
        /// <param name="point">Ponto da checagem.</param>
        /// <param name="drawPosition">Posição em que este elemento foi desenhado.</param>
        public virtual bool Contains(Vector2 point, Vector2 drawPosition) => drawPosition.X <= point.X && point.X < drawPosition.X + RealSize.X && drawPosition.Y <= point.Y && point.Y < drawPosition.Y + RealSize.Y;

        /// <summary>
        /// Calcula o tamanho real deste elemento para a <see cref="UI"/>.
        /// </summary>
        public abstract Vector2 CalculateSize();
        /// <summary>
        /// Realiza o <see cref="CalculateSize"/> e atualiza o <see cref="RealSize"/>.
        /// </summary>
        internal void UpdateRealSize() => RealSize = CalculateSize();

        /// <summary>
        /// Retorna um tamanho com limite baseado no <see cref="Style"/>.
        /// </summary>
        public Vector2 ClampSize(Vector2 size)
        {
            if (Style.MinSize.X > 0) size.X = Math.Max(size.X, Style.MinSize.X);

            if (Style.MinSize.Y > 0) size.Y = Math.Max(size.Y, Style.MinSize.Y);

            if (Style.MaxSize.X > 0) size.X = Math.Min(size.X, Style.MaxSize.X);

            if (Style.MaxSize.Y > 0) size.Y = Math.Min(size.Y, Style.MaxSize.Y);
            
            return size;
        }

        public abstract void Render(Vector2 position);
    }
}