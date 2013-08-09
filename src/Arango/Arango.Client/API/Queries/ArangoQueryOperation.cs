using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoQueryOperation
    {
        private CursorOperation _cursorOperation;
        
        private string _aql  = "";
        private int _batchSize = 0;
        private Dictionary<string, object> _bindVars = new Dictionary<string, object>();
        
        internal ArangoQueryOperation(CursorOperation cursorOperation)
        {
            _cursorOperation = cursorOperation;
        }
        
        public ArangoQueryOperation()
        {
        }
        
        public ArangoQueryOperation Aql(string aql)
        {
            _aql += aql;
            
            return this;
        }
        
        public ArangoQueryOperation BatchSize(int batchSize)
        {
            _batchSize = batchSize;
            
            return this;
        }
        
        public ArangoQueryOperation AddParameter(string key, object value)
        {
            _bindVars.Add(key, value);
            
            return this;
        }
        
        #region AQL generator
        
        // TODO: nester for should be enclosed in brackets
        public ArangoQueryOperation For(string variable, string expression)
        {
            if (_aql.Length == 0)
            {
                _aql = AQL.For;
                
                Join(variable, AQL.In, expression);
            }
            else
            {
                Join(AQL.For, variable, AQL.In, expression);
            }
            
            return this;
        }
        
        public ArangoQueryOperation Filter(string variable)
        {
            Join(AQL.Filter, variable);
            
            return this;
        }
        
        public ArangoQueryOperation Equals<T>(T conditionValue)
        {
            Join(AQL.Equals, ToString(conditionValue));
            
            return this;
        }
        
        public ArangoQueryOperation And(string variable)
        {
            Join(AQL.And, variable);
            
            return this;
        }
        
        public ArangoQueryOperation Return(string variable)
        {
            Join(AQL.Return, variable);
            
            return this;
        }
        
        public ArangoQueryOperation From(string expression)
        {
            Join(AQL.From, expression);
            
            return this;
        }
        
        #endregion
        
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
        
        public override string ToString()
        {
            return _aql;
        }
        
        private string ToString(object value)
        {
            if (value is string)
            {
                return "'" + value + "'";
            }
            else
            {
                return value.ToString();
            }
        }
        
        private void Join(params string[] parts)
        {
            _aql += " " + string.Join(" ", parts);
        }
    }
}
