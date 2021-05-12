using System;

namespace Arango.Tests.Entities
{
    public class IssueNo9Entity
    {
        public Guid SomeOtherId { get; set; }

        public string Name { get; set; }
    
        public Color MyFavoriteColor { get; set; }
        
        public enum Color
        {
            Blue, Red
        }
    }
}
