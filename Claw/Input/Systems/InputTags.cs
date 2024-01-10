using System;

namespace Claw.Input.Systems
{
    /// <summary>
    /// Interface base para as tags de input do <see cref="TaggedInput"/>.
    /// </summary>
    public interface IBaseTag
    {
        void Update(TaggedPlayer player);
    }

    /// <summary>
    /// Representa um input dentro do <see cref="TaggedInput"/>.
    /// </summary>
    public sealed class InputTag : IBaseTag
    {
        public MouseButtons? MouseButton;
        public bool IsDown, IsPressed, IsReleased;
        public float Buffer;
        public Keys Key, AltKey;
        public Buttons? Button, AltButton;
        private float BufferTimer;

        /// <summary>
        /// Cria uma instância de input de botões.
        /// </summary>
        /// <param name="mouse">-1 para não usar o mouse.</param>
        public InputTag(Keys key, Keys altKey, Buttons? button, Buttons? altButton, MouseButtons? mouse = null, float bufferTime = 0)
        {
            MouseButton = mouse;
            Key = key;
            AltKey = altKey;
            Button = button;
            AltButton = altButton;
            Buffer = bufferTime;
        }
        /// <summary>
        /// Cria uma instância de input de botões.
        /// </summary>
        /// <param name="mouse">-1 para não usar o mouse.</param>
        public InputTag(Keys key, Buttons? button, MouseButtons? mouse = null, float bufferTime = 0) : this(key, key, button, button, mouse, bufferTime) { }
        /// <summary>
        /// Cria uma instância de input de botões.
        /// </summary>
        public InputTag(MouseButtons mouse, float bufferTime = 0) : this(Keys.None, Keys.None, null, null, mouse, bufferTime) { }

        public void Update(TaggedPlayer player)
        {
            bool gamepad = Button != null && AltButton != null && Input.GamePadExists(player.GamePad);
            
            IsDown = (MouseButton.HasValue && Input.MouseButtonDown(MouseButton.Value)) ||
                (Input.KeyDown(Key) || Input.KeyDown(AltKey)) ||
                (gamepad && (Input.GamePadButtonDown(player.GamePad, Button.Value) || Input.GamePadButtonDown(player.GamePad, AltButton.Value)));

            IsPressed = BufferTimer > 0 || (MouseButton.HasValue && Input.MouseButtonPressed(MouseButton.Value)) ||
                (Input.KeyPressed(Key) || Input.KeyPressed(AltKey)) ||
                (gamepad && (Input.GamePadButtonPressed(player.GamePad, Button.Value) || Input.GamePadButtonPressed(player.GamePad, AltButton.Value)));

            IsReleased = (MouseButton.HasValue && Input.MouseButtonReleased(MouseButton.Value)) ||
                (Input.KeyReleased(Key) || Input.KeyReleased(AltKey)) ||
                (gamepad && (Input.GamePadButtonReleased(player.GamePad, Button.Value) || Input.GamePadButtonReleased(player.GamePad, AltButton.Value)));
            
            if (BufferTimer > 0) BufferTimer -= Time.UnscaledDeltaTime;
            else if (IsPressed) BufferTimer = Buffer;
        }
    }

    /// <summary>
    /// Representa um input de movimento dentro do <see cref="TaggedInput"/>.
    /// </summary>
    public sealed class MovementTag : IBaseTag
    {
        public bool UseLeftThumb, UseRightThumb;
        public Vector2 CurrentAxis;
        public (Keys Up, Keys Down, Keys Right, Keys Left) Keys;
        public (Keys Up, Keys Down, Keys Right, Keys Left) AltKeys;
        public (Buttons Up, Buttons Down, Buttons Right, Buttons Left)? Buttons;

        /// <summary>
        /// Cria uma instância de uma tag de movimento.
        /// </summary>
        /// <param name="keys">Up, Down, Right, Left.</param>
        /// <param name="altKeys">Up, Down, Right, Left.</param>
        /// <param name="buttons">Up, Down, Right, Left.</param>
        public MovementTag(bool leftThumb, bool rightThumb, (Keys Up, Keys Down, Keys Right, Keys Left) keys, (Keys Up, Keys Down, Keys Right, Keys Left) altKeys, (Buttons Up, Buttons Down, Buttons Right, Buttons Left)? buttons)
        {
            UseLeftThumb = leftThumb;
            UseRightThumb = rightThumb;
            Keys = keys;
            AltKeys = altKeys;
            Buttons = buttons;
        }
        /// <summary>
        /// Cria uma instância de uma tag de movimento.
        /// </summary>
        /// <param name="keys">Up, Down, Right, Left.</param>
        /// <param name="buttons">Up, Down, Right, Left.</param>
        public MovementTag(bool leftThumb, bool rightThumb, (Keys Up, Keys Down, Keys Right, Keys Left) keys, (Buttons Up, Buttons Down, Buttons Right, Buttons Left)? buttons) : this(leftThumb, rightThumb, keys, keys, buttons) { }

        public void Update(TaggedPlayer player)
        {
            bool gamepadExists = Input.GamePadExists(player.GamePad);
            bool gamepad = gamepadExists && Buttons != null;
            
            int up = (Input.KeyDown(Keys.Up) || Input.KeyDown(AltKeys.Up) || (gamepad && Input.GamePadButtonDown(player.GamePad, Buttons.Value.Up))) ? 1 : 0,
                down = (Input.KeyDown(Keys.Down) || Input.KeyDown(AltKeys.Down) || (gamepad && Input.GamePadButtonDown(player.GamePad, Buttons.Value.Down))) ? 1 : 0,
                right = (Input.KeyDown(Keys.Right) || Input.KeyDown(AltKeys.Right) || (gamepad && Input.GamePadButtonDown(player.GamePad, Buttons.Value.Right))) ? 1 : 0,
                left = (Input.KeyDown(Keys.Left) || Input.KeyDown(AltKeys.Left) || (gamepad && Input.GamePadButtonDown(player.GamePad, Buttons.Value.Left))) ? 1 : 0;

            CurrentAxis = new Vector2(right - left, down - up);

            if (CurrentAxis == Vector2.Zero)
            {
                if (UseLeftThumb && gamepadExists)
                {
                    var l = Input.LeftThumbStick(player.GamePad);
                    CurrentAxis = new Vector2(Math.Abs(l.X) >= TaggedInput.DeadAxis.X ? l.X : 0, Math.Abs(l.Y) >= TaggedInput.DeadAxis.Y ? l.Y : 0);
                }
                else if (UseRightThumb && gamepadExists)
                {
                    var r = Input.RightThumbStick(player.GamePad);
                    CurrentAxis = new Vector2(Math.Abs(r.X) >= TaggedInput.DeadAxis.X ? r.X : 0, Math.Abs(r.Y) >= TaggedInput.DeadAxis.Y ? r.Y : 0);
                }
            }
        }
    }
}