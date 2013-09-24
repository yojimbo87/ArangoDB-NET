using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arango.Client
{
    public class ArangoSettings
    {
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
