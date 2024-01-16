using System;

namespace Claw.Graphics
{
    /// <summary>
    /// Representa uma câmera 2D para operações no <see cref="Draw"/>.
    /// </summary>
    public class Camera
    {
        public float Zoom = 1, Rotation;
        public Vector2 Position, Origin, Border, MinPosition, MaxPosition;
        public Rectangle Viewport
        {
            get => viewport;
            set
            {
                if (value != viewport)
                {
                    viewport = value;

                    if (Draw.GetCamera() == this) Game.Instance.Renderer.SetViewport(this);
                }
            }
        }
        public readonly CameraState State;
        private Rectangle viewport;

        public Camera() : this(new Rectangle()) { }
        /// <param name="viewport">Viewport da câmera. Se estiver vazio, ocupará a tela inteira.</param>
        public Camera(Rectangle viewport)
        {
            this.viewport = viewport;
            State = new CameraState(this);
        }
        /// <param name="viewport">Viewport da câmera. Se estiver vazio, ocupará a tela inteira.</param>
        public Camera(Rectangle viewport, float zoom, float rotation, Vector2 position, Vector2 origin, Vector2 border, Vector2 minPosition, Vector2 maxPosition) : this(viewport)
        {
            Zoom = zoom;
            Rotation = rotation;
            Position = position;
            Origin = origin;
            Border = border;
            MinPosition = minPosition;
            MaxPosition = maxPosition;
        }
        /// <param name="viewport">Viewport da câmera. Se estiver vazio, ocupará a tela inteira.</param>
        public Camera(Rectangle viewport, Vector2 position, Vector2 origin, Vector2 minPosition, Vector2 maxPosition) : this(viewport, 1, 0, position, origin, Vector2.Zero, minPosition, maxPosition) { }

        /// <summary>
        /// Segue uma posição específica, sem passar dos limites estipulados.
        /// </summary>
        public virtual void Follow(Vector2 position, FollowMode mode = FollowMode.Border) => Position = CalculateFollow(position, mode);
        /// <summary>
        /// Cálculo de posição usado pelo método <see cref="Follow(Vector2, FollowMode)"/>.
        /// </summary>
        public Vector2 CalculateFollow(Vector2 position, FollowMode mode = FollowMode.Border)
        {
            Vector2 topLeft = State.TopLeft + Border, bottomRight = State.BottomRight;
            Vector2 result = position;

            switch (mode)
            {
                case FollowMode.Border:

                    Rectangle area = new Rectangle(topLeft, bottomRight - Border - topLeft);

                    if (position.X < area.X) area.X = position.X;
                    else if (position.X > area.Right) area.X = position.X - area.Width;

                    if (position.Y < area.Y) area.Y = position.Y;
                    else if (position.Y > area.Bottom) area.Y = position.Y - area.Height;

                    result = (area.Location - Border + Origin) * Zoom;

                    break;
                case FollowMode.AwaysCenter: result = (position - (bottomRight - topLeft) * .5f + Origin) * Zoom; break;
            }

            return Vector2.Clamp(result, MinPosition, MaxPosition * Zoom);
        }
        
        /// <summary>
        /// Converte um ponto da tela em um ponto do mundo, com base no <see cref="State"/>.
        /// </summary>
        public Vector2 ScreenToWorld(Vector2 point) => Vector2.Rotate((point - Viewport.Location - State.Origin) / State.Zoom, Vector2.Zero, -State.Rotation) + State.Position / State.Zoom;
        /// <summary>
        /// Converte um ponto do mundo em um ponto da tela, com base no <see cref="State"/>.
        /// </summary>
        public Vector2 WorldToScreen(Vector2 point) => Vector2.Rotate(point * State.Zoom - State.Position + State.Origin, State.Origin, State.Rotation) + Viewport.Location;
    }
}