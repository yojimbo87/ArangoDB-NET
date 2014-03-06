using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary> 
    /// Expose collection management functionality.
    /// </summary>
    public class ArangoCollectionOperation
    {
        private CollectionOperation _collectionOperation;

        internal ArangoCollectionOperation(CollectionOperation collectionOperation)
        {
            _collectionOperation = collectionOperation;
        }

        /// <summary>
        /// Retrieves collection with specified name.
        /// </summary>
        /// <param name="name">Collection name.</param>
        public ArangoCollection Get(string name)
        {
            return _collectionOperation.Get(name);
        }
        
        /// <summary>
        /// Retrieves properties of collection with specified name.
        /// </summary>
        /// <param name="name">Collection name.</param>
        public ArangoCollection Properties(string name)
        {
            return _collectionOperation.Properties(name);
        }        
        
        /// <summary>
        /// Creates collection with specified configuration.
        /// </summary>
        /// <param name="collection">Collection object which contains configuration.</param>
        public void Create(ArangoCollection collection)
        {
            _collectionOperation.Post(collection);
        }
        
        /// <summary> 
        /// Deletes collection with specified name.
        /// </summary>
        /// <param name="name">Collection name.</param>
        public bool Delete(string name)
        {
            return _collectionOperation.Delete(name);
        }
        
        /// <summary>
        /// Removes all items from specified collection.
        /// </summary>
        /// <param name="name">Collection name.</param>
        public bool Clear(string name)
        {
            return _collectionOperation.PutTruncate(name);
        }
    }
}
