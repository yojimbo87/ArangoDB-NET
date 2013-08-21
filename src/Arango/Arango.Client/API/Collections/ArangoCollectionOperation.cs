using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoCollectionOperation
    {
        private CollectionOperation _collectionOperation;

        internal ArangoCollectionOperation(CollectionOperation collectionOperation)
        {
            _collectionOperation = collectionOperation;
        }

        /// <summary>
        /// Retrieves collection object from database identified by its name.
        /// </summary>
        public ArangoCollection Get(string name)
        {
            return _collectionOperation.Get(name);
        }
        
        /// <summary>
        /// Creates collection in database and assigns additional data to referenced object.
        /// </summary>
        public void Create(ArangoCollection collection)
        {
            _collectionOperation.Post(collection);
        }
        
        /// <summary>
        /// Deletes specified collection from database and returnes boolean value which indicates if the operation was successful.
        /// </summary>
        public bool Delete(string name)
        {
            return _collectionOperation.Delete(name);
        }
        
        /// <summary>
        /// Removes all documnets from specified collection and returns boolean values which indicates if the operation was successful.
        /// </summary>
        public bool Clear(string name)
        {
            return _collectionOperation.PutTruncate(name);
        }
    }
}
