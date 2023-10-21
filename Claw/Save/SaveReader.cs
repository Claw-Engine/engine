using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using Claw.Extensions;

namespace Claw.Save
{
    /// <summary>
    /// Responsável pela desserialização de saves.
    /// </summary>
    internal class SaveReader
    {
        private int index;
        private string content, section;
        private Sections save;
        private Dictionary<int, object> references;

        /// <summary>
        /// Lê o conteúdo de uma string para o save.
        /// </summary>
        public void Read(string content, Sections save)
        {
            index = 0;
            this.content = content;
            this.save = save;
            references = new Dictionary<int, object>();
            
            SetSection();
            ReadKeys();
        }

        /// <summary>
        /// Lê as chaves.
        /// </summary>
        private void ReadKeys()
        {
            JumpEmpty();

            while (CharEquals('"'))
            {
                string key = GetString(out bool isChar);

                JumpEmpty();

                if (CharEquals('='))
                {
                    index++;

                    JumpEmpty();
                    save[section].Set(key, ReadValue());
                }
            }

            JumpEmpty();

            if (CharEquals('['))
            {
                SetSection();
                ReadKeys();
            }
        }

        /// <summary>
        /// Lê o valor da chave, presumindo que os próximos caracteres são um valor.
        /// </summary>
        private object ReadValue()
        {
            object result;
            int? refId = null;

            if (content[index] == '!') refId = GetReference('!');

            JumpEmpty();

            if (content[index] == '"' || content[index] == '\'')
            {
                string text = GetString(out bool isChar);

                if (isChar) result = text[0];
                else result = text;
            }
            else if (content[index] == '#') result = references[GetReference('#')];
            else if (content[index] == '{') result = ReadObject(refId);
            else if (content[index] == '[') result = ReadArray(refId);
            else if (IsNull()) result = null;
            else result = GetNumber();

            JumpEmpty();

            if (CharEquals(','))
            {
                index++;

                JumpEmpty();
            }

            return result;
        }

        /// <summary>
        /// Lê um objeto, presumindo que os próximos caracteres representam um objeto.
        /// </summary>
        private object ReadObject(int? refId)
        {
            SaveObject result = null;
            index++;

            JumpEmpty();

            if (index < content.Length)
            {
                result = new SaveObject();

                if (refId.HasValue) references.Add(refId.Value, result);

                while (!CharEquals('}'))
                {
                    string key = GetString(out bool isChar);

                    JumpEmpty();

                    if (CharEquals(':'))
                    {
                        index++;

                        JumpEmpty();

                        result[key] = ReadValue();
                    }
                }

                if (CharEquals('}')) index++;

                JumpEmpty();
            }

            result.Lock();

            return result;
        }
        /// <summary>
        /// Lê um array, presumindo que os próximos caracteres representam um array.
        /// </summary>
        private SaveArray ReadArray(int? refId)
        {
            SaveArray result = null;
            index++;

            JumpEmpty();

            if (index < content.Length)
            {
                result = new SaveArray();

                if (refId.HasValue) references.Add(refId.Value, result);

                while (!CharEquals(']')) result.Add(ReadValue());

                if (CharEquals(']')) index++;

                JumpEmpty();
            }

            result.Lock();

            return result;
        }

        /// <summary>
        /// Pula os caracteres vazios.
        /// </summary>
        private void JumpEmpty()
        {
            while (index < content.Length && SaveConvert.IsEmpty(content[index])) index++;
        }

        /// <summary>
        /// Altera a seção atual, presumindo que os próximos caracteres são uma seção.
        /// </summary>
        private void SetSection()
        {
            bool started = false;
            StringBuilder builder = new StringBuilder();

            for (; index < content.Length; index++)
            {
                if (!started)
                {
                    if (content[index] == '[') started = true;
                }
                else if (content[index] == ']')
                {
                    index++;

                    break;
                }
                else builder.Append(content[index]);
            }

            section = builder.ToString();

            if (!save.ContainsKey(section)) save.Add(section, new Keys());
        }

        /// <summary>
        /// Obtém um texto, presumindo que os próximos caracteres representam um texto.
        /// </summary>
        private string GetString(out bool isChar)
        {
            bool started = false;
            isChar = false;
            StringBuilder builder = new StringBuilder();

            for (; index < content.Length; index++)
            {
                if (!started)
                {
                    if (content[index] == '"' || content[index] == '\'')
                    {
                        started = true;
                        isChar = content[index] == '\''; 
                    }
                }
                else if (content[index] == '"' || content[index] == '\'')
                {
                    index++;

                    break;
                }
                else builder.Append(content[index]);
            }
            
            if (!isChar) return builder.ToString();
            else return builder[0].ToString();
        }

        /// <summary>
        /// Obtém o index da referência, presumindo que os próximos caracteres repesentam a referência.
        /// </summary>
        private int GetReference(char referenceStart)
        {
            bool started = false;
            StringBuilder builder = new StringBuilder();

            for (; index < content.Length; index++)
            {
                if (!started)
                {
                    if (content[index] == referenceStart) started = true;
                }
                else
                {
                    if (IsNumber(content[index])) builder.Append(content[index]);
                    else break;
                }
            }
            
            return int.Parse(builder.ToString());
        }

        /// <summary>
        /// Checa se os próximos caracteres representam nulo. Caso positivo, pula eles.
        /// </summary>
        private bool IsNull()
        {
            bool isNull = true;

            if (index >= content.Length) return true;

            for (int i = 0; i < 4; i++)
            {
                if (char.ToLower(SaveConvert.Null[i]) != char.ToLower(content[index + i]))
                {
                    isNull = false;

                    break;
                }
            }

            if (isNull) index += SaveConvert.Null.Length;

            return isNull;
        }

        /// <summary>
        /// Obtém o número a seguir, presumindo que os próximos caracteres representam um número.
        /// </summary>
        private object GetNumber()
        {
            StringBuilder builder = new StringBuilder();

            for (; index < content.Length; index++)
            {
                if (IsNumber(content[index])) builder.Append(content[index]);
                else break;
            }

            string result = builder.ToString();

            if (result.Length == 0) return 0;

            if (result.Contains(".")) return float.Parse(result, CultureInfo.InvariantCulture);
            else return long.Parse(result);
        }

        /// <summary>
        /// Compara o caractere atual com o <paramref name="char"/>.
        /// </summary>
        private bool CharEquals(char @char)
        {
            if (index < content.Length) return content[index] == @char;

            return false;
        }

        /// <summary>
        /// Checa se o caractere é válido como um número.
        /// </summary>
        private bool IsNumber(char @char) => @char == '0' || @char == '1' || @char == '2' || @char == '3' || @char == '4' ||
             @char == '5' || @char == '6' || @char == '7' || @char == '8' || @char == '9' || @char == '.' || @char == '-' || @char == '+';
    }
}