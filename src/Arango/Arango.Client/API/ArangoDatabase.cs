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

        #region Create

        public ArangoCollection CreateCollection(string name, ArangoCollectionType type, bool waitForSync, long journalSize)
        {
            var collection = new Collection(_node);

            return collection.Post(name, type, waitForSync, journalSize);
        }

        #endregion

        #region Delete

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

        #endregion

        #region Get

        public ArangoCollection GetCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.Get(id);
        }

        public ArangoCollection GetCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.Get(name);
        }

        public ArangoCollection GetCollectionProperties(long id)
        {
            var collection = new Collection(_node);

            return collection.GetProperties(id);
        }

        public ArangoCollection GetCollectionProperties(string name)
        {
            var collection = new Collection(_node);

            return collection.GetProperties(name);
        }

        #endregion

        #region Truncate

        public bool TruncateCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.TruncateCollection(id);
        }

        public bool TruncateCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.TruncateCollection(name);
        }

        #endregion

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
