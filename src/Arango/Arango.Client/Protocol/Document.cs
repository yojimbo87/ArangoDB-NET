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

        internal ArangoDocument Get(string handle, string revision)
        {
            var request = new Request();
            request.RelativeUri = ApiUri + handle;
            request.Method = RequestMethod.GET.ToString();

            if (!string.IsNullOrEmpty(revision))
            {
                request.Headers.Add("If-None-Match", "\"" + revision + "\"");
            }

            var responseData = Node.Process(request);

            var document = new ArangoDocument();
            document.Handle = handle;
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
