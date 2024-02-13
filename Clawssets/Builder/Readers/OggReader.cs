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
            reader.ReadSignature();

            Audio.Description audio = new Audio.Description();

            IdHeader idHeader = reader.ReadIdHeader(audio);

            return null;
        }

        private static void ReadSignature(this BitReader reader)
        {
            reader.JumpBytes(4);// OggS

            byte version = reader.ReadByte(), flags = reader.ReadByte();
            ulong granulePosition = reader.ReadUInt64();
            uint serialNumber = reader.ReadUInt32(), sequenceNumber = reader.ReadUInt32(), checkSum = reader.ReadUInt32();
            byte totalSegments = reader.ReadByte();

            reader.JumpBytes(1);
        }
        private static void JumpCommon(this BitReader reader) => reader.JumpBytes(7);// PacketType-Vorbis
        private static IdHeader ReadIdHeader(this BitReader reader, Audio.Description audio)
        {
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

            return idHeader;
        }
    }
}