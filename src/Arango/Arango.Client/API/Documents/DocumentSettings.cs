using System;

namespace Arango.Client
{
    public class DocumentSettings
    {
        internal static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public DateTimeFormat DateTimeFormat { get; set; }
        
        public DocumentSettings()
        {
            DateTimeFormat = DateTimeFormat.DateTimeObject;
        }
    }
}
