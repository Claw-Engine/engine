namespace Claw.Graphics.Systems
{
    /// <summary>
    /// Define os tipo de origem possíveis para o <see cref="TextRenderer"/>.
    /// </summary>
    public enum TextOrigin
    {
        TopLeft, Top, TopRight,
        Left, Center, Right,
        BottomLeft, Bottom, BottomRight
    }
    /// <summary>
    /// Define os tipos de efeito do <see cref="TextRenderer"/>.
    /// </summary>
    public enum TextEffect
    {
        None,
        Wave, Scream,
        Pulsate, Rotation,
        MovingHorizontal, MovingVertical
    }
    /// <summary>
    /// Define os tipos de quebra de linha do <see cref="TextRenderer"/>.
    /// </summary>
    public enum TextWrap { NoWrap, Anywhere, Word }
}