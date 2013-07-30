using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arango.Client
{
    public class ArangoSettings
    {
        internal JsonSerializerSettings SerializerSettings { get; set; }

        public bool DeserializeDateTimeAsString { get; set; }
        
        public ArangoSettings()
        {
            SerializerSettings = new JsonSerializerSettings();
            DeserializeDateTimeAsString = false;
        }
    }
}
