
namespace Arango.Client
{
    /// <summary> 
    /// Determines type of the collection.
    /// </summary>
    public enum ArangoCollectionType
    {
        /// <summary> 
        /// Collection which consists of document type data.
        /// </summary>
        Document = 2,
        
        /// <summary> 
        /// Collection which consists of edge type data.
        /// </summary>
        Edge = 3
    }
}
