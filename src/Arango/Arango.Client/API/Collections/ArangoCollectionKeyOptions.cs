
namespace Arango.Client
{
    public class ArangoCollectionKeyOptions
    {
        public ArangoKeyGeneratorType GeneratorType { get; set; }
        public bool AllowUserKeys { get; set; }
        public int Increment { get; set; }
        public int Offset { get; set; }
    }
}
