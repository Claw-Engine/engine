using System;
using System.Collections.Generic;

namespace Claw.Graphics.UI
{
    /// <summary>
    /// Elemento que carrega outros elementos.
    /// </summary>
    public sealed class Container : Element
    {
        /// <summary>
        /// Se verdadeiro, as partes fora deste <see cref="Container"/> serão cortadas por <see cref="RenderTarget"/>.
        /// </summary>
        public bool Scrollable = false;
        public Vector2 ScrollOffset
        {
            get => scrollOffset;
            set => scrollOffset = Vector2.Clamp(value, Vector2.Zero, ScrollMaxOffset);
        }
        /// <summary>
        /// Scroll máximo possível (calculado durante o <see cref="Render(Vector2)"/>).
        /// </summary>
        public Vector2 ScrollMaxOffset { get; private set; } = Vector2.Zero;
        public List<Element> Elements = new List<Element>();
        private Vector2 scrollOffset = Vector2.Zero, previousTopLeft, previousBottomRight;
        private RenderTarget surface;

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

                                    if ((element.Style.Display == Display.InlineBlock && elementPos.X + element.RealSize.X > maxSize.X) || element.Style.Display == Display.Block)
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

                                    if (element.Style.Display == Display.Block)
                                    {
                                        fitHeight += Style.Gap.Y + addY;
                                        addY = element.RealSize.Y;
                                    }
                                    else addY = Math.Max(addY, element.RealSize.Y);
                                }
                            }
                            else
                            {
                                if (previous != null) elementPos.X += Style.Gap.X + previous.RealSize.X;

                                if ((element.Style.Display == Display.InlineBlock && elementPos.X + element.RealSize.X > contentSize.X) || element.Style.Display == Display.Block)
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

            if (Scrollable && (result != RealSize || Style.TopLeftPadding != previousTopLeft || Style.BottomRightPadding != previousBottomRight))
            {
                if (surface != null) surface.Destroy();

                Vector2 surfaceSize = result - Style.TopLeftPadding - Style.BottomRightPadding;
                surface = new RenderTarget((int)surfaceSize.X, (int)surfaceSize.Y);
            }

            previousTopLeft = Style.TopLeftPadding;
            previousBottomRight = Style.BottomRightPadding;
            
            return result;
        }

        public override void Render(Vector2 position)
        {
            if (Scrollable && surface == null) return;

            RenderTarget previousTarget = Game.Instance.Renderer.GetRenderTarget();
            Vector2 drawingPos = position + Style.TopLeftPadding + Style.Offset, scroll = Vector2.Zero;
            Rectangle area = new Rectangle(position, RealSize);
            
            if (Style.NineSlice.Length > 0) NineSlice.Draw(Style.NineSlice, area, 0, Style.Color, UI.ScaleCenter);

            if (Scrollable && surface != null)
            {
                drawingPos = Vector2.Zero;
                scroll = scrollOffset;
                
                Game.Instance.Renderer.SetRenderTarget(surface);
                Game.Instance.Renderer.Clear();
                NineSlice.Draw(Style.NineSlice, new Rectangle(-Style.TopLeftPadding, RealSize), 0, Style.Color, UI.ScaleCenter);
            }

            if (Elements != null && Elements.Count > 0)
            {
                Rectangle contentArea = new Rectangle(Vector2.Zero, RealSize - Style.TopLeftPadding - Style.BottomRightPadding);
                Vector2 elementPos = Vector2.Zero;
                float addY = 0;
                Element element = null, previous = null;

                for (int i = 0; i < Elements.Count; i++)
                {
                    if (element != null && !element.Style.Hide) previous = element;

                    element = Elements[i];

                    if (!element.Style.Hide)
                    {
                        if (previous != null) elementPos.X += Style.Gap.X + previous.RealSize.X;

                        if ((element.Style.Display == Display.InlineBlock && elementPos.X + element.RealSize.X > contentArea.Right) || element.Style.Display == Display.Block)
                        {
                            elementPos.X = contentArea.X;
                            elementPos.Y += Style.Gap.Y + addY;
                            addY = element.RealSize.Y;
                        }
                        else addY = Math.Max(addY, element.RealSize.Y);

                        ScrollMaxOffset = new Vector2(Math.Max(ScrollMaxOffset.X, elementPos.X + element.RealSize.X), Math.Max(ScrollMaxOffset.Y, elementPos.Y + element.RealSize.Y)) - contentArea.Size;

                        Vector2 pos = elementPos + element.Style.Offset - scroll;

                        if (pos.X > contentArea.Right || pos.Y > contentArea.Bottom || pos.X + element.RealSize.X < contentArea.X || pos.Y + element.RealSize.Y < contentArea.Y) continue;

                        element.Render(drawingPos + pos);

                        if (UI.Cursor != null && UI.Cursor.Hover == null)
                        {
                            Vector2 correction = Vector2.Zero;

                            if (Scrollable) correction = position + Style.TopLeftPadding + Style.Offset;

                            UI.Cursor.TrySetHover(element, drawingPos + pos + correction);
                        }
                    }
                }
            }

            if (Scrollable && surface != null)
            {
                Game.Instance.Renderer.SetRenderTarget(previousTarget);
                Draw.Sprite(surface, position + Style.TopLeftPadding, Color.White);
            }
        }
    }
}