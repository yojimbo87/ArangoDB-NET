

namespace Arango.Client
{
    public class ArangoCollection
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public ArangoCollectionType Type { get; set; }
        public bool WaitForSync { get; set; }
        public int JournalSize { get; set; }
        public ArangoCollectionStatus Status { get; set; }
        public bool IsSystem { get; set; }
        public bool IsVolatile { get; set; }
        public ArangoCollectionKeyOptions KeyOptions { get; set; }
    }
}
