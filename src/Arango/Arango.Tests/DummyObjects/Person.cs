using System.Collections.Generic;
using Arango.Client;

namespace Arango.Tests
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        
        [ArangoProperty(Serializable = false)]
        public string ShouldNotBeSerialized { get; set; }
        
        [ArangoProperty(Alias = "aliasedField")]
        public string Aliased { get; set; }
        
        public List<string> Interests { get; set; }
        public List<Person> Followers { get; set; }
        
        public Person()
        {
            Interests = new List<string>();
            Followers = new List<Person>();
        }
    }
}
