using System.Reflection;

namespace Arango.Client
{
    public class ArangoDocument
    {        
        public string Id { get; set; }
        public string Key { get; set; }
        public string Revision { get; set; }
        public Document Document = new Document();
        
        public ArangoDocument() 
        {
        }
        
        public ArangoDocument(string jsonString)
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
        
        public ArangoDocument SetField<T>(string fieldPath, T value)
        {
            Document.SetField(fieldPath, value);
            
            return this;
        }
        
        public ArangoDocument RemoveField(string fieldPath)
        {
            Document.RemoveField(fieldPath);
            
            return this;
        }
        
        public bool HasField(string fieldPath)
        {
            return Document.HasField(fieldPath);
        }
        
        public void MapAttributes<T>(T genericObject)
        {
            // get arango specific fields to generic object if it has properties flagged with attributes
            foreach (PropertyInfo propertyInfo in genericObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var arangoProperty = propertyInfo.GetCustomAttribute<ArangoProperty>();
                
                if (arangoProperty != null)
                {
                    if (arangoProperty.Identity)
                    {
                        propertyInfo.SetValue(genericObject, Id, null);
                    }
                    
                    if (arangoProperty.Key)
                    {
                        propertyInfo.SetValue(genericObject, Key, null);
                    }
                    
                    if (arangoProperty.Revision)
                    {
                        propertyInfo.SetValue(genericObject, Revision, null);
                    }
                }
            }
        }
    }
}

