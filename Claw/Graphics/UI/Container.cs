using System;
using System.Collections.Generic;

namespace Claw.Graphics.UI
{
    /// <summary>
    /// Elemento que carrega outros elementos.
    /// </summary>
    public sealed class Container : Element
    {
        public List<Element> Elements = new List<Element>();

        public override Vector2 CalculateSize()
        {
            if (Style.Hide) return Vector2.Zero;

            Vector2 result = Style.Size;

            if (Elements != null)
            {
                Element element = null, previous = null;
                float addY = 0, fitHeight = 0;
                Vector2 elementPos = Vector2.Zero;
                Vector2 contentSize = Style.Size - Style.TopLeftPadding - Style.BottomRightPadding;
                Vector2 maxSize = Style.MaxSize - Style.TopLeftPadding - Style.BottomRightPadding;

                for (int i = 0; i < Elements.Count; i++)
                {
                    if (element != null && !element.Style.Hide) previous = element;

                    element = Elements[i];

                    if (!element.Style.Hide) element.UpdateRealSize();

                    if (Style.Size.X <= 0 || Style.Size.Y <= 0)
                    {
                        if (!element.Style.Hide)
                        {
                            if (Style.Size.X <= 0)
                            {
                                if (maxSize.X > 0)
                                {
                                    if (previous != null) elementPos.X += Style.Gap.X + previous.RealSize.X;

                                    if (elementPos.X + element.RealSize.X > maxSize.X)
                                    {
                                        if (result.X == 0 && previous != null)
                                        {
                                            result.X = Math.Min(elementPos.X - Style.Gap.X - previous.RealSize.X + Style.TopLeftPadding.X + Style.BottomRightPadding.X, Style.MaxSize.X);
                                            maxSize.X = result.X;
                                        }

                                        elementPos.X = 0;
                                        fitHeight += Style.Gap.Y + addY;
                                        addY = element.RealSize.Y;
                                    }
                                    else addY = Math.Max(addY, element.RealSize.Y);
                                }
                                else
                                {
                                    result.X += element.RealSize.X;
                                    fitHeight = Math.Max(fitHeight, element.RealSize.Y);

                                    if (previous != null) result.X += Style.Gap.X;
                                }
                            }
                            else
                            {
                                if (previous != null) elementPos.X += Style.Gap.X + previous.RealSize.X;

                                if (elementPos.X + element.RealSize.X > contentSize.X)
                                {
                                    elementPos.X = 0;
                                    fitHeight += Style.Gap.Y + addY;
                                    addY = element.RealSize.Y;
                                }
                                else addY = Math.Max(addY, element.RealSize.Y);
                            }
                        }
                    }
                }

                if (Style.Size.Y <= 0) result.Y += fitHeight + addY;
            }

            if (Style.Size.X <= 0) result.X += Style.TopLeftPadding.X + Style.BottomRightPadding.X;

            if (Style.Size.Y <= 0) result.Y += Style.TopLeftPadding.Y + Style.BottomRightPadding.Y;
            
            return result;
        }

        public override void Render(Vector2 position)
        {
            Rectangle area = new Rectangle(position, RealSize);

            if (Style.NineSlice.Length > 0) NineSlice.Draw(Style.NineSlice, area, 0, Style.Color, Game.Instance.UI.ScaleCenter);

            if (Elements != null && Elements.Count > 0)
            {
                Rectangle contentArea = new Rectangle(position + Style.TopLeftPadding, RealSize - Style.TopLeftPadding - Style.BottomRightPadding);
                Vector2 elementPos = contentArea.Location;
                float addY = 0;
                Element element = null, previous = null;

                for (int i = 0; i < Elements.Count; i++)
                {
                    if (element != null && !element.Style.Hide) previous = element;

                    element = Elements[i];

                    if (!element.Style.Hide)
                    {
                        if (previous != null) elementPos.X += Style.Gap.X + previous.RealSize.X;

                        if (elementPos.X + element.RealSize.X > contentArea.Right)
                        {
                            elementPos.X = contentArea.X;
                            elementPos.Y += Style.Gap.Y + addY;
                            addY = element.RealSize.Y;
                        }
                        else addY = Math.Max(addY, element.RealSize.Y);
                        
                        element.Render(elementPos + element.Style.Offset);
                    }
                }
            }
        }
    }
}