using System;

namespace Claw
{
    /// <summary>
    /// Classe pronta de um <see cref="IGameComponent"/> do tipo <see cref="IUpdateable"/>.
    /// </summary>
    public class GameComponent : IGameComponent, IUpdateable
    {
        public int UpdateOrder
        {
            get => _updateOrder;
            set
            {
                if (_updateOrder != value) UpdateOrderChanged?.Invoke(this, EventArgs.Empty);

				_updateOrder = value;
            }
        }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value) EnabledChanged?.Invoke(this, EventArgs.Empty);

				_enabled = value;
            }
        }
        private int _updateOrder;
        private bool _enabled = true;

        public event EventHandler<EventArgs> EnabledChanged, UpdateOrderChanged;

        public virtual void Initialize() { }
        public virtual void Step() { }
    }

    /// <summary>
    /// Classe pronta de um <see cref="IGameComponent"/> do tipo <see cref="IUpdateable"/> e <see cref="IDrawable"/>.
    /// </summary>
    public class DrawableGameComponent : GameComponent, IDrawable
    {
        public int DrawOrder
        {
            get => _drawOrder;
            set
            {
                if (_drawOrder != value) DrawOrderChanged?.Invoke(this, EventArgs.Empty);

				_drawOrder = value;
            }
        }
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible != value) VisibleChanged?.Invoke(this, EventArgs.Empty);

				_visible = value;
            }
        }
        private int _drawOrder;
        private bool _visible = true;

        public event EventHandler<EventArgs> VisibleChanged, DrawOrderChanged;

        public virtual void Render() { }
    }
}