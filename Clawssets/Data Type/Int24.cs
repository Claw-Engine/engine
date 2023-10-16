using System.IO;

namespace Clawssets
{
    /// <summary>
    /// Representa um inteiro de 24 bits.
    /// </summary>
    public struct Int24
    {
        public const int MaxValue = 8_388_607, MinValue = -8_388_608;
        private int value;

        public static implicit operator int(Int24 value) => value.value;
        public static implicit operator Int24(byte[] bytes) => new Int24() { value = (bytes[0] << 16) | (bytes[1] << 8) | bytes[2] };
    }
    /// <summary>
    /// Extensões relacionadas ao <see cref="Int24"/>.
    /// </summary>
    public static class Int24Extensions
    {
        /// <summary>
        /// Lê um inteiro com sinal de 3 bytes do fluxo atual e avança a posição atual do fluxo em quatro bytes.
        /// </summary>
        public static Int24 ReadInt24(this BinaryReader reader) => reader.ReadBytes(3);
    }
}