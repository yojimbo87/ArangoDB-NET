using Arango.Client;

namespace Arango.Tests
{
    public class IssueNo34Entity
    {
        [AliasField("_id")]
        public string Id { get; set; }

        [AliasField("_key")]
        public string Key { get; set; }

        [AliasField("_from")]
        public string From { get; set; }

        [AliasField("_to")]
        public string To { get; set; }

        [AliasField("foo")]
        public string Foo { get; set; }

        [AliasField("bar")]
        public int Bar { get; set; }
    }
}
