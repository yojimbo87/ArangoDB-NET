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
    }
}
