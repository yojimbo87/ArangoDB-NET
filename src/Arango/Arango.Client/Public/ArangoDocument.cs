﻿using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ArangoDocument
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        
        internal ArangoDocument(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Determines whether collection should be created if it does not exist. Default value: false.
        /// </summary>
        public ArangoDocument CreateCollection(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.CreateCollection, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether or not to wait until data are synchronised to disk. Default value: false.
        /// </summary>
        public ArangoDocument WaitForSync(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.WaitForSync, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Conditionally operate on document with specified revision.
        /// </summary>
        public ArangoDocument IfMatch(string revision)
        {
            _parameters.String(ParameterName.IfMatch, revision);
        	
        	return this;
        }
        
        /// <summary>
        /// Conditionally operate on document with specified revision and update policy.
        /// </summary>
        public ArangoDocument IfMatch(string revision, ArangoUpdatePolicy updatePolicy)
        {
            _parameters.String(ParameterName.IfMatch, revision);
            // needs to be string value
            _parameters.String(ParameterName.Policy, updatePolicy.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Conditionally operate on document which current revision does not match specified revision.
        /// </summary>
        public ArangoDocument IfNoneMatch(string revision)
        {
            _parameters.String(ParameterName.IfNoneMatch, revision);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether to keep any attributes from existing document that are contained in the patch document which contains null value. Default value: true.
        /// </summary>
        public ArangoDocument KeepNull(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.KeepNull, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Determines whether the value in the patch document will overwrite the existing document's value. Default value: true.
        /// </summary>
        public ArangoDocument MergeArrays(bool value)
        {
            // needs to be string value
            _parameters.String(ParameterName.MergeArrays, value.ToString().ToLower());
        	
        	return this;
        }
        
        #endregion
        
        #region Create (POST)
        
        /// <summary>
        /// Creates new document within specified collection in current database context.
        /// </summary>
        public ArangoResult<Dictionary<string, object>> Create(string collection, Dictionary<string, object> document)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Document, "");
            
            // required
            request.QueryString.Add(ParameterName.Collection, collection);
            // optional
            request.TrySetQueryStringParameter(ParameterName.CreateCollection, _parameters);
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);

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
        
        #region Check (HEAD)
        
        /// <summary>
        /// Checks for existence of specified document.
        /// </summary>
        public ArangoResult<string> Check(string handle)
        {
            var request = new Request(HttpMethod.HEAD, ApiBaseUri.Document, "/" + handle);
            
            // optional
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
        
        #region Get (GET)
        
        /// <summary>
        /// Retrieves specified document.
        /// </summary>
        public ArangoResult<Dictionary<string, object>> Get(string handle)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Document, "/" + handle);
            
            // optional
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
        
        #region Update (PATCH)
        
        /// <summary>
        /// Updates existing document identified by its handle with new document data.
        /// </summary>
        public ArangoResult<Dictionary<string, object>> Update(string handle, Dictionary<string, object> document)
        {
            var request = new Request(HttpMethod.PATCH, ApiBaseUri.Document, "/" + handle);
            
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
            request.TrySetQueryStringParameter(ParameterName.MergeArrays, _parameters);

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
        
        /// <summary>
        /// Completely replaces existing document identified by its handle with new document data.
        /// </summary>
        public ArangoResult<Dictionary<string, object>> Replace(string handle, Dictionary<string, object> document)
        {
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Document, "/" + handle);
            
            // optional
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            // optional
            request.TrySetHeaderParameter(ParameterName.IfMatch, _parameters);
            // optional
            if (_parameters.Has(ParameterName.IfMatch))
            {
                request.TrySetQueryStringParameter(ParameterName.Policy, _parameters);
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
        
        /// <summary>
        /// Deletes specified document.
        /// </summary>
        public ArangoResult<Dictionary<string, object>> Delete(string handle)
        {
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Document, "/" + handle);
            
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
    }
}
