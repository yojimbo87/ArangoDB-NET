using System.Net;

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
            return Document(handle, "");
        }

        public string Document(string handle, string revision)
        {
            WebHeaderCollection headers = new WebHeaderCollection();

            if (!string.IsNullOrEmpty(revision))
            {
                headers.Add("If-None-Match", "\"" + revision + "\"");
            }

            return _node.Request("_api/document/" + handle, HttpMethod.GET, headers);
        }
    }
}
