using Arango.Client.ExternalLibraries.dictator;

namespace Arango.Tests.Entities
{
    public class Dummy
    {
        [AliasField("foo")]
        public string Foo { get; set; }
        
        [AliasField("bar")]
        public int Bar { get; set; }
        
        [AliasField("baz")]
        public int Baz { get; set; }
    }
}
