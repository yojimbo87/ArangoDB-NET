using System.Collections.Generic;
using Arango.Client.Protocol;

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
    }
}
