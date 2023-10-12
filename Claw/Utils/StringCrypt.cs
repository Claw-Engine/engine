using System;
using System.Collections.Generic;
using System.Text;
using Claw.Extensions;

namespace Claw.Utils
{
    public static class StringCrypt
    {
        /// <summary>
        /// Criptografa/Descriptografa um texto.
        /// </summary>
        /// <param name="encrypt">True para criptografia e false para descriptografia.</param>
        /// <param name="amount">Senha para a criptografia/descriptografia.</param>
        public static string Crypt(string text, bool encrypt, int amount)
        {
            string txtf = string.Empty;
            int mult = encrypt ? 1 : -1;

            for (int i = 0; i < text.Length; i++)
            {
                int txtb = (int)text[i];
                int txtc = txtb + Math.Abs(amount) * mult;
                txtf += Char.ConvertFromUtf32(txtc);
            }

            return txtf;
        }
        /// <summary>
        /// Criptografa/Descriptografa um texto.
        /// </summary>
        /// <param name="encrypt">True para criptografia e false para descriptografia.</param>
        /// <param name="amount">Sequência para a criptografia/descriptografia.</param>
        public static string Crypt(string text, bool encrypt, params int[] amount)
        {
            if (amount.IsEmpty()) throw new Exception("\"amount\" precisa ter pelo menos um valor!");

            string txtf = string.Empty;
            int mult = encrypt ? 1 : -1, amountIndex = 0;

            for (int i = 0; i < text.Length; i++)
            {
                int txtb = (int)text[i];
                int txtc = txtb + Math.Abs(amount[amountIndex]) * mult;
                txtf += Char.ConvertFromUtf32(txtc);
                amountIndex = amountIndex == amount.Length - 1 ? 0 : amountIndex + 1;
            }

            return txtf;
        }
        /// <summary>
        /// Criptografa/Descriptografa um texto.
        /// </summary>
        /// <param name="encrypt">True para criptografia e false para descriptografia.</param>
        /// <param name="password">Senha da criptografia.</param>
        public static string Crypt(string text, bool encrypt, string password)
        {
            if (password.IsEmpty()) throw new Exception("\"password\" precisa ter pelo menos um caractere!");

            string txtf = string.Empty;
            int mult = encrypt ? 1 : -1, amountIndex = 0;

            for (int i = 0; i < text.Length; i++)
            {
                int txtb = (int)text[i];
                int txtc = txtb + (int)password[amountIndex] * mult;
                txtf += Char.ConvertFromUtf32(txtc);
                amountIndex = amountIndex == password.Length - 1 ? 0 : amountIndex + 1;
            }

            return txtf;
        }

        /// <summary>
        /// Converte texto para binário.
        /// </summary>
        public static string StringToBinary(string text)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char character in text) stringBuilder.Append(Convert.ToString(character, 2).PadLeft(8, '0'));

            return stringBuilder.ToString();
        }
        /// <summary>
        /// Converte binário para texto.
        /// </summary>
        public static string BinaryToString(string bynary)
        {
            List<Byte> bytes = new List<byte>();

            for (int i = 0; i < bynary.Length; i += 8) bytes.Add(Convert.ToByte(bynary.Substring(i, 8), 2));

            return Encoding.ASCII.GetString(bytes.ToArray());
        }

        /// <summary>
        /// Converte um texto para hex.
        /// </summary>
        public static string StringToHex(string text)
        {
            var stringBuilder = new StringBuilder();
            var bytes = Encoding.Unicode.GetBytes(text);

            foreach (var t in bytes) stringBuilder.Append(t.ToString("X2"));

            return stringBuilder.ToString();
        }
        /// <summary>
        /// Converte hex para texto.
        /// </summary>
        public static string HexToString(string hex)
        {
            var bytes = new byte[hex.Length / 2];

            for (var i = 0; i < bytes.Length; i++) bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);

            return Encoding.Unicode.GetString(bytes);
        }

        /// <summary>
        /// Usa os métodos Crypt, StringToBinary/BinaryToString e StringToHex/HexToString para criptografar/descriptografar um texto.
        /// </summary>
        /// <param name="encrypt">True para criptografia e false para descriptografia.</param>
        /// <param name="amount">Senha para a criptografia/descriptografia.</param>
        public static string AllCrypt(string text, bool encrypt, int amount)
        {
            if (encrypt) return StringToBinary(StringToHex(Crypt(text, encrypt, amount)));

            return Crypt(HexToString(BinaryToString(text)), encrypt, amount);
        }
        /// <summary>
        /// Usa os métodos Crypt, StringToBinary/BinaryToString e StringToHex/HexToString para criptografar/descriptografar um texto.
        /// </summary>
        /// <param name="encrypt">True para criptografia e false para descriptografia.</param>
        /// <param name="amount">Sequência para a criptografia/descriptografia.</param>
        public static string AllCrypt(string text, bool encrypt, params int[] amount)
        {
            if (encrypt) return StringToBinary(StringToHex(Crypt(text, encrypt, amount)));

            return Crypt(HexToString(BinaryToString(text)), encrypt, amount);
        }
        /// <summary>
        /// Usa os métodos Crypt, StringToBinary/BinaryToString e StringToHex/HexToString para criptografar/descriptografar um texto.
        /// </summary>
        /// <param name="encrypt">True para criptografia e false para descriptografia.</param>
        /// <param name="password">Senha da criptografia.</param>
        public static string AllCrypt(string text, bool encrypt, string password)
        {
            if (encrypt) return StringToBinary(StringToHex(Crypt(text, encrypt, password)));

            return Crypt(HexToString(BinaryToString(text)), encrypt, password);
        }
    }
}