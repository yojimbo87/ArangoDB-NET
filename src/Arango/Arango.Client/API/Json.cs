using System;
using System.Collections.Generic;
using ServiceStack.Text;

namespace Arango.Client
{
    public class Json : Dictionary<string, string>
    {
        public Json() { }

        public Json(string json)
        {
            Load(json);
        }

        public string Get(string fieldPath)
        {
            string obj = null;

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Dictionary<string, string> innerObject = this;

                foreach (var field in fields)
                {
                    JsonObject json = ToJsonObject(innerObject);

                    if (iteration == fields.Length)
                    {
                        obj = json.Get(field);
                        break;
                    }

                    innerObject = ToDictionary(json.GetUnescaped(field));
                    iteration++;
                }
            }
            else
            {
                JsonObject json = ToJsonObject(this);
                obj = json.Get(fieldPath);
            }

            return obj;
        }

        public T Get<T>(string fieldPath) where T : new()
        {
            T obj = new T();

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                Dictionary<string, string> innerObject = this;

                foreach (var field in fields)
                {
                    JsonObject json = ToJsonObject(innerObject);

                    if (iteration == fields.Length)
                    {
                        obj = json.Get<T>(field);
                        break;
                    }

                    innerObject = json.GetUnescaped(field).ToStringDictionary();
                    iteration++;
                }
            }
            else
            {
                JsonObject json = ToJsonObject(this);
                obj = json.Get<T>(fieldPath);
            }

            return obj;
        }

        public void Set<T>(string fieldPath, T value)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                List<Dictionary<string, string>> innerObjects = new List<Dictionary<string, string>>();
                Dictionary<string, string> innerObject = this;
                innerObjects.Add(innerObject);

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        if (innerObject.ContainsKey(field))
                        {
                            innerObject[field] = ToJson<T>(value);
                        }
                        else
                        {
                            innerObject.Add(field, ToJson<T>(value));
                        }
                        break;
                    }

                    JsonObject json = ToJsonObject(innerObject);
                    innerObject = json.GetUnescaped(field).ToStringDictionary();
                    innerObjects.Add(innerObject);
                    iteration++;
                }

                iteration--;

                foreach (var field in fields)
                {
                    if (iteration > 0)
                    {
                        Dictionary<string, string> obj = innerObjects[iteration - 1];
                        obj[fields[iteration - 1]] = ToJson(innerObjects[iteration]);
                    }
                    else
                    {
                        Dictionary<string, string> obj = innerObjects[0];
                        obj[fields[0]] = ToJson(innerObjects[1]);
                    }

                    iteration--;
                }
            }
            else
            {
                if (this.ContainsKey(fieldPath))
                {
                    this[fieldPath] = ToJson<T>(value);
                }
                else
                {
                    this.Add(fieldPath, ToJson<T>(value));
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
                Dictionary<string, string> innerObject = null;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        contains = innerObject.ContainsKey(field);
                    }

                    JsonObject json = ToJsonObject(innerObject);
                    innerObject = json.GetUnescaped(field).ToStringDictionary();
                    iteration++;
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
            Dictionary<string, string> obj = JsonObject.Parse(json).ToStringDictionary();

            foreach(string key in obj.Keys)
            {
                this.Add(key, obj[key]);
            }
        }

        public string Stringify()
        {
            return ToJsonObject(this).ToJson<JsonObject>();
        }

        private JsonObject ToJsonObject(Dictionary<string, string> dictionary)
        {
            JsonObject obj = new JsonObject();

            foreach (KeyValuePair<string, string> kv in dictionary)
            {
                obj.Add(kv.Key, kv.Value);
            }

            return obj;
        }

        private Dictionary<string, string> ToDictionary(string json)
        {
            JsonObject obj = JsonObject.Parse(json);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            foreach (var kv in obj)
            {
                dictionary.Add(kv.Key, obj.GetUnescaped(kv.Key));
            }

            return dictionary;
        }

        private string ToJson<T>(T value)
        {
            // prevent double quoting string value
            if (value is string)
            {
                return value.ToString();
            }
            else
            {
                return value.ToJson<T>();
            }
        }
    }
}
