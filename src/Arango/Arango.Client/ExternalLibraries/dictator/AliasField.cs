using System;

namespace Arango.Client
{
    /// <summary>
    /// Specified alias will be used as field name to convert property to or from document format.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AliasField : Attribute
    {
        public string Alias { get; set; }
        
        public AliasField(string alias)
        {
            Alias = alias;
        }
    }
}
