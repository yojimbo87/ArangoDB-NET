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

        public ArangoCollection GetCollection(int id)
        {
            var collection = new Collection(_node);

            return collection.Get(id);
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
