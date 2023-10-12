using System;

namespace Claw.Graphics
{
    /// <summary>
    /// Representa uma câmera 2D para operações no <see cref="Draw"/>.
    /// </summary>
    public sealed class Camera
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
        public void Follow(Vector2 position)
        {
            if (Position.X + Border.X != position.X && Position.X * Zoom - Border.X != position.X) Position.X = position.X - Border.X;
            if (Position.Y + Border.Y != position.Y && Position.Y * Zoom - Border.Y != position.Y) Position.Y = position.Y - Border.Y;

            Position = Vector2.Clamp(Position, MinPosition, MaxPosition);
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
        }
    }
}