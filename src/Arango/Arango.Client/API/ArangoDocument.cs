//using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Arango.Client
{
    public class ArangoDocument
    {
        public string ID { get; set; }
        public string Revision { get; set; }
        public string Data { get; set; }
        public dynamic Json { get; set; }

        public ArangoDocument()
        {
            Json = new ExpandoObject();
        }

        public bool Has(string fieldName)
        {
            if (fieldName.Contains("."))
            {
                var fields = fieldName.Split('.');
                var iteration = 1;
                var json = (IDictionary<string, object>)Json;

                foreach (var field in fields)
                {
                    if (json.ContainsKey(field))
                    {
                        if (iteration == fields.Length)
                        {
                            return true;
                        }

                        json = (IDictionary<string, object>)json[field];
                        iteration++;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return ((IDictionary<string, object>)Json).ContainsKey(fieldName);
            }

            return false;
        }
    }
}
