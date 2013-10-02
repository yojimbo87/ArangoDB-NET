using System;
using System.Collections.Generic;
using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client.Protocol
{
    internal class DatabaseOperation
    {
        private string _apiUri { get { return "_api/database"; } }
        private Connection _connection { get; set; }
        
        internal DatabaseOperation(Connection connection)
        {
            _connection = connection;
        }
        
        #region GET
        
        // get list of all databases, only possible from within the _system database
        internal List<string> Get()
        {
            var request = new Request(RequestType.Document, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri);
            
            var response = _connection.Process(request);
            var databases = new List<string>();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    databases = response.Document.List<string>("result");
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

            return databases;
        }
        
        #endregion
        
        #region POST
        
        // only possible from within the _system database
        internal bool Post(string databaseName)
        {
            var request = new Request(RequestType.Document, HttpMethod.Post);
            request.RelativeUri = string.Join("/", _apiUri);
            
            var bodyDocument = new Document()
                .String("name", databaseName);
            
            request.Body = bodyDocument.Serialize();
            
            var response = _connection.Process(request);
            var created = false;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    created = response.Document.Bool("result");
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

            return created;
        }
        
        #endregion
        
        #region DELETE
        
        // only possible from within the _system database
        internal bool Delete(string databaseName)
        {
            var request = new Request(RequestType.Document, HttpMethod.Delete);
            request.RelativeUri = string.Join("/", _apiUri, databaseName);
            
            var response = _connection.Process(request);
            var deleted = false;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    deleted = response.Document.Bool("result");
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

            return deleted;
        }
        
        #endregion
    }
}
