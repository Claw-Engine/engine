using System;
using System.Collections;
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

        /// <summary>
        /// Carrega um arquivo OGG.
        /// </summary>
        public static Audio.Description Load(BitReader reader)
        {
            Audio.Description audio = new Audio.Description();

            IdHeader idHeader = reader.ReadIdHeader(audio);
            
            reader.ReadCommentHeader();
            while (true) { }
            return null;
        }

        private static void JumpCommon(this BitReader reader) => reader.JumpBytes(7);// PacketType-Vorbis
        private static void ReadFraming(this BitReader reader)
        {
            reader.ReadBit();

            if ((char)reader.ReadByte() != 'O')// Por algum motivo, framing está armazenado em 1 byte ao invés de 1 bit
            {
                reader.BackBytes(2);
                reader.BackBit();
                reader.ReadByte();
            }
            else reader.BackBytes(2);
        }
        private static IdHeader ReadIdHeader(this BitReader reader, Audio.Description audio)
        {
            reader.JumpBytes(4 + 1 + 1 + 8 + 4 + 4 + 4 + 1 + 1);// Signature + 1

            IdHeader idHeader = new IdHeader();

            reader.JumpCommon();

            idHeader.VorbisVersion = reader.ReadUInt32();
            audio.Channels = reader.ReadByte();
            audio.SampleRate = reader.ReadInt32();
            idHeader.BitrateMaximum = reader.ReadInt32();
            idHeader.BitrateNominal = reader.ReadInt32();
            idHeader.BitrateMinimum = reader.ReadInt32();
            idHeader.BlockSizes = reader.ReadByte();

            reader.ReadFraming();
            
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

            reader.ReadFraming();
        }
    }
}