using System;

namespace Claw.Graphics.UI
{
    /// <summary>
    /// Define como os elementos devem ser desenhados.
    /// </summary>
    public class Style
    {
        public bool Hide;
        public float TransitionAmount, FontScale = 1;
        public string NineSlice = string.Empty, InnerNineSlice = string.Empty;
        public Display Display = Display.InlineBlock;
        public Vector2 Size, MinSize, MaxSize, Gap, TopLeftPadding, BottomRightPadding, Offset;
        public Color Color = Color.White, TextColor = Color.Black;
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
        public virtual void TransitionTo(Style other)
        {
            if (other != null)
            {
                Hide = other.Hide;
                FontScale = TransitionFunction(FontScale, other.FontScale, TransitionAmount);
                NineSlice = other.NineSlice;
                InnerNineSlice = other.InnerNineSlice;
                Display = other.Display;
                Size = TransitionTo(Size, other.Size);
                MinSize = TransitionTo(MinSize, other.MinSize);
                MaxSize = TransitionTo(MaxSize, other.MaxSize);
                Gap = TransitionTo(Gap, other.Gap);
                TopLeftPadding = TransitionTo(TopLeftPadding, other.TopLeftPadding);
                BottomRightPadding = TransitionTo(BottomRightPadding, other.BottomRightPadding);
                Offset = TransitionTo(Offset, other.Offset);
                Color = TransitionTo(Color, other.Color);
                TextColor = TransitionTo(TextColor, other.TextColor);
                Font = other.Font;
            }
        }
        protected Vector2 TransitionTo(Vector2 a, Vector2 b) => new Vector2(TransitionFunction(a.X, b.X, TransitionAmount), TransitionFunction(a.Y, b.Y, TransitionAmount));
        protected Color TransitionTo(Color a, Color b) => new Color(TransitionFunction(a.R, b.R, TransitionAmount), TransitionFunction(a.G, b.G, TransitionAmount),
            TransitionFunction(a.B, b.B, TransitionAmount), TransitionFunction(a.A, b.A, TransitionAmount));
    }
}