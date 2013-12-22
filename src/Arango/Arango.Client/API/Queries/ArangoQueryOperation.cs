using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary> 
    /// Expose AQL querying functionality.
    /// </summary>
    public class ArangoQueryOperation
    {
        private CursorOperation _cursorOperation;
        
        private int _batchSize = 0;
        private Dictionary<string, object> _bindVars = new Dictionary<string, object>();
        private string _aql = "";
        
        internal ArangoQueryOperation(CursorOperation cursorOperation)
        {
            _cursorOperation = cursorOperation;
        }
        
        /// <summary> 
        /// Appends AQL query.
        /// </summary>
        /// <param name="aql">AQL query to be appended.</param>
        public ArangoQueryOperation Aql(string aql)
        {
            _aql += aql;
            
            return this;
        }
        
        /// <summary> 
        /// Specifies maximum number of result documents to be transferred from the server to the client in one roundtrip.
        /// </summary>
        /// <param name="aql">Size of the batch being transferred in one roundtrip.</param>
        public ArangoQueryOperation BatchSize(int batchSize)
        {
            _batchSize = batchSize;
            
            return this;
        }
        
        /// <summary> 
        /// Specifies bind parameter and it's value.
        /// </summary>
        /// <param name="key">Key of the bind parameter. '@' sign will be added automatically.</param>
        /// <param name="value">Value of the bind parameter.</param>
        public ArangoQueryOperation AddParameter(string key, object value)
        {
            _bindVars.Add(key, value);
            
            return this;
        }
        
        #region ToList
        
        /// <summary> 
        /// Executes AQL query and returns list of documents.
        /// </summary>
        /// <param name="count">Variable where will be stored total number of result documents available after execution.</param>
        public List<Document> ToList(out int count)
        {
            var items = _cursorOperation.Post(_aql.ToString(), true, out count, _batchSize, _bindVars);
            
            return items.Cast<Document>().ToList();
        }
        
        /// <summary> 
        /// Executes AQL query and returns list of documents.
        /// </summary>
        public List<Document> ToList()
        {
            var count = 0;
            var items = _cursorOperation.Post(_aql.ToString(), false, out count, _batchSize, _bindVars);
            
            return items.Cast<Document>().ToList();
        }
        
        /// <summary> 
        /// Executes AQL query and returns list of objects.
        /// </summary>
        /// <param name="count">Variable where will be stored total number of result objects available after execution.</param>
        public List<T> ToList<T>(out int count) where T: class, new()
        {
            var type = typeof(T);
            var items = _cursorOperation.Post(_aql.ToString(), true, out count, _batchSize, _bindVars);
            var genericCollection = new List<T>();

            if (type.IsPrimitive ||
                (type == typeof(string)) ||
                (type == typeof(DateTime)) ||
                (type == typeof(decimal)))
            {
                foreach (object item in items)
                {
                    genericCollection.Add((T)Convert.ChangeType(item, type));
                }
            }
            else if (type == typeof(Document))
            {
                genericCollection = items.Cast<T>().ToList();
            }
            else
            {
                foreach (object item in items)
                {
                    var document = (Document)item;
                    var genericObject = Activator.CreateInstance(type);
                    
                    document.ToObject(genericObject);
                    document.MapAttributesTo(genericObject);
                    
                    genericCollection.Add((T)genericObject);
                }
            }
            
            return genericCollection;
        }
        
        /// <summary> 
        /// Executes AQL query and returns list of objects.
        /// </summary>
        public List<T> ToList<T>()
        {
            var type = typeof(T);
            var count = 0;
            var items = _cursorOperation.Post(_aql.ToString(), false, out count, _batchSize, _bindVars);
            var genericCollection = new List<T>();
            
            if (type.IsPrimitive ||
                (type == typeof(string)) ||
                (type == typeof(DateTime)) ||
                (type == typeof(decimal)))
            {
                foreach (object item in items)
                {
                    genericCollection.Add((T)Convert.ChangeType(item, type));
                }
            }
            else if (type == typeof(Document))
            {
                genericCollection = items.Cast<T>().ToList();
            }
            else
            {
                foreach (object item in items)
                {
                    var document = (Document)item;
                    var genericObject = Activator.CreateInstance(type);
                    
                    document.ToObject(genericObject);
                    document.MapAttributesTo(genericObject);
                    
                    genericCollection.Add((T)genericObject);
                }
            }
            
            return genericCollection;
        }
        
        #endregion
        
        #region ToObject
        
        /// <summary> 
        /// Executes AQL query and returns first document available in the result list.
        /// </summary>
        public Document ToObject()
        {
            return ToList().FirstOrDefault();
        }
        
        /// <summary> 
        /// Executes AQL query and returns first object available in the result list.
        /// </summary>
        public T ToObject<T>()
        {
            return ToList<T>().FirstOrDefault();
        }
        
        #endregion
        
        #region ToString
        
        /// <summary> 
        /// Returns AQL query string.
        /// </summary>
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
        
        #endregion
    }
}
