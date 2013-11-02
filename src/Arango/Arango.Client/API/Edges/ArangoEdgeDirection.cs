
namespace Arango.Client
{
    /// <summary> 
    /// Determines direction of the edge between two vertices.
    /// </summary>
    public enum ArangoEdgeDirection
    {
        /// <summary> 
        /// Direction could be both inbound or outbound.
        /// </summary>
        Any,
        
        /// <summary> 
        /// Direction is inbound.
        /// </summary>
        In,
        
        /// <summary> 
        /// Direction is outbound.
        /// </summary>
        Out
    }
}
