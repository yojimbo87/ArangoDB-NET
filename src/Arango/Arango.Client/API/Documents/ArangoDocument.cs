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
        public Document Document { get; set; }
        
        public string Id 
        { 
            get
            {
                return Document.GetField<string>("_id");
            }
            
            set 
            {
                Document.SetField("_id", value);
            }
        }
        
        public string Key
        { 
            get
            {
                return Document.GetField<string>("_key");
            }
            
            set 
            {
                Document.SetField("_key", value);
            }
        }
        
        public string Revision
        { 
            get
            {
                return Document.GetField<string>("_rev");
            }
            
            set 
            {
                Document.SetField("_rev", value);
            }
        }
        
        public ArangoDocument() {}
        
        public ArangoDocument(string jsonString)
        {
            Document.Deserialize(jsonString);
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
        
        public bool HasField(string fieldPath)
        {
            return Document.HasField(fieldPath);
        }
    }
}

