using System;
using System.Runtime.InteropServices;

namespace Claw.Graphics
{
    /// <summary>
    /// Representa uma textura no jogo.
    /// </summary>
    public class Texture
    {
        public static Texture Pixel { get; internal set; }
        public readonly int Width, Height;
        public Vector2 Size => new Vector2(Width, Height);
        public BlendMode BlendMode
        {
            get => blendMode;
            set
            {
                blendMode = value;

                SDL.SDL_SetTextureBlendMode(sdlTexture, (SDL.SDL_BlendMode)value);
            }
        }
        private BlendMode blendMode = BlendMode.None;
        internal readonly IntPtr sdlTexture;

        internal Texture(int width, int height, IntPtr texture)
        {
            Width = width;
            Height = height;
            sdlTexture = texture;
        }
        public Texture(int width, int height) : this(width, height, Game.Instance.Renderer.CreateTexture(width, height, SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING)) { }
        public Texture(int width, int height, params uint[] pixels) : this(width, height, Game.Instance.Renderer.CreateTexture(width, height, pixels)) { }
        
        /// <summary>
        /// Altera os pixels desta textura.
        /// </summary>
        public virtual void SetData(params uint[] pixels)
        {
            GCHandle handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);

            SDL.SDL_UpdateTexture(sdlTexture, IntPtr.Zero, handle.AddrOfPinnedObject(), Width * sizeof(UInt32));
            handle.Free();
        }
    }
}