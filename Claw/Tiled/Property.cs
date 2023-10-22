using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Claw.Tiled
{
    /// <summary>
    /// Representa uma propriedade do Tiled.
    /// </summary>
    public sealed class Property
    {
        public string name = string.Empty, type = string.Empty;
        public object value = null;

        /// <summary>
        /// Carrega uma propriedade do Tiled.
        /// </summary>
        internal static Property ReadProperty(BinaryReader reader)
        {
            Property property = new Property();

            property.name = reader.ReadString();
            property.type = reader.ReadString();

            switch (property.type)
            {
                case "bool": property.value = reader.ReadBoolean(); break;
                case "color": case "string":
                    property.value = reader.ReadString();
                    break;
                case "float": property.value = reader.ReadSingle(); break;
                case "int": case "object":
                    property.value = reader.ReadInt64();
                    break;
            }

            return property;
        }
        /// <summary>
        /// Carrega um array de propriedades do Tiled.
        /// </summary>
        internal static Property[] ReadProperties(BinaryReader reader)
        {
            int propertyCount = reader.ReadInt32();
            Property[] properties = new Property[propertyCount];

            for (int i = 0; i < propertyCount; i++) properties[i] = ReadProperty(reader);

            return properties;
        }
    }
}