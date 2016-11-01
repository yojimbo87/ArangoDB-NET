using System;﻿
using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ADocument
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        
        internal ADocument(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public ADocument WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Conditionally operate on document with specified revision.
        /// </summary>
        public ADocument IfMatch(string revision)
        {
            _parameters.String(ParameterName.IfMatch, revision);
        	
        	return this;
        }
        
        /// <summary>
        /// Conditionally operate on document which current revision does not match specified revision.
        /// </summary>
        public ADocument IfNoneMatch(string revision)
        {
            _parameters.String(ParameterName.IfNoneMatch, revision);
        	
        	return this;
        }

        /// <summary>
        /// Determines whether to '_rev' field in the given document is ignored. If this is set to false, then the '_rev' attribute given in the body document is taken as a precondition. The document is only replaced if the current revision is the one specified.
        /// </summary>
        public ADocument IgnoreRevs(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.IgnoreRevs, value.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Determines whether to keep any attributes from existing document that are contained in the patch document which contains null value. Default value: true.
        /// </summary>
        public ADocument KeepNull(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.KeepNull, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the value in the patch document will overwrite the existing document's value. Default value: true.
        /// </summary>
        public ADocument MergeObjects(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.MergeObjects, value.ToString().ToLower());
        	
        	return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete new document under the attribute 'new' in the result.
        /// </summary>
        public ADocument ReturnNew()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnNew, true.ToString().ToLower());

            return this;
        }

        /// <summary>
        /// Determines whether to return additionally the complete previous revision of the changed document under the attribute 'old' in the result.
        /// </summary>
        public ADocument ReturnOld()
        {
            // needs to be string value
            _parameters.String(ParameterName.ReturnOld, true.ToString().ToLower());

            return this;
        }

        #endregion

        #region Create (POST)

        /// <summary>
        /// Creates new document within specified collection in current database context.
        /// </summary>
        public AResult<Dictionary<string, object>> Create(string collectionName, string json)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Document, "/" + collectionName);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnNew, _parameters);

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
        /// Creates new document within specified collection in current database context.
        /// </summary>
        public AResult<Dictionary<string, object>> Create(string collectionName, Dictionary<string, object> document)
        {
            return Create(collectionName, JSON.ToJSON(document, ASettings.JsonParameters));
        }

        /// <summary>
        /// Creates new document within specified collection in current database context.
        /// </summary>
        public AResult<Dictionary<string, object>> Create<T>(string collectionName, T obj)
        {
            //return Create(collectionName, JSON.ToJSON(DictionaryExtensions.StripObject(obj), ASettings.JsonParameters));
            return Create(collectionName, Dictator.ToDocument(obj));
        }

        #endregion

        #region Create edge (POST)

        /// <summary>
        /// Creates new edge document with document data in current database context.
        /// </summary>
        /// <exception cref="ArgumentException">Specified document does not contain '_from' and '_to' fields.</exception>
        public AResult<Dictionary<string, object>> CreateEdge(string collectionName, Dictionary<string, object> document)
        {
            if (!document.Has("_from") && !document.Has("_to"))
            {
                throw new ArgumentException("Specified document does not contain '_from' and '_to' fields.");
            }

            return Create(collectionName, JSON.ToJSON(document, ASettings.JsonParameters));
        }

        /// <summary>
        /// Creates new edge document within specified collection between two document vertices in current database context.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'from' and 'to' ID values have invalid format.</exception>
        public AResult<Dictionary<string, object>> CreateEdge(string collectionName, string fromID, string toID)
        {
            if (!IsID(fromID))
            {
                throw new ArgumentException("Specified 'from' value (" + fromID + ") has invalid format.");
            }

            if (!IsID(toID))
            {
                throw new ArgumentException("Specified 'to' value (" + toID + ") has invalid format.");
            }

            var document = new Dictionary<string, object>
            {
                { "_from", fromID  },
                { "_to", toID  },
            };

            return Create(collectionName, document);
        }

        /// <summary>
        /// Creates new edge with document data within specified collection between two document vertices in current database context.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'from' and 'to' ID values have invalid format.</exception>
        public AResult<Dictionary<string, object>> CreateEdge(string collectionName, string fromID, string toID, Dictionary<string, object> document)
        {
            if (!IsID(fromID))
            {
                throw new ArgumentException("Specified 'from' value (" + fromID + ") has invalid format.");
            }

            if (!IsID(toID))
            {
                throw new ArgumentException("Specified 'to' value (" + toID + ") has invalid format.");
            }

            document.From(fromID);
            document.To(toID);

            return Create(collectionName, JSON.ToJSON(document, ASettings.JsonParameters));
        }

        /// <summary>
        /// Creates new edge with document data within specified collection between two document vertices in current database context.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'from' and 'to' ID values have invalid format.</exception>
        public AResult<Dictionary<string, object>> CreateEdge<T>(string collectionName, string fromID, string toID, T obj)
        {
            if (!IsID(fromID))
            {
                throw new ArgumentException("Specified 'from' value (" + fromID + ") has invalid format.");
            }

            if (!IsID(toID))
            {
                throw new ArgumentException("Specified 'to' value (" + toID + ") has invalid format.");
            }

            return CreateEdge(collectionName, fromID, toID, Dictator.ToDocument(obj));
        }

        #endregion

        #region Check (HEAD)

        /// <summary>
        /// Checks for existence of specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public AResult<string> Check(string id)
        {
            if (!IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.HEAD, ApiBaseUri.Document, "/" + id);
            
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
        /// Retrieves specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public AResult<T> Get<T>(string id)
        {
            if (!IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.GET, ApiBaseUri.Document, "/" + id);
            
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

        /// <summary>
        /// Retrieves specified document.
        /// </summary>
        public AResult<Dictionary<string, object>> Get(string id)
        {
            return Get<Dictionary<string, object>>(id);
        }

        #endregion

        #region Get in/out/any edges (GET)

        /// <summary>
        /// Retrieves list of edges from specified edge type collection to specified document vertex with given direction.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'startVertexID' value has invalid format.</exception>
        public AResult<List<Dictionary<string, object>>> GetEdges(string collectionName, string startVertexID, ADirection direction)
        {
            if (!IsID(startVertexID))
            {
                throw new ArgumentException("Specified 'startVertexID' value (" + startVertexID + ") has invalid format.");
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
        /// Updates existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Update(string id, string json)
        {
            if (!IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.PATCH, ApiBaseUri.Document, "/" + id);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.KeepNull, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.MergeObjects, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.IgnoreRevs, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnNew, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnOld, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);

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
        /// Updates existing document identified by its handle with new document data.
        /// </summary>
        public AResult<Dictionary<string, object>> Update(string id, Dictionary<string, object> document)
        {
            return Update(id, JSON.ToJSON(document, ASettings.JsonParameters));
        }
        
        /// <summary>
        /// Updates existing document identified by its handle with new document data.
        /// </summary>
        public AResult<Dictionary<string, object>> Update<T>(string id, T obj)
        {
            return Update(id, Dictator.ToDocument(obj));
        }
        
        #endregion
        
        #region Replace (PUT)
        
        /// <summary>
        /// Completely replaces existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Replace(string id, string json)
        {
            if (!IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Document, "/" + id);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.IgnoreRevs, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnNew, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnOld, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            
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
        /// Completely replaces existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Replace(string id, Dictionary<string, object> document)
        {
            return Replace(id, JSON.ToJSON(document, ASettings.JsonParameters));
        }
        
        /// <summary>
        /// Completely replaces existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Replace<T>(string id, T obj)
        {
            return Replace(id, Dictator.ToDocument(obj));
        }

        #endregion

        #region Replace edge (PUT)

        /// <summary>
        /// Completely replaces existing edge identified by its handle with new edge data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified document does not contain '_from' and '_to' fields.</exception>
        public AResult<Dictionary<string, object>> ReplaceEdge(string id, Dictionary<string, object> document)
        {
            if (!document.Has("_from") && !document.Has("_to"))
            {
                throw new ArgumentException("Specified document does not contain '_from' and '_to' fields.");
            }

            return Replace(id, JSON.ToJSON(document, ASettings.JsonParameters));
        }

        /// <summary>
        /// Completely replaces existing edge identified by its handle with new edge data. This helper method injects 'fromID' and 'toID' fields into given document to construct valid edge document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'from' or 'to' ID values have invalid format.</exception>
        public AResult<Dictionary<string, object>> ReplaceEdge(string id, string fromID, string toID, Dictionary<string, object> document)
        {
            if (!IsID(fromID))
            {
                throw new ArgumentException("Specified 'from' value (" + fromID + ") has invalid format.");
            }

            if (!IsID(toID))
            {
                throw new ArgumentException("Specified 'to' value (" + toID + ") has invalid format.");
            }

            document.From(fromID);
            document.To(toID);

            return Replace(id, JSON.ToJSON(document, ASettings.JsonParameters));
        }

        /// <summary>
        /// Completely replaces existing edge identified by its handle with new edge data. This helper method injects 'fromID' and 'toID' fields into given document to construct valid edge document.
        /// </summary>
        public AResult<Dictionary<string, object>> ReplaceEdge<T>(string id, string fromID, string toID, T obj)
        {
            return ReplaceEdge(id, fromID, toID, Dictator.ToDocument(obj));
        }

        #endregion

        #region Delete (DELETE)

        /// <summary>
        /// Deletes specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified 'id' value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Delete(string id)
        {
            if (!IsID(id))
            {
                throw new ArgumentException("Specified 'id' value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Document, "/" + id);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.ReturnOld, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            
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
        
        #region Static methods
        
        /// <summary>
        /// Determines if specified value has valid document `_id` format. 
        /// </summary>
        public static bool IsID(string id)
        {
            if (id.Contains("/"))
            {
                var split = id.Split('/');
                
                if ((split.Length == 2) && (split[0].Length > 0) && (split[1].Length > 0))
                {
                    return IsKey(split[1]);
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Determines if specified value has valid document `_key` format. 
        /// </summary>
        public static bool IsKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            
            return ASettings.KeyRegex.IsMatch(key);
        }
        
        /// <summary>
        /// Determines if specified value has valid document `_rev` format. 
        /// </summary>
        public static bool IsRev(string revision)
        {
            if (string.IsNullOrEmpty(revision))
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Constructs document ID from specified collection and key values.
        /// </summary>
        public static string Identify(string collection, long key)
        {
            if (string.IsNullOrEmpty(collection))
            {
                return null;
            }
            
            return collection + "/" + key;
        }
        
        /// <summary>
        /// Constructs document ID from specified collection and key values. If key format is invalid null value is returned.
        /// </summary>
        public static string Identify(string collection, string key)
        {
            if (string.IsNullOrEmpty(collection))
            {
                return null;
            }
            
            if (IsKey(key))
            {
                return collection + "/" + key;
            }
            
            return null;
        }
        
        /// <summary>
        /// Parses key value out of specified document ID. If ID has invalid value null is returned. 
        /// </summary>
        public static string ParseKey(string id)
        {
            if (id.Contains("/"))
            {
                var split = id.Split('/');
                
                if ((split.Length == 2) && (split[0].Length > 0) && (split[1].Length > 0))
                {
                    if (IsKey(split[1]))
                    {
                        return split[1];
                    }
                }
            }
            
            return null;
        }
        
        #endregion
    }
}
