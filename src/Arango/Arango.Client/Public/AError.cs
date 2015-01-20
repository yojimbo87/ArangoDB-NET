using System;

namespace Arango.Client
{
    public class AEerror
    {
        /// <summary>
        /// Integer value of the operation response HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }
        
        /// <summary>
        /// Integer value indicating ArangoDB internal error code.
        /// </summary>
        public int Number { get; set; }
        
        /// <summary>
        /// String value containing error description.
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Exception object with further information about failure.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
