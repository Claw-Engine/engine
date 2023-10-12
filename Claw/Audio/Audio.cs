using System;
using System.IO;

namespace Claw.Audio
{
    /// <summary>
    /// Representa um áudio no jogo.
    /// </summary>
    public class Audio
    {
        private readonly IntPtr audio;

        internal Audio(IntPtr audio) => this.audio = audio;

        public void PlayTest()
        {
            SDL.SDL_AudioSpec want = new SDL.SDL_AudioSpec(), have;

            Clawdio.set_want(ref want, audio);

            uint dev = SDL.SDL_OpenAudioDevice(IntPtr.Zero, 0, ref want, out have, (int)SDL.SDL_AUDIO_ALLOW_FORMAT_CHANGE);
            
            SDL.SDL_PauseAudioDevice(dev, 0);
        }
    }
    /// <summary>
    /// Representa um efeito sonoro no jogo.
    /// </summary>
    public class SoundEffect : Audio
    {
        internal SoundEffect(IntPtr audio) : base(audio) { }

        /// <summary>
        /// Carrega um efeito sonoro.
        /// </summary>
        internal static SoundEffect LoadSFX(string filePath) => new SoundEffect(Clawdio.read_audio(filePath, false));
    }
    /// <summary>
    /// Representa uma música no jogo.
    /// </summary>
    public class Music : Audio
    {
        internal Music(IntPtr audio) : base(audio) { }

        /// <summary>
        /// Carrega uma música.
        /// </summary>
        internal static SoundEffect LoadMusic(string filePath) => new SoundEffect(Clawdio.read_audio(filePath, true));
    }
}