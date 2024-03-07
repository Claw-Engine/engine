using System;
using System.Collections.Generic;
using Claw.Extensions;

namespace Claw.Graphics
{
    /// <summary>
    /// Desenha formas, textos e sprites de forma simplificada.
    /// </summary>
    public static class Draw
    {
        /// <summary>
        /// Diz se o <see cref="Draw"/> deverá ignorar a câmera (falso por padrão).
        /// </summary>
        public static bool IgnoreCamera = false;
        private static Camera camera;
        private static BlendMode? forcedBlendMode;
        private static Texture pixelTexture;
        private static Rectangle pixelArea = new Rectangle(0, 0, 1, 1);

        internal static void Initialize() => pixelTexture = Texture.Pixel;

        #region Sprite
        /// <summary>
        /// Desenha uma sprite.
        /// </summary>
        /// <param name="rotation">Graus.</param>
        /// <param name="origin">De 0 a 1.</param>
        public static void Sprite(Sprite sprite, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, Flip flip)
        {
            Rectangle src;
            TextureAtlas.CurrentPage = sprite.Texture;
            pixelTexture = sprite.Texture;

            if (sourceRectangle.HasValue) src = new Rectangle(sourceRectangle.Value.Location + new Vector2(sprite.X, sprite.Y), sourceRectangle.Value.Size);
            else src = new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height);

            Sprite(sprite.Texture, position, src, color, rotation, origin, scale, flip);
        }
        /// <summary>
        /// Desenha uma sprite.
        /// </summary>
        /// <param name="rotation">Graus.</param>
        /// <param name="origin">De 0 a 1.</param>
        public static void Sprite(Sprite sprite, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, Flip flip) => Sprite(sprite, position, sourceRectangle, color, rotation, origin, new Vector2(scale), flip);
        /// <summary>
        /// Desenha uma sprite.
        /// </summary>
        public static void Sprite(Sprite sprite, Vector2 position, Rectangle? sourceRectangle, Color color) => Sprite(sprite, position, sourceRectangle, color, 0, Vector2.Zero, Vector2.One, Flip.None);
        /// <summary>
        /// Desenha uma sprite.
        /// </summary>
        public static void Sprite(Sprite sprite, Vector2 position, Color color) => Sprite(sprite, position, null, color, 0, Vector2.Zero, Vector2.One, Flip.None);

        /// <summary>
        /// Desenha uma sprite.
        /// </summary>
        /// <param name="rotation">Graus.</param>
        /// <param name="origin">De 0 a 1.</param>
        public static void Sprite(Texture texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, Flip flip)
        {
            if (!forcedBlendMode.HasValue || forcedBlendMode.Value == texture.BlendMode) DrawSprite(texture, position, sourceRectangle, color, rotation, origin, scale, flip);
            else
            {
                BlendMode originalBlend = texture.BlendMode;
                texture.BlendMode = forcedBlendMode.Value;

                DrawSprite(texture, position, sourceRectangle, color, rotation, origin, scale, flip);

                texture.BlendMode = originalBlend;
            }
        }
        /// <summary>
        /// Desenha uma sprite.
        /// </summary>
        /// <param name="rotation">Graus.</param>
        /// <param name="origin">De 0 a 1.</param>
        public static void Sprite(Texture texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, Flip flip) => Sprite(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale), flip);
        /// <summary>
        /// Desenha uma sprite.
        /// </summary>
        public static void Sprite(Texture texture, Vector2 position, Rectangle? sourceRectangle, Color color) => Sprite(texture, position, sourceRectangle, color, 0, Vector2.Zero, Vector2.One, Flip.None);
        /// <summary>
        /// Desenha uma sprite.
        /// </summary>
        public static void Sprite(Texture texture, Vector2 position, Color color) => Sprite(texture, position, null, color, 0, Vector2.Zero, Vector2.One, Flip.None);
        /// <summary>
        /// Desenha uma sprite.
        /// </summary>
        /// <param name="rotation">Graus.</param>
        /// <param name="origin">De 0 a 1.</param>
        private static void DrawSprite(Texture texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, Flip flip)
        {
            if (camera != null && !IgnoreCamera)
            {
                Vector2 camPos = new Vector2((int)camera.State.Position.X, (int)camera.State.Position.Y), camOrigin = new Vector2((int)camera.State.Origin.X, (int)camera.State.Origin.Y);
                position = Vector2.Rotate(position * camera.State.Zoom - camPos + camOrigin, camOrigin, (int)camera.State.Rotation);
                rotation += camera.State.Rotation;
                scale *= camera.State.Zoom;
            }

            Flip scaleFlip = Flip.None;

            if (scale.X < 0 && scale.Y > 0) scaleFlip = Flip.Horizontal;
            else if (scale.X > 0 && scale.Y < 0) scaleFlip = Flip.Vertical;
            else if (scale.X < 0 && scale.Y < 0) scaleFlip = Flip.Both;
            
            Rectangle src = sourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height),
                dest = new Rectangle(position - (origin * src.Size) * scale, src.Size * Vector2.Abs(scale));

            Game.Instance.Renderer.DrawTexture(texture, src, dest, color, dest.Size * origin, rotation, flip ^ scaleFlip);
        }
        #endregion

        #region Text
        /// <summary>
        /// Desenha um texto.
        /// </summary>
        /// <param name="rotation">Graus.</param>
        /// <param name="origin">De 0 a 1.</param>
        public static void Text(SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, Flip flip)
        {
            if (text != null && text.Length > 0)
            {
                switch (flip)
                {
                    case Flip.Horizontal: scale.X *= -1; break;
                    case Flip.Vertical: scale.Y *= -1; break;
                    case Flip.Both: scale *= -1; break;
                }

                Vector2 measure = font.MeasureString(text) * scale,
                    center = position + measure * origin;
                Vector2 basePos = Vector2.Zero;
                float charHeight = 0;

                for (int i = 0; i < text.Length; i++)
                {
                    char glyphChar = text[i];

                    switch (glyphChar)
                    {
                        case '\r': continue;
                        case '\n':
                            basePos.X = 0;
                            basePos.Y += charHeight + font.Spacing.Y;
                            charHeight = 0;
                            break;
                        case ' ':
                            if (font.Glyphs.ContainsKey(glyphChar)) goto default;

                            basePos.X += font.Spacing.X;
                            break;
                        default:
                            charHeight = Math.Max(charHeight, font.Glyphs[glyphChar].Area.Height);
                            SpriteFont.Glyph glyph = font.Glyphs[glyphChar];

                            if (i > 0) basePos.X += glyph.KerningPair.Get(text[i - 1], 0);
                            
                            Draw.Sprite(font.Sprite, Vector2.Rotate(basePos * scale + position, center, rotation) - measure * origin, glyph.Area, color, rotation, Vector2.Zero, scale, 0);

                            basePos.X += glyph.Area.Width;

                            if (i != text.Length - 1) basePos.X += font.Spacing.X;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Desenha um texto.
        /// </summary>
        /// <param name="rotation">Graus.</param>
        /// <param name="origin">De 0 a 1.</param>
        public static void Text(SpriteFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, Flip flip) => Text(font, text, position, color, rotation, origin, new Vector2(scale), flip);
        /// <summary>
        /// Desenha um texto.
        /// </summary>
        public static void Text(SpriteFont font, string text, Vector2 position, Color color) => Text(font, text, position, color, 0, Vector2.Zero, Vector2.One, Flip.None);
        #endregion

        #region Shapes
        /// <summary>
        /// Desenha um pixel.
        /// </summary>
        public static void Pixel(int scale, Vector2 position, Color color) => Sprite(pixelTexture, position, pixelArea, color, 0, Vector2.Zero, scale, Flip.None);
        /// <summary>
        /// Desenha um pixel.
        /// </summary>
        public static void Pixel(Vector2 scale, Vector2 position, Color color) => Sprite(pixelTexture, position, pixelArea, color, 0, Vector2.Zero, scale, Flip.None);

        /// <summary>
        /// Desenha uma linha.
        /// </summary>
        public static void Line(float lineWidth, Vector2 start, Vector2 end, Color color)
        {
            float angle = Vector2.GetAngle(start, end), length = Vector2.Distance(start, end);

            Sprite(pixelTexture, start, pixelArea, color, angle, Vector2.Zero, new Vector2(length, lineWidth), Flip.None);
        }
        /// <summary>
        /// Desenha uma linha.
        /// </summary>
        public static void Line(float lineWidth, Line line, Color color) => Line(lineWidth, line.Start, line.End, color);

        /// <summary>
        /// Desenha um polígono.
        /// </summary>
        public static void Polygon(float lineWidth, Color color, params Vector2[] points)
        {
            if (points.Length > 0)
            {
                for (int i = 0; i < points.Length - 1; i++) Line(lineWidth, points[i], points[i + 1], color);

                Line(lineWidth, points[points.Length - 1], points[0], color);
            }
        }
        /// <summary>
        /// Desenha um polígono.
        /// </summary>
        public static void Polygon(float lineWidth, Color color, params Line[] lines)
        {
            foreach (Line line in lines) Line(lineWidth, line.Start, line.End, color);
        }

        /// <summary>
        /// Desenha um retângulo.
        /// </summary>
        public static void Rectangle(float lineWidth, Rectangle rectangle, Color color)
        {
            Vector2[] points = new Vector2[]
            {
                new Vector2(rectangle.Left, rectangle.Top),
                new Vector2(rectangle.Right, rectangle.Top),
                new Vector2(rectangle.Right, rectangle.Bottom),
                new Vector2(rectangle.Left, rectangle.Bottom)
            };

            Polygon(lineWidth, color, points);
        }
        /// <summary>
        /// Desenha um retângulo cheio.
        /// </summary>
        public static void FilledRectangle(float lineWidth, Rectangle rectangle, Color outline, Color fill)
        {
            Rectangle fillRectangle = new Rectangle(rectangle.Left + lineWidth, rectangle.Top + lineWidth, rectangle.Width - lineWidth * 2, rectangle.Height - lineWidth * 2);

            Pixel(fillRectangle.Size, fillRectangle.Location, fill);
            Rectangle(lineWidth, rectangle, outline);
        }

        /// <summary>
        /// Desenha um círculo.
        /// </summary>
        public static void Circle(float lineWidth, float radius, Vector2 center, Color color, int segments = 16) => Oval(lineWidth, new Vector2(radius), center, color, segments);
        /// <summary>
        /// Desenha um círculo cheio.
        /// </summary>
        public static void FilledCircle(float lineWidth, float radius, Vector2 center, Color outline, Color fill, int segments = 16) => FilledOval(lineWidth, new Vector2(radius), center, outline, fill, segments);
        /// <summary>
        /// Desenha um formato oval.
        /// </summary>
        public static void Oval(float lineWidth, Vector2 radius, Vector2 center, Color color, int segments = 16)
        {
            Vector2[] points = new Vector2[segments];
            double increment = Math.PI * 2 / segments, theta = 0;

            for (int i = 0; i < segments; i++)
            {
                points[i] = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                theta += increment;

                if (i > 0) Line(lineWidth, points[i - 1], points[i], color);
            }

            Line(lineWidth, points[points.Length - 1], points[0], color);
        }
        /// <summary>
        /// Desenha um formato oval cheio.
        /// </summary>
        public static void FilledOval(float lineWidth, Vector2 radius, Vector2 center, Color outline, Color fill, int segments = 16)
        {
            Vector2 fillRadius = radius - new Vector2(lineWidth - 1);
            float fillWidth = (float)Math.Sqrt(radius.X * radius.Y);

            Oval(fillWidth, fillRadius, center, fill, segments);
            Oval(lineWidth, radius, center, outline, segments);
        }
        #endregion

        #region Other
        /// <summary>
        /// Desenha uma barra de vida circular.
        /// </summary>
        public static void CircleHealthBar(float lineWidth, float radius, Vector2 center, Color color, float life, float maxLife, int segments = 16) => OvalHealthBar(lineWidth, new Vector2(radius), center, color, life, maxLife, segments);
        /// <summary>
        /// Desenha uma barra de vida circular cheia.
        /// </summary>
        public static void FilledCircleHealthBar(float lineWidth, float radius, Vector2 center, Color outline, Color fill, float life, float maxLife, int segments = 16) => FilledOvalHealthBar(lineWidth, new Vector2(radius), center, outline, fill, life, maxLife, segments);
        /// <summary>
        /// Desenha uma barra de vida oval.
        /// </summary>
        public static void OvalHealthBar(float lineWidth, Vector2 radius, Vector2 center, Color color, float life, float maxLife, int segments = 16)
        {
            Vector2[] points = new Vector2[segments];
            float maxSegments = (int)(life / maxLife * segments);
            double increment = Math.PI * 2 / segments, theta = 0;
            
            for (int i = 0; i < points.Length; i++)
            {
                if (i > maxSegments) break;

                points[i] = center + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
                theta += increment;

                if (i > 0) Line(lineWidth, points[i - 1], points[i], color);
            }

            if (life == maxLife) Line(lineWidth, points[points.Length - 1], points[0], color);
        }
        /// <summary>
        /// Desenha uma barra de vida oval cheia.
        /// </summary>
        public static void FilledOvalHealthBar(float lineWidth, Vector2 radius, Vector2 center, Color outline, Color fill, float life, float maxLife, int segments = 16)
        {
            Vector2 fillRadius = radius - new Vector2(lineWidth - 1);
            float fillWidth = (float)Math.Sqrt(radius.X * radius.Y);

            OvalHealthBar(fillWidth, fillRadius, center, fill, life, maxLife, segments);
            OvalHealthBar(lineWidth, radius, center, outline, life, maxLife, segments);
        }

        /// <summary>
        /// Desenha uma curva de Bézier.
        /// </summary>
        public static void BezierCurve(float lineWidth, int segments, Color color, Vector2 point0, Vector2 point1, Vector2 point2, Vector2 point3)
        {
            for (int i = 0; i < segments; i++)
            {
                float t = (float)i / segments;
                Vector2 pixel = Mathf.CalculateBezierPoint(t, point0, point1, point2, point3);

                Sprite(pixelTexture, pixel, pixelArea, color, 0, Vector2.Zero, lineWidth, Flip.None);
            }
        }
        /// <summary>
        /// Desenha uma curva de Bézier.
        /// </summary>
        public static void BezierCurve(float lineWidth, int segments, Color color, Vector2 point0, Vector2 point1, Vector2 point2) => BezierCurve(lineWidth, segments, color, point0, point1, point1, point2);
        /// <summary>
        /// Desenha uma curva de Bézier.
        /// </summary>
        public static void BezierCurve(float lineWidth, int segments, Color color, Vector2 point0, Vector2 point1) => BezierCurve(lineWidth, segments, color, point0, point0, point1, point1);

        /// <summary>
        /// Desenha um colisor e um quadrado com a área que ele ocupa.
        /// </summary>
        public static void DebugCollider(float lineWidth, Polygon polygon, Color color)
        {
            Rectangle(lineWidth, polygon.BoundingBox, color);
            Polygon(lineWidth, color, polygon.LinesInWorld);
        }
        #endregion

        /// <summary>
        /// Se for diferente de nulo, o <see cref="Draw"/> forçará todas as texturas a terem esse <see cref="BlendMode"/>.
        /// </summary>
        public static void ForceBlendMode(BlendMode? blendMode) => forcedBlendMode = blendMode;

        /// <summary>
        /// Retorna a câmera atual.
        /// </summary>
        public static Camera GetCamera() => camera;
        /// <summary>
        /// Aplica a câmera e o viewport.
        /// </summary>
        /// <param name="camera">Se nulo, reseta o viewport e remove a câmera.</param>
        public static void SetCamera(Camera camera)
        {
            if (camera != null)
            {
                Draw.camera = camera;

                Game.Instance.Renderer.SetViewport(camera);
                camera.State.Update();
            }
            else
            {
                camera = null;

                Game.Instance.Renderer.ResetViewport();
            }
        }
        /// <summary>
        /// Atualiza a câmera.
        /// </summary>
        internal static void UpdateCamera() => camera?.State.Update();
    }
}