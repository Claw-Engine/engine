using System;
using System.IO;
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
            get => _blendMode;
            set
            {
                _blendMode = value;

                SDL.SDL_SetTextureBlendMode(sdlTexture, (SDL.SDL_BlendMode)value);
            }
        }
        private BlendMode _blendMode = BlendMode.None;
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

		/// <summary>
		/// Carrega uma textura, com base num arquivo binário.
		/// </summary>
		internal static Texture ReadTexture(string filePath)
		{
			StreamReader stream = new StreamReader(filePath);
			BinaryReader reader = new BinaryReader(stream.BaseStream);

            return ReadTexture(reader);
		}
		/// <summary>
		/// Carrega uma textura, com base num arquivo binário.
		/// </summary>
		internal static Texture ReadTexture(BinaryReader reader)
		{
			int width = reader.ReadInt32(), height = reader.ReadInt32();

			if (width <= 0 || height <= 0) return null;

			uint[] pixels = new uint[width * height];
			byte a, b, g, r;

			for (int i = 0; i < pixels.Length; i++)
			{
				b = reader.ReadByte();
				g = reader.ReadByte();
				r = reader.ReadByte();
				a = reader.ReadByte();
				pixels[i] = ((uint)a << 24) | ((uint)b << 16) | ((uint)g << 8) | ((uint)r);
			}

			return new Texture(width, height, pixels);
		}
	}
}