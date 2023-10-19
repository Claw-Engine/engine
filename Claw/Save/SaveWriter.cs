using System;
using System.Text;

namespace Claw.Save
{
    /// <summary>
    /// Responsável pela serialização de saves.
    /// </summary>
    internal class SaveWriter
    {
        private Sections save;
        private StringBuilder builder;

        /// <summary>
        /// Escreve o save numa string.
        /// </summary>
        public void Write(Sections save)
        {
            this.save = save;
            builder = new StringBuilder();
        }
        /// <summary>
        /// Retorna o que resultado da serialização.
        /// </summary>
        public string Close()
        {
            if (builder == null || save == null) return string.Empty;

            string result = builder.ToString();
            save = null;
            builder = null;

            return result;
        }
    }
}