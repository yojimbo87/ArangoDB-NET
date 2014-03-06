using System;
using System.Collections.Generic;
using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client.Protocol
{
    public class ServerOperation
    {
        private string _apiUri { get { return "_admin/server"; } }
        private Connection _connection { get; set; }

        internal ServerOperation(Connection connection)
        {
            _connection = connection;
        }
        
        #region ROLE
        
        internal ArangoServerRole Role()
        {
            var request = new Request(RequestType.Server, HttpMethod.Get);
            request.RelativeUri = _apiUri + "/role";           
            
            var response = _connection.Process(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    string roleString = response.Document.String("role"); 
                    return new ArangoServerRole(roleString);
                    
                case HttpStatusCode.NotFound:
                    // ArangoDB pre 2.0 does not have this handler
                    return new ArangoServerRole(ArangoServerRole.RoleType.UNKNOWN);
                    
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

            return null;
        }
        
        #endregion
    }
}