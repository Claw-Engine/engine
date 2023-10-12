using System.IO;

namespace Claw.Tiled
{
    ///<summary>
    ///Representa um tileset do Tiled.
    ///</summary>
    public sealed class Tileset
    {
        public int columns = 0, firstgid = 0, imagewidth = 0, imageheight = 0, margin = 0, spacing = 0, tilecount = 0, tilewidth = 0, tileheight = 0;
        public string image = string.Empty, name = string.Empty;

        /// <summary>
        /// Carrega um tileset do Tiled.
        /// </summary>
        internal static Tileset ReadTileset(BinaryReader reader)
        {
            Tileset tileset = new Tileset();

            tileset.columns = reader.ReadInt32();
            tileset.firstgid = reader.ReadInt32();
            tileset.imagewidth = reader.ReadInt32();
            tileset.imageheight = reader.ReadInt32();
            tileset.margin = reader.ReadInt32();
            tileset.spacing = reader.ReadInt32();
            tileset.tilecount = reader.ReadInt32();
            tileset.tilewidth = reader.ReadInt32();
            tileset.tileheight = reader.ReadInt32();

            tileset.image = reader.ReadString();
            tileset.name = reader.ReadString();

            return tileset;
        }
        /// <summary>
        /// Carrega um array de tilesets do Tiled.
        /// </summary>
        internal static Tileset[] ReadTilesets(BinaryReader reader)
        {
            int tilesetCount = reader.ReadInt32();
            Tileset[] tilesets = new Tileset[tilesetCount];

            for (int i = 0; i < tilesetCount; i++) tilesets[i] = ReadTileset(reader);

            return tilesets;
        }
    }
}