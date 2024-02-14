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
            public bool FramingFlag;
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
        private static void JumpSignature(this BitReader reader) => reader.JumpBytes(4 + 1 + 1 + 8 + 4 + 4 + 4 + 1);
        private static IdHeader ReadIdHeader(this BitReader reader, Audio.Description audio)
        {
            reader.JumpSignature();
            reader.JumpBytes(1);

            IdHeader idHeader = new IdHeader();

            reader.JumpCommon();

            idHeader.VorbisVersion = reader.ReadUInt32();
            audio.Channels = reader.ReadByte();
            audio.SampleRate = reader.ReadInt32();
            idHeader.BitrateMaximum = reader.ReadInt32();
            idHeader.BitrateNominal = reader.ReadInt32();
            idHeader.BitrateMinimum = reader.ReadInt32();
            idHeader.BlockSizes = reader.ReadByte();
            idHeader.FramingFlag = reader.ReadBit();

            if ((char)reader.ReadByte() != 'O')
            {
                reader.BackBytes(2);
                reader.BackBit();
                
                idHeader.FramingFlag = reader.ReadByte() == 1;
            }
            else reader.BackBytes(2);
            
            if (idHeader.BitrateNominal == 0 && idHeader.BitrateMaximum > 0 && idHeader.BitrateMinimum > 0) idHeader.BitrateNominal = (idHeader.BitrateMaximum + idHeader.BitrateMinimum) / 2;
            
            return idHeader;
        }
        private static void ReadCommentHeader(this BitReader reader)
        {
            reader.JumpSignature();
            reader.JumpCommon();

            uint vendorLength = reader.ReadUInt32();

            reader.JumpBytes(vendorLength);

            uint userCommentListLength = reader.ReadUInt32();
            uint len;
            
            for (uint i = 0; i < userCommentListLength; i++)
            {
                len = reader.ReadUInt32();
                
                reader.JumpBytes(len);
            }

            reader.ReadBit();
        }
    }
}