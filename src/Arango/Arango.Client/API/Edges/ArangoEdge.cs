
namespace Arango.Client
{
    public class ArangoEdge
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Revision { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public Document Document = new Document();
        
        public ArangoEdge() 
        {
        }
        
        public ArangoEdge(string jsonString)
        {
            Document.Deserialize(jsonString);
        }
        
        public string Serialize()
        {
            return Document.Serialize(Document);
        }
        
        public T GetField<T>(string fieldPath)
        {
            return Document.GetField<T>(fieldPath);
        }
        
        public ArangoEdge SetField<T>(string fieldPath, T value)
        {
            Document.SetField(fieldPath, value);
            
            return this;
        }
        
        public ArangoEdge RemoveField(string fieldPath)
        {
            Document.RemoveField(fieldPath);
            
            return this;
        }
        
        public bool HasField(string fieldPath)
        {
            return Document.HasField(fieldPath);
        }
    }
}
