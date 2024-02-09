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
        public Style Style
        {
            get => style;
            set
            {
                if (value == null) Style = new Style();
                else style = value;
            }
        }
        private Style style = new Style();

        /// <summary>
        /// Calcula o tamanho real deste elemento para a <see cref="UI"/>.
        /// </summary>
        public abstract Vector2 CalculateSize();
        /// <summary>
        /// Realiza o <see cref="CalculateSize"/> e atualiza o <see cref="RealSize"/>.
        /// </summary>
        internal void UpdateRealSize() => RealSize = CalculateSize();

        public abstract void Render(Vector2 position);
    }
}