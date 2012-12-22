//using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Arango.Client
{
    public class ArangoDocument
    {
        public string Handle { get; set; }
        public string Revision { get; set; }
        public string Json { get; set; }
        public dynamic Object { get; set; }

        public ArangoDocument()
        {
            Object = new ExpandoObject();
        }

        public bool Has(string fieldName)
        {
            if (fieldName.Contains("."))
            {
                var fields = fieldName.Split('.');
                var iteration = 1;
                var json = (IDictionary<string, object>)Object;

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
                return ((IDictionary<string, object>)Object).ContainsKey(fieldName);
            }

            return false;
        }
    }
}
