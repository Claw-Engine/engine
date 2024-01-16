using System;

namespace Claw.Graphics
{
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