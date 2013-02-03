using System;
using System.Collections.Generic;
using ServiceStack.Text;

namespace Arango.Client
{
    public class Json : JsonObject
    {
        public Json() { }

        public Json(string json)
        {
            Load(json);
        }

        public string GetValue(string fieldPath)
        {
            string obj = null;

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                JsonObject innerObject = null;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        obj = innerObject.Get(field);
                        break;
                    }

                    innerObject = this.Get<JsonObject>(field);
                    iteration++;
                }
            }
            else
            {
                obj = this.Get(fieldPath);
            }

            return obj;
        }

        public T GetValue<T>(string fieldPath) where T : new()
        {
            T obj = new T();

            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                JsonObject innerObject = this;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        obj = innerObject.Get<T>(field);
                        break;
                    }

                    innerObject = innerObject.Get<JsonObject>(field);
                    iteration++;
                }
            }
            else
            {
                obj = this.Get<T>(fieldPath);
            }

            return obj;
        }

        public void SetValue<T>(string fieldPath, T value)
        {
            if (fieldPath.Contains("."))
            {
                var fields = fieldPath.Split('.');
                int iteration = 1;
                List<JsonObject> innerObjects = new List<JsonObject>();
                JsonObject innerObject = this;
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

                    innerObject = innerObject.Get<JsonObject>(field);
                    innerObjects.Add(innerObject);
                    iteration++;
                }

                iteration--;

                foreach (var field in fields)
                {
                    if (iteration > 0)
                    {
                        JsonObject obj = innerObjects[iteration - 1];
                        obj[fields[iteration - 1]] = ToJson(innerObjects[iteration]);
                    }
                    else
                    {
                        JsonObject obj = innerObjects[0];
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
                JsonObject innerObject = null;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        contains = innerObject.ContainsKey(field);
                    }

                    innerObject = this.Get<JsonObject>(field);
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
            JsonObject obj = JsonObject.Parse(json);

            foreach(string key in obj.Keys)
            {
                this.Add(key, obj.GetUnescaped(key));
            }
        }

        public string Stringify()
        {
            return this.ToJson<JsonObject>();
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
