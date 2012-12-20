using System.Net;

namespace Arango.Client.Protocol
{
    internal class ResponseData
    {
        internal HttpStatusCode StatusCode { get; set; }
        internal WebHeaderCollection Headers { get; set; }
        internal string Content { get; set; }
    }
}
