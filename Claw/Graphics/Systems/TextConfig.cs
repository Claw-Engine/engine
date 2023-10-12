namespace Claw.Graphics.Systems
{
    /// <summary>
    /// Define as configurações para a renderização do <see cref="TextRenderer"/>.
    /// </summary>
    public sealed class TextConfig
    {
        public float? Rotation = null;
        public Color? Color = null;
        public Vector2? Scale = null;
        public TextOrigin? Origin = null;
        public TextEffect? Effect = null;
        public SpriteFont Font = null;
        public Flip? Flip = null;

        public TextConfig() { }
        public TextConfig(float? rotation = null, Color? color = null, Vector2? scale = null, TextOrigin? origin = null, TextEffect? effect = null, SpriteFont font = null, Flip? flip = null)
        {
            Rotation = rotation;
            Color = color;
            Scale = scale;
            Origin = origin;
            Effect = effect;
            Font = font;
            Flip = flip;
        }

        /// <summary>
        /// Copia os valores de um outro <see cref="TextConfig"/>.
        /// </summary>
        public void Copy(TextConfig other)
        {
            Rotation = other.Rotation;
            Color = other.Color;
            Scale = other.Scale;
            Origin = other.Origin;
            Effect = other.Effect;
            Font = other.Font;
            Flip = other.Flip;
        }
    }
}