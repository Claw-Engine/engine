using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Claw.Utils;

namespace Claw.Save
{
    /// <summary>
    /// Funções prontas para lidar com um save.
    /// </summary>
    public static class Save
    {
        private const int Amount = 10;
        private static bool useCrypt = true;
        private static string path = string.Empty;
        private static Sections save;
        private static Dictionary<ISaveValue, object> references;

        /// <summary>
        /// Abre o save.
        /// </summary>
        public static void Open(string path, bool useCrypt = true)
        {
            Save.path = path;
            Save.useCrypt = useCrypt;
            references = new Dictionary<ISaveValue, object>();
            string content = string.Empty;

            if (!File.Exists(path)) File.Create(path).Close();
            else content = File.ReadAllText(path);

            if (content.Length > 0)
            {
                if (useCrypt) content = StringCrypt.AllCrypt(content, false, Amount);

                save = SaveConvert.Deserialize(content);
            }

            if (save == null) save = new Sections();
        }

        /// <summary>
        /// Escreve numa sessão do save.
        /// </summary>
        public static void Write(string section, string key, object value)
        {
            if (save != null)
            {
                if (!SectionExists(section)) save.Add(section, new Keys());

                if (!save[section].Keys.Contains(key)) save[section].Add(key, value);
                else save[section][key] = value;
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
                else if (!save[section].Keys.Contains(key)) return defaultValue;

                Type type = typeof(T);

                if (save[section][key] is ISaveValue) return (T)((ISaveValue)save[section][key]).Cast(type, references);
                else if (type.IsEnum) return (T)Enum.Parse(type, save[section][key].ToString());

                return (T)save[section][key];
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
                File.WriteAllText(path, string.Empty);

                save = null;
                path = string.Empty;
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
            string content = SaveConvert.Serialize(save);

            if (useCrypt) content = StringCrypt.AllCrypt(content, true, Amount);

            if (path.Length > 0) File.WriteAllText(path, content);

            save = null;
            references = null;
            path = string.Empty;
        }
    }
}