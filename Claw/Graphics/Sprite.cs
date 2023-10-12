using System;

namespace Claw.Graphics
{
    /// <summary>
    /// Representa uma sprite, dentro de um <see cref="TextureAtlas"/>.
    /// </summary>
    public sealed class Sprite
    {
        public readonly Texture Texture;
        public readonly int X, Y, Width, Height;
        public readonly string Name;

        public Sprite(Texture texture, string name, int x, int y, int width, int height)
        {
            Texture = texture;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Name = name;
        }
        public Sprite(Texture texture, string name, Rectangle area) : this(texture, name, (int)area.X, (int)area.Y, (int)area.Width, (int)area.Height) { }
        public Sprite(Texture texture, string name, Vector2 location, Vector2 size) : this(texture, name, (int)location.X, (int)location.Y, (int)size.X, (int)size.Y) { }

        public override string ToString() => string.Format("Sprite({0})", Name);
    }
}