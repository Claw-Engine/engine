using System;

namespace Claw.Save
{
    /// <summary>
    /// Responsável pela desserialização de saves.
    /// </summary>
    internal class SaveReader
    {
        private string content;
        private Sections save;

        /// <summary>
        /// Lê o conteúdo de uma string para o save.
        /// </summary>
        public void Read(string content, Sections save)
        {
            this.content = content;
            this.save = save;
        }
    }
}