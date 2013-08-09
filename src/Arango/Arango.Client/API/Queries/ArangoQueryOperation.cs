using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoQueryOperation
    {
        private CursorOperation _cursorOperation;
        
        private string _aql { get; set; }
        private int _batchSize { get; set; }
        private Dictionary<string, object> _bindVars { get; set; }
        
        internal ArangoQueryOperation(CursorOperation cursorOperation)
        {
            _cursorOperation = cursorOperation;
            _bindVars = new Dictionary<string, object>();
        }
        
        public ArangoQueryOperation AQL(string aql)
        {
            _aql = aql;
            
            return this;
        }
        
        public ArangoQueryOperation BatchSize(int batchSize)
        {
            _batchSize = batchSize;
            
            return this;
        }
        
        public ArangoQueryOperation AddVar(string key, object value)
        {
            _bindVars.Add(key, value);
            
            return this;
        }
        
        #region ToList
        
        public List<Document> ToList(out int count)
        {
            return _cursorOperation.Post(_aql, true, out count, _batchSize, _bindVars);
        }
        
        public List<Document> ToList()
        {
            int count = 0;
            
            return _cursorOperation.Post(_aql, false, out count, _batchSize, _bindVars);
        }
        
        public List<T> ToList<T>(out int count) where T : class, new()
        {
            List<Document> documents = _cursorOperation.Post(_aql, true, out count, _batchSize, _bindVars);
            List<T> genericCollection = new List<T>();
            
            foreach (Document document in documents)
            {
                T genericObject = document.To<T>();
                document.MapAttributesTo(genericObject);
                
                genericCollection.Add(genericObject);
            }
            
            return genericCollection;
        }
        
        public List<T> ToList<T>() where T : class, new()
        {
            int count = 0;
            List<Document> documents = _cursorOperation.Post(_aql, false, out count, _batchSize, _bindVars);
            List<T> genericCollection = new List<T>();
            
            foreach (Document document in documents)
            {
                T genericObject = document.To<T>();
                document.MapAttributesTo(genericObject);
                
                genericCollection.Add(genericObject);
            }
            
            return genericCollection;
        }
        
        #endregion
        
        public string ToString()
        {
            return _aql;
        }
    }
}
