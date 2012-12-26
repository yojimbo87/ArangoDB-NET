
namespace Arango.Client
{
    public class ArangoCollection
    {
        /// <summary>
        /// Unique identifier of the collection.
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Name of the collection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determines if creating or changing a document will wait until the data has been synchronised to disk.
        /// </summary>
        public bool WaitForSync { get; set; }

        /// <summary>
        /// Maximum size of a journal or datafile in bytes which also limits the maximum size of a single object. Must be at least 1MB.
        /// </summary>
        public long JournalSize { get; set; }

        /// <summary>
        /// Status of the collection.
        /// </summary>
        public ArangoCollectionStatus Status { get; set; }

        /// <summary>
        /// Type of the collection.
        /// </summary>
        public ArangoCollectionType Type { get; set; }

        /// <summary>
        /// Number of documents in the collection.
        /// </summary>
        public long DocumentsCount { get; set; }

        /// <summary>
        /// Number of living documents.
        /// </summary>
        public long AliveDocumentsCount { get; set; }

        /// <summary>
        /// Total size of live documents in bytes.
        /// </summary>
        public long AliveDocumentsSize { get; set; }

        /// <summary>
        /// Number of dead documents.
        /// </summary>
        public long DeadDocumentsCount { get; set; }

        /// <summary>
        /// Total size of dead documents in bytes.
        /// </summary>
        public long DeadDocumentsSize { get; set; }

        /// <summary>
        /// Total number of deletion markers.
        /// </summary>
        public long DeadDeletetionCount { get; set; }

        /// <summary>
        /// Number of active datafiles.
        /// </summary>
        public long DataFilesCount { get; set; }

        /// <summary>
        /// Total filesize of datafiles in bytes.
        /// </summary>
        public long DataFilesSize { get; set; }

        /// <summary>
        /// Number of journal files.
        /// </summary>
        public long JournalsCount { get; set; }

        /// <summary>
        /// Total filesize of journal files in bytes.
        /// </summary>
        public long JournalsFileSize { get; set; }
    }
}
