using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ServiceStack.Text;

namespace Arango.Client
{
    public class Json : Dictionary<string, object>
    {
        public Json() { }

        public Json(string json)
        {
            Load(json);
        }

        public string Get(string fieldPath)
        {
            string value = null;

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Json innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        value = innerObject[field].ToString();
                        break;
                    }

                    if (innerObject.ContainsKey(field))
                    {
                        innerObject = (Json)innerObject[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    value = this[fieldPath].ToString();
                }
            }

            return value;
        }

        public T Get<T>(string fieldPath) where T : new()
        {
            T value = new T();

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Json innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (innerObject[field] is string)
                        {
                            value = TConverter.ChangeType<T>(innerObject[field]);
                        }
                        else
                        {
                            value = (T)innerObject[field];
                        }
                        break;
                    }

                    if (innerObject.ContainsKey(field))
                    {
                        innerObject = (Json)innerObject[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    if (this[fieldPath] is string)
                    {
                        value = TConverter.ChangeType<T>(this[fieldPath]);
                    }
                    else
                    {
                        value = (T)this[fieldPath];
                    }
                }
            }

            return value;
        }

        public void Set<T>(string fieldPath, T value)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Json innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (innerObject.ContainsKey(field))
                        {
                            innerObject[field] = value;
                        }
                        else
                        {
                            innerObject.Add(field, value);
                        }
                        break;
                    }

                    if (innerObject.ContainsKey(field))
                    {
                        innerObject = (Json)innerObject[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    this[fieldPath] = value;
                }
                else
                {
                    this.Add(fieldPath, value);
                }
            }
        }

        public bool Has(string fieldPath)
        {
            bool contains = false;

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Json innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        contains = innerObject.ContainsKey(field);
                        break;
                    }

                    if (innerObject.ContainsKey(field))
                    {
                        innerObject = (Json)innerObject[field];
                        iteration++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                contains = this.ContainsKey(fieldPath);
            }

            return contains;
        }

        public void Load(string json)
        {
            foreach (var item in ParseObject(json))
            {
                this.Add(item.Key, item.Value);
            }
        }

        public string Stringify()
        {
            return this.ToJson();
        }

        private Json ParseObject(string json)
        {
            Json embeddedObject = new Json();
            JsonObject jsonObject = JsonObject.Parse(json);

            foreach (var item in jsonObject)
            {
                string value = jsonObject.GetUnescaped(item.Key);

                if (value.Length >= 2)
                {
                    // array
                    if ((value[0] =='[') && (value[value.Length - 1] == ']'))
                    {
                        // array of embedded objects
                        if ((value[1] == '{') && (value[value.Length - 2] == '}'))
                        {
                            JsonArrayObjects objects = value.FromJson<JsonArrayObjects>();
                            List<Json> jsonArray = new List<Json>();

                            foreach (JsonObject j in objects)
                            {
                                jsonArray.Add(ParseObject(j.SerializeToString()));
                            }

                            embeddedObject.Add(item.Key, jsonArray);
                        }
                        // array of primitive values or empty array
                        else
                        {
                            embeddedObject.Add(item.Key, ParseArray(value));
                        }
                    }
                    // embedded object
                    else if ((value[0] == '{') && (value[value.Length - 1] == '}'))
                    {
                        embeddedObject.Add(item.Key, ParseObject(value));
                    }
                    // primitive value
                    else
                    {
                        embeddedObject.Add(item.Key, value.FromJson<object>());
                    }
                }
                // primitive value
                else
                {
                    embeddedObject.Add(item.Key, value.FromJson<object>());
                }
            }

            return embeddedObject;
        }

        private List<string> ParseArray(string array)
        {
            List<string> collection = new List<string>();

            foreach (var item in array.FromJson<List<string>>())
            {
                collection.Add(item);
            }

            return collection;
        }
    }

    internal static class TConverter
    {
        internal static T ChangeType<T>(object value)
        {
            return (T)ChangeType(typeof(T), value);
        }
        internal static object ChangeType(Type t, object value)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(t);
            return tc.ConvertFrom(value);
        }
        internal static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {

            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
        }
    }
}
