
namespace Arango.Client
{
    public class ArangoCollection
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool WaitForSync { get; set; }
        public long JournalSize { get; set; }
        public ArangoCollectionStatus Status { get; set; }
        public ArangoCollectionType Type { get; set; }
    }
}
