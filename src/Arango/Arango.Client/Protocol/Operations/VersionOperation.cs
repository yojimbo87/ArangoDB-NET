using System;
using System.Collections.Generic;
using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client.Protocol
{
    public class VersionOperation
    {
        private string _apiUri { get { return "_admin/version"; } }
        private Connection _connection { get; set; }

        internal VersionOperation(Connection connection)
        {
            _connection = connection;
        }
        
        #region GET
        
        internal ArangoVersion Get()
        {
            var request = new Request(RequestType.Version, HttpMethod.Get);
            request.RelativeUri = _apiUri;           
            
            var response = _connection.Process(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    string versionString = response.Document.String("version"); 
                    return new ArangoVersion(versionString);
                    
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