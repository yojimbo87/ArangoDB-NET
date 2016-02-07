using System;
using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class AEdge
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        
        internal AEdge(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Determines whether collection should be created if it does not exist. Default value: false.
        /// </summary>
        public AEdge CreateCollection(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.CreateCollection, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public AEdge WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Conditionally operate on edge with specified revision.
        /// </summary>
        public AEdge IfMatch(string revision)
        {
            _parameters.String(ParameterName.IfMatch, revision);
        	
        	return this;
        }
        
        /// <summary>
        /// Conditionally operate on edge with specified revision and update policy.
        /// </summary>
        public AEdge IfMatch(string revision, AUpdatePolicy updatePolicy)
        {
            _parameters.String(ParameterName.IfMatch, revision);
            // needs to be string value
            _parameters.String(ParameterName.Policy, updatePolicy.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Conditionally operate on edge which current revision does not match specified revision.
        /// </summary>
        public AEdge IfNoneMatch(string revision)
        {
            _parameters.String(ParameterName.IfNoneMatch, revision);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether to keep any attributes from existing edge that are contained in the patch edge which contains null value. Default value: true.
        /// </summary>
        public AEdge KeepNull(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.KeepNull, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the value in the patch edge will overwrite the existing edge's value. Default value: true.
        /// </summary>
        public AEdge MergeObjects(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.MergeObjects, value.ToString().ToLower());
        	
        	return this;
        }
        
        #endregion
        
        #region Create (POST)
        
        /// <summary>
        /// Creates new edge within specified collection between two document vertices in current database context.
        /// </summary>
        public AResult<Dictionary<string, object>> Create(string collectionName, string fromID, string toID)
        {
            return Create(collectionName, fromID, toID, "{}");
        }
        
        /// <summary>
        /// Creates new edge with document data within specified collection between two document vertices in current database context.
        /// </summary>
        public AResult<Dictionary<string, object>> Create(string collectionName, string fromID, string toID, string json)
        {
            if (!ADocument.IsID(fromID))
            {
                throw new ArgumentException("Specified fromID value (" + fromID + ") has invalid format.");
            }
            
            if (!ADocument.IsID(toID))
            {
                throw new ArgumentException("Specified toID value (" + toID + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.POST, ApiBaseUri.Edge, "");
            
            // required
            request.QueryString.Add(ParameterName.Collection, collectionName);
            // required
            request.QueryString.Add(ParameterName.From, fromID);
            // required
            request.QueryString.Add(ParameterName.To, toID);
            // optional
            request.TrySetQueryStringParameter(ParameterName.CreateCollection, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);

            request.Body = json;
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
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
        
        /// <summary>
        /// Creates new edge with document data within specified collection between two document vertices in current database context.
        /// </summary>
        public AResult<Dictionary<string, object>> Create(string collectionName, string fromID, string toID, Dictionary<string, object> document)
        {
            return Create(collectionName, fromID, toID, JSON.ToJSON(document, ASettings.JsonParameters));
        }
        
        /// <summary>
        /// Creates new edge with document data within specified collection between two document vertices in current database context.
        /// </summary>
        public AResult<Dictionary<string, object>> Create<T>(string collectionName, string fromID, string toID, T obj)
        {
            return Create(collectionName, fromID, toID, JSON.ToJSON(obj, ASettings.JsonParameters));
        }
        
        #endregion
        
        #region Check (HEAD)
        
        /// <summary>
        /// Checks for existence of specified edge.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<string> Check(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.HEAD, ApiBaseUri.Edge, "/" + id);
            
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: If revision is different -> HTTP 200. If revision is identical -> HTTP 304.
            request.TrySetHeaderParameter(ParameterName.IfNoneMatch, _parameters);
            
            var response = _connection.Send(request);
            var result = new AResult<string>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if ((response.Headers["ETag"] ?? "").Trim().Length > 0)
                    {
                        result.Value = response.Headers["ETag"].Replace("\"", "");
                        result.Success = (result.Value != null);
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
        
        #region Get (GET)
        
        /// <summary>
        /// Retrieves specified edge.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Get(string id)
        {
            return Get<Dictionary<string, object>>(id);
        }
        
        /// <summary>
        /// Retrieves specified edge.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<T> Get<T>(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.GET, ApiBaseUri.Edge, "/" + id);
            
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional: If revision is different -> HTTP 200. If revision is identical -> HTTP 304.
            request.TrySetHeaderParameter(ParameterName.IfNoneMatch, _parameters);
            
            var response = _connection.Send(request);
            var result = new AResult<T>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<T>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 412:
                    body = response.ParseBody<T>();

                    result.Value = body;
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
        
        /// <summary>
        /// Retrieves list of edges from specified edge type collection to specified document vertex with given direction.
        /// </summary>
        public AResult<List<Dictionary<string, object>>> Get(string collectionName, string startVertexID, ADirection direction)
        {
            if (!ADocument.IsID(startVertexID))
            {
                throw new ArgumentException("Specified startVertexID value (" + startVertexID + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.GET, ApiBaseUri.Edges, "/" + collectionName);
            
            // required
            request.QueryString.Add(ParameterName.Vertex, startVertexID);
            // required
            request.QueryString.Add(ParameterName.Direction, direction.ToString().ToLower());
            
            var response = _connection.Send(request);
            var result = new AResult<List<Dictionary<string, object>>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    
                    if (result.Success)
                    {
                        result.Value = body.List<Dictionary<string, object>>("edges");
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
        
        /// <summary>
        /// Updates existing edge identified by its handle with new edge data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Update(string id, string json)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.PATCH, ApiBaseUri.Edge, "/" + id);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional
            if (_parameters.Has(ParameterName.IfMatch))
            {
                request.TrySetQueryStringParameter(ParameterName.Policy, _parameters);
            }
            // optional
            request.TrySetQueryStringParameter(ParameterName.KeepNull, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.MergeObjects, _parameters);

            request.Body = json;
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 412:
                    body = response.ParseBody<Dictionary<string, object>>();

                    result.Value = body;
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
        
        /// <summary>
        /// Updates existing edge identified by its handle with new edge data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Update(string id, Dictionary<string, object> document)
        {
            return Update(id, JSON.ToJSON(document, ASettings.JsonParameters));
        }
        
        /// <summary>
        /// Updates existing edge identified by its handle with new edge data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Update<T>(string id, T obj)
        {
            return Update(id, JSON.ToJSON(obj, ASettings.JsonParameters));
        }
        
        #endregion
        
        #region Replace (PUT)
        
        /// <summary>
        /// Completely replaces existing edge identified by its handle with new edge data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Replace(string id, string json)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Edge, "/" + id);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional
            if (_parameters.Has(ParameterName.IfMatch))
            {
                request.TrySetQueryStringParameter(ParameterName.Policy, _parameters);
            }

            request.Body = json;
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                case 202:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 412:
                    body = response.ParseBody<Dictionary<string, object>>();

                    result.Value = body;
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
        
        /// <summary>
        /// Completely replaces existing edge identified by its handle with new edge data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Replace(string id, Dictionary<string, object> document)
        {
            return Replace(id, JSON.ToJSON(document, ASettings.JsonParameters));
        }
        
        /// <summary>
        /// Completely replaces existing edge identified by its handle with new edge data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Replace<T>(string id, T obj)
        {
            return Replace(id, JSON.ToJSON(obj, ASettings.JsonParameters));
        }
        
        #endregion
        
        #region Delete (DELETE)
        
        /// <summary>
        /// Deletes specified edge.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Delete(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Edge, "/" + id);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional
            if (_parameters.Has(ParameterName.IfMatch))
            {
                request.TrySetQueryStringParameter(ParameterName.Policy, _parameters);
            }
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                case 202:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
                    result.Value = body;
                    break;
                case 412:
                    body = response.ParseBody<Dictionary<string, object>>();

                    result.Value = body;
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
