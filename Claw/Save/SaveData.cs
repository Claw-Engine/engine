using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Globalization;
using Claw.Extensions;

namespace Claw.Save
{
    /// <summary>
    /// Interface para valores do save.
    /// </summary>
    public interface ISaveValue
    {
        void Lock();
        object Cast(Type type, Dictionary<ISaveValue, object> references);
    }

    /// <summary>
    /// Representação de um objeto no save.
    /// </summary>
    public class SaveObject : IEnumerable<KeyValuePair<string, object>>, ISaveValue
    {
        public object this[string property]
        {
            get => internalDictionary.Get(property, null);
            set
            {
                if (!locked) internalDictionary[property] = value;
            }
        }

        private bool locked = false;
        private Dictionary<string, object> internalDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Tranca o objeto para não receber mais mudanças.
        /// </summary>
        public void Lock() => locked = true;

        /// <summary>
        /// Converte um <see cref="SaveObject"/> em um objeto.
        /// </summary>
        public object Cast(Type type, Dictionary<ISaveValue, object> references)
        {
            if (type == GetType()) return this;
            else if (references.ContainsKey(this)) return references[this];

            object instance = Activator.CreateInstance(type);

            references.Add(this, instance);

            SetupSetters(type, out Dictionary<string, (PropertySetter, Type)> setters);

            foreach (KeyValuePair<string, object> pair in internalDictionary)
            {
                if (pair.Value is ISaveValue)
                {
                    KeyValuePair<ISaveValue, object> found = references.FirstOrDefault((p) => p.Key == pair.Value);

                    if (found.Equals(default(KeyValuePair<ISaveValue, object>)))
                    {
                        object value = ((ISaveValue)pair.Value).Cast(setters[pair.Key].Item2, references);

                        setters[pair.Key].Item1(instance, value);
                        references.Add(((ISaveValue)pair.Value), value);
                    }
                    else setters[pair.Key].Item1(instance, found.Value);
                }
                else setters[pair.Key].Item1(instance, Convert.ChangeType(pair.Value, setters[pair.Key].Item2));
            }

            return instance;
        }
        /// <summary>
        /// Obtém os setters para cada propriedade.
        /// </summary>
        private void SetupSetters(Type type, out Dictionary<string, (PropertySetter, Type)> setters)
        {
            setters = new Dictionary<string, (PropertySetter, Type)>();
            FieldInfo[] fields = type.GetFields(SaveConvert.Flags);
            PropertyInfo[] properties = type.GetProperties(SaveConvert.Flags);

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Name.EndsWith(">k__BackingField") || fields[i].GetCustomAttribute<SaveIgnoreAttribute>() != null) continue;

                SavePropertyAttribute attribute = fields[i].GetCustomAttribute<SavePropertyAttribute>();

                if (fields[i].IsPrivate && attribute == null) continue;

                string name = attribute == null || attribute.Name == null ? fields[i].Name : attribute.Name;

                setters.Add(name, (fields[i].SetValue, fields[i].FieldType));
            }

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].GetCustomAttribute<SaveIgnoreAttribute>() != null) continue;

                SavePropertyAttribute attribute = properties[i].GetCustomAttribute<SavePropertyAttribute>();

                if (properties[i].GetMethod.IsPrivate && attribute == null) continue;

                string name = attribute == null || attribute.Name == null ? properties[i].Name : attribute.Name;

                setters.Add(name, (properties[i].SetValue, properties[i].PropertyType));
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => internalDictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)internalDictionary).GetEnumerator();
        }
    }

    /// <summary>
    /// Representação de um array no save.
    /// </summary>
    public class SaveArray : Collection<object>, ISaveValue
    {
        private bool locked = false;

        /// <summary>
        /// Tranca o objeto para não receber mais mudanças.
        /// </summary>
        public void Lock() => locked = true;

        /// <summary>
        /// Insere um elemento no array.
        /// </summary>
        protected override void InsertItem(int index, object item)
        {
            if (!locked) base.InsertItem(index, item);
        }
        /// <summary>
        /// Remove um elemento do array.
        /// </summary>
        protected override void RemoveItem(int index)
        {
            if (!locked) base.RemoveItem(index);
        }
        /// <summary>
        /// Limpa o array.
        /// </summary>
        protected override void ClearItems()
        {
            if (!locked) base.ClearItems();
        }
        /// <summary>
        /// Altera um elemento do array.
        /// </summary>
        protected override void SetItem(int index, object item)
        {
            if (!locked) base.SetItem(index, item);
        }

        /// <summary>
        /// Converte o <see cref="SaveArray"/> em um <see cref="IEnumerable{T}"/>.
        /// </summary>
        public object Cast(Type type, Dictionary<ISaveValue, object> references)
        {
            if (type == GetType()) return this;
            else if (references.ContainsKey(this)) return references[this];

            if (type.GetInterface("ICollection") != null)
            {
                dynamic result = null;

                if (type.GetInterface("IDictionary") != null)
                {
                    result = Activator.CreateInstance(type);
                    Type[] types = type.GetGenericArguments();

                    references.Add(this, result);
                    FillDictionary(result, types[0], types[1], references);
                }
                else if (type.IsArray)
                {
                    Type arrayType = type.GetElementType();
                    result = Array.CreateInstance(arrayType, Count);
                    int index = 0;

                    references.Add(this, result);
                    FillList(arrayType, (element) =>
                    {
                        result[index] = element;
                        index++;
                    }, references);
                }
                else if (type.IsGenericType)
                {
                    result = Activator.CreateInstance(type);

                    references.Add(this, result);
                    FillList(type.GetGenericArguments()[0], result.Add, references);
                }
                else throw new ArgumentException(string.Format("\"{0}\" é inválido!", type.FullName));

                return result;
            }
            else throw new ArgumentException(string.Format("\"{0}\" é inválido!", type.FullName));
        }
        /// <summary>
        /// Preenche os elementos de uma lista com os deste <see cref="SaveArray"/>
        /// </summary>
        private void FillList(Type type, Action<dynamic> add, Dictionary<ISaveValue, object> references)
        {
            for (int i = 0; i < Count; i++)
            {
                dynamic result = null;

                if (Items[i] is ISaveValue) result = GetValue((ISaveValue)Items[i], type, references);
                else result = Convert.ChangeType(Items[i], type);

                add(result);
            }
        }
        /// <summary>
        /// Preenche um dicionário com os elementos deste <see cref="SaveArray"/>.
        /// </summary>
        private void FillDictionary(dynamic dictionary, Type keyType, Type valueType, Dictionary<ISaveValue, object> references)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Items[i].GetType() == GetType())
                {
                    SaveArray pair = (SaveArray)Items[i];
                    dynamic key = pair[0];
                    dynamic value = pair[1];

                    if (key is ISaveValue) key = GetValue((ISaveValue)key, keyType, references);
                    else key = Convert.ChangeType(key, keyType);

                    if (value is ISaveValue) value = GetValue((ISaveValue)value, valueType, references);
                    else value = Convert.ChangeType(value, valueType);

                    dictionary.Add(key, value);
                }
            }
        }
        /// <summary>
        /// Se o valor não estiver nas referências, faz o casting e adiciona. Se estiver, retorna ele.
        /// </summary>
        private object GetValue(ISaveValue value, Type type, Dictionary<ISaveValue, object> references)
        {
            KeyValuePair<ISaveValue, object> found = references.FirstOrDefault((p) => p.Key == value);
            object result = null;

            if (found.Equals(default(KeyValuePair<ISaveValue, object>)))
            {
                result = value.Cast(type, references);

                references.Add(value, result);
            }
            else result = found.Value;

            return result;
        }
    }
}