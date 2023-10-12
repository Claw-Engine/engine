using System;
using Claw.Graphics;
using System.Collections.Generic;

namespace Claw
{
    /// <summary>
    /// Interface para componentes.
    /// </summary>
    public interface IGameComponent
    {
        void Initialize();
    }

    /// <summary>
    /// Interface para componentes que fazem parte do Update.
    /// </summary>
    public interface IUpdateable
    {
        bool Enabled { get; }
        int UpdateOrder { get; }

        event EventHandler<EventArgs> EnabledChanged, UpdateOrderChanged;

        void Step();
    }

    /// <summary>
    /// Interface para componentes que fazem parte do Draw.
    /// </summary>
    public interface IDrawable
    {
        bool Visible { get; }
        int DrawOrder { get; }
        
        event EventHandler<EventArgs> VisibleChanged, DrawOrderChanged;

        void Render();
    }

    /// <summary>
    /// Interface para elementos com animação.
    /// </summary>
    public interface IAnimatable
    {
        Vector2 Origin { get; set; }
        Sprite Sprite { get; set; }
        Rectangle? SpriteArea { get; set; }
    }
}