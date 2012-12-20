using System.Net;

namespace Arango.Client.Protocol
{
    internal class RequestData
    {
        internal string RelativeUri { get; set; }
        internal string Method { get; set; }
        internal WebHeaderCollection Headers { get; set; }

        internal RequestData()
        {
            Headers = new WebHeaderCollection();
        }
    }
}
