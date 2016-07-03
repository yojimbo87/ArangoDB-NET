using System;﻿
using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class AIndex
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        
        internal AIndex(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        /// <summary>
        /// Determines an array of attribute paths in the collection with hash, fulltext, geo or skiplist indexes.
        /// </summary>
        public AIndex Fields(params string[] values)
        {
            _parameters.Array(ParameterName.Fields, values);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines if the order within the array is longitude followed by latitude in geo index.
        /// </summary>
        public AIndex GeoJson(bool value)
        {
            _parameters.Bool(ParameterName.GeoJson, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines minimum character length of words for fulltext index. Will default to a server-defined value if unspecified. It is thus recommended to set this value explicitly when creating the index.
        /// </summary>
        public AIndex MinLength(int value)
        {
            _parameters.Int(ParameterName.MinLength, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines if the hash or skiplist index should be sparse.
        /// </summary>
        public AIndex Sparse(bool value)
        {
            _parameters.Bool(ParameterName.Sparse, value);
        	
        	return this;
        }
        
        /// <summary>
        /// Determines type of the index.
        /// </summary>
        public AIndex Type(AIndexType value)
        {
            // needs to be string value
            _parameters.String(ParameterName.Type, value.ToString().ToLower());
        	
        	return this;
        }
        
        /// <summary>
        /// Determines if the hash or skiplist index should be unique.
        /// </summary>
        public AIndex Unique(bool value)
        {
            _parameters.Bool(ParameterName.Unique, value);
        	
        	return this;
        }
        
        #endregion
        
        #region Create index (POST)
        
        /// <summary>
        /// Creates index within specified collection in current database context.
        /// </summary>
        public AResult<Dictionary<string, object>> Create(string collectionName)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Index, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required
            request.QueryString.Add(ParameterName.Collection, collectionName);
            
            // required
            bodyDocument.String(ParameterName.Type, _parameters.String(ParameterName.Type));
            
            switch (_parameters.Enum<AIndexType>(ParameterName.Type))
            {
                case AIndexType.Fulltext:
                    Request.TrySetBodyParameter(ParameterName.Fields, _parameters, bodyDocument);
                    Request.TrySetBodyParameter(ParameterName.MinLength, _parameters, bodyDocument);
                    break;
                case AIndexType.Geo:
                    Request.TrySetBodyParameter(ParameterName.Fields, _parameters, bodyDocument);
                    Request.TrySetBodyParameter(ParameterName.GeoJson, _parameters, bodyDocument);
                    break;
                case AIndexType.Hash:
                    Request.TrySetBodyParameter(ParameterName.Fields, _parameters, bodyDocument);
                    Request.TrySetBodyParameter(ParameterName.Sparse, _parameters, bodyDocument);
                    Request.TrySetBodyParameter(ParameterName.Unique, _parameters, bodyDocument);
                    break;
                case AIndexType.Skiplist:
                    Request.TrySetBodyParameter(ParameterName.Fields, _parameters, bodyDocument);
                    Request.TrySetBodyParameter(ParameterName.Sparse, _parameters, bodyDocument);
                    Request.TrySetBodyParameter(ParameterName.Unique, _parameters, bodyDocument);
                    break;
                default:
                    break;
            }
            
            request.Body = JSON.ToJSON(bodyDocument, ASettings.JsonParameters);
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                case 201:
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
        
        #endregion
        
        #region Get index (GET)
        
        /// <summary>
        /// Retrieves specified index.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Get(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.GET, ApiBaseUri.Index, "/" + id);
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();
                    
                    result.Success = (body != null);
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
        
        #region Delete index (DELETE)
        
        /// <summary>
        /// Deletes specified index.
        /// </summary>
        /// <exception cref="ArgumentException">Specified id value has invalid format.</exception>
        public AResult<Dictionary<string, object>> Delete(string id)
        {
            if (!ADocument.IsID(id))
            {
                throw new ArgumentException("Specified id value (" + id + ") has invalid format.");
            }
            
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Index, "/" + id);
            
            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
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
        
        #endregion
    }
}
