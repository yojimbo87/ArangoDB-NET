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

        public string GetDocument(string handle)
        {
            return GetDocument(handle, "");
        }

        public string GetDocument(string handle, string revision)
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
