using System;

namespace Arango.Client
{
    public class ArangoError
    {
        public int StatusCode { get; set; }
        public int Number { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
