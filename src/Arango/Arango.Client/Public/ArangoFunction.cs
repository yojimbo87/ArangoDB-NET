using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class ArangoFunction
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;
        
        internal ArangoFunction(Connection connection)
        {
            _connection = connection;
        }
        
        #region Parameters
        
        public ArangoFunction Group(bool value)
        {
            _parameters.String(ParameterName.Group, value.ToString().ToLower());
            
            return this;
        }
        
        public ArangoFunction IsDeterministic(bool value)
        {
            _parameters.Bool(ParameterName.IsDeterministic, value);
            
            return this;
        }
        
        public ArangoFunction Namespace(string value)
        {
            _parameters.String(ParameterName.Namespace, value);
            
            return this;
        }
        
        #endregion
        
        #region Register (POST)

        public ArangoResult<bool> Register(string name, string code)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.AqlFunction, "");
            var bodyDocument = new Dictionary<string, object>();
            
            // required: fully qualified name of the user function
            bodyDocument.String(ParameterName.Name, name);
            // required: string representation of the function body
            bodyDocument.String(ParameterName.Code, code);
            // optional: function return value solely depends on the input value and return value is the same for repeated calls with same input
            Request.TrySetBodyParameter(ParameterName.IsDeterministic, _parameters, bodyDocument);
            
            request.Body = JSON.ToJSON(bodyDocument);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                case 201:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = true;
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
        
        #region Unregister (DELETE)

        public ArangoResult<bool> Unregister(string name)
        {
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.AqlFunction, "/" + name);
            
            // optional: If set to true, then the function name provided in name is treated as a namespace prefix, 
            // and all functions in the specified namespace will be deleted. If set to false, the function name provided 
            // in name must be fully qualified, including any namespaces.
            request.TrySetQueryStringParameter(ParameterName.Group, _parameters);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<bool>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.Document)
                    {
                        result.Success = true;
                        result.Value = true;
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
        
        #region List (GET)

        public ArangoResult<List<Dictionary<string, object>>> List()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.AqlFunction, "");
            
            // optional: If set to true, then the function name provided in name is treated as a namespace prefix, 
            // and all functions in the specified namespace will be deleted. If set to false, the function name provided 
            // in name must be fully qualified, including any namespaces.
            request.TrySetQueryStringParameter(ParameterName.Group, _parameters);
            
            var response = _connection.Send(request);
            var result = new ArangoResult<List<Dictionary<string, object>>>(response);
            
            switch (response.StatusCode)
            {
                case 200:
                    if (response.DataType == DataType.List)
                    {
                        result.Value = ((IEnumerable)response.Data).Cast<Dictionary<string, object>>().ToList();
                        result.Success = (result.Value != null);
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
    }
}
