using System;

namespace Claw.Graphics
{
    /// <summary>
    /// Classe responsável por <see cref="BlendMode"/>s customizados.
    /// </summary>
    public static class BlendFactory
    {
        /// <summary>
        /// Cria um blend mode customizado.
        /// </summary>
        public static BlendMode Build(Blend sourceColor = Blend.One, Blend destinationColor = Blend.Zero, BlendFunction colorFunction = BlendFunction.Add,
            Blend sourceAlpha = Blend.One, Blend destinationAlpha = Blend.Zero, BlendFunction alphaFunction = BlendFunction.Add)
        {
            return (BlendMode)SDL.SDL_ComposeCustomBlendMode((SDL.SDL_BlendFactor)sourceColor, (SDL.SDL_BlendFactor)destinationColor, (SDL.SDL_BlendOperation)colorFunction,
                (SDL.SDL_BlendFactor)sourceAlpha, (SDL.SDL_BlendFactor)destinationAlpha, (SDL.SDL_BlendOperation)alphaFunction);
        }
    }
}