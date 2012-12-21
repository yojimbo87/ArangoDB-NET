using System.Net;

namespace Arango.Client.Protocol
{
    internal class Document
    {
        private string ApiUri { get { return "_api/document/"; } }
        private ArangoNode Node { get; set; }

        internal Document(ArangoNode node)
        {
            Node = node;
        }

        internal ArangoDocument Get(string id, string revision)
        {
            var requestData = new RequestData();
            requestData.RelativeUri = ApiUri + id;
            requestData.Method = RequestMethod.GET.ToString();

            if (!string.IsNullOrEmpty(revision))
            {
                requestData.Headers.Add("If-None-Match", "\"" + revision + "\"");
            }

            var responseData = Node.Process(requestData);

            var document = new ArangoDocument();
            document.ID = id;
            document.Data = responseData.Content;

            switch (responseData.StatusCode)
            {
                case HttpStatusCode.OK:
                    document.Revision = responseData.Headers.Get("etag");
                    break;
                case HttpStatusCode.NotModified:
                    document.Revision = responseData.Headers.Get("etag");
                    break;
                default:
                    break;
            }

            return document;
        }
    }
}
