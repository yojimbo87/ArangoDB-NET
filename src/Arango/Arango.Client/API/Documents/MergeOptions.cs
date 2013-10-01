
namespace Arango.Client
{
    /// <summary> 
    /// Options which are possible to select when merging fields of two documents.
    /// </summary>
    public enum MergeOptions
    {
        /// <summary> 
        /// Union values of intersecting fields.
        /// </summary>
        MergeFields,
        
        /// <summary> 
        /// Replace values of intersecting fields with content from second document.
        /// </summary>
        ReplaceFields,
        
        /// <summary> 
        /// Keep values of first document in intersecting fields.
        /// </summary>
        KeepFields
    }
}
