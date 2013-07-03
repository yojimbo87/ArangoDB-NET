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

        public ArangoCollection Get(string name)
        {
            return _collectionOperation.Get(name);
        }
        
        public void Create(ArangoCollection collection)
        {
            _collectionOperation.Post(collection);
        }
        
        public bool Delete(string name)
        {
            return _collectionOperation.Delete(name);
        }
    }
}
