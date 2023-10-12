using System;
using System.Runtime.InteropServices;

namespace Claw
{
    internal static class Clawdio
    {
        private const string nativeLibName = "Clawdio";

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr read_audio(string path, bool isMusic);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_want(ref SDL.SDL_AudioSpec want, IntPtr audio);
    }
}