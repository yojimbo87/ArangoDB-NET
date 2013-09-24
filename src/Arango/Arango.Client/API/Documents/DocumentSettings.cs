using System;
using Newtonsoft.Json;

namespace Arango.Client
{
    public class DocumentSettings
    {
        internal static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public DateTimeFormat DateTimeFormat { get; set; }
        public static JsonSerializerSettings SerializerSettings { get; set; }
        
        public DocumentSettings()
        {
            DateTimeFormat = DateTimeFormat.DateTimeObject;
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.DateParseHandling = DateParseHandling.None;
        }
    }
}
