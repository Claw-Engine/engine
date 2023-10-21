using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Claw.Extensions;

namespace Claw.Save
{
    /// <summary>
    /// Ponte para serialização e desserialização de saves.
    /// </summary>
    internal static class SaveConvert
    {
        public const string Null = "NULL";
        public const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        /// <summary>
        /// Serializa o save.
        /// </summary>
        public static string Serialize(Sections sections)
        {
            SaveWriter writer = new SaveWriter();

            writer.Write(sections);

            return writer.Close();
        }
        /// <summary>
        /// Desserializa o save.
        /// </summary>
        public static Sections Deserialize(string content)
        {
            Sections save = new Sections();
            SaveReader reader = new SaveReader();

            reader.Read(content, save);

            return save;
        }

        /// <summary>
        /// Diz se o caractere é vazio.
        /// </summary>
        public static bool IsEmpty(char @char) => @char == ' ' || @char == '\n' || @char == '\r' || @char == '\t';

        /// <summary>
        /// Diz se o tipo passa por referência.
        /// </summary>
        public static bool PassByReference(this Type type) => (type.IsClass || type.IsArray) && type != typeof(string);
        /// <summary>
        /// Diz se o tipo é um objeto.
        /// </summary>
        public static bool IsObject(this Type type) => (type.IsClass || type.IsValueType) && !type.IsPrimitive && type != typeof(double) && type != typeof(string) && !type.IsArray && type.GetInterface("IEnumerable") == null;
    }
}