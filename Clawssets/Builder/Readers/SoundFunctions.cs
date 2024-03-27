using System;
using System.IO;

namespace Clawssets.Builder.Readers
{
    /// <summary>
    /// Funções para a leitura de sons.
    /// </summary>
    public static class SoundFunctions
    {
        /// <summary>
        /// Lê um canal.
        /// </summary>
        public static float ReadChannel(this BinaryReader reader, int bitDepth)
        {
            switch (bitDepth)
            {
                case 8: return Normalize(reader.ReadSByte(), bitDepth);
                case 16: return Normalize(reader.ReadInt16(), bitDepth);
                case 24: return Normalize(reader.ReadInt24(), bitDepth);
                default: return Normalize(reader.ReadInt32(), bitDepth);
            }
        }
        /// <summary>
        /// Normaliza um valor para ter o range entre -1 e 1.
        /// </summary>
        public static float Normalize(float value, int bitDepth) => (float)(value / Math.Pow(2, bitDepth - 1));
    }
}