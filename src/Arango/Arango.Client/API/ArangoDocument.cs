//using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Arango.Client
{
    public class ArangoDocument
    {
        public string Handle { get; set; }
        public string Revision { get; set; }
        public dynamic JsonObject { get; set; }

        public ArangoDocument()
        {
            JsonObject = new ExpandoObject();
        }

        public bool Has(string fieldName)
        {
            if (fieldName.Contains("."))
            {
                var fields = fieldName.Split('.');
                var iteration = 1;
                var json = (IDictionary<string, object>)JsonObject;

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
                return ((IDictionary<string, object>)JsonObject).ContainsKey(fieldName);
            }

            return false;
        }
    }
}
