using System;
using System.Collections.Generic;
using System.Linq;
using Claw.Modules;

namespace Claw
{
    /// <summary>
    /// Controla o sistema de tags (case insensitive).
    /// </summary>
    internal static class TagManager
    {
        private static Dictionary<string, List<BaseModule>> tags = new Dictionary<string, List<BaseModule>>();

        /// <summary>
        /// Retorna o número de módulos dentro de uma tag.
        /// </summary>
        public static int Count(string tag)
        {
            tag = tag.ToLower();

            if (tags.ContainsKey(tag)) return tags[tag].Count;

            return 0;
        }

        /// <summary>
        /// Adiciona uma tag em um módulo.
        /// </summary>
        public static void AddModule(string tag, BaseModule module)
        {
            if (!tags.ContainsKey(tag)) tags.Add(tag, new List<BaseModule>());

            if (!tags[tag].Contains(module)) tags[tag].Add(module);
        }
        /// <summary>
        /// Remove uma tag de um módulo.
        /// </summary>
        public static void RemoveModule(string tag, BaseModule module)
        {
            if (tags.ContainsKey(tag) && tags[tag].Contains(module)) tags[tag].Remove(module);
        }

		/// <summary>
		/// Retorna o primeiro módulo com a determinada tag.
		/// </summary>
		public static BaseModule GetModule(string tag, bool filterEnabled)
		{
			tag = tag.ToLower();

			if (tag.Length == 0 || !tags.ContainsKey(tag) || tags[tag].Count == 0) return null;

			if (!filterEnabled) return tags[tag][0];
			else return tags[tag].FirstOrDefault(m => m.Enabled);

			return null;
		}
		/// <summary>
		/// Retorna os módulos com a determinada tag.
		/// </summary>
		public static IEnumerable<BaseModule> GetModules(string tag, bool filterEnabled)
		{
			tag = tag.ToLower();

			if (tag.Length == 0 || !tags.ContainsKey(tag)) return new List<BaseModule>();

			if (!filterEnabled) return tags[tag];

			return tags[tag].Where(m => m.Enabled);
		}
	}
}