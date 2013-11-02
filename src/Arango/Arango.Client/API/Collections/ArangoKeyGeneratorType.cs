
namespace Arango.Client
{
    /// <summary> 
    /// Determines key generation type.
    /// </summary>
    public enum ArangoKeyGeneratorType
    {
        /// <summary>
        /// Auto-generates key values that are strings with ever-increasing numbers.
        /// </summary>
        Traditional = 1,
        
        /// <summary>
        /// Auto-generates deterministic key values. Both the start value and the increment value can be defined when the collection is created. The default start value is 0 and the default increment is 1.
        /// </summary>
        Autoincrement = 2
    }
}
