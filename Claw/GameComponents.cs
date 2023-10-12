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
            get => updateOrder;
            set
            {
                if (updateOrder != value) UpdateOrderChanged?.Invoke(this, EventArgs.Empty);

                updateOrder = value;
            }
        }
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value) EnabledChanged?.Invoke(this, EventArgs.Empty);

                enabled = value;
            }
        }
        private int updateOrder;
        private bool enabled = true;

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
            get => drawOrder;
            set
            {
                if (drawOrder != value) DrawOrderChanged?.Invoke(this, EventArgs.Empty);

                drawOrder = value;
            }
        }
        public bool Visible
        {
            get => visible;
            set
            {
                if (visible != value) VisibleChanged?.Invoke(this, EventArgs.Empty);

                visible = value;
            }
        }
        private int drawOrder;
        private bool visible = true;

        public event EventHandler<EventArgs> VisibleChanged, DrawOrderChanged;

        public virtual void Render() { }
    }
}