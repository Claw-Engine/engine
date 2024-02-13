using System;
using System.IO;
using System.Collections;

namespace Clawssets.Builder.Readers
{
    public class BitReader
    {
        public bool BigEndian;
        public readonly Stream BaseStream;
        private int bit;
        private byte currentByte;

        public BitReader(Stream stream, bool bigEndian = false)
        {
            BaseStream = stream;
            currentByte = (byte)stream.ReadByte();
        }

        public void JumpBytes(uint count)
        {
            for (int i = 0; i < 8 * count; i++) ReadBit();
        }

        public bool ReadBit()
        {
            if (bit == 8)
            {
                bit = 0;
                currentByte = (byte)BaseStream.ReadByte();
            }

            bool value;

            if (BigEndian) value = (currentByte & (1 << bit)) > 0;
            else value = (currentByte & (1 << (7 - bit))) > 0;
            
            bit++;

            return value;
        }
        public byte ReadByte() => (byte)ReadBits(8);
        public sbyte ReadSByte() => (sbyte)ReadBits(8);
        public short ReadInt16() => (short)ReadBits(16);
        public ushort ReadUInt16() => (ushort)ReadBits(16);
        public Int24 ReadInt24() => (int)ReadBits(24);
        public int ReadInt32() => (int)ReadBits(32);
        public uint ReadUInt32() => (uint)ReadBits(32);
        public long ReadInt64() => (long)ReadBits(64);
        public ulong ReadUInt64() => (ulong)ReadBits(64);
        private long ReadBits(int count)
        {
            long value = 0;

            for (byte i = 0; i < count; i++)
            {
                value <<= 1;

                if (ReadBit()) value |= 1;
            }

            return value;
        }
    }
}