namespace Claw.Graphics.UI
{
    /// <summary>
    /// Representa o cursor da <see cref="UI"/>.
    /// </summary>
    public class UICursor : IAnimatable
    {
        public Element Selected;

        public float Scale = 1, Rotation;
        public Flip Flip = Flip.None;
        public Vector2 Position;
        public Color Color = Color.White;

        public Sprite Sprite { get; set; }
        public Vector2 Origin { get; set; }
        public Rectangle? SpriteArea { get; set; }
        public Animator Animator;
    }
}