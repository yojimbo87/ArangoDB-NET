using System;
using Newtonsoft.Json;

namespace Arango.Client
{
    /// <summary> 
    /// Global settings which can affect specific functionality of document objects.
    /// </summary>
    /// <remarks>
    /// By default DateTimeFormat is set to DateTimeObject and date formatted strings are not parsed to DateTime objects, but are read as strings.
    /// </remarks>
    public class DocumentSettings
    {
        /// <summary> 
        /// Unix time epoch from which will be unix timestamp calculated.
        /// </summary>
        internal static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        /// <summary> 
        /// Gets or sets format in which will be DateTime object stored.
        /// </summary>
        public DateTimeFormat DateTimeFormat { get; set; }
        
        /// <summary> 
        /// Settings of JSON serializer.
        /// </summary>
        public static JsonSerializerSettings SerializerSettings { get; set; }
        
        public DocumentSettings()
        {
            DateTimeFormat = DateTimeFormat.DateTimeObject;
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.DateParseHandling = DateParseHandling.None;
        }
    }
}
