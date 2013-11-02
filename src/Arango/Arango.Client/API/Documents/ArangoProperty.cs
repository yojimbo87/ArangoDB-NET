using System;

namespace Arango.Client
{
    /// <summary> 
    /// Represents Arango specific properties which can be set to generic objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ArangoProperty : Attribute
    {
        /// <summary> 
        /// Determines if the object property could be serialized. Default is true.
        /// </summary>
        public bool Serializable { get; set; }
        
        /// <summary> 
        /// Determines if the object property could be deserialized. Default is true.
        /// </summary>
        public bool Deserializable { get; set; }
        
        /// <summary> 
        /// Determines if the object property is mapped to document identifier (_id field). Default is false.
        /// </summary>
        public bool Identity { get; set; }
        
        /// <summary> 
        /// Determines if the object property is mapped to document key (_key field). Default is false.
        /// </summary>
        public bool Key { get; set; }
        
        /// <summary> 
        /// Determines if the object property is mapped to document revision (_rev field). Default is false.
        /// </summary>
        public bool Revision { get; set; }
        
        /// <summary> 
        /// Determines if the object property is mapped to incoming edge identifier (_from field). Default is false.
        /// </summary>
        public bool From { get; set; }
        
        /// <summary> 
        /// Determines if the object property is mapped to outgoing edge identifier (_to field). Default is false.
        /// </summary>
        public bool To { get; set; }
        
        /// <summary> 
        /// Specifies object property alias which will be used in document serialization or deserialization.
        /// </summary>
        public string Alias { get; set; }
        
        public ArangoProperty()
        {
            Serializable = true;
            Deserializable = true;
            Identity = false;
            Key = false;
            Revision = false;
            From = false;
            To = false;
        }
    }
}
