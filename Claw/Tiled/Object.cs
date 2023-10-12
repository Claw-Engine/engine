using System.IO;

namespace Claw.Tiled
{
    /// <summary>
    /// Representa um objeto do Tiled.
    /// </summary>
    public sealed class Object
    {
        public int id = 0, width = 0, height = 0;
        public bool visible = true;
        public float x = 0, y = 0, rotation = 0;
        public string name = string.Empty, type = string.Empty;
        public Property[] properties = new Property[0];

        /// <summary>
        /// Carrega um objeto do Tiled.
        /// </summary>
        internal static Object ReadObject(BinaryReader reader)
        {
            Object obj = new Object();

            obj.id = reader.ReadInt32();
            obj.width = reader.ReadInt32();
            obj.height = reader.ReadInt32();

            obj.visible = reader.ReadBoolean();

            obj.x = reader.ReadSingle();
            obj.y = reader.ReadSingle();
            obj.rotation = reader.ReadSingle();

            obj.name = reader.ReadString();
            obj.type = reader.ReadString();

            obj.properties = Property.ReadProperties(reader);

            return obj;
        }
        /// <summary>
        /// Carrega um array de objetos do Tiled.
        /// </summary>
        internal static Object[] ReadObjects(BinaryReader reader)
        {
            int objectCount = reader.ReadInt32();
            Object[] objects = new Object[objectCount];

            for (int i = 0; i < objectCount; i++) objects[i] = ReadObject(reader);

            return objects;
        }
    }
}