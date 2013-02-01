using System;
using System.Collections.Generic;
using ServiceStack.Text;

namespace Arango.Client
{
    public class Json
    {
        private JsonObject _jsonObject;

        public Json()
        {
            _jsonObject = new JsonObject();
        }

        public Json(string json)
        {
            Load(json);
        }

        public string Get(string fieldName)
        {
            string obj = null;

            if (fieldName.Contains("."))
            {
                var fields = fieldName.Split('.');
                int iteration = 1;
                JsonObject innerObject = null;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        obj = innerObject.Get(field);
                        break;
                    }

                    innerObject = _jsonObject.Get<JsonObject>(field);
                    iteration++;
                }
            }
            else
            {
                obj = _jsonObject.Get(fieldName);
            }

            return obj;
        }

        public T Get<T>(string fieldName) where T : new()
        {
            T obj = new T();

            if (fieldName.Contains("."))
            {
                var fields = fieldName.Split('.');
                int iteration = 1;
                JsonObject innerObject = _jsonObject;

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
                obj = _jsonObject.Get<T>(fieldName);
            }

            return obj;
        }

        public void Set<T>(string fieldName, T value)
        {
            if (fieldName.Contains("."))
            {
                var fields = fieldName.Split('.');
                int iteration = 1;
                List<JsonObject> innerObjects = new List<JsonObject>();
                JsonObject innerObject = _jsonObject;
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
                if (_jsonObject.ContainsKey(fieldName))
                {
                    _jsonObject[fieldName] = ToJson<T>(value);
                }
                else
                {
                    _jsonObject.Add(fieldName, ToJson<T>(value));
                }
            }
        }

        public bool Has(string fieldName)
        {
            bool contains = false;

            if (fieldName.Contains("."))
            {
                var fields = fieldName.Split('.');
                int iteration = 1;
                JsonObject innerObject = null;

                foreach (var field in fields)
                {
                    if (iteration == fields.Length)
                    {
                        contains = innerObject.ContainsKey(field);
                    }

                    innerObject = _jsonObject.Get<JsonObject>(field);
                    iteration++;
                }
            }
            else
            {
                contains = _jsonObject.ContainsKey(fieldName);
            }

            return contains;
        }

        public void Load(string json)
        {
            _jsonObject = JsonObject.Parse(json);
        }

        public string Stringify()
        {
            return _jsonObject.ToJson();
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
