using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class AQuery
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        readonly StringBuilder _query = new StringBuilder();
        readonly Dictionary<string, object> _bindVars = new Dictionary<string, object>();
        
        internal AQuery(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Sets AQL query code.
        /// </summary>
        public AQuery Aql(string query)
        {
        	var cleanQuery = Minify(query);
        	
        	if (_query.Length > 0)
        	{
        		_query.Append(" ");
        	}
        	
        	_query.Append(cleanQuery);
        	
            return this;
        }
        
        /// <summary>
        /// Maps key/value bind parameter to the AQL query.
        /// </summary>
        public AQuery BindVar(string key, object value)
        {
            _bindVars.Object(key, value);
            
            return this;
        }
        
        /// <summary>
        /// Determines whether the number of retrieved documents should be returned in `Extra` property of `AResult` instance. Default value: false.
        /// </summary>
        public AQuery Count(bool value)
        {
        	_parameters.Bool(ParameterName.Count, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the number of documents in the result set should be returned. Default value: false.
        /// </summary>
        public AQuery Ttl(int value)
        {
        	_parameters.Int(ParameterName.TTL, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines maximum number of result documents to be transferred from the server to the client in one roundtrip. If not set this value is server-controlled.
        /// </summary>
        public AQuery BatchSize(int value)
        {
        	_parameters.Int(ParameterName.BatchSize, value);
        	
        	return this;
        }
        
        #endregion
        
        #region Retrieve list result (POST)
        
        /// <summary>
        /// Retrieves result value as list of documents.
        /// </summary>
        public AResult<List<Dictionary<string, object>>> ToDocuments()
        {
            var type = typeof(Dictionary<string, object>);
            var listResult = ToList();
            var result = new AResult<List<Dictionary<string, object>>>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success)
            {
                result.Value = listResult.Value.Select(o => Convert.ChangeType(o, type)).Cast<Dictionary<string, object>>().ToList();
            }
            
            return result;
        }
        
        /// <summary>
        /// Retrieves result value as list of generic objects.
        /// </summary>
        public AResult<List<T>> ToList<T>()
        {
            var type = typeof(T);
            var listResult = ToList();
            var result = new AResult<List<T>>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success)
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
        
        /// <summary>
        /// Retrieves result value as list of objects.
        /// </summary>
        public AResult<List<object>> ToList()
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Cursor, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required
            bodyDocument.String(ParameterName.Query, _query.ToString());
            // optional
            Request.TrySetBodyParameter(ParameterName.Count, _parameters, bodyDocument);
            // optional
            Request.TrySetBodyParameter(ParameterName.BatchSize, _parameters, bodyDocument);
            // optional
            Request.TrySetBodyParameter(ParameterName.TTL, _parameters, bodyDocument);
            // optional
            if (_bindVars.Count > 0)
            {
                bodyDocument.Document(ParameterName.BindVars, _bindVars);
            }
            
            // TODO: options parameter
            
            request.Body = JSON.ToJSON(bodyDocument, ASettings.JsonParameters);
            
            var response = _connection.Send(request);
            var result = new AResult<List<object>>(response);
            
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
        
        #region Retrieve single result (POST)

        /// <summary>
        /// Retrieves result value as single document.
        /// </summary>
        public AResult<Dictionary<string, object>> ToDocument()
        {
            var type = typeof(Dictionary<string, object>);
            var listResult = ToList();
            var result = new AResult<Dictionary<string, object>>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success)
            {
                result.Value = (Dictionary<string, object>)Convert.ChangeType(listResult.Value[0], type);
            }
            
            return result;
        }
        
        /// <summary>
        /// Retrieves result value as single generic object.
        /// </summary>
        public AResult<T> ToObject<T>()
        {
            var listResult = ToList<T>();
            var result = new AResult<T>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success)
            {
                result.Value = (T)listResult.Value[0];
            }
            
            return result;
        }
        
        /// <summary>
        /// Retrieves result value as single object.
        /// </summary>
        public AResult<object> ToObject()
        {
            var listResult = ToList();
            var result = new AResult<object>();
            
            result.StatusCode = listResult.StatusCode;
            result.Success = listResult.Success;
            result.Extra = listResult.Extra;
            result.Error = listResult.Error;
            
            if (listResult.Success)
            {
                result.Value = listResult.Value[0];
            }
            
            return result;
        }
        
        #endregion
        
        #region More results in cursor (PUT)
        
        internal AResult<List<object>> Put(string cursorID)
        {
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Cursor, "/" + cursorID);
            
            var response = _connection.Send(request);
            var result = new AResult<List<object>>(response);
            
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
        
        #region Parse (POST)

        /// <summary>
        /// Analyzes specified AQL query.
        /// </summary>
        public AResult<Dictionary<string, object>> Parse(string query)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Query, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required
            bodyDocument.String(ParameterName.Query, Minify(query));
            
            request.Body = JSON.ToJSON(bodyDocument, ASettings.JsonParameters);
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
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
        
        /// <summary>
        /// Transforms specified query into minified version with removed leading and trailing whitespaces except new line characters.
        /// </summary>
        public static string Minify(string inputQuery)
        {
        	var query = inputQuery.Replace("\r", "");
        	
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
        
        #region Delete cursor (DELETE)

        /// <summary>
        /// Deletes specified AQL query cursor.
        /// </summary>
        public AResult<bool> DeleteCursor(string cursorID)
        {
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Cursor, "/" + cursorID);
            
            var response = _connection.Send(request);
            var result = new AResult<bool>(response);
            
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
    }
}
