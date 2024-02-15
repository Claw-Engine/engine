using System;
using System.IO;
using System.Collections;
using System.Text;

namespace Clawssets.Builder.Readers
{
    /// <summary>
    /// Implementação Stream de leitura de bits isolados.
    /// </summary>
    public class BitReader
    {
        public bool BigEndian;
        public bool OnByteStart => bit == 0 || bit == 8;
        public readonly Stream BaseStream;
        private int bit;
        private byte currentByte;

        public BitReader(Stream stream, bool bigEndian = false)
        {
            BaseStream = stream;
            currentByte = (byte)stream.ReadByte();
        }

        /// <summary>
        /// Pula um determinado número de bits.
        /// </summary>
        public void JumpBits(uint count)
        {
            for (int i = 0; i < count; i++) ReadBit();
        }
        /// <summary>
        /// Pula um determinado número de bytes.
        /// </summary>
        public void JumpBytes(uint count) => JumpBits(8 * count);
        
        /// <summary>
        /// Volta 1 bit.
        /// </summary>
        public void BackBit()
        {
            if (bit == 0)
            {
                bit = 7;
                BaseStream.Position--;
                currentByte = (byte)BaseStream.ReadByte();
            }
            else bit--;
        }
        /// <summary>
        /// Volta 8 bits.
        /// </summary>
        public void BackBytes(long count)
        {
            BaseStream.Position -= count;
            currentByte = (byte)BaseStream.ReadByte();
        }

        /// <summary>
        /// Lê 1 bit.
        /// </summary>
        public bool ReadBit()
        {
            if (bit == 8)
            {
                bit = 0;
                currentByte = (byte)BaseStream.ReadByte();
            }

            bool value = (currentByte & (1 << (7 - bit))) > 0;
            
            bit++;

            return value;
        }
        /// <summary>
        /// Lê 8 bits, sem sinal.
        /// </summary>
        public byte ReadByte() => (byte)ReadBits(8);
        /// <summary>
        /// Lê 1 char.
        /// </summary>
        public char ReadChar() => (char)ReadByte();
        /// <summary>
        /// Lê uma string.
        /// </summary>
        public string ReadString(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++) builder.Append(ReadChar());

            return builder.ToString();
        }
        /// <summary>
        /// Lê 8 bits, com sinal.
        /// </summary>
        public sbyte ReadSByte() => (sbyte)ReadBits(8);
        /// <summary>
        /// Lê 16 bits, com sinal.
        /// </summary>
        public short ReadInt16() => (short)ReadBits(16);
        /// <summary>
        /// Lê 16 bits, sem sinal.
        /// </summary>
        public ushort ReadUInt16() => (ushort)ReadBits(16);
        /// <summary>
        /// Lê 32 bits, com sinal.
        /// </summary>
        public int ReadInt32() => (int)ReadBits(32);
        /// <summary>
        /// Lê 32 bits, sem sinal.
        /// </summary>
        public uint ReadUInt32() => (uint)ReadBits(32);
        /// <summary>
        /// Lê 64 bits, com sinal.
        /// </summary>
        public long ReadInt64() => (long)ReadBits(64);
        /// <summary>
        /// Lê 64 bits, sem sinal.
        /// </summary>
        public ulong ReadUInt64() => (ulong)ReadBits(64);
        /// <summary>
        /// Lê até 64 bits, como um <see cref="long"/>.
        /// </summary>
        public ulong ReadBits(byte count)
        {
            ulong value = 0;

            if (count > 8 && count % 8 == 0)
            {
                byte[] bytes = new byte[count / 8];
                sbyte byteIndex = -1;

                for (byte i = 0; i < count; i++)
                {
                    if (i % 8 == 0) byteIndex++;

                    bytes[byteIndex] <<= 1;

                    if (ReadBit()) bytes[byteIndex] |= 1;
                }

                if (BitConverter.IsLittleEndian == BigEndian) Array.Reverse(bytes); // Se a ordem de bytes do arquivo for diferente da do sistema
                
                switch (count)
                {
                    case 16: value = BitConverter.ToUInt16(bytes, 0); break;
                    case 24: value = (UInt24)bytes; break;
                    case 32: value = BitConverter.ToUInt32(bytes, 0); break;
                    case 64: value = BitConverter.ToUInt64(bytes, 0); break;
                }
            }
            else for (byte i = 0; i < count; i++)
            {
                value <<= 1;

                if (ReadBit()) value |= 1;
            }

            return value;
        }
    }
}