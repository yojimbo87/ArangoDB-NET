namespace Arango.Client.Protocol
{
    public class Body<T>
    {
        // standard response fields
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public int Code { get; set; }
        public int ErrorNum { get; set; }
        public T Result { get; set; }
        
        // operation specific fields
        public string ID { get; set; }
        public long Count { get; set; }
        public bool HasMore { get; set; }
        public bool Cached { get; set; }
        public object Extra { get; set; }
    }
}
