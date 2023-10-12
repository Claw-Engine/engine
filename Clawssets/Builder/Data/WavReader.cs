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
            ReadSample readSample = null;

            while (reader.BaseStream.Position < reader.BaseStream.Length - 1)
            {
                header = reader.ReadChars(4);
                uint chunkSize = reader.ReadUInt32();

                if (readSample == null && header[0] == 'f' && header[1] == 'm' && header[2] == 't')
                {
                    int compressionCode = reader.ReadInt16();
                    audio.Channels = (byte)reader.ReadInt16();
                    audio.SampleRate = reader.ReadInt32();
                    reader.BaseStream.Position += 6; // Average Bytes Per Second(4);Block Align(2)
                    readSample += GetSampleReader(reader.ReadInt16());
                    reader.BaseStream.Position += chunkSize - 16;
                    
                    if (compressionCode != 1)
                    {
                        Console.WriteLine("Erro: O compilador aceita apenas arquivos WAV não-comprimidos!");

                        return null;
                    }
                }
                else if (readSample != null && header[0] == 'd' && header[1] == 'a' && header[2] == 't' && header[3] == 'a')
                {
                    audio.Samples = new int[chunkSize / audio.Channels];
                    
                    for (int i = 0; i < audio.Samples.Length; i++) audio.Samples[i] = readSample(reader);
                }
                else reader.BaseStream.Position += chunkSize;
            }

            if (audio.Samples == null) audio.Samples = new int[0];

            return audio;
        }

        private delegate int ReadSample(BinaryReader reader);
        private static int ReadSample8(BinaryReader reader) => reader.ReadSByte();
        private static int ReadSample16(BinaryReader reader) => reader.ReadInt16();
        private static int ReadSample24(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(3);

            return (bytes[0] << 16) | (bytes[1] << 8) | bytes[2];
        }
        private static int ReadSample32(BinaryReader reader) => reader.ReadInt32();
        private static ReadSample GetSampleReader(int bitDepth)
        {
            switch (bitDepth)
            {
                case 8: return ReadSample8;
                case 16: return ReadSample16;
                case 24: return ReadSample24;
                default: return ReadSample32;
            }
        }
    }
}