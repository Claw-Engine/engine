using System.IO;

namespace Claw.Tiled
{
    /// <summary>
    /// Representa um mapa do Tiled.
    /// </summary>
    public sealed class Map
    {
        public int compressionlevel = 0, width = 0, height = 0, nextlayerid = 0, nextobjectid = 0, tilewidth = 0, tileheight = 0;
        public bool infinite = false;
        public string orientation = "orthogonal", renderorder = "right-down", tiledversion = "1.10.1", type = "map", version = "1.0";
        public Layer[] layers = new Layer[0];
        public Tileset[] tilesets = new Tileset[0];

        /// <summary>
        /// Carrega um mapa do Tiled.
        /// </summary>
        internal static Map ReadMap(string filePath)
        {
            StreamReader stream = new StreamReader(filePath);
            BinaryReader reader = new BinaryReader(stream.BaseStream);
            Map map = new Map();

            map.compressionlevel = reader.ReadInt32();
            map.width = reader.ReadInt32();
            map.height = reader.ReadInt32();
            map.nextlayerid = reader.ReadInt32();
            map.nextobjectid = reader.ReadInt32();
            map.tilewidth = reader.ReadInt32();
            map.tileheight = reader.ReadInt32();

            map.infinite = reader.ReadBoolean();

            map.orientation = reader.ReadString();
            map.renderorder = reader.ReadString();
            map.tiledversion = reader.ReadString();
            map.type = reader.ReadString();
            map.version = reader.ReadString();

            map.layers = Layer.ReadLayers(reader);

            map.tilesets = Tileset.ReadTilesets(reader);

            stream.Close();
            reader.Close();

            return map;
        }
    }
}