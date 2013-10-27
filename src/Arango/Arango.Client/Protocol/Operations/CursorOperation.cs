using System;
using System.Collections.Generic;
using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client.Protocol
{
    internal class CursorOperation
    {
        private string _apiUri { get { return "_api/cursor"; } }
        private Connection _connection { get; set; }

        internal CursorOperation(Connection connection)
        {
            _connection = connection;
        }
        
        #region POST
        
        internal List<object> Post(string query, bool returnCount, out int count, int batchSize, Dictionary<string, object> bindVars)
        {
            var request = new Request(RequestType.Cursor, HttpMethod.Post);
            request.RelativeUri = _apiUri;
            
            var bodyDocument = new Document();
            
            // set AQL string
            bodyDocument.String("query", query);
                
            // (optional) set number of found documents
            if (returnCount)
            {
                bodyDocument.Bool("count", returnCount);
            }
            
            // (optional) set how much documents should be returned
            if (batchSize > 0)
            {
                bodyDocument.Int("batchSize", batchSize);
            }
            
            // (optional) set list of bind parameters
            if ((bindVars != null) && (bindVars.Count > 0))
            {
                var bindVarsDocument = new Document();
                
                foreach (KeyValuePair<string, object> item in bindVars)
                {
                    bindVarsDocument.Object(item.Key, item.Value);
                }
                
                bodyDocument.Object("bindVars", bindVarsDocument);
            }
            
            request.Body = bodyDocument.Serialize();
            
            var response = _connection.Process(request);
            var items = new List<object>();
            count = 0;
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    items.AddRange(response.Document.List<object>("result"));
                    
                    // get count of returned document if present
                    if (returnCount)
                    {
                        count = response.Document.Int("count");
                    }
                   
                    // get more results if present
                    if (response.Document.Bool("hasMore"))
                    {
                        long cursorId = response.Document.Long("id");
                        
                        items.AddRange(Put(cursorId));
                    }
                    break;
                default:
                    items = null;
                    
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
            
            return items;
        }
        
        #endregion
        
        #region PUT
        
        internal List<object> Put(long id)
        {
            var request = new Request(RequestType.Cursor, HttpMethod.Put);
            request.RelativeUri = string.Join("/", _apiUri, id);
            
            var response = _connection.Process(request);
            var items = new List<object>();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    items.AddRange(response.Document.List<object>("result"));
                    
                    // get more results if present
                    if (response.Document.Bool("hasMore"))
                    {
                        long cursorId = response.Document.Long("id");
                        
                        items.AddRange(Put(cursorId));
                    }
                    break;
                default:
                    items = null;
                    
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
            
            return items;
        }
        
        #endregion
    }
}
