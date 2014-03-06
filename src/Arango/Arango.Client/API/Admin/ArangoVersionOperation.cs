using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary> 
    /// Expose Arango version functionality.
    /// </summary>
    public class ArangoVersionOperation
    {
        private VersionOperation _versionOperation;
        
        internal ArangoVersionOperation(VersionOperation versionOperation)
        {
            _versionOperation = versionOperation;
        }
        
        #region Get
        
        /// <summary> 
        /// Retrieves the version of the server
        /// </summary>
        public ArangoVersion Get()
        {
        	return _versionOperation.Get();
        }
                
        #endregion
    }
}
