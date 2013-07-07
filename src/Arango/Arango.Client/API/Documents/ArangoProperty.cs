using System;

namespace Arango.Client
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ArangoProperty : Attribute
    {
        public string Alias { get; set; }
        public bool Serializable { get; set; }
        
        public ArangoProperty()
        {
            Serializable = true;
        }
    }
}
