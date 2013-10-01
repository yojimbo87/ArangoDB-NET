
namespace Arango.Client
{
    /// <summary> 
    /// Format options for DateTime type fields.
    /// </summary>
    public enum DateTimeFormat
    {
        /// <summary> 
        /// Native DateTime object.
        /// </summary>
        DateTimeObject = 0,
        
        /// <summary> 
        /// ISO 8601 date string.
        /// </summary>
        Iso8601String = 1,
        
        /// <summary> 
        /// Unix timestamp 64bit integer value.
        /// </summary>
        UnixTimeStamp = 2
    }
}
