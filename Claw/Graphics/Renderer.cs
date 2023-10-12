using System;
using System.Runtime.InteropServices;

namespace Claw.Graphics
{
    /// <summary>
    /// Representa o renderizador da <see cref="Window"/>.
    /// </summary>
    public sealed class Renderer : IDisposable
    {
        public Color ClearColor
        {
            get
            {
                SDL.SDL_GetRenderDrawColor(sdlRenderer, out byte r, out byte g, out byte b, out byte a);

                return new Color(r, g, b, a);
            }
            set => SDL.SDL_SetRenderDrawColor(sdlRenderer, value.R, value.G, value.B, value.A);
        }
        private IntPtr sdlRenderer;
        private RenderTarget currentTarget;

        internal Renderer(IntPtr renderer) => sdlRenderer = renderer;
        ~Renderer() => Dispose();

        public void Dispose()
        {
            SDL.SDL_DestroyRenderer(sdlRenderer);

            sdlRenderer = IntPtr.Zero;
        }

        /// <summary>
        /// Retorna o alvo da renderização atual.
        /// </summary>
        public RenderTarget GetRenderTarget() => currentTarget;
        /// <summary>
        /// Altera o alvo da renderização.
        /// </summary>
        /// <param name="renderTarget">Nulo para desenhar na <see cref="Window"/>.</param>
        public void SetRenderTarget(RenderTarget renderTarget)
        {
            currentTarget = renderTarget;

            SDL.SDL_SetRenderTarget(sdlRenderer, renderTarget != null ? renderTarget.sdlTexture : IntPtr.Zero);
        }

        /// <summary>
        /// Desenha uma imagem.
        /// </summary>
        /// <param name="angle">Graus.</param>
        public void DrawTexture(Texture texture, Rectangle source, Rectangle destination, Color color, Vector2 origin, float angle, Flip flip)
        {
            SDL.SDL_Rect src = source.ToSDL();
            SDL.SDL_FRect dest = destination.ToSDLF();
            SDL.SDL_FPoint center = origin.ToSDLF();

            SDL.SDL_SetTextureColorMod(texture.sdlTexture, color.R, color.G, color.B);
            SDL.SDL_SetTextureAlphaMod(texture.sdlTexture, color.A);
            DrawTexture(texture.sdlTexture, ref src, ref dest, ref center, (double)angle, (SDL.SDL_RendererFlip)flip);
            SDL.SDL_SetTextureAlphaMod(texture.sdlTexture, 255);
            SDL.SDL_SetTextureColorMod(texture.sdlTexture, 255, 255, 255);
        }
        /// <summary>
        /// Desenha uma imagem.
        /// </summary>
        /// <param name="angle">Graus.</param>
        private void DrawTexture(IntPtr texture, ref SDL.SDL_Rect src, ref SDL.SDL_FRect dest, ref SDL.SDL_FPoint center, double angle, SDL.SDL_RendererFlip flip) => SDL.SDL_RenderCopyExF(sdlRenderer, texture, ref src, ref dest, (double)angle, ref center, flip);
        /// <summary>
        /// Desenha um pixel.
        /// </summary>
        internal void DrawPixel(int x, int y) => SDL.SDL_RenderDrawPoint(sdlRenderer, x, y);

        /// <summary>
        /// Cria uma textura com as dimensões especificadas.
        /// </summary>
        internal IntPtr CreateTexture(int width, int height, SDL.SDL_TextureAccess access) => SDL.SDL_CreateTexture(sdlRenderer, SDL.SDL_PIXELFORMAT_ABGR8888, (int)access, width, height);
        /// <summary>
        /// Cria uma textura com as dimensões e pixels especificados.
        /// </summary>
        internal IntPtr CreateTexture(int width, int height, uint[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr surface = SDL.SDL_CreateRGBSurfaceWithFormatFrom(handle.AddrOfPinnedObject(), width, height, 32, width * 4, SDL.SDL_PIXELFORMAT_ABGR8888);
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(sdlRenderer, surface);

            handle.Free();
            SDL.SDL_FreeSurface(surface);

            return texture;
        }

        /// <summary>
        /// Aplica o viewport da câmera.
        /// </summary>
        internal void SetViewport(Camera camera)
        {
            if (!camera.Viewport.IsEmpty)
            {
                SDL.SDL_Rect view = camera.Viewport.ToSDL();

                SDL.SDL_RenderSetViewport(sdlRenderer, ref view);
            }
            else SDL.SDL_RenderSetViewport(sdlRenderer, IntPtr.Zero);
        }
        /// <summary>
        /// Remove o viewport.
        /// </summary>
        internal void ResetViewport() => SDL.SDL_RenderSetViewport(sdlRenderer, IntPtr.Zero);

        /// <summary>
        /// Limpa a tela com o <see cref="ClearColor"/>.
        /// </summary>
        public void Clear() => SDL.SDL_RenderClear(sdlRenderer);
        /// <summary>
        /// Desenha o que foi renderizado.
        /// </summary>
        internal void Present() => SDL.SDL_RenderPresent(sdlRenderer);
    }
}