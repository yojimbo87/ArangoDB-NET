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

        public string GetDocument(string handle)
        {
            return GetDocument(handle, "");
        }

        public string GetDocument(string handle, string revision)
        {
            Document document = new Document();
            document.RequestMethod = RequestMethod.GET;
            document.Handle = handle;
            document.Revision = revision;

            ResponseData response = document.Request(_node);

            return response.Content;
        }
    }
}
