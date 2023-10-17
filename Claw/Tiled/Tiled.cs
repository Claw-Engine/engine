using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
        /// <summary>
        /// Define se os objetos irão ser adicionados dentro no jogo apenas quando o parse estiver completo (falso por padrão).
        /// </summary>
        public static bool AddAllAtOnce = false;
        public static Config Config;
        private static Sprite notFoundPalette;

        private static List<GameObject> waiting;
        private static Dictionary<int, LinkObjectData> links;
        
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

            waiting = new List<GameObject>();
            links = new Dictionary<int, LinkObjectData>();
            var hasTile = LayerForeach(map.layers, tiledMap);
            links = null;

            if (AddAllAtOnce)
            {
                foreach (GameObject gameObject in waiting) Game.Instance.Components.Add(gameObject);
            }

            waiting = null;

            if (hasTile) Game.Instance.Tilemap = tiledMap;

            GameObject.InstantlyAdd = previousInstantlyAdd;
        }

        /// <summary>
        /// Retorna o valor de uma propriedade caso ela exista e seja do tipo esperado.
        /// </summary>
        private static T GetPropertyValue<T>(Property property, string type, T defaultValue)
        {
            if (property.type == type)
            {
                if (property.value is Int64 || property.value is int)
                {
                    object integger = Convert.ToInt32(property.value);

                    return (T)integger;
                }
                else return (T)property.value;
            }
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
                        foreach (Object tObject in layer.objects)
                        {
                            if (GetPropertyValue(tObject.properties, "TiledIgnore", "bool", false)) continue;
                            else if (GetPropertyValue(tObject.properties, "NotGameObject", "bool", false))
                            {
                                Config.Instantiate<object>(tObject.type);

                                continue;
                            }

                            GameObject gameObject = Config.Instantiate<GameObject>(tObject.type);
                            gameObject.Position = new Vector2(tObject.x, tObject.y);
                            gameObject.Name = tObject.name;
                            gameObject.Rotation = tObject.rotation;

                            if (!links.ContainsKey(tObject.id)) links.Add(tObject.id, new LinkObjectData() { Me = gameObject, WaitingMe = new List<Tuple<int, string>>() });
                            else
                            {
                                for (int i = links[tObject.id].WaitingMe.Count - 1; i >= 0; i--)
                                {
                                    var tuple = links[tObject.id].WaitingMe[i];

                                    SetProp(links[tuple.Item1].Me, new Property() { name = tuple.Item2, type = "object", value = gameObject });
                                    links[tObject.id].WaitingMe.RemoveAt(i);
                                }
                            }

                            foreach (Property property in tObject.properties)
                            {
                                if (property.name == "Tags")
                                {
                                    string[] tags = GetPropertyValue(tObject.properties, "Tags", "string", string.Empty).Split(',');

                                    for (int i = 0; i < tags.Length; i++) gameObject.AddTag(tags[i].ToLower());
                                }
                                else if (property.name == "Scale")
                                {
                                    if (property.type == "string")
                                    {
                                        if (property.value.ToString().Contains(','))
                                        {
                                            string[] value = property.value.ToString().Replace(" ", "").Split(',');
                                            gameObject.Scale = new Vector2(float.Parse(value[0]), float.Parse(value[1]));
                                        }
                                        else
                                        {
                                            string value = property.value.ToString().Replace(" ", "");
                                            gameObject.Scale = new Vector2(float.Parse(value));
                                        }
                                    }
                                    else if (property.type == "int" || property.type == "float") gameObject.Scale = new Vector2(Convert.ToSingle(property.value));
                                }
                                else if (property.name == "Color") gameObject.Color = new Color(property.value.ToString(), Color.HexFormat.ARGB);
                                else if (property.type == "object")
                                {
                                    int objID = Convert.ToInt32(property.value);

                                    if (links.ContainsKey(objID))
                                    {
                                        if (links[objID].Me == null) links[objID].WaitingMe.Add(new Tuple<int, string>(tObject.id, property.name));
                                        else
                                        {
                                            property.value = links[objID].Me;
                                            
                                            SetProp(gameObject, property);
                                        }
                                    }
                                    else links.Add(objID, new LinkObjectData() { Me = null, WaitingMe = new List<Tuple<int, string>>() { new Tuple<int, string>(tObject.id, property.name) } });
                                }
                                else SetProp(gameObject, property);
                            }

                            if (!AddAllAtOnce) Game.Instance.Components.Add(gameObject);
                            else waiting.Add(gameObject);
                        }
                        break;
                }
            }

            return hasTile;
        }

        /// <summary>
        /// Seta o valor da propriedade de um objeto.
        /// </summary>
        private static void SetProp(GameObject gameObject, Property property)
        {
            if (gameObject != null)
            {
                var propInfo = gameObject.GetType().GetProperty(property.name);
                
                if (propInfo != null)
                {
                    if (property.type == "color") propInfo.SetValue(gameObject, new Color(property.value.ToString(), Color.HexFormat.ARGB));
                    else if (property.type == "int") propInfo.SetValue(gameObject, Convert.ToInt32(property.value));
                    else propInfo.SetValue(gameObject, property.value);
                }
            }
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
        internal T Instantiate<T>(string type) => (T)Activator.CreateInstance(assemblyName, string.Format("{0}.{1}", prefixNamespace, type)).Unwrap();

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
    internal struct LinkObjectData
    {
        public GameObject Me;
        public List<Tuple<int, string>> WaitingMe;
    }
}