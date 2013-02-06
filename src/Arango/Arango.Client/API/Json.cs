using System;
using System.Collections;
using System.Collections.Generic;
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
            string obj = null;

            /*if (fieldPath.Contains("."))
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
            }*/

            return obj;
        }

        public T Get<T>(string fieldPath) where T : new()
        {
            T obj = new T();

            /*if (fieldPath.Contains("."))
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

                    //innerObject = json.GetUnescaped(field).ToStringDictionary();
                    innerObject = ToDictionary(json.GetUnescaped(field));
                    iteration++;
                }
            }
            else
            {
                JsonObject json = ToJsonObject(this);
                obj = json.Get<T>(fieldPath);
            }*/

            return obj;
        }

        public void Set<T>(string fieldPath, T value)
        {
            /*if (fieldPath.Contains("."))
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
            }*/
        }

        public bool Has(string fieldPath)
        {
            bool contains = false;

            /*if (fieldPath.Contains("."))
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
            }*/

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

        private List<object> ParseArray(string array)
        {
            List<object> collection = new List<object>();

            // when parsing data from json string - everything field value will be string by default
            foreach (var item in array.FromJson<List<object>>())
            {
                collection.Add(item);
            }

            return collection;
        }
    }
}
