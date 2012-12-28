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

        #region GET

        internal ArangoDocument Get(string handle)
        {
            var request = new Request();
            request.RelativeUri = ApiUri + handle;
            request.Method = RequestMethod.GET.ToString();

            return Get(request);
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

            return Get(request);
        }

        private ArangoDocument Get(Request request)
        {
            var response = Node.Process(request);

            var document = new ArangoDocument();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    document.JsonObject = new JsonParser().Deserialize(response.JsonString);
                    document.Handle = document.JsonObject._id;
                    document.Revision = response.Headers.Get("etag");
                    break;
                case HttpStatusCode.NotModified:
                    document.Revision = response.Headers.Get("etag");
                    break;
                default:
                    break;
            }

            return document;
        }

        #endregion
    }
}
