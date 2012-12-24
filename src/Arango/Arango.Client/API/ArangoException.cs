using System;
using System.Net;

namespace Arango.Client
{
    public class ArangoException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string WebExceptionMessage { get; set; }

        public ArangoException()
        {
        }

        public ArangoException(HttpStatusCode httpStatusCode, string message, string webExceptionMessage) 
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
            WebExceptionMessage = webExceptionMessage;
        }

        public ArangoException(HttpStatusCode httpStatusCode, string message, string webExceptionMessage, Exception inner)
            : base(message, inner)
        {
            HttpStatusCode = httpStatusCode;
            WebExceptionMessage = webExceptionMessage;
        }
    }
}
