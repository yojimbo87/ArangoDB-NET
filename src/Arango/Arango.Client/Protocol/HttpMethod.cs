using System;

namespace Arango.Client.Protocol
{
    internal static class HttpMethod
    {
        internal static string Get { get { return "GET"; } }
        internal static string Post { get { return "POST"; } }
        internal static string Put { get { return "PUT"; } }
        internal static string Delete { get { return "DELETE"; } }
        internal static string Head { get { return "HEAD"; } }
        internal static string Patch { get { return "PATCH"; } }
    }
}

