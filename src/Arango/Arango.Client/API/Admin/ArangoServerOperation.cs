using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary> 
    /// Expose Arango server functionality.
    /// </summary>
    public class ArangoServerOperation
    {
        private ServerOperation _serverOperation;
        
        internal ArangoServerOperation(ServerOperation serverOperation)
        {
            _serverOperation = serverOperation;
        }
        
        #region Role
        
        /// <summary> 
        /// Retrieves the server role
        /// </summary>
        public ArangoServerRole Role()
        {
        	return _serverOperation.Role();
        }
                
        #endregion
    }
}
