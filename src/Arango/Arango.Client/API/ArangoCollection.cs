
namespace Arango.Client
{
    public class ArangoCollection
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool WaitForSync { get; set; }
        public long JournalSize { get; set; }
        public ArangoCollectionStatus Status { get; set; }
        public ArangoCollectionType Type { get; set; }
        public long DocumentsCount { get; set; }
        public long AliveDocumentsCount { get; set; }
        public long AliveDocumentsSize { get; set; }
        public long DeadDocumentsCount { get; set; }
        public long DeadDocumentsSize { get; set; }
        public long DeadDeletetionCount { get; set; }
        public long DataFilesCount { get; set; }
        public long DataFilesSize { get; set; }
        public long JournalsCount { get; set; }
        public long JournalsFileSize { get; set; }
    }
}
