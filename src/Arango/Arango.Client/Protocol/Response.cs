using System.Dynamic;
using System.Net;

namespace Arango.Client.Protocol
{
    internal class Response
    {
        internal HttpStatusCode StatusCode { get; set; }
        internal WebHeaderCollection Headers { get; set; }
        internal string Json { get; set; }
        internal dynamic Object { get; set; }

        internal Response()
        {
            Object = new ExpandoObject();
        }
    }
}
