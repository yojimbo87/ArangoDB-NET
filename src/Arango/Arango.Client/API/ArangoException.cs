using System;
using System.Net;

namespace Arango.Client
{
    /// <summary> 
    /// Represents Arango specific exception.
    /// </summary>
    public class ArangoException : Exception
    {
        /// <summary>
        /// HTTP status code which caused the exception.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// Exception message of the web request object.
        /// </summary>
        public string WebExceptionMessage { get; set; }

        /// <summary>
        /// Creates Arango specific exception.
        /// </summary>
        public ArangoException()
        {
        }

        /// <summary>
        /// Creates Arango specific exception with specified message.
        /// </summary>
        /// <param name="message">Message which describes the error.</param>
        public ArangoException(string message) 
            : base(message)
        {
        }
        
        /// <summary>
        /// Creates Arango specific exception with specified parameters.
        /// </summary>
        /// <param name="httpStatusCode">HTTP status code of new exception.</param>
        /// <param name="message">Message which describes the error.</param>
        /// <param name="webExceptionMessage">Message of exception copied from web exception where it originated.</param>
        public ArangoException(HttpStatusCode httpStatusCode, string message, string webExceptionMessage) 
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
            WebExceptionMessage = webExceptionMessage;
        }

        /// <summary>
        /// Creates Arango specific exception with specified parameters.
        /// </summary>
        /// <param name="httpStatusCode">HTTP status code of new exception.</param>
        /// <param name="message">MMessage which describes the error.</param>
        /// <param name="webExceptionMessage">Message of exception copied from web exception where it originated.</param>
        /// <param name="innerException">Exception that is the cause of current exception.</param>
        public ArangoException(HttpStatusCode httpStatusCode, string message, string webExceptionMessage, Exception innerException)
            : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
            WebExceptionMessage = webExceptionMessage;
        }
    }
}

