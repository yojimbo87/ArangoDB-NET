using System;

namespace Arango.Client
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ArangoProperty : Attribute
    {
        public bool Serializable { get; set; }
        public bool Identity { get; set; }
        public bool Key { get; set; }
        public bool Revision { get; set; }
        public bool From { get; set; }
        public bool To { get; set; }
        public string Alias { get; set; }
        
        public ArangoProperty()
        {
            Serializable = true;
            Identity = false;
            Key = false;
            Revision = false;
            From = false;
            To = false;
        }
    }
}
