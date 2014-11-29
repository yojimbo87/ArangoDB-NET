using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoResult<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public T Value { get; set; }
        public ArangoError Error { get; set; }
        
        internal ArangoResult(Response response)
        {
            StatusCode = response.StatusCode;
            Error = response.Error;
        }
    }
}
