using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ArangoDatabase
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        
        #region Parameters
        
        public ArangoDatabase ExcludeSystem(bool value)
        {
            // string because value will be stored in query string
            _parameters.String(ParameterName.ExcludeSystem, value.ToString().ToLower());
        	
        	return this;
        }
        
        #endregion
        
        public ArangoCollection Collection
        {
            get
            {
                return new ArangoCollection(_connection);
            }
        }
        
        public ArangoDocument Document
        {
            get
            {
                return new ArangoDocument(_connection);
            }
        }
        
        public ArangoDatabase(string alias)
        {
            _connection = ArangoSettings.GetConnection(alias);
        }
        
        #region Get current database (GET)
        
        public ArangoResult<Dictionary<string, object>> GetCurrent()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Database, "/current");
            
            var response = _connection.Send(request);
            var result = new ArangoResult<Dictionary<string, object>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>).Document("result");
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
        
        #region Get list of accessible databases (GET)
        
        public ArangoResult<List<string>> GetAccessibleDatabases()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Database, "/user");
            
            var response = _connection.Send(request);
            var result = new ArangoResult<List<string>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>).List<string>("result");
                    }
                    break;
                case 400:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Get list of all databases (GET)
        
        public ArangoResult<List<string>> GetAllDatabases()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Database, "");
            
            var response = _connection.Send(request);
            var result = new ArangoResult<List<string>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>).List<string>("result");
                    }
                    break;
                case 400:
                case 403:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Get list of all collections (GET)
        
        public ArangoResult<List<Dictionary<string, object>>> GetAllCollections()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Collection, "");
            
            // optional: whether or not system collections should be excluded from the result
            request.TrySetQueryStringParameter(ParameterName.ExcludeSystem, _parameters);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<List<Dictionary<string, object>>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>).List<Dictionary<string, object>>("collections");
                    }
                    break;
                case 400:
                case 403:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Create database (POST)
        
        public ArangoResult<bool> Create(string databaseName)
        {
            return Create(databaseName, null);
        }
        
        public ArangoResult<bool> Create(string databaseName, List<ArangoUser> users)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Database, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required: database name
            bodyDocument.String("name", databaseName);
            
            // optional: list of users
            if ((users != null) && (users.Count > 0))
            {
                var userList = new List<Dictionary<string, object>>();
                
                foreach (var user in users)
                {
                    var userItem = new Dictionary<string, object>();
                    
                    if (!string.IsNullOrEmpty(user.Username))
                    {
                        userItem.String("username", user.Username);
                    }
                    
                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        userItem.String("passwd", user.Password);
                    }
                    
                    userItem.Bool("active", user.Active);
                    
                    if (user.Extra != null)
                    {
                        userItem.Document("extra", user.Extra);
                    }
                    
                    userList.Add(userItem);
                }
                
                bodyDocument.List("users", userList);
            }
            
            request.Body = JSON.ToJSON(bodyDocument);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 201:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>).Bool("result");
                    }
                    break;
                case 400:
                case 403:
                case 404:
                default:
                    // Arango error
                    break;
            }
            
            _parameters.Clear();
            
            return result;
        }
        
        #endregion
        
        #region Drop database (DELETE)
        
        public ArangoResult<bool> Drop(string databaseName)
        {
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Database, "/" + databaseName);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = (response.Data as Dictionary<string, object>).Bool("result");
                    }
                    break;
                case 400:
                case 403:
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
