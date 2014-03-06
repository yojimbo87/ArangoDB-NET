using System.Collections.Generic;

namespace Arango.Client
{
    /// <summary>
    /// Stores collection data.
    /// </summary>
    public class ArangoCollection
    {
        /// <summary>
        /// Collection identifier.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Collection name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Collection key.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Collection type.
        /// </summary>
        public ArangoCollectionType Type { get; set; }
        
        /// <summary>
        /// Determines if the response should wait until data has been synced to disk.
        /// </summary>
        public bool WaitForSync { get; set; }
        
        /// <summary>
        /// Maximum size setting for journals/datafiles.
        /// </summary>
        public int JournalSize { get; set; }

        /// <summary>
        /// Number of shards for the collection
        /// </summary>        
        public int? NumberOfShards { get; set; }

        /// <summary>
        /// Specifies the shard keys of a collection.
        /// </summary>        
        public List<string> ShardKeys { get; set; }
        
        /// <summary>
        /// Collection status.
        /// </summary>
        public ArangoCollectionStatus Status { get; set; }
        
        /// <summary>
        /// Determines if the collection is system.
        /// </summary>
        public bool IsSystem { get; set; }
        
        /// <summary>
        /// Determines if the data will be kept only in memory and will not write or sync the data to disk.
        /// </summary>
        public bool IsVolatile { get; set; }
        
        /// <summary>
        /// Specifies collection key options.
        /// </summary>
        public ArangoCollectionKeyOptions KeyOptions { get; set; }
    }
}
