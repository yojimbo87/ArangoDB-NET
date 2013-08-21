using System.Reflection;

namespace Arango.Client
{
    public class ArangoDocument
    {        
        public string Id { get; set; }
        public string Key { get; set; }
        public string Revision { get; set; }
        public Document Document = new Document();
        
        /// <summary>
        /// Creates document.
        /// </summary>
        public ArangoDocument() 
        {
        }
        
        /// <summary>
        /// Creates document from specified JSON string.
        /// </summary>
        public ArangoDocument(string json)
        {
            Document = Document.Deserialize(json);
        }
        
        /// <summary>
        /// Serializes document data to JSON string.
        /// </summary>
        public string Serialize()
        {
            return Document.Serialize(Document);
        }
        
        /// <summary>
        /// Retrieves specified field from document. Can be complex path, e.g. foo.bar.baz
        /// </summary>
        public T GetField<T>(string fieldPath)
        {
            return Document.GetField<T>(fieldPath);
        }
        
        /// <summary>
        /// Sets specified value to specified field in document.
        /// </summary>
        public ArangoDocument SetField<T>(string fieldPath, T value)
        {
            Document.SetField(fieldPath, value);
            
            return this;
        }
        
        /// <summary>
        /// Removes specified field from document.
        /// </summary>
        public ArangoDocument RemoveField(string fieldPath)
        {
            Document.RemoveField(fieldPath);
            
            return this;
        }
        
        /// <summary>
        /// Checks if the specified field is present in document.
        /// </summary>
        public bool HasField(string fieldPath)
        {
            return Document.HasField(fieldPath);
        }
        
        /// <summary>
        /// Maps ArangoDB document specific attributes (_id, _key, _rev, _from, _to) from document to specified object.
        /// </summary>
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
                }
            }
        }
        
        /// <summary>
        /// Maps ArangoDB document specific attributes (_id, _key, _rev, _from, _to) specified object to document.
        /// </summary>
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
                }
            }
        }
    }
}

