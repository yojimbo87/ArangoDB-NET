
namespace Arango.Client
{
    /// <summary>
    /// Status of the Arango collection.
    /// </summary>
    public enum ArangoCollectionStatus
    {
        New = 1,
        Unloaded = 2,
        Loaded = 3,
        Unloading = 4,
        Deleted = 5,
        Corrupted
    }
}
