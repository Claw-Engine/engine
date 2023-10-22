using System.Collections.Generic;

namespace Claw.Graphics.Systems
{
    /// <summary>
    /// Representa a HUD do jogo.
    /// </summary>
    public sealed class HUD
    {
        public Vector2 Offset = Vector2.Zero, Scale = Vector2.One;
        public Dictionary<string, HUDContainer> Containers = new Dictionary<string, HUDContainer>();

        public HUDContainer this[string container] => Containers[container];
        public HUDElement this[string container, int element] => Containers[container][element];

        public HUD() { }

        public void Step()
        {
            foreach (KeyValuePair<string, HUDContainer> container in Containers) container.Value.Step(this);
        }
        public void Render()
        {
            bool ignoreCam = Draw.IgnoreCamera;
            Draw.IgnoreCamera = true;

            foreach (KeyValuePair<string, HUDContainer> container in Containers) container.Value.Render(this);

            Draw.IgnoreCamera = ignoreCam;
        }
    }
    /// <summary>
    /// Representa uma lista de <see cref="HUDElement"/>s.
    /// </summary>
    public class HUDContainer
    {
        public bool Active = true;
        public List<HUDElement> Elements = new List<HUDElement>();

        public HUDElement this[int element] => Elements[element];

        public virtual void Step(HUD hud)
        {
            if (Active)
            {
                foreach (HUDElement element in Elements) element.Step(hud);
            }
        }
        public virtual void Render(HUD hud)
        {
            if (Active)
            {
                foreach (HUDElement element in Elements) element.Render(hud);
            }
        }
    }
    /// <summary>
    /// Base de um elemento da HUD do jogo.
    /// </summary>
    public class HUDElement
    {
        public float Opacity = 1, Rotation = 0;
        public Color Color = Color.White;
        public Flip Flip = Flip.None;
        public Vector2 Position = Vector2.Zero, Scale = Vector2.One, Origin = Vector2.Zero;

        public HUDElement() { }

        /// <summary>
        /// Retorna a posição ajustada no hud.
        /// </summary>
        public Vector2 GetPosition(HUD hud) => (Position + hud.Offset) * hud.Scale;

        /// <summary>
        /// Retorna a escala ajustada no hud.
        /// </summary>
        public Vector2 GetScale(HUD hud) => Scale * hud.Scale;

        public virtual void Step(HUD hud) { }
        public virtual void Render(HUD hud) { }
    }
    /// <summary>
    /// Representa uma imagem na HUD do jogo.
    /// </summary>
    public sealed class HUDImage : HUDElement, IAnimatable
    {
        public Sprite Sprite { get; set; }
        public Rectangle? SpriteArea { get; set; }
        public new Vector2 Origin { get; set; }
        public Animator Animator
        {
            get => animator;
            set
            {
                if (animator != null && animator != value) animator.Animatable = null;

                if (value != null) value.Animatable = this;

                animator = value;
            }
        }
        private Animator animator;

        public HUDImage(Sprite sprite, Rectangle spriteArea, Vector2 position, Vector2 scale, Vector2 origin)
        {
            Sprite = sprite;
            SpriteArea = spriteArea;
            Position = position;
            Scale = scale;
            Origin = origin;
        }
        public HUDImage(Animator animator, Vector2 position, Vector2 scale)
        {
            Animator = animator;
            Position = position;
            Scale = scale;

            if (animator != null && animator.AnimationCount > 0) animator.Play(0);
        }

        public override void Step(HUD hud) => animator?.Step();
        public override void Render(HUD hud)
        {
            if (Sprite != null) Draw.Sprite(Sprite, GetPosition(hud), SpriteArea, Color * Opacity, Rotation, Origin, GetScale(hud), Flip);
        }
    }
    /// <summary>
    /// Representa um texto na HUD do jogo.
    /// </summary>
    public sealed class HUDText : HUDElement
    {
        public string Text = string.Empty;
        public SpriteFont Font;

        public HUDText(string text, SpriteFont font, Vector2 position, Vector2 scale, Vector2 origin)
        {
            Text = text;
            Font = font;
            Position = position;
            Scale = scale;
            Origin = origin;
        }

        public override void Render(HUD hud)
        {
            if (Text != null && Text.Length > 0 && Font != null) Draw.Text(Font, Text, GetPosition(hud), Color * Opacity, Rotation, Origin, GetScale(hud), Flip);
        }
    }
}