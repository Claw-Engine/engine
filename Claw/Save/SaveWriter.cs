using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Claw.Save
{
    /// <summary>
    /// Responsável pela serialização de saves.
    /// </summary>
    internal class SaveWriter
    {
        private static readonly string SectionFormat = "[{0}]", ReferenceFormat = "#{0}", IdFormat = "!{0}", KeyFormat = "\"{0}\" = ", ValueFormat = "{0},",
            StringFormat = "\"{0}\"", CharFormat = "'{0}'", TypeFormat = "({0})", PairFormat = "\"{0}\":{1}, ";
        private static readonly string NewLine = Environment.NewLine;

        private Sections save;
        private StringBuilder builder;
        private Dictionary<int, object> references;

        /// <summary>
        /// Escreve o save numa string.
        /// </summary>
        public void Write(Sections save)
        {
            this.save = save;
            builder = new StringBuilder();
            references = new Dictionary<int, object>();
            int count = 0, len = 0;
            string header = string.Empty;

            foreach (KeyValuePair<string, Keys> section in save)
            {
                if (section.Value.Count > 0)
                {
                    if (builder.Length > 0) builder.Append(NewLine);

                    header = string.Format(SectionFormat, section.Key);

                    builder.Append(header);
                    builder.Append(NewLine);

                    foreach (KeyValuePair<string, object> key in section.Value)
                    {
                        if (key.Value != null)
                        {
                            builder.AppendFormat(KeyFormat, key.Key);
                            builder.AppendFormat(ValueFormat, Stringfy(key.Value));
                            builder.Append(NewLine);

                            count++;
                        }
                    }

                    if (count == 0) builder.Remove(builder.Length - header.Length - NewLine.Length * 2, header.Length + NewLine.Length * 2);

                    count = 0;
                }
            }

            if (builder.Length > 0) builder.Remove(builder.Length - NewLine.Length, NewLine.Length);
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

        /// <summary>
        /// Transforma um valor em string.
        /// </summary>
        private string Stringfy(object value)
        {
            if (value == null) return SaveConvert.Null;
            else
            {
                StringBuilder builder = new StringBuilder();
                Type type = value.GetType();

                string refId = GetReference(value, out bool isNew);

                if (!isNew && refId.Length > 0) return string.Format(ReferenceFormat, refId);

                if (type.IsEnum) builder.AppendFormat(StringFormat, value.ToString());
                else
                {
                    switch (type.FullName)
                    {
                        case "System.String": builder.AppendFormat(StringFormat, value); break;
                        case "System.Char": builder.AppendFormat(CharFormat, value); break;
                        default:

                            if (isNew && refId.Length > 0) builder.AppendFormat(IdFormat, refId);

                            if (type.GetInterface("IEnumerable") != null)
                            {
                                if (value is SaveObject obj)
                                {
                                    builder.Append("{ ");

                                    foreach (KeyValuePair<string, object> pair in obj) builder.AppendFormat(PairFormat, pair.Key, Stringfy(pair.Value));

                                    builder.Remove(builder.Length - 2, 2);

                                    builder.Append(" }");
                                }
                                else
                                {
                                    dynamic enumerable = value;
                                    
                                    builder.Append('[');

                                    if (type.GetInterface("IDictionary") != null)
                                    {
                                        foreach (dynamic element in enumerable)
                                        {
                                            builder.Append('[');
                                            builder.AppendFormat(ValueFormat, Stringfy(element.Key));
                                            builder.AppendFormat(ValueFormat, Stringfy(element.Value));
                                            builder.Remove(builder.Length - 1, 1);
                                            builder.Append(']');
                                            builder.Append(',');
                                        }
                                    }
                                    else
                                    {
                                        foreach (dynamic element in enumerable) builder.AppendFormat(ValueFormat, Stringfy(element));
                                    }

                                    builder.Remove(builder.Length - 1, 1);
                                    builder.Append(']');
                                }
                            }
                            else if (type.IsObject())
                            {
                                builder.Append("{ ");
                                builder.Append(ObjectToString(value));
                                builder.Append(" }");
                            }
                            else builder.Append(value.ToString().Replace(',', '.'));
                            break;
                    }
                }

                return builder.ToString();
            }
        }
        /// <summary>
        /// Transforma um objeto em string.
        /// </summary>
        private string ObjectToString(object instance)
        {
            Type type = instance.GetType();
            StringBuilder builder = new StringBuilder();
            FieldInfo[] fields = type.GetFields(SaveConvert.Flags);
            PropertyInfo[] properties = type.GetProperties(SaveConvert.Flags);

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name.EndsWith(">k__BackingField") || fields[i].GetCustomAttribute<SaveIgnoreAttribute>() != null) continue;
                
                SavePropertyAttribute attribute = fields[i].GetCustomAttribute<SavePropertyAttribute>();

                if (fields[i].IsPrivate && attribute == null) continue;

                string name = attribute == null || attribute.Name == null ? fields[i].Name : attribute.Name;

                builder.AppendFormat(PairFormat, name, Stringfy(fields[i].GetValue(instance)));
            }

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name.EndsWith(">k__BackingField") || properties[i].GetCustomAttribute<SaveIgnoreAttribute>() != null) continue;

                SavePropertyAttribute attribute = properties[i].GetCustomAttribute<SavePropertyAttribute>();

                if (properties[i].GetMethod.IsPrivate && attribute == null) continue;

                string name = attribute == null || attribute.Name == null ? properties[i].Name : attribute.Name;

                builder.AppendFormat(PairFormat, name, Stringfy(properties[i].GetValue(instance)));
            }

            builder.Remove(builder.Length - 2, 2);

            return builder.ToString();
        }

        /// <summary>
        /// Se o valor passa por referência, adiciona na lista de referências e retorna a string formatada da referência.
        /// </summary>
        private string GetReference(object value, out bool isNew)
        {
            string refId = string.Empty;
            isNew = false;

            if (value != null && value.GetType().PassByReference())
            {
                KeyValuePair<int, object> found = references.FirstOrDefault((pair) => pair.Value == value);

                if (found.Equals(default(KeyValuePair<int, object>)))
                {
                    references.Add(references.Count, value);

                    refId = (references.Count - 1).ToString();
                    isNew = true;
                }
                else refId = found.Key.ToString();
            }

            return refId;
        }
    }
}