using System;
using System.Collections.Generic;
using System.Linq;

namespace Claw.Extensions
{
    /// <summary>
    /// Extensões para coleções.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Retorna um texto com todas as linhas da coleção.
        /// </summary>
        public static string ToText(this IEnumerable<string> lines)
        {
            string[] arrayLines = lines.ToArray();
            string text = string.Empty;

            for (int i = 0; i < arrayLines.Length; i++)
            {
                if (i != 0) text += new[] { Environment.NewLine } + arrayLines[i];
                else text += arrayLines[i];
            }

            return text;
        }

        /// <summary>
        /// Checa se a coleção tem pelo menos X itens.
        /// </summary>
        public static bool HasMin<T>(this IEnumerable<T> enumerable, int count) => enumerable != null && enumerable.Count() >= count;
        /// <summary>
        /// Checa se a coleção tem exatamente X itens.
        /// </summary>
        public static bool Has<T>(this IEnumerable<T> enumerable, int count) => enumerable != null && enumerable.Count() == count;
        /// <summary>
        /// Checa se a coleção tem entre X e Y itens.
        /// </summary>
        public static bool HasMinMax<T>(this IEnumerable<T> enumerable, int min, int max) => enumerable != null && (enumerable.Count() >= min || enumerable.Count() <= max);
        /// <summary>
        /// Checa se a coleção tem no máximo X itens.
        /// </summary>
        public static bool HasMax<T>(this IEnumerable<T> enumerable, int count) => enumerable != null && enumerable.Count() <= count;
        /// <summary>
        /// Checa se a coleção é nula ou está vazia.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || enumerable.Count() == 0;

        /// <summary>
        /// Troca dois elementos de posição em um array.
        /// </summary>
        public static void Swap<T>(this T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }
        /// <summary>
        /// Troca dois elementos de posição em uma lista.
        /// </summary>
        public static void Swap<T>(this List<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }
        /// <summary>
        /// Troca dois elementos de posição em um dicionário.
        /// </summary>
        public static void Swap<T1, T2>(this Dictionary<T1, T2> map, T1 index1, T1 index2)
        {
            T2 temp = map[index1];
            map[index1] = map[index2];
            map[index2] = temp;
        }

        /// <summary>
        /// Pega um valor no index, se ele existir. Senão, retorna um valor padrão.
        /// </summary>
        public static T2 Get<T1, T2>(this Dictionary<T1, T2> map, T1 index, T2 defaultValue)
        {
            if (map.ContainsKey(index)) return map[index];

            return defaultValue;
        }
        /// <summary>
        /// Adiciona um index, se ele não existir. Em seguinda, seta o seu valor.
        /// </summary>
        public static void Set<T1, T2>(this Dictionary<T1, T2> map, T1 index, T2 value)
        {
            if (map.ContainsKey(index)) map[index] = value;
            else map.Add(index, value);
        }
    }
}