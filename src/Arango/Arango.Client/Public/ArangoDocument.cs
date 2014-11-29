﻿using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ArangoDocument
    {
        const string _apiUri = "_api/document";
        readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();
        readonly Connection _connection;
        
        internal ArangoDocument(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        public ArangoDocument CreateCollection(bool value)
        {
            _parameters[ParameterName.CreateCollection] = value.ToString().ToLower();
        	
        	return this;
        }
        
        public ArangoDocument WaitForSync(bool value)
        {
            _parameters[ParameterName.WaitForSync] = value.ToString().ToLower();
        	
        	return this;
        }
        
        #endregion
        
        #region Create (POST)
        
        public ArangoResult<Dictionary<string, object>> Create(string collection, Dictionary<string, object> document)
        {
            var request = new Request(HttpMethod.POST, _apiUri, "");
            
            // required: target collection name
            request.QueryString.Add(ParameterName.Collection, collection);
            // required: document to be created
            request.Body = JSON.ToJSON(document);
            // optional: determines if target collection should be created if it doesn't exist
            request.TrySetQueryStringParameter(ParameterName.CreateCollection, _parameters);
            // optional: wait until data are synchronised to disk
            request.TrySetQueryStringParameter(ParameterName.WaitForSync, _parameters);
            
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
            
            return result;
        }
        
        #endregion
    }
}
