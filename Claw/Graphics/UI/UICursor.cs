namespace Claw.Graphics.UI
{
    /// <summary>
    /// Representa o cursor da <see cref="UI"/>.
    /// </summary>
    public class UICursor : IAnimatable
    {
        /// <summary>
        /// Diferença entre <see cref="Position"/> e a posição de desenho do <see cref="Hover"/>.
        /// </summary>
        public Vector2 HoverDifference;
        public Element Hover;

        public float Scale = 1, Rotation;
        public Flip Flip = Flip.None;
        public Vector2 Position;
        public Color Color = Color.White;

        public Sprite Sprite { get; set; }
        public Vector2 Origin { get; set; }
        public Rectangle? SpriteArea { get; set; }
        public Animator Animator;

        /// <summary>
        /// Altera <see cref="Hover"/> e <see cref="HoverDifference"/>, caso o cursor esteja dentro do elemento.
        /// </summary>
        public void TrySetHover(Element element, Vector2 elementDrawPos)
        {
            if (element.Contains(Position, elementDrawPos))
            {
                Hover = element;
                HoverDifference = Position - elementDrawPos;
            }
        }
    }
}