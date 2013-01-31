using System.Collections.Generic;
using System.Dynamic;

namespace Arango.Client
{
    public class ArangoDocument
    {
        public string ID { get; set; }
        public string Revision { get; set; }
        public Json JsonObject { get; set; }

        public ArangoDocument()
        {
            JsonObject = new Json();
        }

        public bool Has(string fieldName)
        {
            return JsonObject.Has(fieldName);
        }
    }
}
