using System;
using System.Collections.Generic;
using System.Linq;

namespace Claw
{
    /// <summary>
    /// Controla o sistema de tags (case insensitive).
    /// </summary>
    internal static class TagManager
    {
        private static Dictionary<string, List<GameObject>> tags = new Dictionary<string, List<GameObject>>();

        /// <summary>
        /// Retorna o número de objetos dentro de uma tag.
        /// </summary>
        public static int Count(string tag)
        {
            tag = tag.ToLower();

            if (tags.ContainsKey(tag)) return tags[tag].Count;

            return 0;
        }

        /// <summary>
        /// Adiciona uma tag em um objeto.
        /// </summary>
        public static void AddObject(string tag, GameObject gameObject)
        {
            if (!tags.ContainsKey(tag)) tags.Add(tag, new List<GameObject>());

            if (!tags[tag].Contains(gameObject)) tags[tag].Add(gameObject);
        }
        /// <summary>
        /// Remove uma tag de um objeto.
        /// </summary>
        public static void RemoveObject(string tag, GameObject gameObject)
        {
            if (tags.ContainsKey(tag) && tags[tag].Contains(gameObject)) tags[tag].Remove(gameObject);
        }

        /// <summary>
        /// Retorna o primeiro objeto com a determinada tag.
        /// </summary>
        public static GameObject GetObject(string tag, bool filterEnabled)
        {
            tag = tag.ToLower();

            if (tag.Length == 0 || !tags.ContainsKey(tag) || tags[tag].Count == 0) return null;

            if (!filterEnabled) return tags[tag][0];
            else return tags[tag].FirstOrDefault(gO => gO.Enabled);

            return null;
        }
        /// <summary>
        /// Retorna os objetos com a determinada tag.
        /// </summary>
        public static IEnumerable<GameObject> GetObjects(string tag, bool filterEnabled)
        {
            tag = tag.ToLower();

            if (tag.Length == 0 || !tags.ContainsKey(tag)) return new List<GameObject>();

            if (!filterEnabled) return tags[tag];

            return tags[tag].Where(gO => gO.Enabled);
        }
    }
}