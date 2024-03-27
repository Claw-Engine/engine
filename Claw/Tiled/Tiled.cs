using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Globalization;
using Claw.Maps;
using Claw.Graphics;
using Claw.Extensions;
using Claw.Modules;
using Claw.Physics;

namespace Claw.Tiled
{
    /// <summary>
    /// Uma classe para carregar mapas do Tiled.
    /// </summary>
    public static class Tiled
    {
        public static Config Config;
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private static List<BaseModule> waiting;
        private static Dictionary<int, LinkObjectData> links;
        private static Dictionary<Type, Dictionary<string, (PropertySetter, Type)>> reflectionCache = new Dictionary<Type, Dictionary<string, (PropertySetter, Type)>>();
        private static Dictionary<string, Type> mapTypes = new Dictionary<string, Type>()
        {
            { "orthogonal", typeof(OrthogonalTilemap) },
            { "isometric", typeof(IsometricTilemap) },
            { "staggered", typeof(StaggeredTilemap) },
            { "hexagonal", typeof(HexagonalTilemap) }
        };

        /// <summary>
        /// Altera o tipo que será usado para instanciar um mapa com determinada orientação.
        /// </summary>
        public static void SetType<T>(string orientation) where T : Tilemap => mapTypes[orientation] = typeof(T);

        /// <summary>
        /// Carrega um mapa do Tiled.
        /// </summary>
        public static void Load(Map map, bool runDestroy = false, bool deleteAll = true)
        {
            if (Config == null) throw new Exception("\"Config\" não pode ser nulo!");

            SceneManager.ClearScene(runDestroy, deleteAll);

            Tilemap tiledMap = (Tilemap)Activator.CreateInstance(mapTypes[map.orientation], new Vector2(map.width, map.height), new Vector2(map.tilewidth, map.tileheight));

            if (map.tilesets != null && map.tilesets.Length > 0)
            {
                var tilesets = map.tilesets.OrderBy(t => t.firstgid).ToArray();

                foreach (Tileset tileset in map.tilesets) tiledMap.AddPalette(Config.GetPalette(tileset.name), new Vector2(tileset.tilewidth, tileset.tileheight), tileset.margin, tileset.spacing);
            }

            if (map.layers == null || map.layers.Length == 0) return;

            waiting = new List<BaseModule>();
            links = new Dictionary<int, LinkObjectData>();
            var hasTile = LayerForeach(map.layers, tiledMap);
            links = null;

            if (hasTile) Game.Instance.Tilemap = tiledMap;

            for (int i = 0; i < waiting.Count; i++) Game.Instance.Modules.Add(waiting[i]);

            waiting = null;
        }

        /// <summary>
        /// Limpa o cache para reflection.
        /// </summary>
        public static void ClearCache() => reflectionCache.Clear();

        /// <summary>
        /// Retorna o valor de uma propriedade caso ela exista e seja do tipo esperado.
        /// </summary>
        private static T GetPropertyValue<T>(Property property, string type, T defaultValue)
        {
            if (property.type == type) return (T)Cast(property, typeof(T));
            else return defaultValue;
        }
        /// <summary>
        /// Retorna o valor da primeira propriedade com o nome dado caso ela exista e seja do tipo esperado.
        /// </summary>
        private static T GetPropertyValue<T>(Property[] properties, string name, string type, T defaultValue)
        {
            if (properties == null) return defaultValue;

            var property = properties.FirstOrDefault(p => p.name == name);

            if (property == null) return defaultValue;

            return GetPropertyValue(property, type, defaultValue);
        }

        /// <summary>
        /// Transforma as layers de tiles em <see cref="Layer"/> e os objetos em <see cref="BaseModule"/>.
        /// </summary>
        private static bool LayerForeach(Layer[] layers, Tilemap tiledMap)
        {
            bool hasTile = false;
            
            foreach (Layer layer in layers)
            {
                if (GetPropertyValue(layer.properties, "TiledIgnore", "bool", false)) continue;

                switch (layer.type)
                {
                    case "tilelayer"://Layer de tiles
                        hasTile = true;
                        var renderOrder = GetPropertyValue(layer.properties, "RenderOrder", "int", tiledMap.LayerCount);
                        
                        if (layer.chunks.Length == 0) tiledMap.AddLayer(renderOrder, layer.name, layer.visible, layer.opacity, new Color(layer.tintcolor), layer.data);
                        else
                        {
                            var size = tiledMap.Size;

                            if (layer.width > size.X) size.X = layer.width;

                            if (layer.height > size.Y) size.Y = layer.height;

                            if (size != tiledMap.Size) tiledMap.Size = size;

                            var index = tiledMap.AddLayer(renderOrder, layer.name, layer.visible, layer.opacity, new Color(layer.tintcolor), new int[(int)(tiledMap.Size.X * tiledMap.Size.Y)]);

                            foreach (Chunk chunk in layer.chunks) tiledMap[index].SetChunkTiles(new Rectangle(chunk.x, chunk.y, chunk.width, chunk.height), chunk.data);
                        }
                        break;
                    case "group"://Pasta
                        bool groupHasTile = LayerForeach(layer.layers, tiledMap);
                        hasTile = hasTile || groupHasTile;
                        break;
                    case "objectgroup"://Layer de objetos
                        foreach (Object tObject in layer.objects) HandleObject(tObject);
                        break;
                }
            }

            return hasTile;
        }

        /// <summary>
        /// Lida com a conversão de um objeto do Tiled.
        /// </summary>
        private static void HandleObject(Object tObject)
        {
            if (GetPropertyValue(tObject.properties, "TiledIgnore", "bool", false)) return;

            object obj = Config.Instantiate(tObject.type);
            Type type = obj.GetType();

            if (!reflectionCache.TryGetValue(type, out Dictionary<string, (PropertySetter, Type)> properties)) properties = SetupCache(type);

            if (!links.ContainsKey(tObject.id)) links.Add(tObject.id, new LinkObjectData() { Me = obj, WaitingMe = new List<(int, string)>() });
            else
            {
                for (int i = links[tObject.id].WaitingMe.Count - 1; i >= 0; i--)
                {
                    var tuple = links[tObject.id].WaitingMe[i];
                    object waiting = links[tuple.objId].Me;

                    SetProperty(waiting, tObject, new Property() { name = tuple.propertyName, type = "instance", value = obj }, reflectionCache[waiting.GetType()]);
                    links[tObject.id].WaitingMe.RemoveAt(i);
                }
            }

            if (obj is BaseModule module)
            {
				module.Transform.Position = new Vector2(tObject.x, tObject.y);
				module.Name = tObject.name;
				module.Transform.Rotation = tObject.rotation;

                foreach (Property property in tObject.properties)
                {
                    if (property.name == "Tags")
                    {
                        string[] tags = GetPropertyValue(property, "string", string.Empty).Split(',');

                        for (int i = 0; i < tags.Length; i++) module.AddTag(tags[i]);
                    }
                    else SetProperty(module, tObject, property, properties);
                }

                if (!tObject.point && !GetPropertyValue(tObject.properties, "IgnoreBody", "bool", false) && module is RigidBody body)
                {
                    if (tObject.polygon.Length > 0)
                    {
                        Vector2[] vertices = new Vector2[tObject.polygon.Length];

                        for (int i = 0; i < vertices.Length; i++) vertices[i] = new Vector2(tObject.polygon[i].x, tObject.polygon[i].y);

                        body.Shape = new PolygonShape(Vector2.Zero, vertices);
                    }
                    else if (tObject.ellipse)
                    {
						if (tObject.width == tObject.height) body.Shape = new CircleShape(tObject.width * .5f, new Vector2(tObject.width * .5f));
                        else
                        {
                            int segments = GetPropertyValue(tObject.properties, "Segments", "int", 32);
                            Vector2 radius = new Vector2(tObject.width * .5f, tObject.height * .5f);
							Vector2[] vertices = new Vector2[segments];
							double increment = Math.PI * 2 / segments, theta = 0;
                            
							for (int i = 0; i < segments; i++)
							{
								vertices[i] = radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
								theta += increment;
							}

							body.Shape = new PolygonShape(radius, vertices);
						}
                    }
                    else body.Shape = new PolygonShape(Vector2.Zero, new Vector2(tObject.width, tObject.height));
                }

				waiting.Add(module);
			}
            else for (int i = 0; i < tObject.properties.Length; i++) SetProperty(obj, tObject, tObject.properties[i], properties);
		}

        /// <summary>
        /// Seta o valor da propriedade de um objeto.
        /// </summary>
        private static void SetProperty(object @object, Object tObject, Property property, Dictionary<string, (PropertySetter, Type)> properties)
        {
            if (@object != null)
            {
                if (properties.TryGetValue(property.name, out (PropertySetter SetValue, Type Type) propertyInfo))
                {
                    if (property.type == "object")
                    {
                        int objId = Convert.ToInt32(property.value);

                        if (links.TryGetValue(objId, out LinkObjectData reference))
                        {
                            if (reference.Me == null) reference.WaitingMe.Add((tObject.id, property.name));
                            else propertyInfo.SetValue(@object, reference.Me);
                        }
                        else links.Add(objId, new LinkObjectData() { Me = null, WaitingMe = new List<(int, string)>() { (tObject.id, property.name) } });
                    }
                    else if (property.type == "instance") propertyInfo.SetValue(@object, property.value);
                    else propertyInfo.SetValue(@object, Cast(property, propertyInfo.Type));
                }
            }
        }

        /// <summary>
        /// Prepara as propriedades de um tipo para o cache.
        /// </summary>
        private static Dictionary<string, (PropertySetter, Type)> SetupCache(Type type)
        {
            Dictionary<string, (PropertySetter, Type)> setters = new Dictionary<string, (PropertySetter, Type)>();
            PropertyInfo[] properties = type.GetProperties(Flags);
            FieldInfo[] fields = type.GetFields(Flags);

            reflectionCache.Add(type, setters);

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].SetMethod != null) setters.Add(properties[i].Name, (properties[i].SetValue, properties[i].PropertyType));
            }

            for (int i = 0; i < fields.Length; i++) setters.Add(fields[i].Name, (fields[i].SetValue, fields[i].FieldType));

            return setters;
        }

        /// <summary>
        /// Realiza a conversão dos valores de propriedade, se necessário.
        /// </summary>
        private static object Cast(Property property, Type type)
        {
            switch (property.type)
            {
                case "int":
                    if (type.IsEnum) return Enum.ToObject(type, Convert.ToInt32(property.value));
                    else if (type == typeof(Color)) return new Color(Convert.ToUInt32(property.value));
                    else if (type == typeof(Vector2)) return new Vector2(Convert.ToSingle(property.value));
                    else if (type == typeof(Vector3)) return new Vector3(Convert.ToSingle(property.value));
                    else if (type == typeof(Quaternion)) return new Quaternion(Convert.ToSingle(property.value));

                    return Convert.ToInt32(property.value);
                    break;
                case "string":
                    string text = property.value.ToString();

                    if (type.IsEnum) return Enum.Parse(type, text);
                    else if (type != typeof(string))
                    {
                        Quaternion result = StringToQuaternion(text);

                        if (type == typeof(Vector2)) return new Vector2(result.X, result.Y);
                        else if (type == typeof(Vector3)) return new Vector3(result.X, result.Y, result.Z);
                        else if (type == typeof(Quaternion)) return result;
                        else if (type == typeof(Line)) return new Line(result.X, result.Y, result.Z, result.W);
                        else if (type == typeof(Rectangle)) return new Rectangle(result.X, result.Y, result.Z, result.W);
                        else if (type == typeof(Color))
                        {
                            if (text.Contains('.')) return new Color(result.X, result.Y, result.Z, result.W);

                            return new Color((int)result.X, (int)result.Y, (int)result.Z, (int)result.W);
                        }
                    }

                    return text;
                    break;
                case "color": return new Color(property.value.ToString(), Color.HexFormat.ARGB);
                case "float":
                    if (type == typeof(Vector2)) return new Vector2(Convert.ToSingle(property.value));
                    else if (type == typeof(Vector3)) return new Vector3(Convert.ToSingle(property.value));
                    else if (type == typeof(Quaternion)) return new Quaternion(Convert.ToSingle(property.value));
                    else if (type == typeof(Color))
                    {
                        float value = Convert.ToSingle(property.value);

                        return new Color(value, value, value, value);
                    }

                    return property.value;
                    break;
                default: return property.value;
            }
        }
        /// <summary>
        /// Transforma uma string em um <see cref="Quaternion"/>.
        /// </summary>
        private static Quaternion StringToQuaternion(string quaternion)
        {
            Quaternion result = new Quaternion();

            if (quaternion.Contains(','))
            {
                string[] value = quaternion.Replace(" ", "").Split(',');

                if (value.Length >= 1) result.X = float.Parse(value[0]);

                if (value.Length >= 2) result.X = float.Parse(value[1]);

                if (value.Length >= 3) result.X = float.Parse(value[2]);

                if (value.Length >= 4) result.X = float.Parse(value[3]);
            }
            else
            {
                string value = quaternion.Replace(" ", "");

                result = new Quaternion(float.Parse(value));
            }

            return result;
        }

        /// <summary>
        /// Dados para lista de espera para objetos linkados à outros.
        /// </summary>
        private class LinkObjectData
        {
            public object Me;
            public List<(int objId, string propertyName)> WaitingMe;
        }
    }
}