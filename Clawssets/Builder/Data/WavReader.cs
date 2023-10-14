using System;
using System.IO;

namespace Clawssets.Builder.Data
{
    /// <summary>
    /// Leitor de WAV.
    /// </summary>
    public static class WavReader
    {
        /// <summary>
        /// Carrega um arquivo WAVE.
        /// </summary>
        public static Audio.Description Load(BinaryReader reader)
        {
            Audio.Description audio = new Audio.Description();
            reader.BaseStream.Position += 12; // RIFF(4);SIZE(4);WAVE(4)

            char[] header;
            int bitDepth = -1;

            while (reader.BaseStream.Position < reader.BaseStream.Length - 1)
            {
                header = reader.ReadChars(4);
                uint chunkSize = reader.ReadUInt32();

                if (bitDepth == -1 && header.CompareHeader("fmt "))
                {
                    int compressionCode = reader.ReadInt16();
                    audio.Channels = (byte)reader.ReadInt16();
                    audio.SampleRate = reader.ReadInt32();
                    reader.BaseStream.Position += 6; // Average Bytes Per Second(4);Block Align(2)
                    bitDepth = reader.ReadInt16();
                    reader.BaseStream.Position += chunkSize - 16;
                    
                    if (compressionCode != 1)
                    {
                        Console.WriteLine("Erro: O compilador aceita apenas arquivos WAV não-comprimidos!");

                        return null;
                    }
                }
                else if (bitDepth != -1 && header.CompareHeader("data"))
                {
                    audio.Samples = new float[chunkSize / audio.Channels];
                    
                    for (int i = 0; i < audio.Samples.Length; i++) audio.Samples[i] = ReadSample(bitDepth, reader);
                }
                else reader.BaseStream.Position += chunkSize;
            }

            if (audio.Samples == null) audio.Samples = new float[0];

            return audio;
        }
        
        private static bool CompareHeader(this char[] a, string b) => a[0] == b[0] && a[1] == b[1] && a[2] == b[2] && a[3] == b[3];
        private static float ReadSample(int bitDepth, BinaryReader reader)
        {
            switch (bitDepth)
            {
                case 8: return Normalize(reader.ReadSByte(), sbyte.MinValue, sbyte.MaxValue);
                case 16: return Normalize(reader.ReadInt16(), ushort.MinValue, ushort.MaxValue);
                case 24: return Normalize(reader.ReadInt24(), Int24.MinValue, Int24.MaxValue);
                default: return Normalize(reader.ReadInt32(), int.MinValue, int.MaxValue);
            }
        }
        private static float Normalize(float value, int min, int max)
        {
            if (value >= 0) return value / max;

            return -(value / min);
        }

        /// <summary>
        /// Representa um inteiro de 24 bits.
        /// </summary>
        private struct Int24
        {
            public const int MaxValue = 8_388_607, MinValue = -8_388_608;
            private int value;

            public static implicit operator int(Int24 value) => value.value;
            public static implicit operator Int24(byte[] bytes) => new Int24() { value = (bytes[0] << 16) | (bytes[1] << 8) | bytes[2] };
        }

        private static Int24 ReadInt24(this BinaryReader reader) => reader.ReadBytes(3);
    }
}