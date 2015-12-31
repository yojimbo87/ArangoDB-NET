using System;
using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ATransaction
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        readonly List<string> _readCollections = new List<string>();
        readonly List<string> _writeCollections = new List<string>();
        readonly Dictionary<string, object> _transactionParams = new Dictionary<string, object>();
        
        internal ATransaction(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Maps read collection to current transaction.
        /// </summary>
        public ATransaction ReadCollection(string collectionName)
        {
            _readCollections.Add(collectionName);
        	
        	return this;
        }
        
        /// <summary>
        /// Maps write collection to current transaction.
        /// </summary>
        public ATransaction WriteCollection(string collectionName)
        {
            _writeCollections.Add(collectionName);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public ATransaction WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Determines a numeric value that can be used to set a timeout for waiting on collection locks. Setting value to 0 will make ArangoDB not time out waiting for a lock.
        /// </summary>
        public ATransaction LockTimeout(int value)
        {
            _parameters.Int(ParameterName.LockTimeout, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Maps key/value parameter to current transaction.
        /// </summary>
        public ATransaction Param(string key, object value)
        {
            _transactionParams.Object(key, value);
            
            return this;
        }
        
        #endregion
        
        /// <summary>
        /// Executes specified transaction.
        /// </summary>
        public AResult<T> Execute<T>(string action)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Transaction, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required
            bodyDocument.String(ParameterName.Action, action);
            // required
            if (_readCollections.Count > 0)
            {
                bodyDocument.List(ParameterName.Collections + ".read", _readCollections);
            }
            // required
            if (_writeCollections.Count > 0)
            {
                bodyDocument.List(ParameterName.Collections + ".write", _writeCollections);
            }
            // optional
            Request.TrySetBodyParameter(ParameterName.WaitForSync, _parameters, bodyDocument);
            // optional
            Request.TrySetBodyParameter(ParameterName.LockTimeout, _parameters, bodyDocument);
            // optional
            if (_transactionParams.Count > 0)
            {
                bodyDocument.Document(ParameterName.Params, _transactionParams);
            }

            request.Body = JSON.ToJSON(bodyDocument, ASettings.JsonParameters);
            
            var response = _connection.Send(request);
            var result = new AResult<T>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Body<T>>();
                    
                    result.Success = (body != null);
                    result.Value = body.Result;
                    break;
                case 400:
                case 404:
                case 409:
                case 500:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            _readCollections.Clear();
            _writeCollections.Clear();
            _transactionParams.Clear();
            
            return result;
        }
    }
}
