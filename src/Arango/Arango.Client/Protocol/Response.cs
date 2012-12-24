using System.Dynamic;
using System.Net;

namespace Arango.Client.Protocol
{
    internal class Response
    {
        internal HttpStatusCode StatusCode { get; set; }
        internal WebHeaderCollection Headers { get; set; }
        internal string JsonString { get; set; }
        internal dynamic JsonObject { get; set; }

        internal Response()
        {
            JsonObject = new ExpandoObject();
        }
    }
}
