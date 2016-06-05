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
        /// Determines whether collection should be created if it does not exist. Default value: false.
        /// </summary>
        public ADocument CreateCollection(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.CreateCollection, value.ToString().ToLower());
        	
        	return this;
        }
        
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
        /// Conditionally operate on document with specified revision and update policy.
        /// </summary>
        public ADocument IfMatch(string revision, AUpdatePolicy updatePolicy)
        {
            _parameters.String(ParameterName.IfMatch, revision);
            // needs to be string value
            _parameters.String(ParameterName.Policy, updatePolicy.ToString().ToLower());
        	
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
        
        #endregion
        
        #region Create (POST)
        
        /// <summary>
        /// Creates new document within specified collection in current database context.
        /// </summary>
        public AResult<Dictionary<string, object>> Create(string collectionName, string json)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Document);
            
            // required
            request.QueryString.Add(ParameterName.Collection, collectionName);
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
        
        #region Check (HEAD)
        
        /// <summary>
        /// Checks for existence of specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<string> Check(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
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
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Get(string id)
        {
            return Get<Dictionary<string, object>>(id);
        }
        
        /// <summary>
        /// Retrieves specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<T> Get<T>(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
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
        
        #endregion
        
        #region Update (PATCH)
        
        /// <summary>
        /// Updates existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Update(string id, string json)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.PATCH, ApiBaseUri.Document, "/" + id);
            
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
        /// Updates existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Update(string id, Dictionary<string, object> document)
        {
            return Update(id, JSON.ToJSON(document, ASettings.JsonParameters));
        }
        
        /// <summary>
        /// Updates existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Update<T>(string id, T obj)
        {
            return Update(id, Dictator.ToDocument(obj));
        }
        
        #endregion
        
        #region Replace (PUT)
        
        /// <summary>
        /// Completely replaces existing document identified by its handle with new document data.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Replace(string id, string json)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Document, "/" + id);
            
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
        
        #region Delete (DELETE)
        
        /// <summary>
        /// Deletes specified document.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Delete(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Document, "/" + id);
            
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
            
            long outRev;

            return long.TryParse(revision, out outRev);
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
