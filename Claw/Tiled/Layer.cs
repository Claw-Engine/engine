using System.IO;

namespace Claw.Tiled
{
    /// <summary>
    /// Representa uma layer do Tiled.
    /// </summary>
    public sealed class Layer
    {
        public int id = 0, x = 0, y = 0, width = 0, height = 0;
        public bool visible = true;
        public float opacity = 1;
        public string name = string.Empty, tintcolor = "#FFFFFF", color = "#000000", type = "tilelayer", draworder = "topdown";
        public int[] data = new int[0];
        public Chunk[] chunks = new Chunk[0];
        public Object[] objects = new Object[0];
        public Layer[] layers = new Layer[0];
        public Property[] properties = new Property[0];

        /// <summary>
        /// Carrega uma camada do Tiled.
        /// </summary>
        internal static Layer ReadLayer(BinaryReader reader)
        {
            Layer layer = new Layer();

            layer.id = reader.ReadInt32();
            layer.x = reader.ReadInt32();
            layer.y = reader.ReadInt32();
            layer.width = reader.ReadInt32();
            layer.height = reader.ReadInt32();
            
            layer.visible = reader.ReadBoolean();

            layer.opacity = reader.ReadSingle();

            layer.name = reader.ReadString();
            layer.tintcolor = reader.ReadString();
            layer.color = reader.ReadString();
            layer.type = reader.ReadString();
            layer.draworder = reader.ReadString();

            int dataCount = reader.ReadInt32();
            layer.data = new int[dataCount];

            for (int i = 0; i < dataCount; i++) layer.data[i] = reader.ReadInt32();

            layer.chunks = Chunk.ReadChunks(reader);

            layer.objects = Object.ReadObjects(reader);

            layer.layers = Layer.ReadLayers(reader);

            layer.properties = Property.ReadProperties(reader);

            return layer;
        }
        /// <summary>
        /// Carrega um array de camadas do Tiled.
        /// </summary>
        internal static Layer[] ReadLayers(BinaryReader reader)
        {
            int layerCount = reader.ReadInt32();
            Layer[] layers = new Layer[layerCount];

            for (int i = 0; i < layerCount; i++) layers[i] = ReadLayer(reader);

            return layers;
        }
    }
}