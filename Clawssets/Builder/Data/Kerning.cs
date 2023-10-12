using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Clawssets.Builder.Data
{
    /// <summary>
    /// Wrapper para obter os pares kerning com o gdi32.dll.
    /// </summary>
    public static class Kerning
    {
        #region Implementação do wrapper
        private const string nativeLibName = "gdi32.dll";

        /// <summary>
        /// Estrutura de dados de um par.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Pair
        {
            public ushort wFirst;
            public ushort wSecond;
            public int iKernAmount;
        }

        [DllImport(nativeLibName, SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        private static extern uint GetKerningPairs(IntPtr hdc, uint nPairs, [Out] Pair[] pairs);

        [DllImport(nativeLibName)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);
        #endregion
        
        /// <summary>
        /// Retorna os pares de uma fonte, filtrando através de uma lista de caracteres.
        /// </summary>
        public static List<Pair> GetKerningPairs(this Font font, string text)
        {
            Pair[] pairs = GetKerningPairs(font);
            List<Pair> result = new List<Pair>();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '\n')
                {
                    Pair pair = pairs.FirstOrDefault((p) => p.wSecond == text[i]);

                    if (pair.iKernAmount != 0) result.Add(pair);
                }
            }

            return result;
        }
        /// <summary>
        /// Retorna todos os pares possíveis de uma fonte.
        /// </summary>
        private static Pair[] GetKerningPairs(this Font font)
        {
            Pair[] pairs;
            Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
            graphics.PageUnit = GraphicsUnit.Pixel;
            IntPtr hdc = graphics.GetHdc(), hFont = ((Font)font.Clone()).ToHfont();
            IntPtr old = SelectObject(hdc, hFont);

            try
            {
                uint numPairs = GetKerningPairs(hdc, 0, null);

                if (numPairs > 0)
                {
                    pairs = new Pair[numPairs];
                    numPairs = GetKerningPairs(hdc, numPairs, pairs);

                    return pairs;
                }
                else return null;
            }
            finally
            {
                old = SelectObject(hdc, old);

                graphics.ReleaseHdc();
            }
        }
    }
}