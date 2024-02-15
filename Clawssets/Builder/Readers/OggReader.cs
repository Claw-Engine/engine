using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using Claw.Audio;
using Clawssets.Builder.Data;

namespace Clawssets.Builder.Readers
{
    /// <summary>
    /// Leitor de OGG.
    /// </summary>
    public static class OggReader
    {
        private class IdHeader
        {
            public uint VorbisVersion;
            public int BitrateMaximum, BitrateNominal, BitrateMinimum;
            public byte BlockSizes;
        }

        private class Setup
        {
            public CodeBook[] CodebookConfigs;
        }

        private class CodeBook
        {
            public byte[] SyncPattern;
            public ushort Dimensions;
            public int Entries;
            public uint?[] EntryCodewordLength;
            public bool Ordered;
            public ushort[] Multiplicands; 
        }

        /// <summary>
        /// Carrega um arquivo OGG.
        /// </summary>
        public static Audio.Description Load(BitReader reader)
        {
            Audio.Description audio = new Audio.Description();

            reader.ReadIdHeader(audio, out IdHeader idHeader);
            reader.ReadCommentHeader();
            reader.ReadSetup(out Setup setup);
            while (true) { }
            return null;
        }

        private static void JumpCommon(this BitReader reader) => reader.JumpBytes(7);// PacketType-Vorbis
        private static IdHeader ReadIdHeader(this BitReader reader, Audio.Description audio, out IdHeader idHeader)
        {
            reader.JumpBytes(4 + 1 + 1 + 8 + 4 + 4 + 4 + 1 + 1);// Signature + 1
            reader.JumpCommon();

            idHeader = new IdHeader();
            idHeader.VorbisVersion = reader.ReadUInt32();
            audio.Channels = reader.ReadByte();
            audio.SampleRate = reader.ReadInt32();
            idHeader.BitrateMaximum = reader.ReadInt32();
            idHeader.BitrateNominal = reader.ReadInt32();
            idHeader.BitrateMinimum = reader.ReadInt32();
            idHeader.BlockSizes = reader.ReadByte();

            reader.ReadByte();

            if (idHeader.BitrateNominal == 0 && idHeader.BitrateMaximum > 0 && idHeader.BitrateMinimum > 0) idHeader.BitrateNominal = (idHeader.BitrateMaximum + idHeader.BitrateMinimum) / 2;
            
            return idHeader;
        }
        private static void ReadCommentHeader(this BitReader reader)
        {
            reader.JumpBytes(43);// Gap
            reader.JumpCommon();
            
            reader.JumpBytes(reader.ReadUInt32());

            uint len = reader.ReadUInt32();

            for (uint i = 0; i < len; i++) reader.JumpBytes(reader.ReadUInt32());

            reader.ReadByte();
        }
        private static void ReadSetup(this BitReader reader, out Setup setup)
        {
            reader.JumpCommon();

            setup = new Setup();
            setup.CodebookConfigs = new CodeBook[reader.ReadByte() + 1];
            CodeBook codebook;

            for (int i = 0; i < setup.CodebookConfigs.Length; i++)
            {
                codebook = setup.CodebookConfigs[i] = new CodeBook();
                codebook.SyncPattern = new byte[3] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
                codebook.Dimensions = reader.ReadUInt16(); ;
                codebook.Entries = (int)reader.ReadBits(24);
                codebook.EntryCodewordLength = new uint?[codebook.Entries];
                codebook.Ordered = reader.ReadBit();

                if (!codebook.Ordered)
                {
                    bool sparse = reader.ReadBit();

                    for (int j = 0; j < codebook.Entries; j++)
                    {
                        if (sparse)
                        {
                            bool flag = reader.ReadBit();

                            if (flag) codebook.EntryCodewordLength[j] = (uint)reader.ReadBits(5) + 1;
                            else codebook.EntryCodewordLength[j] = null;
                        }
                        else codebook.EntryCodewordLength[j] = (uint)reader.ReadBits(5) + 1;
                    }
                }
                else
                {
                    uint currentEntry = 0;
                    uint currentLength = (uint)reader.ReadBits(5) + 1;
                
                    do
                    {
                        byte number = Ilog(codebook.Entries - currentEntry);

                        for (uint j = currentEntry; j <= currentEntry + number - 1; j++) codebook.EntryCodewordLength[j] = currentLength;

                        currentEntry = currentEntry + number;
                        currentLength++;
                    }while (currentEntry < 3);
                }

                byte lookupType = (byte)reader.ReadBits(4);

                switch (lookupType)
                {
                    case 0: break;
                    case 1: case 2:
                        float minimumValue = Float32Unpack(reader.ReadUInt32());
                        float deltaValue = Float32Unpack(reader.ReadUInt32());
                        byte valueBits =  (byte)(reader.ReadBits(4) + 1);
                        bool sequenceP = reader.ReadBit();
                        int lookupValues;

                        if (lookupType == 1) lookupValues = Lookup1Values(codebook.Entries, codebook.Dimensions);
                        else lookupValues = codebook.Entries * codebook.Dimensions;

                        codebook.Multiplicands = new ushort[lookupValues];

                        for (int j = 0; j < lookupValues; j++) codebook.Multiplicands[j] = (ushort)reader.ReadBits(valueBits);
                        break;
                }
            }
        }

        private static byte Ilog(uint x)
        {
            byte result = 0;

            Process:
            if (x > 0)
            {
                result++;
                x >>= 1;

                goto Process;
            }

            return result;
        }
        private static float Float32Unpack(uint x)
        {
            uint mantisa = x & 0x1fffff;
            uint sign = x & 0x80000000;
            uint exponent = (x & 0x7fe00000) >> 21;
            float result = (float)(mantisa * Math.Pow(2, (exponent - 788)));

            if (sign != 0) result = -result;

            return result;
        }
        private static int Lookup1Values(int entries, ushort dimensions)
        {
            int result = (int)Math.Floor(Math.Exp(Math.Log(entries) / dimensions));

            if (Math.Floor(Math.Pow(result + 1, dimensions)) <= entries) result++;

            return result;
        }
    }
}