using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoFunctionOperation
    {
        private FunctionOperation _functionOperation;
        
        internal ArangoFunctionOperation(FunctionOperation functionOperation)
        {
            _functionOperation = functionOperation;
        }
        
        #region Create
        
        public bool Create(string name, string code)
        {
            return _functionOperation.Post(name, code);
        }
        
        #endregion
        
        #region Replace
        
        public bool Replace(string name, string code)
        {
            return _functionOperation.Post(name, code);
        }
        
        #endregion
        
        #region Delete
        
        public bool Delete(string name)
        {
            return _functionOperation.Delete(name, null);
        }
        
        public bool Delete(string name, bool group)
        {
            return _functionOperation.Delete(name, group ? "true" : "false");
        }
        
        #endregion
    }
}
