using System;

namespace Claw.Graphics.UI
{
    /// <summary>
    /// Elemento de botão com texto.
    /// </summary>
    public class Button : Element
    {
        public string Text = string.Empty;
        public event Action OnClick;

        /// <summary>
        /// Executa o <see cref="OnClick"/>.
        /// </summary>
        public void Click() => OnClick?.Invoke();

        public override Vector2 CalculateSize()
        {
            Vector2 result = Style.Size;
            Vector2 measure = new Vector2(64, 32);

            if (Style.Font != null) measure = Style.Font.MeasureString(Text) * Style.FontScale;

            if (result.X <= 0) result.X = measure.X + Style.TopLeftPadding.X + Style.BottomRightPadding.X;

            if (result.Y <= 0) result.Y = measure.Y + Style.TopLeftPadding.Y + Style.BottomRightPadding.Y;

            return ClampSize(result);
        }

        public override void Render(Vector2 position)
        {
            if (Style.NineSlice.Length > 0) NineSlice.Draw(Style.NineSlice, new Rectangle(position, RealSize), 0, Style.Color, UI.ScaleCenter);
            else Draw.Pixel(RealSize, position, Style.Color);
            
            if (Style.Font != null && Text.Length > 0) Draw.Text(Style.Font, Text, position + RealSize * .5f, Style.TextColor, 0, new Vector2(.5f), Style.FontScale, 0);
        }
    }
}