using System;
using System.Collections.Generic;
using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client.Protocol
{
    internal class FunctionOperation
    {
        private string _apiUri { get { return "_api/aqlfunction"; } }
        private Connection _connection { get; set; }
        
        internal FunctionOperation(Connection connection)
        {
            _connection = connection;
        }
        
        #region GET
        
        internal List<Document> Get(string @namespace)
        {
            var request = new Request(RequestType.Function, HttpMethod.Get);
            request.RelativeUri = _apiUri;
            
            // (optional)
            if (!string.IsNullOrEmpty(@namespace))
            {
                request.QueryString.Add("namespace", @namespace);
            }
            
            var response = _connection.Process(request);
            List<Document> functions;
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    functions = response.List;
                    break;
                default:
                    functions = null;
                    
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }
            
            return functions;
        }
        
        #endregion
        
        #region POST
        
        internal bool Post(string name, string code)
        {
            var request = new Request(RequestType.Function, HttpMethod.Post);
            request.RelativeUri = _apiUri;
            
            var bodyDocument = new Document();
            // set name of the function
            bodyDocument.String("name", name);
            // set function code
            bodyDocument.String("code", code);
            
            request.Body = bodyDocument.Serialize();
            
            var response = _connection.Process(request);
            var isSuccess = false;
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                    isSuccess = true;
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }
            
            return isSuccess;
        }
        
        #endregion
        
        #region DELETE
        
        internal bool Delete(string name, string group)
        {
            var request = new Request(RequestType.Function, HttpMethod.Delete);
            request.RelativeUri = string.Join("/", _apiUri, name);
            
            // (optional)
            if (!string.IsNullOrEmpty(group))
            {
                request.QueryString.Add("group", group);
            }
            
            var response = _connection.Process(request);
            var isRemoved = false;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    isRemoved = true;
                    break;
                default:
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }
            
            return isRemoved;
        }
        
        #endregion
    }
}
