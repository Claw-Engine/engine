using System;
using System.Collections.Generic;
using System.Linq;
using Claw.Graphics;

namespace Claw
{
    /// <summary>
    /// A classe base dos objetos.
    /// </summary>
    public class GameObject : IGameComponent, IUpdateable, IDrawable, IAnimatable, IDisposable
    {
        /// <summary>
        /// Com essa opção desativada, será necessário adicionar o objeto aos componentes do jogo manualmente.
        /// </summary>
        public static bool InstantlyAdd = true;

        public int UpdateOrder
        {
            get => updateOrder;
            set
            {
                if (updateOrder != value)
                {
                    updateOrder = value;

                    OnUpdateOrderChanged(this, EventArgs.Empty);
                }
            }
        }
        public int DrawOrder
        {
            get => drawOrder;
            set
            {
                if (drawOrder != value)
                {
                    drawOrder = value;

                    OnDrawOrderChanged(this, EventArgs.Empty);
                }
            }
        }
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;

                    OnEnabledChanged(this, EventArgs.Empty);
                }
            }
        }
        public bool Visible
        {
            get => visible;
            set
            {
                if (visible != value)
                {
                    visible = value;

                    OnVisibleChanged(this, EventArgs.Empty);
                }
            }
        }
        public Game Game => Game.Instance;
        private int updateOrder, drawOrder;
        private bool enabled = true, visible = true, disposed = false;

        public bool DontDestroy = false;
        public bool Exists => Game.Components.GameObjects.IndexOf(this) >= 0;
        public string Name = string.Empty;
        public GameObject Parent
        {
            get => parent;
            set
            {
                if (parent != null) parent.children.Remove(this);

                if (value == null) parent = null;
                else if (value != this && value.parent != this)
                {
                    parent = value;

                    value.children.Add(this);
                }
            }
        }
        public string[] Tags => tags.ToArray();
        public GameObject[] Children => children.ToArray();
        private GameObject parent;
        internal List<string> tags = new List<string>();
        internal List<GameObject> children = new List<GameObject>();

        public float Rotation
        {
            get
            {
                if (Parent != null) return rotation + Parent.Rotation;

                return rotation;
            }
            set => rotation = value;
        }
        public Vector2 Position
        {
            get
            {
                if (Parent != null) return Vector2.Rotate(PositionInParent * Parent.Scale + Parent.Position, Parent.Position, Parent.Rotation);

                return position;
            }
            set => position = value;
        }
        public Vector2 Scale
        {
            get
            {
                if (Parent != null) return scale * Parent.Scale;

                return scale;
            }
            set => scale = value;
        }
        public float RotationInParent
        {
            get
            {
                if (Parent != null) return rotation - Parent.rotation;

                return 0;
            }
            set
            {
                if (Parent != null) Rotation = Parent.Rotation + value;
                else Rotation = value;
            }
        }
        public Vector2 PositionInParent
        {
            get
            {
                if (parent != null) return position - parent.position;

                return Vector2.Zero;
            }
            set
            {
                if (parent != null) Position = parent.Position + value;
                else Position = value;
            }
        }
        public Vector2 ScaleInParent
        {
            get
            {
                if (parent != null) return Scale / parent.Scale;

                return new Vector2(1, 1);
            }
            set
            {
                if (parent != null) Scale = parent.Scale * value;
                else Scale = value;
            }
        }
        public Vector2 Facing => Vector2.FindFacing(rotation);
        public List<Polygon> Colliders = new List<Polygon>();
        private float rotation = 0;
        private Vector2 position = Vector2.Zero, scale = new Vector2(1);

        public float Opacity = 1;
        public Color Color = Color.White;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public Sprite Sprite { get; set; }
        public Rectangle? SpriteArea { get; set; }
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
        public Flip Flip = Flip.None;
        private Animator animator;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public GameObject()
        {
            if (InstantlyAdd) Game.Components.Add(this);
        }
        ~GameObject() => Dispose(false);

        /// <summary>
        /// É executado quando o componente é adicionado ao jogo.
        /// </summary>
        public virtual void Initialize() { }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                animator?.Dispose();

                if (Colliders != null)
                {
                    for (int i = 0; i < Colliders.Count; i++) Colliders[i].Dispose();

                    Colliders.Clear();

                    Colliders = null;
                }

                animator = null;
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Adiciona uma tag no objeto.
        /// </summary>
        /// <param name="tag">Case insensitive.</param>
        public void AddTag(string tag)
        {
            if (tag.Length == 0) return;

            tag = tag.ToLower();

            TagManager.AddObject(tag, this);

            if (!tags.Contains(tag)) tags.Add(tag);
        }
        /// <summary>
        /// Remove uma tag do objeto.
        /// </summary>
        /// <param name="tag">Case insensitive.</param>
        public void RemoveTag(string tag)
        {
            if (tag.Length == 0) return;

            tag = tag.ToLower();

            TagManager.RemoveObject(tag, this);

            if (tags.Contains(tag)) tags.Remove(tag);
        }
        /// <summary>
        /// Diz se este <see cref="GameObject"/> possui uma tag específica.
        /// </summary>
        public bool HasTag(string tag) => tags.Contains(tag.ToLower());

        public virtual void Step() => animator?.Step();
        public virtual void Render()
        {
            if (Sprite != null) Draw.Sprite(Sprite, Position, SpriteArea, Color * Opacity, Rotation, Origin, Scale, Flip);
        }
        
        /// <summary>
        /// Destrói um objeto.
        /// </summary>
        protected void Destroy(GameObject gameObject, bool runDestroy = true) => gameObject.SelfDestroy(runDestroy);
        /// <summary>
        /// Destrói o objeto.
        /// </summary>
        public void SelfDestroy(bool runDestroy = true)
        {
            if (children.Count > 0)
            {
                foreach (GameObject gameObject in children) gameObject.SelfDestroy(runDestroy);
            }

            Game.Components.Remove(this);

            for (int i = 0; i < tags.Count; i++) TagManager.RemoveObject(tags[i], this);

            tags.Clear();

            if (runDestroy) OnDestroy();

            Dispose();
        }
        /// <summary>
        /// Chamado quando o objeto é destruído.
        /// </summary>
        protected virtual void OnDestroy() { }

        /// <summary>
        /// Chamado quando o <see cref="Enabled"/> muda. Usado pelo evento <see cref="EnabledChanged"/>.
        /// </summary>
        /// <param name="sender">Este componente.</param>
        /// <param name="args">Argumentos para o evento.</param>
        protected virtual void OnEnabledChanged(object sender, EventArgs args) => EnabledChanged?.Invoke(sender, args);
        /// <summary>
        /// Chamado quando o <see cref="UpdateOrder"/> muda. Usado pelo evento <see cref="UpdateOrderChanged"/>.
        /// </summary>
        /// <param name="sender">Este componente.</param>
        /// <param name="args">Argumentos para o evento.</param>
        protected virtual void OnUpdateOrderChanged(object sender, EventArgs args) => UpdateOrderChanged?.Invoke(sender, args);
        /// <summary>
        /// Chamado quando o <see cref="Visible"/> muda.
        /// </summary>
        /// <param name="sender">Este componente.</param>
        /// <param name="args">Argumentos para o evento.</param>
        protected virtual void OnVisibleChanged(object sender, EventArgs args) => VisibleChanged?.Invoke(sender, args);
        /// <summary>
        /// Chamado quando o <see cref="DrawOrder"/> muda.
        /// </summary>
        /// <param name="sender">Este componente.</param>
        /// <param name="args">Argumentos para o evento.</param>
        protected virtual void OnDrawOrderChanged(object sender, EventArgs args) => DrawOrderChanged?.Invoke(sender, args);
    }
}