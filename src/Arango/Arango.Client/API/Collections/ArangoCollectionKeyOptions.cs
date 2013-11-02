
namespace Arango.Client
{
    /// <summary> 
    /// Determines key options of the collection.
    /// </summary>
    public class ArangoCollectionKeyOptions
    {
        /// <summary> 
        /// Specifies key generator type.
        /// </summary>
        public ArangoKeyGeneratorType GeneratorType { get; set; }
        
        /// <summary> 
        /// Specifies key generator typeif set to true, then it is allowed to supply own key values in the _key attribute of a document. If set to false, then the key generator will solely be responsible for generating keys and supplying own key values in the _key attribute of documents is considered an error.
        /// </summary>
        public bool AllowUserKeys { get; set; }
        
        /// <summary>
        /// Increment value for autoincrement key generator. Not used for other key generator types.
        /// </summary>
        public int Increment { get; set; }
        
        /// <summary>
        /// Initial offset value for autoincrement key generator. Not used for other key generator types.
        /// </summary>
        public int Offset { get; set; }
    }
}
