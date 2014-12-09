using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ArangoEdge
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        
        internal ArangoEdge(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        public ArangoEdge CreateCollection(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.CreateCollection, value.ToString().ToLower());
        	
        	return this;
        }
        
        public ArangoEdge IfMatch(string revision)
        {
            _parameters.String(ParameterName.IfMatch, revision);
        	
        	return this;
        }
        
        public ArangoEdge IfMatch(string revision, ArangoUpdatePolicy updatePolicy)
        {
            _parameters.String(ParameterName.IfMatch, revision);
            // needs to be string value
            _parameters.String(ParameterName.Policy, updatePolicy.ToString().ToLower());
        	
        	return this;
        }
        
        public ArangoEdge IfNoneMatch(string revision)
        {
            _parameters.String(ParameterName.IfNoneMatch, revision);
        	
        	return this;
        }
        
        public ArangoEdge WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
        	return this;
        }
        
        #endregion
        
        #region Get (GET)
        
        public ArangoResult<Dictionary<string, object>> Get(string handle)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Edge, "/" + handle);
            
            // optional: conditionally fetch a document based on a target revision
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: If revision is different -> HTTP 200. If revision is identical -> HTTP 304.
            request.TrySetHeaderParameter(ParameterName.IfNoneMatch, _parameters);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>);
                    }
                    break;
                case 412:
                    if (response.DataType == DataType.Document)
                    {
                        result.Value = (response.Data as Dictionary<string, object>);
                    }
                    break;
                case 304:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Create (POST)
        
        public ArangoResult<Dictionary<string, object>> Create(string collection, string fromHandle, string toHandle)
        {
            return Create(collection, fromHandle, toHandle, null);
        }
        
        public ArangoResult<Dictionary<string, object>> Create(string collection, string fromHandle, string toHandle, Dictionary<string, object> document)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Edge, "");
            
            // required: target collection name
            request.QueryString.Add(ParameterName.Collection, collection);
            // required: the document handle of the start point must be passed in from handle
            request.QueryString.Add(ParameterName.From, fromHandle);
            // required: the document handle of the end point must be passed in to handle
            request.QueryString.Add(ParameterName.To, toHandle);
            // optional: determines if target collection should be created if it doesn't exist
            request.TrySetQueryStringParameter(ParameterName.CreateCollection, _parameters);
            // optional: wait until data are synchronised to disk
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // required: document to be created
            if (document == null)
            {
                document = new Dictionary<string, object>();
            }
            request.Body = JSON.ToJSON(document);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>);
                    }
                    break;
                case 400:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
    }
}
