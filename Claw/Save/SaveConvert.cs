using System;

namespace Claw.Save
{
    /// <summary>
    /// Ponte para serialização e desserialização de saves.
    /// </summary>
    internal static class SaveConvert
    {
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
    }
}