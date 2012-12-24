using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoDatabase
    {
        #region Properties

        private ArangoNode _node;

        #endregion

        public ArangoDatabase(string alias)
        {
            _node = ArangoClient.GetNode(alias);
        }

        #region Collection

        public ArangoCollection CreateCollection(string name, ArangoCollectionType type, bool waitForSync, long journalSize)
        {
            var collection = new Collection(_node);

            return collection.Post(name, type, waitForSync, journalSize);
        }

        public long DeleteCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.Delete(id);
        }

        public long DeleteCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.Delete(name);
        }

        public ArangoCollection GetCollection(int id)
        {
            var collection = new Collection(_node);

            return collection.Get(id);
        }

        public ArangoCollection GetCollection(string collectionName)
        {
            var collection = new Collection(_node);

            return collection.Get(collectionName);
        }

        #endregion

        #region Document

        public ArangoDocument GetDocument(string handle)
        {
            return GetDocument(handle, "");
        }

        public ArangoDocument GetDocument(string handle, string revision)
        {
            var document = new Document(_node);

            return document.Get(handle, revision);
        }

        #endregion
    }
}
