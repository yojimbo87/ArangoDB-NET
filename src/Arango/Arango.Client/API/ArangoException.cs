using System;
using System.Net;

namespace Arango.Client
{
    public class ArangoException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public ArangoException()
        {
        }

        public ArangoException(HttpStatusCode httpStatusCode, string message) 
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public ArangoException(HttpStatusCode httpStatusCode, string message, Exception inner)
            : base(message, inner)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
