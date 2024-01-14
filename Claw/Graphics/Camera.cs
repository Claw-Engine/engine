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
        public virtual void Follow(Vector2 position) => Position = CalculateFollow(position);
        /// <summary>
        /// Cálculo de posição usado pelo método <see cref="Follow(Vector2)"/>.
        /// </summary>
        public Vector2 CalculateFollow(Vector2 position)
        {
            Vector2 topLeft = State.TopLeft + Border;
            Rectangle area = new Rectangle(topLeft, State.BottomRight - Border - topLeft);

            if (position.X < area.X) area.X = position.X;
            else if (position.X > area.Right) area.X = position.X - area.Width;

            if (position.Y < area.Y) area.Y = position.Y;
            else if (position.Y > area.Bottom) area.Y = position.Y - area.Height;

            return Vector2.Clamp((area.Location - Border + Origin) * Zoom, MinPosition, MaxPosition * Zoom);
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
    /// <summary>
    /// Estado da câmera para operações que envolvem zoom, rotação, posição e origem da câmera.
    /// </summary>
    public sealed class CameraState
    {
        public static readonly CameraState Neutral = new CameraState(null);
        public readonly Camera Camera;
        public float Zoom { get; private set; } = 1;
        public float Rotation { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Origin { get; private set; }
        public Vector2 TopLeft { get; private set; }
        public Vector2 BottomRight
        {
            get
            {
                if (Camera == null) return Game.Instance.Window.Size;

                return bottomRight;
            }
        }
        private Vector2 bottomRight;
        internal Rectangle viewport => Camera != null ? Camera.Viewport : new Rectangle();

        internal CameraState(Camera camera) => Camera = camera;

        /// <summary>
        /// Atualiza o estado da câmera.
        /// </summary>
        public void Update()
        {
            Zoom = Camera.Zoom;
            Rotation = Camera.Rotation;
            Position = Camera.Position;
            Origin = Camera.Origin;
            TopLeft = (Position - Origin) / Zoom;
            bottomRight = Game.Instance.Window.Size;

            if (viewport.Size != Vector2.Zero) bottomRight = viewport.End;

            bottomRight = Camera.ScreenToWorld(bottomRight);
        }
    }
}