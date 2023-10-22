using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Globalization;
using Claw.Graphics;
using Claw.Extensions;

namespace Claw.Tiled
{
    /// <summary>
    /// Uma classe para carregar mapas do Tiled.
    /// </summary>
    public static class Tiled
    {
        public static Config Config;
        private static Sprite notFoundPalette;

        private static List<IGameComponent> waiting;
        private static Dictionary<int, LinkObjectData> links;
        private static Dictionary<Type, Dictionary<string, PropertyInfo>> reflectionCache;

        /// <summary>
        /// Carrega um mapa do Tiled.
        /// </summary>
        public static void Load(Map map, bool runDestroy = false, bool deleteAll = true)
        {
            if (Config == null) throw new Exception("\"Config\" não pode ser nulo!");
            else if (notFoundPalette == null) notFoundPalette = new Sprite(Texture.Pixel, "_pixel_", new Rectangle(0, 0, 1, 1));

            SceneManager.ClearScene(runDestroy, deleteAll);

            bool previousInstantlyAdd = GameObject.InstantlyAdd;
            GameObject.InstantlyAdd = false;
            Tilemap tiledMap = new Tilemap();
            tiledMap.Size = new Vector2(map.width, map.height);
            tiledMap.GridSize = new Vector2(map.tilewidth, map.tileheight);

            if (map.tilesets != null && map.tilesets.Length > 0)
            {
                var tilesets = map.tilesets.OrderBy(t => t.firstgid).ToArray();

                foreach (Tileset tileset in map.tilesets) tiledMap.AddPalette(Config.GetPalette(tileset.name) ?? notFoundPalette, tileset.margin, tileset.spacing);
            }

            if (map.layers == null || map.layers.Length == 0) return;

            waiting = new List<IGameComponent>();
            links = new Dictionary<int, LinkObjectData>();
            reflectionCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            var hasTile = LayerForeach(map.layers, tiledMap);
            links = null;
            reflectionCache = null;

            foreach (IGameComponent obj in waiting) Game.Instance.Components.Add(obj);

            waiting = null;

            if (hasTile) Game.Instance.Tilemap = tiledMap;

            GameObject.InstantlyAdd = previousInstantlyAdd;
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
        /// Transforma as layers de tiles em <see cref="Layer"/> e os objetos em <see cref="GameObject"/>.
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
                        var drawOrder = GetPropertyValue(layer.properties, "DrawOrder", "int", tiledMap.LayerCount);
                        var priority = Mathf.Clamp(GetPropertyValue(layer.properties, "Priority", "float", 0), 0, 1);

                        if (layer.chunks.Length == 0) tiledMap.AddLayer(drawOrder, layer.name, layer.visible, priority, layer.opacity, new Color(layer.tintcolor), layer.data);
                        else
                        {
                            var size = tiledMap.Size;

                            if (layer.width > size.X) size.X = layer.width;

                            if (layer.height > size.Y) size.Y = layer.height;

                            if (size != tiledMap.Size) tiledMap.Size = size;

                            var index = tiledMap.AddLayer(drawOrder, layer.name, layer.visible, priority, layer.opacity, new Color(layer.tintcolor), new int[(int)(tiledMap.Size.X * tiledMap.Size.Y)]);

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

            if (!reflectionCache.TryGetValue(type, out Dictionary<string, PropertyInfo> properties)) properties = SetupCache(type);

            if (!links.ContainsKey(tObject.id)) links.Add(tObject.id, new LinkObjectData() { Me = obj, WaitingMe = new List<Tuple<int, string>>() });
            else
            {
                for (int i = links[tObject.id].WaitingMe.Count - 1; i >= 0; i--)
                {
                    var tuple = links[tObject.id].WaitingMe[i];
                    object waiting = links[tuple.Item1].Me;

                    SetProperty(waiting, tObject, new Property() { name = tuple.Item2, type = "instance", value = obj }, reflectionCache[waiting.GetType()]);
                    links[tObject.id].WaitingMe.RemoveAt(i);
                }
            }

            if (obj is GameObject gameObject)
            {
                gameObject.Position = new Vector2(tObject.x, tObject.y);
                gameObject.Name = tObject.name;
                gameObject.Rotation = tObject.rotation;

                foreach (Property property in tObject.properties)
                {
                    if (property.name == "Tags")
                    {
                        string[] tags = GetPropertyValue(tObject.properties, "Tags", "string", string.Empty).Split(',');

                        for (int i = 0; i < tags.Length; i++) gameObject.AddTag(tags[i]);
                    }
                    else if (property.name == "Color") gameObject.Color = new Color(property.value.ToString(), Color.HexFormat.ARGB);
                    else SetProperty(gameObject, tObject, property, properties);
                }
            }
            else
            {
                foreach (Property property in tObject.properties) SetProperty(obj, tObject, property, properties);
            }

            if (obj is IGameComponent component) waiting.Add(component);
        }

        /// <summary>
        /// Seta o valor da propriedade de um objeto.
        /// </summary>
        private static void SetProperty(object @object, Object tObject, Property property, Dictionary<string, PropertyInfo> properties)
        {
            if (@object != null)
            {
                if (properties.TryGetValue(property.name, out PropertyInfo propertyInfo))
                {
                    if (property.type == "object")
                    {
                        int objId = Convert.ToInt32(property.value);

                        if (links.TryGetValue(objId, out LinkObjectData reference))
                        {
                            if (reference.Me == null) reference.WaitingMe.Add(new Tuple<int, string>(tObject.id, property.name));
                            else propertyInfo.SetValue(@object, reference.Me);
                        }
                        else links.Add(objId, new LinkObjectData() { Me = null, WaitingMe = new List<Tuple<int, string>>() { new Tuple<int, string>(tObject.id, property.name) } });
                    }
                    else if (property.type == "instance") propertyInfo.SetValue(@object, property.value);
                    else propertyInfo.SetValue(@object, Cast(property, propertyInfo.PropertyType));
                }
            }
        }

        /// <summary>
        /// Prepara as propriedades de um tipo para o cache.
        /// </summary>
        private static Dictionary<string, PropertyInfo> SetupCache(Type type)
        {
            Dictionary<string, PropertyInfo> setters = new Dictionary<string, PropertyInfo>();
            PropertyInfo[] properties = type.GetProperties();

            reflectionCache.Add(type, setters);

            for (int i = 0; i < properties.Length; i++) setters.Add(properties[i].Name, properties[i]);

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
                    if (type == typeof(Color)) return new Color(Convert.ToUInt32(property.value));
                    else if (type == typeof(Vector2)) return new Vector2(Convert.ToSingle(property.value));
                    else if (type == typeof(Vector3)) return new Vector3(Convert.ToSingle(property.value));
                    else if (type == typeof(Quaternion)) return new Quaternion(Convert.ToSingle(property.value));

                    return Convert.ToInt32(property.value);
                    break;
                case "string":
                    if (type == typeof(Sprite)) return TextureAtlas.Sprites[property.value.ToString()];
                    else if (type != typeof(string))
                    {
                        Quaternion result = StringToQuaternion(property.value.ToString());

                        if (type == typeof(Vector2)) return new Vector2(result.X, result.Y);
                        else if (type == typeof(Vector3)) return new Vector3(result.X, result.Y, result.Z);
                        else if (type == typeof(Quaternion)) return result;
                    }

                    return property.value;
                    break;
                case "color": return new Color(property.value.ToString(), Color.HexFormat.ARGB);
                case "float":
                    if (type == typeof(Vector2)) return new Vector2(Convert.ToSingle(property.value));
                    else if (type == typeof(Vector3)) return new Vector3(Convert.ToSingle(property.value));
                    else if (type == typeof(Quaternion)) return new Quaternion(Convert.ToSingle(property.value));

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
    }
    /// <summary>
    /// Configurações para o <see cref="Tiled"/>.
    /// </summary>
    public sealed class Config
    {
        private string assemblyName, prefixNamespace;
        private Dictionary<string, Sprite> palettes = new Dictionary<string, Sprite>();

        public Config(string prefixNamespace)
        {
            this.prefixNamespace = prefixNamespace;
            assemblyName = Game.Instance.GetType().Assembly.FullName;
        }

        /// <summary>
        /// Adiciona as paletas do Tiled.
        /// </summary>
        public void AddPalettes(string[] palettes, Sprite[] palettesTexture)
        {
            for (int i = 0; i < palettes.Length; i++) this.palettes.Add(palettes[i], palettesTexture[i]);
        }

        /// <summary>
        /// Cria uma instância, com base no namespace.
        /// </summary>
        internal object Instantiate(string type) => Activator.CreateInstance(assemblyName, string.Format("{0}.{1}", prefixNamespace, type)).Unwrap();

        /// <summary>
        /// Checa se uma paleta existe no <see cref="palettes"/>.
        /// </summary>
        internal bool FindPalette(string tileset)
        {
            KeyValuePair<string, Sprite> keyValuePair = palettes.FirstOrDefault(a => a.Key == tileset);

            return !keyValuePair.Equals(default(KeyValuePair<string, Sprite>));
        }
        /// <summary>
        /// Retorna uma paleta com o nome dado caso ela exista. Caso contrário ele retornará a textura padrão.
        /// </summary>
        internal Sprite GetPalette(string tileset)
        {
            if (FindPalette(tileset)) return palettes[tileset];

            return TextureAtlas.Sprites.Count > 0 ? TextureAtlas.Sprites.First().Value : null;
        }
    }
    /// <summary>
    /// Dados para lista de espera para objetos linkados à outros.
    /// </summary>
    internal class LinkObjectData
    {
        public object Me;
        public List<Tuple<int, string>> WaitingMe;
    }
}