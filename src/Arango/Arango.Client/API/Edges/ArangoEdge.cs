using System.Reflection;

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
        
        public void MapAttributesTo<T>(T genericObject)
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
                    
                    if (arangoProperty.From)
                    {
                        propertyInfo.SetValue(genericObject, From, null);
                    }
                    
                    if (arangoProperty.To)
                    {
                        propertyInfo.SetValue(genericObject, To, null);
                    }
                }
            }
        }
        
        public void MapAttributesFrom<T>(T genericObject)
        {
            // get arango specific fields to generic object if it has properties flagged with attributes
            foreach (PropertyInfo propertyInfo in genericObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var arangoProperty = propertyInfo.GetCustomAttribute<ArangoProperty>();
                
                if (arangoProperty != null)
                {
                    if (arangoProperty.Identity)
                    {
                        Id = (string)propertyInfo.GetValue(genericObject);
                    }
                    
                    if (arangoProperty.Key)
                    {
                        Key = (string)propertyInfo.GetValue(genericObject);
                    }
                    
                    if (arangoProperty.Revision)
                    {
                        Revision = (string)propertyInfo.GetValue(genericObject);
                    }
                    
                    if (arangoProperty.From)
                    {
                        From = (string)propertyInfo.GetValue(genericObject);
                    }
                    
                    if (arangoProperty.To)
                    {
                        To = (string)propertyInfo.GetValue(genericObject);
                    }
                }
            }
        }
    }
}
