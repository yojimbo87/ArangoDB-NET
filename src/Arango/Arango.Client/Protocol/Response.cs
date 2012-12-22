using System.Dynamic;
using System.Net;

namespace Arango.Client.Protocol
{
    internal class Response
    {
        internal HttpStatusCode StatusCode { get; set; }
        internal WebHeaderCollection Headers { get; set; }
        internal string Content { get; set; }
        internal dynamic Data { get; set; }

        internal Response()
        {
            Data = new ExpandoObject();
        }
    }
}
