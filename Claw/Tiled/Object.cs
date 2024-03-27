using System;
using System.IO;

namespace Claw.Tiled
{
    /// <summary>
    /// Representa um objeto do Tiled.
    /// </summary>
    public sealed class Object
    {
        public int id = 0;
        public bool visible = true, point = false, ellipse = false;
        public float x = 0, y = 0, rotation = 0, width = 0, height = 0;
        public string name = string.Empty, type = string.Empty;
        public Property[] properties = null;
        public Vertice[] polygon = null;

		/// <summary>
		/// Carrega um objeto do Tiled.
		/// </summary>
		internal static Object ReadObject(BinaryReader reader)
        {
            Object obj = new Object();

            obj.id = reader.ReadInt32();

            obj.visible = reader.ReadBoolean();
			obj.point = reader.ReadBoolean();
			obj.ellipse = reader.ReadBoolean();

			obj.x = reader.ReadSingle();
            obj.y = reader.ReadSingle();
            obj.rotation = reader.ReadSingle();
			obj.width = reader.ReadSingle();
			obj.height = reader.ReadSingle();

			obj.name = reader.ReadString();
            obj.type = reader.ReadString();

            obj.properties = Property.ReadProperties(reader);
            obj.polygon = Vertice.ReadPolygon(reader);

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