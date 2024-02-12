using System;

namespace Claw.Graphics.UI
{
    /// <summary>
    /// Elemento de interação com arrasto para definição de valores.
    /// </summary>
    public class Slider : Element
    {
        public bool Vertical = false;
        /// <summary>
        /// Valores de 0 a 1.
        /// </summary>
        public float Value
        {
            get => value;
            set
            {
                if (value != this.value)
                {
                    this.value = Mathf.Clamp(value, 0, 1);

                    Sync?.Invoke(this.value);
                }
            }
        }
        /// <summary>
        /// Ação realizada quando <see cref="Value"/> for alterado.
        /// </summary>
        public event Action<float> Sync;
        private const float InnerSize = .25f;
        private float value;

        public override Vector2 CalculateSize()
        {
            Vector2 result = ClampSize(Style.Size);

            if (Vertical)
            {
                if (result.X <= 0) result.X = 6 + Style.TopLeftPadding.X + Style.BottomRightPadding.X;

                if (result.Y <= 0) result.Y = 64 + Style.TopLeftPadding.Y + Style.BottomRightPadding.Y;
            }
            else
            {
                if (result.X <= 0) result.X = 64 + Style.TopLeftPadding.X + Style.BottomRightPadding.X;

                if (result.Y <= 0) result.Y = 6 + Style.TopLeftPadding.Y + Style.BottomRightPadding.Y;
            }

            return result;
        }

        /// <summary>
        /// Altera o <see cref="Value"/>, baseando-se na diferença (<see cref="UICursor.HoverDifference"/>).
        /// </summary>
        public void SetValue(Vector2 difference)
        {
            Vector2 inner = RealSize * InnerSize;
            Vector2 max = RealSize - inner;
            difference -= inner * .5f;

            if (Vertical) Value = difference.Y / max.Y;
            else Value = difference.X / max.X;
        }

        public override void Render(Vector2 position)
        {
            if (Style.NineSlice.Length > 0) NineSlice.Draw(Style.NineSlice, new Rectangle(position, RealSize), 0, Style.Color, UI.ScaleCenter);
            else Draw.Pixel(RealSize, position, Style.Color);

            Vector2 innerOffset = Vector2.Zero;
            Vector2 contentSize = RealSize - Style.TopLeftPadding - Style.BottomRightPadding;
            Vector2 innerSize = contentSize;

            if (Vertical)
            {
                innerSize.Y = contentSize.Y * InnerSize;
                innerOffset.Y = (contentSize.Y - innerSize.Y) * value;
            }
            else
            {
                innerSize.X = contentSize.X * InnerSize;
                innerOffset.X = (contentSize.X - innerSize.X) * value;
            }
            
            if (Style.InnerNineSlice.Length > 0) NineSlice.Draw(Style.InnerNineSlice, new Rectangle(position + innerOffset + Style.TopLeftPadding, innerSize), 0, Style.Color, UI.ScaleCenter);
            else Draw.Pixel(innerSize, position + innerOffset + Style.TopLeftPadding, new Color(255 - Style.Color.R, 255 - Style.Color.G, 255 - Style.Color.B, 127));
        }
    }
}