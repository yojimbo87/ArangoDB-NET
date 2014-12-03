using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ArangoCollection
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        
        internal ArangoCollection(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        #region Checksum options
        
        public ArangoCollection WithRevisions(bool value)
        {
            // needs to be in string format
            _parameters.String(ParameterName.WithRevisions, value.ToString().ToLower());
        	
        	return this;
        }
        
        public ArangoCollection WithData(bool value)
        {
            // needs to be in string format
            _parameters.String(ParameterName.WithData, value.ToString().ToLower());
        	
        	return this;
        }
        
        #endregion
        
        public ArangoCollection DoCompact(bool value)
        {
            _parameters.Bool(ParameterName.DoCompact, value);
        	
        	return this;
        }
        
        public ArangoCollection IsSystem(bool value)
        {
            _parameters.Bool(ParameterName.IsSystem, value);
        	
        	return this;
        }
        
        public ArangoCollection IsVolatile(bool value)
        {
            _parameters.Bool(ParameterName.IsVolatile, value);
        	
        	return this;
        }
        
        public ArangoCollection JournalSize(long value)
        {
            _parameters.Long(ParameterName.JournalSize, value);
        	
        	return this;
        }
        
        #region Key options
        
        public ArangoCollection AllowUserKeys(bool value)
        {
            _parameters.Bool(ParameterName.KeyOptionsAllowUserKeys, value);
        	
        	return this;
        }
        
        public ArangoCollection KeyGeneratorType(ArangoKeyGeneratorType value)
        {
            // needs to be in string format
            _parameters.Enum(ParameterName.KeyOptionsType, value.ToString().ToLower());
        	
        	return this;
        }
        
        public ArangoCollection KeyIncrement(long value)
        {
            _parameters.Long(ParameterName.KeyOptionsIncrement, value);
        	
        	return this;
        }
        
        public ArangoCollection KeyOffset(long value)
        {
            _parameters.Long(ParameterName.KeyOptionsOffset, value);
        	
        	return this;
        }
        
        #endregion
        
        public ArangoCollection NumberOfShards(int value)
        {
            _parameters.Int(ParameterName.NumberOfShards, value);
        	
        	return this;
        }
        
        public ArangoCollection ShardKeys(List<string> value)
        {
            _parameters.List(ParameterName.ShardKeys, value);
        	
        	return this;
        }
        
        public ArangoCollection Type(ArangoCollectionType value)
        {
            _parameters.Enum(ParameterName.Type, value);
        	
        	return this;
        }
        
        public ArangoCollection WaitForSync(bool value)
        {
            _parameters.Bool(ParameterName.WaitForSync, value);
        	
        	return this;
        }
        
        #endregion
        
        #region Create collection (POST)
        
        public ArangoResult<Dictionary<string, object>> Create(string collectionName)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Collection, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required: collection name
            bodyDocument.String(ParameterName.Name, collectionName);
            // optional: wait until data are synchronised to disk
            Request.TrySetParameter(ParameterName.WaitForSync, _parameters, bodyDocument);
            // optional: whether or not the collection will be compacted
            Request.TrySetParameter(ParameterName.DoCompact, _parameters, bodyDocument);
            // optional: maximal size of a journal or datafile in bytes
            Request.TrySetParameter(ParameterName.JournalSize, _parameters, bodyDocument);
            // optional: if true, create a system collection
            Request.TrySetParameter(ParameterName.IsSystem, _parameters, bodyDocument);
            // optional: if true, collection data is kept in-memory only and not made persistent
            Request.TrySetParameter(ParameterName.IsVolatile, _parameters, bodyDocument);
            // optional: the type of the collection to create
            Request.TrySetParameter(ParameterName.Type, _parameters, bodyDocument);
            // optional: in a cluster, this value determines the number of shards to create for the collection
            Request.TrySetParameter(ParameterName.NumberOfShards, _parameters, bodyDocument);
            // optional: in a cluster, this attribute determines which document attributes are used to determine the target shard for documents
            Request.TrySetParameter(ParameterName.ShardKeys, _parameters, bodyDocument);
            // optional: specifies the type of the key generator
            Request.TrySetParameter(ParameterName.KeyOptionsType, _parameters, bodyDocument);
            // optional: if set to true, then it is allowed to supply own key values in the _key attribute of a document
            Request.TrySetParameter(ParameterName.KeyOptionsAllowUserKeys, _parameters, bodyDocument);
            // optional: increment value for autoincrement key generator
            Request.TrySetParameter(ParameterName.KeyOptionsIncrement, _parameters, bodyDocument);
            // optional: initial offset value for autoincrement key generator
            Request.TrySetParameter(ParameterName.KeyOptionsOffset, _parameters, bodyDocument);
            
            request.Body = JSON.ToJSON(bodyDocument);
            
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
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Delete collection (DELETE)
        
        public ArangoResult<Dictionary<string, object>> Delete(string collectionName)
        {
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Collection, "/" + collectionName);
            
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
        
        #region Truncate collection (PUT)
        
        public ArangoResult<Dictionary<string, object>> Truncate(string collectionName)
        {
            var request = new Request(HttpMethod.PUT, ApiBaseUri.Collection, "/" + collectionName + "/truncate");
            
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
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Get collection (GET)
        
        public ArangoResult<Dictionary<string, object>> Get(string collectionName)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + collectionName);

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
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Get collection properties (GET)
        
        public ArangoResult<Dictionary<string, object>> GetProperties(string collectionName)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + collectionName + "/properties");

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
        
        #region Get collection documents count (GET)
        
        public ArangoResult<Dictionary<string, object>> GetCount(string collectionName)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + collectionName + "/count");

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
        
        #region Get collection figures (GET)
        
        public ArangoResult<Dictionary<string, object>> GetFigures(string collectionName)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + collectionName + "/figures");

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
        
        #region Get collection revision (GET)
        
        public ArangoResult<Dictionary<string, object>> GetRevision(string collectionName)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + collectionName + "/revision");

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
        
        #region Get collection checksum (GET)
        
        public ArangoResult<Dictionary<string, object>> GetChecksum(string collectionName)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "/" + collectionName + "/checksum");

            // optional: whether or not to include document body data in the checksum calculation
            request.TrySetQueryStringParameter(ParameterName.WithData, _parameters);
            // optional: whether or not to include document revision ids in the checksum calculation
            request.TrySetQueryStringParameter(ParameterName.WithRevisions, _parameters);
            
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
