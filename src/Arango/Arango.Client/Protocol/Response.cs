using System.Collections.Generic;
using System.Net;

namespace Arango.Client.Protocol
{
    internal class Response
    {
        internal HttpStatusCode StatusCode { get; set; }
        internal WebHeaderCollection Headers { get; set; }
        internal string JsonString { get; set; }
        internal Document Document { get; set; }
        internal List<Document> List { get; set; }
        internal bool IsException { get; set; }
    }
}

