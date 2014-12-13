using System.Collections.Generic;
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
        
        #region ToList (POST)

        public ArangoResult<List<object>> ToList()
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Cursor, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required: 
            _parameters.String(ParameterName.Query, _query.ToString());
            Request.TrySetBodyParameter(ParameterName.Query, _parameters, bodyDocument);
            // optional:
            Request.TrySetBodyParameter(ParameterName.Count, _parameters, bodyDocument);
            // optional:
            Request.TrySetBodyParameter(ParameterName.BatchSize, _parameters, bodyDocument);
            // optional:
            Request.TrySetBodyParameter(ParameterName.TTL, _parameters, bodyDocument);
            // optional:
            if (_bindVars.Count > 0)
            {
                _parameters.Document(ParameterName.BindVars, _bindVars);
                Request.TrySetBodyParameter(ParameterName.Count, _parameters, bodyDocument);
            }
            
            request.Body = JSON.ToJSON(bodyDocument);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<List<object>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                    if (response.DataType == DataType.Document)
                    {
                        var responseDocument = (response.Data as Dictionary<string, object>);
                        
                        result.Success = true;
                        result.Value = new List<object>();
                        result.Value.AddRange(responseDocument.List<object>("result"));
                        
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
                        
                        result.Success = true;
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
                    break;
                case 400:
                case 404:
                default:
                    break;
            }
            
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
