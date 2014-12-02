using System.Collections.Generic;

namespace Arango.Client
{
    public class ArangoUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public Dictionary<string, object> Extra { get; set; }
        
        public ArangoUser()
        {
            Active = true;
        }
    }
}
