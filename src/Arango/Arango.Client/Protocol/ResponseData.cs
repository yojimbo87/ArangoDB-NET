using System.Net;

namespace Arango.Client.Protocol
{
    internal class ResponseData
    {
        internal HttpStatusCode StatusCode { get; set; }
        internal string Content { get; set; }
    }
}
