using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary> 
    /// Expose AQL function management functionality.
    /// </summary>
    public class ArangoFunctionOperation
    {
        private FunctionOperation _functionOperation;
        
        internal ArangoFunctionOperation(FunctionOperation functionOperation)
        {
            _functionOperation = functionOperation;
        }
        
        #region Get
        
        /// <summary> 
        /// Retrieves list of all registered AQL user functions.
        /// </summary>
        public List<Document> Get()
        {
            return Get(null);
        }
        
        /// <summary> 
        /// Retrieves list of registered AQL user functions from specified namespace.
        /// </summary>
        /// <param name="namespace">Namespace from which will be AQL user functions retrieved.</param>
        public List<Document> Get(string @namespace)
        {
            return _functionOperation.Get(@namespace);
        }
        
        #endregion
        
        #region Create
        
        /// <summary> 
        /// Creates AQL user function with specified name and code.
        /// </summary>
        /// <param name="name">Name of the AQL user function.</param>
        /// <param name="code">String representation of function body.</param>
        public bool Create(string name, string code)
        {
            return _functionOperation.Post(name, code);
        }
        
        #endregion
        
        #region Replace
        
        /// <summary> 
        /// Replaces existing AQL user function with new one.
        /// </summary>
        /// <param name="name">Name of the AQL user function.</param>
        /// <param name="code">String representation of function body.</param>
        public bool Replace(string name, string code)
        {
            return _functionOperation.Post(name, code);
        }
        
        #endregion
        
        #region Delete
        
        /// <summary> 
        /// Deletes specified AQL user function.
        /// </summary>
        /// <param name="name">Name of the AQL user function to be deleted.</param>
        public bool Delete(string name)
        {
            return _functionOperation.Delete(name, null);
        }
        
        /// <summary> 
        /// Deletes specified AQL user function.
        /// </summary>
        /// <param name="name">Name of the AQL user function to be deleted.</param>
        /// <param name="group">If set to true, then the function name provided in name is treated as a namespace prefix, and all functions in the specified namespace will be deleted. If set to false, the function name provided in name must be fully qualified, including any namespaces.</param>
        public bool Delete(string name, bool group)
        {
            return _functionOperation.Delete(name, group ? "true" : "false");
        }
        
        #endregion
    }
}
