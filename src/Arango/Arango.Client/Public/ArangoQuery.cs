using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ArangoQuery
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        StringBuilder _query = new StringBuilder();
        Dictionary<string, object> _bindVars = new Dictionary<string, object>();
        
        internal ArangoQuery(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        public ArangoQuery Aql(string query)
        {
        	var cleanQuery = CleanAqlString(query);
        	
        	if (_query.Length > 0)
        	{
        		_query.Append(" ");
        	}
        	
        	_query.Append(cleanQuery);
        	
            return this;
        }
        
        public ArangoQuery Count(bool value)
        {
        	_parameters.Bool(ParameterName.Count, value);
        	
        	return this;
        }
        
        public ArangoQuery BatchSize(int value)
        {
        	_parameters.Int(ParameterName.BatchSize, value);
        	
        	return this;
        }
        
        public ArangoQuery Ttl(int value)
        {
        	_parameters.Int(ParameterName.TTL, value);
        	
        	return this;
        }
        
        public ArangoQuery BindVar(string key, object value)
        {
            _bindVars.Object(key, value);
            
            return this;
        }
        
        #endregion
        
        #region ToDocument/ToDocuments

        public ArangoResult<Dictionary<string, object>> ToDocument()
        {
            var type = typeof(Dictionary<string, object>);
            var listResult = ToList();
            var result = new ArangoResult<Dictionary<string, object>>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success && (listResult.Value != null))
            {
                result.Value = (Dictionary<string, object>)Convert.ChangeType(listResult.Value[0], type);
            }
            
            return result;
        }
        
        public ArangoResult<List<Dictionary<string, object>>> ToDocuments()
        {
            var type = typeof(Dictionary<string, object>);
            var listResult = ToList();
            var result = new ArangoResult<List<Dictionary<string, object>>>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success && (listResult.Value != null))
            {
                result.Value = listResult.Value.Select(o => Convert.ChangeType(o, type)).Cast<Dictionary<string, object>>().ToList();
            }
            
            return result;
        }
        
        #endregion
        
        #region ToObject

        public ArangoResult<T> ToObject<T>()
        {
            var listResult = ToList<T>();
            var result = new ArangoResult<T>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success && (listResult.Value != null))
            {
                result.Value = (T)listResult.Value[0];
            }
            
            return result;
        }
        
        public ArangoResult<object> ToObject()
        {
            var listResult = ToList();
            var result = new ArangoResult<object>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success && (listResult.Value != null))
            {
                result.Value = listResult.Value[0];
            }
            
            return result;
        }
        
        #endregion
        
        #region ToList (POST)
        
        public ArangoResult<List<T>> ToList<T>()
        {
            var type = typeof(T);
            var listResult = ToList();
            var result = new ArangoResult<List<T>>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success && (listResult.Value != null))
            {
                if (type.IsPrimitive ||
        		    (type == typeof(string)) ||
	                (type == typeof(DateTime)) ||
	                (type == typeof(decimal)))
        		{
        			result.Value = listResult.Value.Select(o => Convert.ChangeType(o, type)).Cast<T>().ToList();
        		}
                else if (type.IsClass && (type.Name != "String"))
                {
                    result.Value = listResult.Value.Select(o => ((Dictionary<string, object>)o).ToObject<T>()).ToList();
                }
                else
        		{
        			result.Value = listResult.Value.Select(o => Convert.ChangeType(o, type)).Cast<T>().ToList();
        		}
            }
            
            return result;
        }
        
        public ArangoResult<List<object>> ToList()
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Cursor, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required: AQL query string
            bodyDocument.String(ParameterName.Query, _query.ToString());
            // optional: boolean flag that indicates whether the number of documents in the result set should be returned
            Request.TrySetBodyParameter(ParameterName.Count, _parameters, bodyDocument);
            // optional: maximum number of result documents to be transferred from the server to the client in one roundtrip
            Request.TrySetBodyParameter(ParameterName.BatchSize, _parameters, bodyDocument);
            // optional: time-to-live for the cursor (in seconds)
            Request.TrySetBodyParameter(ParameterName.TTL, _parameters, bodyDocument);
            // optional: key/value list of bind parameters
            if (_bindVars.Count > 0)
            {
                bodyDocument.Document(ParameterName.BindVars, _bindVars);
            }
            
            // TODO: options parameter
            
            request.Body = JSON.ToJSON(bodyDocument);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<List<object>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                    if (response.DataType == DataType.Document)
                    {
                        var responseDocument = (response.Data as Dictionary<string, object>);
                        
                        result.Success = (responseDocument != null);
                        
                        if (result.Success)
                        {
                            result.Value = new List<object>();
                            result.Value.AddRange(responseDocument.List<object>("result"));
                            result.Extra = responseDocument.CloneExcept("code", "error", "hasMore", "result");
                            
                            if (responseDocument.IsBool("hasMore") && responseDocument.Bool("hasMore"))
                            {
                                var cursorID = responseDocument.String("id");
                                var putResult = Put(cursorID);
                                
                                result.Success = putResult.Success;
                                result.StatusCode = putResult.StatusCode;
                                
                                if (putResult.Success)
                                {
                                    result.Value.AddRange(putResult.Value);
                                }
                                else
                                {
                                    result.Error = putResult.Error;
                                }
                            }
                        }
                    }
                    break;
                case 400:
                case 404:
                case 405:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            _bindVars.Clear();
            _query.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Parse (POST)

        public ArangoResult<Dictionary<string, object>> Parse(string query)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Query, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required: AQL query string
            bodyDocument.String(ParameterName.Query, CleanAqlString(query));
            
            request.Body = JSON.ToJSON(bodyDocument);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        result.Value = (response.Data as Dictionary<string, object>).CloneExcept("code", "error");
                        result.Success = (result.Value != null);
                    }
                    break;
                case 400:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            _bindVars.Clear();
            _query.Clear();
            
            return result;
        }
        
        #endregion
        
        #region PUT
        
        internal ArangoResult<List<object>> Put(string cursorID)
        {
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Cursor, "/" + cursorID);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<List<object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        var responseDocument = (response.Data as Dictionary<string, object>);
                        
                        result.Success = (responseDocument != null);
                        
                        if (result.Success)
                        {
                            result.Value = new List<object>();
                            result.Value.AddRange(responseDocument.List<object>("result"));
                            
                            if (responseDocument.IsBool("hasMore") && responseDocument.Bool("hasMore"))
                            {
                                var resultCursorID = responseDocument.String("id");
                                var putResult = Put(resultCursorID);
                                
                                result.Success = putResult.Success;
                                result.StatusCode = putResult.StatusCode;
                                
                                if (putResult.Success)
                                {
                                    result.Value.AddRange(putResult.Value);
                                }
                                else
                                {
                                    result.Error = putResult.Error;
                                }
                            }
                        }
                    }
                    break;
                case 400:
                case 404:
                default:
                    break;
            }
            
            return result;
        }
        
        #endregion
        
        #region Delete cursor (DELETE)

        public ArangoResult<bool> DeleteCursor(string cursorID)
        {
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Cursor, "/" + cursorID);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 202:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = true;
                    }
                    break;
                case 400:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            _bindVars.Clear();
            _query.Clear();
            
            return result;
        }
        
        #endregion
        
        public static string CleanAqlString(string dirtyQuery)
        {
        	var query = dirtyQuery.Replace("\r", "");
        	
        	var cleanQuery = new StringBuilder();

        	var lastAcceptedIndex = 0;
        	var startRejecting = true;
        	var acceptedLength = 0;
        	
        	for (int i = 0; i < query.Length; i++)
        	{
        		if (startRejecting)
        		{
        			if ((query[i] != '\n') && (query[i] != '\t') && (query[i] != ' '))
	        		{
        				
	    				lastAcceptedIndex = i;
	    				startRejecting = false;
	        		}
        		}
    			
        		if (!startRejecting)
    			{
        			if (query[i] == '\n')
	    			{
	    				cleanQuery.Append(query.Substring(lastAcceptedIndex, acceptedLength + 1));
	    				
	    				acceptedLength = 0;
	    				lastAcceptedIndex = i;
	    				startRejecting = true;
	    			}
        			else if (i == (query.Length - 1))
        			{
        				cleanQuery.Append(query.Substring(lastAcceptedIndex, acceptedLength + 1));
        			}
        			else
        			{
        				acceptedLength++;
        			}
    			}
        	}
        	
        	return cleanQuery.ToString();
        }
    }
}
