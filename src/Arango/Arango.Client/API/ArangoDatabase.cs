using System;
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

        public string Document(string handle)
        {
            return _node.Request("_api/document/" + handle, HttpMethod.GET);
        }
    }
}
