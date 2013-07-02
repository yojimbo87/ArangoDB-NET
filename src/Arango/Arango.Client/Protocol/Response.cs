using System.Net;

namespace Arango.Client.Protocol
{
    internal class Response
    {
        internal HttpStatusCode StatusCode { get; set; }
        internal WebHeaderCollection Headers { get; set; }
        internal string JsonString { get; set; }
        internal Document Document { get; set; }

        internal Response()
        {
            Document = new Document();
        }
    }
}

