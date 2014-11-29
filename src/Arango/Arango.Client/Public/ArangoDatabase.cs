using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ArangoDatabase
    {
        const string _apiUri = "_api/database";
        readonly Connection _connection;
        
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
        
        #region GET
        
        public ArangoResult<Dictionary<string, object>> GetCurrent()
        {
            var request = new Request(HttpMethod.GET, _apiUri, "/current");
            
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
            
            return result;
        }
        
        public ArangoResult<List<string>> GetAccessibleDatabases()
        {
            var request = new Request(HttpMethod.GET, _apiUri, "/user");
            
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
            
            return result;
        }
        
        public ArangoResult<List<string>> GetAllDatabases()
        {
            var request = new Request(HttpMethod.GET, _apiUri, "");
            
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
            
            return result;
        }
        
        #endregion
        
        #region Create (POST)
        
        public ArangoResult<bool> Create(string databaseName)
        {
            return Create(databaseName, null);
        }
        
        public ArangoResult<bool> Create(string databaseName, List<DatabaseUser> users)
        {
            var request = new Request(HttpMethod.POST, _apiUri, "");
            var body = new Dictionary<string, object>();
            
            // required: database name
            body.String("name", databaseName);
            
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
                
                body.List("users", userList);
            }
            
            request.Body = JSON.ToJSON(body);
            
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
            
            return result;
        }
        
        #endregion
        
        #region Drop (DELETE)
        
        public ArangoResult<bool> Drop(string databaseName)
        {
            var request = new Request(HttpMethod.DELETE, _apiUri, "/" + databaseName);
            
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
            
            return result;
        }
        
        #endregion
    }
}
