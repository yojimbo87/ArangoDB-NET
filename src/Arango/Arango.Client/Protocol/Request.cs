using System.Net;

namespace Arango.Client.Protocol
{
    internal class Request
    {
        internal string RelativeUri { get; set; }
        internal string Method { get; set; }
        internal WebHeaderCollection Headers { get; set; }

        internal Request()
        {
            Headers = new WebHeaderCollection();
        }
    }
}
