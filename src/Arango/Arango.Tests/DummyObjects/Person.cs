using System.Collections.Generic;
using Arango.Client;

namespace Arango.Tests
{
    public class Person
    {
        [ArangoProperty(Identity = true)]
        public string ThisIsId { get; set; }
        
        [ArangoProperty(Key = true)]
        public string ThisIsKey { get; set; }
        
        [ArangoProperty(Revision = true)]
        public string ThisIsRevision { get; set; }
        
        [ArangoProperty(From = true)]
        public string ThisIsFrom { get; set; }
        
        [ArangoProperty(To = true)]
        public string ThisIsTo { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public Person Father { get; set; }
        
        [ArangoProperty(Serializable = false)]
        public string ShouldBeOnlyDeserialized { get; set; }
        
        [ArangoProperty(Serializable = false, Deserializable = false)]
        public string ShouldNotBeSerializedOrDeserialized { get; set; }
        
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
