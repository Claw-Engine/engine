using System;
using System.IO;
using Newtonsoft.Json;
using Claw.Tiled;
using System.Runtime.Serialization.Formatters.Binary;

namespace Clawssets.Builder
{
    /// <summary>
    /// Classe para compilação de mapas.
    /// </summary>
    public static class MapBuilder
    {
        /// <summary>
        /// Compila os mapas para a pasta do grupo.
        /// </summary>
        public static void Build(AssetGroup group, string basePath)
        {
            string directory = Path.Combine(AssetBuilder.BuildDirectory, group.Name);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            for (int i = 0; i < group.Files.Count; i++)
            {
                string path = Path.GetFullPath(Path.Combine(basePath, group.Files[i]));
                string mapExtension = Path.GetExtension(path).ToLower();
                Map map;

                switch (mapExtension)
                {
                    case ".tmj": case ".json":
                        string json = File.ReadAllText(path);
                        map = JsonConvert.DeserializeObject<Map>(json);
                        break;
                    default:
                        Console.WriteLine("Erro: O tipo \"Map\" não tem suporte para a extensão \"{0}\"", mapExtension);
                        return;
                }

                string outputPath = Path.Combine(directory, Path.GetFileNameWithoutExtension(path) + AssetBuilder.AssetExtension);

                StreamWriter writer = new StreamWriter(outputPath);
                BinaryWriter binWriter = new BinaryWriter(writer.BaseStream);

                binWriter.Write(map);
                binWriter.Close();
            }

            Console.WriteLine("Grupo {0} compilado com sucesso!", group.Name);
        }

        #region Extensões do BinaryWriter
        /// <summary>
        /// Escreve os dados do mapa num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Map map)
        {
            writer.Write(map.compressionlevel);
            writer.Write(map.width);
            writer.Write(map.height);
            writer.Write(map.nextlayerid);
            writer.Write(map.nextobjectid);
            writer.Write(map.tilewidth);
            writer.Write(map.tileheight);
            
            writer.Write(map.infinite);
            
            writer.Write(map.orientation);
            writer.Write(map.renderorder);
            writer.Write(map.tiledversion);
            writer.Write(map.type);
            writer.Write(map.version);

            writer.Write(map.layers);
            writer.Write(map.tilesets);
        }

        /// <summary>
        /// Escreve os dados de um array de camadas num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Layer[] layers)
        {
            writer.Write(layers.Length);

            for (int i = 0; i < layers.Length; i++) writer.Write(layers[i]);
        }
        /// <summary>
        /// Escreve os dados de uma camada num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Layer layer)
        {
            writer.Write(layer.id);
            writer.Write(layer.x);
            writer.Write(layer.y);
            writer.Write(layer.width);
            writer.Write(layer.height);
            
            writer.Write(layer.visible);
            
            writer.Write(layer.opacity);
            
            writer.Write(layer.name);
            writer.Write(layer.tintcolor);
            writer.Write(layer.color);
            writer.Write(layer.type);
            writer.Write(layer.draworder);

            writer.Write(layer.data.Length);

            for (int i = 0; i < layer.data.Length; i++) writer.Write(layer.data[i]);

            writer.Write(layer.chunks);

            writer.Write(layer.objects);

            writer.Write(layer.layers);

            writer.Write(layer.properties);
        }

        /// <summary>
        /// Escreve os dados de um array de chunks num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Chunk[] chunks)
        {
            writer.Write(chunks.Length);

            for (int i = 0; i < chunks.Length; i++) writer.Write(chunks[i]);
        }
        /// <summary>
        /// Escreve os dados de um chunk num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Chunk chunk)
        {
            writer.Write(chunk.x);
            writer.Write(chunk.y);
            writer.Write(chunk.width);
            writer.Write(chunk.height);

            writer.Write(chunk.data.Length);

            for (int i = 0; i < chunk.data.Length; i++) writer.Write(chunk.data[i]);
        }

        /// <summary>
        /// Escreve os dados de um array de objetos num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Claw.Tiled.Object[] objs)
        {
            writer.Write(objs.Length);

            for (int i = 0; i < objs.Length; i++) writer.Write(objs[i]);
        }
        /// <summary>
        /// Escreve os dados de um objeto num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Claw.Tiled.Object obj)
        {
            writer.Write(obj.id);
            writer.Write(obj.width);
            writer.Write(obj.height);

            writer.Write(obj.visible);

            writer.Write(obj.x);
            writer.Write(obj.y);
            writer.Write(obj.rotation);

            writer.Write(obj.name);
            writer.Write(obj.type);

            writer.Write(obj.properties);
        }

        /// <summary>
        /// Escreve os dados de um array de propriedades num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Property[] properties)
        {
            writer.Write(properties.Length);

            for (int i = 0; i < properties.Length; i++) writer.Write(properties[i]);
        }
        /// <summary>
        /// Escreve os dados de um propriedade num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Property property)
        {
            writer.Write(property.name);
            writer.Write(property.type);

            byte[] value = property.value.Serializate();

            writer.Write(value.Length);
            writer.Write(value);
        }

        /// <summary>
        /// Escreve os dados de um array de tilesets num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Tileset[] tilesets)
        {
            writer.Write(tilesets.Length);

            for (int i = 0; i < tilesets.Length; i++) writer.Write(tilesets[i]);
        }
        /// <summary>
        /// Escreve os dados de um tileset num arquivo binário.
        /// </summary>
        private static void Write(this BinaryWriter writer, Tileset tileset)
        {
            writer.Write(tileset.columns);
            writer.Write(tileset.firstgid);
            writer.Write(tileset.imagewidth);
            writer.Write(tileset.imageheight);
            writer.Write(tileset.margin);
            writer.Write(tileset.spacing);
            writer.Write(tileset.tilecount);
            writer.Write(tileset.tilewidth);
            writer.Write(tileset.tileheight);

            writer.Write(tileset.image);
            writer.Write(tileset.name);
        }

        /// <summary>
        /// Transforma um <see cref="object"/> num array de bytes.
        /// </summary>
        private static byte[] Serializate(this object serialize)
        {
            MemoryStream memory = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(memory, serialize);
            memory.Close();

            return memory.ToArray();
        }
        #endregion
    }
}