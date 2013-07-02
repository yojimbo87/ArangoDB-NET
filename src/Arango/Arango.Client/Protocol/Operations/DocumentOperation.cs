using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client.Protocol
{
    internal class DocumentOperation
    {
        private string _apiUri { get { return "_api/document"; } }
        private Connection _connection { get; set; }

        internal DocumentOperation(Connection connection)
        {
            _connection = connection;
        }

        #region GET

        internal ArangoDocument Get(string id)
        {
            var request = new Request();
            request.RelativeUri = string.Join("/", _apiUri, id);
            request.Method = HttpMethod.Get;

            return Get(request);
        }

        private ArangoDocument Get(Request request)
        {
            var response = _connection.Process(request);
            ArangoDocument document = new ArangoDocument();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    document.Document = response.Document;
                    break;
                case HttpStatusCode.NotModified:
                    document.Revision = response.Headers.Get("etag").Replace("\"", "");
                    break;
                default:
                    break;
            }

            return document;
        }

        #endregion
    }
}

