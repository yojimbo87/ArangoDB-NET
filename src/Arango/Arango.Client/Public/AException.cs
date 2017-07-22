using System;

namespace Arango.Client
{
    /// <summary>
    /// Represents ArangoDB client specific exception which can happen when HTTP request returns status code indicating an error (e.g. 4xx or 5xx).
    /// </summary>
    public class AException : Exception
    {
        /// <summary>
        /// Integer value of the operation response HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Integer value indicating ArangoDB internal error code.
        /// </summary>
        public int Number { get; set; }

        public AException()
        {
        }

        public AException(string message) : base(message)
        {
        }

        public AException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
