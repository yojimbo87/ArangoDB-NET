using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoResult<T>
    {
        /// <summary>
        /// Determines whether the operation ended with success.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Integer value of the operation response HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }
        
        /// <summary>
        /// Generic object which type and value depends on performed operation.
        /// </summary>
        public T Value { get; set; }
        
        /// <summary>
        /// If operation ended with failure, this property would contain instance of ArangoError object which contains further information about the error.
        /// </summary>
        public ArangoError Error { get; set; }
        
        /// <summary>
        /// Document which might contain additional information on performed operation.
        /// </summary>
        public Dictionary<string, object> Extra { get; set; }
        
        internal ArangoResult() { }
        
        internal ArangoResult(Response response)
        {
            StatusCode = response.StatusCode;
            Error = response.Error;
        }
    }
}
