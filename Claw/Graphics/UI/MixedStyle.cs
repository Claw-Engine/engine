using System;
using System.Collections.Generic;

namespace Claw.Graphics.UI
{
    /// <summary>
    /// Combinação de <see cref="Style"/>s.
    /// </summary>
    public sealed class MixedStyle : Style
    {
        public List<Style> Styles { get; private set; }

        public MixedStyle() => Styles = new List<Style>();
        public MixedStyle(params Style[] styles) => Styles = new List<Style>(styles);
        public MixedStyle(List<Style> styles) => Styles = styles;

        /// <summary>
        /// Sincroniza este estilo com os estilos em <see cref="Styles"/>.
        /// </summary>
        public void Update()
        {
            if (Styles.Count == 0) return;

            Set(Styles[0]);

            for (int i = 1; i < Styles.Count; i++) Override(Styles[i]);
        }
        /// <summary>
        /// Altera todos os valores para <paramref name="other"/>.
        /// </summary>
        private void Set(Style other)
        {
            if (other != null)
            {
                Hide = other.Hide;
                FontScale = other.FontScale;
                NineSlice = other.NineSlice;
                InnerNineSlice = other.InnerNineSlice;
                Display = other.Display;
                Size = other.Size;
                MinSize = other.MinSize;
                MaxSize = other.MaxSize;
                Gap = other.Gap;
                TopLeftPadding = other.TopLeftPadding;
                BottomRightPadding = other.BottomRightPadding;
                Offset = other.Offset;
                Color = other.Color;
                TextColor = other.TextColor;
                Font = other.Font;
            }
        }
        /// <summary>
        /// Altera todos os valores para <paramref name="other"/> (que não sejam default).
        /// </summary>
        private void Override(Style other)
        {
            if (other != null)
            {
                if (other.TransitionAmount != 0) TransitionAmount = other.TransitionAmount;

                if (other.TransitionFunction != null) TransitionFunction = other.TransitionFunction;

                if (other.Hide != false) Hide = other.Hide;
                
                if (other.FontScale != 1) FontScale = other.FontScale;
                
                if (other.NineSlice.Length > 0) NineSlice = other.NineSlice;
                
                if (other.InnerNineSlice.Length > 0) InnerNineSlice = other.InnerNineSlice;

                if (other.Display != Display.InlineBlock) Display = other.Display;
                
                if (other.Size != Vector2.Zero) Size = other.Size;
                
                if (other.MinSize != Vector2.Zero) MinSize = other.MinSize;
                
                if (other.MaxSize != Vector2.Zero) MaxSize = other.MaxSize;
                
                if (other.Gap != Vector2.Zero) Gap = other.Gap;
                
                if (other.TopLeftPadding != Vector2.Zero) TopLeftPadding = other.TopLeftPadding;
                
                if (other.BottomRightPadding != Vector2.Zero) BottomRightPadding = other.BottomRightPadding;
                
                if (other.Offset != Vector2.Zero) Offset = other.Offset;
                
                if (other.Color != Color.White) Color = other.Color;
                
                if (other.TextColor != Color.Black) TextColor = other.TextColor;
                
                if (other.Font != null) Font = other.Font;
            }
        }
    }
}