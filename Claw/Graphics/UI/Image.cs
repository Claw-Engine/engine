using System;

namespace Claw.Graphics.UI
{
    public class Image : Element
    {
        public Sprite Sprite;
        public Rectangle? SpriteArea;

        public override Vector2 CalculateSize()
        {
            Vector2 result = Style.Size;

            if (Sprite != null)
            {
                if (result.X <= 0)
                {
                    if (SpriteArea.HasValue) result.X = SpriteArea.Value.Width;
                    else result.X = Sprite.Width;
                }

                if (result.Y <= 0)
                {
                    if (SpriteArea.HasValue) result.Y = SpriteArea.Value.Height;
                    else result.Y = Sprite.Height;
                }
            }

            return ClampSize(result);
        }

        public override void Render(Vector2 position)
        {
            if (Sprite != null) Draw.Sprite(Sprite, position + Style.TopLeftPadding, SpriteArea, Style.Color, 0, Vector2.Zero, new Vector2(RealSize.X / Sprite.Width, RealSize.Y / Sprite.Height), 0);
        }
    }
}