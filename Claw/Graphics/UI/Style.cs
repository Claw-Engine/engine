using System;

namespace Claw.Graphics.UI
{
    /// <summary>
    /// Define como os elementos devem ser desenhados.
    /// </summary>
    public sealed class Style
    {
        public bool Hide;
        public float TransitionAmount, FontScale = 1;
        public string NineSlice = string.Empty;
        public Vector2 Size, Gap, TopLeftPadding, BottomRightPadding, Offset;
        public Color Color = Color.White;
        public SpriteFont Font;
        public ValueTransition TransitionFunction = DefaultTransition;

        public delegate float ValueTransition(float value, float destiny, float amount);
        public static float DefaultTransition(float value, float destiny, float amount) => Mathf.DeltaLerp(value, destiny, amount, false);
        /// <summary>
        /// Realiza a transição dos valores para copiar um outro <see cref="Style"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="TransitionAmount"/> e <see cref="TransitionFunction"/> são ignorados.
        /// </remarks>
        public void TransitionTo(Style other)
        {
            if (other != null)
            {
                Hide = other.Hide;
                FontScale = TransitionFunction(FontScale, other.FontScale, TransitionAmount);
                NineSlice = other.NineSlice;
                Size = TransitionTo(Size, other.Size);
                Gap = TransitionTo(Gap, other.Gap);
                TopLeftPadding = TransitionTo(TopLeftPadding, other.TopLeftPadding);
                BottomRightPadding = TransitionTo(BottomRightPadding, other.BottomRightPadding);
                Offset = TransitionTo(Offset, other.Offset);
                Color = TransitionTo(Color, other.Color);
                Font = other.Font;
            }
        }
        private Vector2 TransitionTo(Vector2 a, Vector2 b) => new Vector2(TransitionFunction(a.X, b.X, TransitionAmount), TransitionFunction(a.Y, b.Y, TransitionAmount));
        private Color TransitionTo(Color a, Color b) => new Color(TransitionFunction(a.R, b.R, TransitionAmount), TransitionFunction(a.G, b.G, TransitionAmount),
            TransitionFunction(a.B, b.B, TransitionAmount), TransitionFunction(a.A, b.A, TransitionAmount));
    }
}