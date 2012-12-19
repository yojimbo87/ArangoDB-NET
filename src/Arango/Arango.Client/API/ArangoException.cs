using System;
using System.Net;

namespace Arango.Client
{
    public class ArangoException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ErrorDocument { get; set; }

        public ArangoException()
        {
        }

        public ArangoException(HttpStatusCode httpStatusCode, string errorDocument, string message) 
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
            ErrorDocument = errorDocument;
        }

        public ArangoException(HttpStatusCode httpStatusCode, string errorDocument, string message, Exception inner)
            : base(message, inner)
        {
            HttpStatusCode = httpStatusCode;
            ErrorDocument = errorDocument;
        }
    }
}
