using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoResult<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public T Value { get; set; }
        public ArangoError Error { get; set; }
        public Dictionary<string, object> Extra { get; set; }
        
        internal ArangoResult() { }
        
        internal ArangoResult(Response response)
        {
            StatusCode = response.StatusCode;
            Error = response.Error;
        }
    }
}
