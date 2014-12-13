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
        
        public ArangoEdge KeepNull(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.KeepNull, value.ToString().ToLower());
        	
        	return this;
        }
        
        public ArangoEdge MergeArrays(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.MergeArrays, value.ToString().ToLower());
        	
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
        
        #region Get in/out/any (GET)
        
        public ArangoResult<List<Dictionary<string, object>>> Get(string collectionName, string startVertexID, ArangoDirection direction)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Edges, "/" + collectionName);
            
            // required: the id of the start vertex
            request.QueryString.Add(ParameterName.Vertex, startVertexID);
            // required: selects in or out direction for edges. If not set, any edges are returned.
            request.QueryString.Add(ParameterName.Direction, direction.ToString().ToLower());
            
            var response = _connection.Send(request);
            var result = new ArangoResult<List<Dictionary<string, object>>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>).List<Dictionary<string, object>>("edges");
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
        
        #region Update (PATCH)
        
        public ArangoResult<Dictionary<string, object>> Update(string handle, Dictionary<string, object> document)
        {
            var request = new Request(HttpMethod.PATCH, ApiBaseUri.Edge, "/" + handle);
            
            // optional: conditionally update a document based on a target revision id
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: if revision was provided - check the presence of update policy parameter
            if (_parameters.Has(ParameterName.IfMatch))
            {
                request.TrySetQueryStringParameter(ParameterName.Policy, _parameters);
            }
            // optional: remove any attributes from the existing document that are contained in the patch document 
            // with an attribute value of null
            request.TrySetQueryStringParameter(ParameterName.KeepNull, _parameters);
            // optional: If set to false, the value in the patch document will overwrite the existing document's value. 
            // If set to true, arrays will be merged.
            request.TrySetQueryStringParameter(ParameterName.MergeArrays, _parameters);
            // optional: wait until data are synchronised to disk
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // required: serialize updating document
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
                case 412:
                    if (response.DataType == DataType.Document)
                    {
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
        
        #region Replace (PUT)
        
        public ArangoResult<Dictionary<string, object>> Replace(string handle, Dictionary<string, object> document)
        {
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Edge, "/" + handle);
            
            // optional: conditionally replace a document based on a target revision id
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: if revision was provided - check the presence of update policy parameter
            if (_parameters.Has(ParameterName.IfMatch))
            {
                request.TrySetQueryStringParameter(ParameterName.Policy, _parameters);
            }
            // optional: wait until data are synchronised to disk
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // required: serialize replacing document
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
                case 412:
                    if (response.DataType == DataType.Document)
                    {
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
        
        #region Delete (DELETE)
        
        public ArangoResult<Dictionary<string, object>> Delete(string handle)
        {
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Edge, "/" + handle);
            
            // optional: conditionally replace a document based on a target revision id
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: if revision was provided - check the presence of update policy parameter
            if (_parameters.Has(ParameterName.IfMatch))
            {
                request.TrySetQueryStringParameter(ParameterName.Policy, _parameters);
            }
            // optional: wait until data are synchronised to disk
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                case 202:
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
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Check (HEAD)
        
        public ArangoResult<string> Check(string handle)
        {
            var request = new Request(HttpMethod.HEAD, ApiBaseUri.Edge, "/" + handle);
            
            // optional: conditionally fetch a document based on a target revision
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: If revision is different -> HTTP 200. If revision is identical -> HTTP 304.
            request.TrySetHeaderParameter(ParameterName.IfNoneMatch, _parameters);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<string>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if ((response.Headers["ETag"] ?? "").Trim().Length > 0)
                    {
                        result.Success = true;
                        result.Value = response.Headers["ETag"].Replace("\"", "");
                    }
                    break;
                case 304:
                case 412:
                    if ((response.Headers["ETag"] ?? "").Trim().Length > 0)
                    {
                        result.Value = response.Headers["ETag"].Replace("\"", "");
                    }
                    break;
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
