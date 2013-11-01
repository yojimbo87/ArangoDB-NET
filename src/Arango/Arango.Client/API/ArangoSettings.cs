using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arango.Client
{
    /// <summary> 
    /// Global settings which can affect specific functionality of Arango client.
    /// </summary>
    public class ArangoSettings
    {
        /// <summary>
        /// DateTime serialization and deserialization format. By default set to UnixTimeStamp.
        /// </summary>
        public DateTimeFormat DateTimeFormat 
        { 
            get
            {
                return Document.Settings.DateTimeFormat; 
            }
            
            set
            {
                Document.Settings.DateTimeFormat = value;
            }
        }
    }
}
