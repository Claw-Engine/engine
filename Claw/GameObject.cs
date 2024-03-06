using System;
using System.Collections.Generic;
using System.Linq;
using Claw.Graphics;

namespace Claw
{
    /// <summary>
    /// A classe base dos objetos.
    /// </summary>
    public class GameObject : IGameModule, IUpdateable, IDrawable, IAnimatable, IDisposable
    {
        /// <summary>
        /// Com essa opção desativada, será necessário adicionar o objeto aos módulos do jogo manualmente.
        /// </summary>
        public static bool InstantlyAdd = true;

        public int UpdateOrder
        {
            get => _updateOrder;
            set
            {
                if (_updateOrder != value)
                {
					_updateOrder = value;

                    UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public int DrawOrder
        {
            get => _drawOrder;
            set
            {
                if (_drawOrder != value)
                {
					_drawOrder = value;

					DrawOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
					_enabled = value;

                    EnabledChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible != value)
                {
					_visible = value;

                    VisibleChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public Game Game => Game.Instance;
        private int _updateOrder, _drawOrder;
        private bool _enabled = true, _visible = true, disposed = false;

        public bool DontDestroy = false;
        public bool Exists => Game.Modules.GameObjects.IndexOf(this) >= 0;
        public string Name = string.Empty;
        public GameObject Parent
        {
            get => parent;
            set
            {
                if (parent != null) parent.children.Remove(this);

                if (value == null)
                {
                    _position = Position;
                    _scale = Scale;
                    _rotation = Rotation;
                    parent = null;
                }
                else if (value != this && value.parent != this)
                {
                    parent = value;
                    _rotation = _rotation - Parent.Rotation;
                    _scale = Parent.Scale / _scale;
                    _position = _position - Parent.Position;

                    value.children.Add(this);
                }
            }
        }
        public string[] Tags => tags.ToArray();
        private GameObject parent;
        internal List<string> tags = new List<string>();
        internal List<GameObject> children = new List<GameObject>();

        public float Rotation
        {
            get
            {
                if (Parent != null) return _rotation + Parent.Rotation;

                return _rotation;
            }
            set => _rotation = value;
        }
        public Vector2 Position
        {
            get
            {
                if (Parent != null) return Vector2.Rotate(_position * Parent.Scale + Parent.Position, Parent.Position, Parent.Rotation);

                return _position;
            }
            set => _position = value;
        }
        public Vector2 Scale
        {
            get
            {
                if (Parent != null) return _scale * Parent.Scale;

                return _scale;
            }
            set => _scale = value;
        }
        public Vector2 Facing => Vector2.FindFacing(_rotation);
        public List<Polygon> Colliders = new List<Polygon>();
        private float _rotation = 0;
        private Vector2 _position = Vector2.Zero, _scale = new Vector2(1);

        public float Opacity = 1;
        public Color Color = Color.White;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public Sprite Sprite { get; set; }
        public Rectangle? SpriteArea { get; set; }
        public Animator Animator
        {
            get => _animator;
            set
            {
                if (_animator != null && _animator != value) _animator.Animatable = null;

                if (value != null) value.Animatable = this;

				_animator = value;
            }
        }
        public Flip Flip = Flip.None;
        private Animator _animator;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public GameObject()
        {
            if (InstantlyAdd) Game.Modules.Add(this);
        }
        ~GameObject() => Dispose(false);

        /// <summary>
        /// É executado quando o módulo é adicionado ao jogo.
        /// </summary>
        public virtual void Initialize() { }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                _animator?.Dispose();

                if (Colliders != null)
                {
                    for (int i = 0; i < Colliders.Count; i++) Colliders[i].Dispose();

                    Colliders.Clear();

                    Colliders = null;
                }

                _animator = null;
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

        /// <summary>
        /// Retorna a quantidade de filhos deste objeto.
        /// </summary>
        public int ChildCount() => children.Count;
        /// <summary>
        /// Retorna um filho deste objeto.
        /// </summary>
        public GameObject GetChild(int index) => children[index];

        public virtual void Step() => _animator?.Step();
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
            Parent = null;

            if (children.Count > 0)
            {
                for (int i = children.Count - 1; i >= 0; i--) children[i].SelfDestroy(runDestroy);
            }

            Game.Modules.Remove(this);

            for (int i = 0; i < tags.Count; i++) TagManager.RemoveObject(tags[i], this);

            tags.Clear();

            if (runDestroy) OnDestroy();

            Dispose();
        }
        /// <summary>
        /// Chamado quando o objeto é destruído.
        /// </summary>
        protected virtual void OnDestroy() { }
    }
}