using System.IO;

namespace Claw.Tiled
{
    /// <summary>
    /// Representa um chunk do Tiled.
    /// </summary>
    public sealed class Chunk
    {
        public int x = 0, y = 0, width = 0, height = 0;
        public int[] data = new int[0];

        /// <summary>
        /// Carrega um chunk do Tiled.
        /// </summary>
        internal static Chunk ReadChunk(BinaryReader reader)
        {
            Chunk chunk = new Chunk();

            chunk.x = reader.ReadInt32();
            chunk.y = reader.ReadInt32();
            chunk.width = reader.ReadInt32();
            chunk.height = reader.ReadInt32();

            int dataCount = reader.ReadInt32();
            chunk.data = new int[dataCount];

            for (int i = 0; i < dataCount; i++) chunk.data[i] = reader.ReadInt32();

            return chunk;
        }
        /// <summary>
        /// Carrega um array de chunks do Tiled.
        /// </summary>
        internal static Chunk[] ReadChunks(BinaryReader reader)
        {
            int chunkCount = reader.ReadInt32();
            Chunk[] chunks = new Chunk[chunkCount];

            for (int i = 0; i < chunkCount; i++) chunks[i] = ReadChunk(reader);

            return chunks;
        }
    }
}