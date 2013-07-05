using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Arango.Client.Protocol;

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
    }
}

