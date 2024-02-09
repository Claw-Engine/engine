using System;

namespace Claw.Graphics.UI
{
    public class Label : Element
    {
        public string Text;

        public override Vector2 CalculateSize()
        {
            Vector2 result = Style.Size;

            if (Text != null && Text.Length > 0 && Style.Font != null)
            {
                Vector2 measure = Style.Font.MeasureString(Text) * Style.FontScale;
                
                if (result.X <= 0) result.X = measure.X;

                if (result.Y <= 0) result.Y = measure.Y;
            }

            return ClampSize(result);
        }

        public override void Render(Vector2 position)
        {
            if (Text != null && Text.Length > 0 && Style.Font != null)
            {
                Vector2 measure = Style.Font.MeasureString(Text) * Style.FontScale;

                Draw.Text(Style.Font, Text, position + Style.TopLeftPadding, Style.TextColor, 0, Vector2.Zero, (RealSize / measure) * Style.FontScale, 0);
            }
        }
    }
}