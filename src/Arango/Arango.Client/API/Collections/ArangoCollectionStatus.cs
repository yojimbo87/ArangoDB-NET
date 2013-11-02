
namespace Arango.Client
{
    /// <summary> 
    /// Determines current status of the collection.
    /// </summary>
    public enum ArangoCollectionStatus
    {
        /// <summary> 
        /// Collection is new born.
        /// </summary>
        New = 1,
        
        /// <summary> 
        /// Collection is unloaded.
        /// </summary>
        Unloaded = 2,
        
        /// <summary> 
        /// Collection is loaded.
        /// </summary>
        Loaded = 3,
        
        /// <summary> 
        /// Collection is in the process of being unloaded.
        /// </summary>
        Unloading = 4,
        
        /// <summary> 
        /// Collection is deleted.
        /// </summary>
        Deleted = 5,
        
        /// <summary> 
        /// Collection is corrupted.
        /// </summary>
        Corrupted
    }
}
