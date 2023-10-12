using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Claw.Utils;

namespace Claw
{
    /// <summary>
    /// Funções prontas para lidar com um save.
    /// </summary>
    public static class Save
    {
        /// <summary>
        /// Configurações usadas para ler e escrever os arquivos json.
        /// </summary>
        public static JsonSerializerSettings Settings { get; private set; } = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Formatting = Formatting.None
        };
        private const int amount = 10;
        private static bool useCrypt = true;
        private static string path = string.Empty;
        private static Dictionary<string, Dictionary<string, object>> save;

        /// <summary>
        /// Abre o save.
        /// </summary>
        public static void Open(string path, bool useCrypt = true)
        {
            Save.path = path;
            Save.useCrypt = useCrypt;
            string content = string.Empty;

            if (!File.Exists(path)) File.Create(path).Close();
            else content = File.ReadAllText(path);

            if (content.Length > 0)
            {
                if (useCrypt) content = StringCrypt.AllCrypt(content, false, amount);

                save = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(content, Settings);
            }

            if (save == null) save = new Dictionary<string, Dictionary<string, object>>();
        }

        /// <summary>
        /// Escreve numa sessão do save.
        /// </summary>
        public static void Write(string section, string key, object value)
        {
            if (save != null)
            {
                if (!SectionExists(section)) save.Add(section, new Dictionary<string, object>());

                if (!KeyExists(section, key)) save[section].Add(key, null);

                save[section][key] = value;
            }
            else throw new Exception("O save não está aberto!");
        }

        /// <summary>
        /// Lê uma chave de uma sessão do save.
        /// </summary>
        public static T Read<T>(string section, string key, T defaultValue)
        {
            if (save != null)
            {
                if (!SectionExists(section)) return defaultValue;
                else if (!KeyExists(section, key)) return defaultValue;
                
                if (typeof(T) == typeof(int) && save[section][key].GetType() == typeof(Int64))
                {
                    object integger = Convert.ToInt32(save[section][key]);

                    return (T)integger;
                }
                else if (typeof(T) == typeof(int[]) && save[section][key].GetType() == typeof(Int64[]))
                {
                    object array = (int[])save[section][key];

                    return (T)array;
                }

                return save[section][key] is JObject ? ((JObject)save[section][key]).ToObject<T>() : (T)save[section][key];
            }
            else throw new Exception("O save não está aberto!");
        }

        /// <summary>
        /// Limpa o save.
        /// </summary>
        public static void Clear()
        {
            if (path.Length > 0)
            {
                save.Clear();
                File.Delete(path);
                FileStream f = File.Create(path);
                f.Close();
            }
            else throw new Exception("O save não está aberto!");
        }

        /// <summary>
        /// Remove uma sessão inteira do save.
        /// </summary>
        public static void RemoveSection(string section)
        {
            if (save != null) save.Remove(section);
            else throw new Exception("O save não está aberto!");
        }

        /// <summary>
        /// Remove uma chave de uma sessão do save.
        /// </summary>
        public static void RemoveKey(string section, string key)
        {
            if (save != null) save[section].Remove(key);
            else throw new Exception("O save não está aberto!");
        }

        /// <summary>
        /// Verifica se a sessão existe.
        /// </summary>
        public static bool SectionExists(string section) => save.Keys.Contains(section);

        /// <summary>
        /// Verifica se a chave existe.
        /// </summary>
        public static bool KeyExists(string section, string key)
        {
            if (!SectionExists(section)) return false;
            else return save[section].Keys.Contains(key);
        }

        /// <summary>
        /// Fecha o save.
        /// </summary>
        public static void Close()
        {
            string filePath = path;
            string json = CloseJson();

            if (filePath.Length > 0) File.WriteAllText(filePath, json);
        }
        /// <summary>
        /// Fecha o save, retornando um json.
        /// </summary>
        private static string CloseJson()
        {
            path = string.Empty;

            if (save != null)
            {
                string json = JsonConvert.SerializeObject(save, Settings);

                if (useCrypt) json = StringCrypt.AllCrypt(json, true, amount);
                
                save = null;

                return json;
            }

            return string.Empty;
        }
    }
}