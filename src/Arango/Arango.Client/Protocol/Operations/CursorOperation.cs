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
        
        // TODO: bindVars optional parameter
        // http://www.arangodb.org/manuals/current/HttpCursor.html#HttpCursorPost
        internal List<Document> Post(string query, bool returnCount, out int count, int batchSize)
        {
            var request = new Request(RequestType.Cursor, HttpMethod.Post);
            request.RelativeUri = _apiUri;
            
            Document bodyDocument = new Document();
            
            // set AQL string
            bodyDocument.SetField("query", query);
                
            // (optional) set number of found documents
            if (returnCount)
            {
                bodyDocument.SetField("count", returnCount);
            }
            
            // (optional) set how much documents should be returned
            if (batchSize > 0)
            {
                bodyDocument.SetField("batchSize", batchSize);
            }
            
            request.Body = Document.Serialize(bodyDocument);
            
            var response = _connection.Process(request);
            List<Document> documents = new List<Document>();
            count = 0;
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    documents.AddRange(response.Document.GetField<List<Document>>("result"));
                    
                    // get count of returned document if present
                    if (returnCount)
                    {
                        count = response.Document.GetField<int>("count");
                    }
                   
                    // get more results if present
                    if (response.Document.GetField<bool>("hasMore"))
                    {
                        int cursorId = response.Document.GetField<int>("id");
                        
                        documents.AddRange(Put(cursorId));
                    }
                    break;
                default:
                    documents = null;
                    
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.GetField<string>("driverErrorMessage"),
                            response.Document.GetField<string>("driverExceptionMessage"),
                            response.Document.GetField<Exception>("driverInnerException")
                        );
                    }
                    break;
            }
            
            return documents;
        }
        
        #endregion
        
        #region PUT
        
        internal List<Document> Put(int id)
        {
            var request = new Request(RequestType.Cursor, HttpMethod.Put);
            request.RelativeUri = string.Join("/", _apiUri, id);
            
            var response = _connection.Process(request);
            List<Document> documents = new List<Document>();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    documents.AddRange(response.Document.GetField<List<Document>>("result"));
                    
                    // get more results if present
                    if (response.Document.GetField<bool>("hasMore"))
                    {
                        int cursorId = response.Document.GetField<int>("id");
                        
                        documents.AddRange(Put(cursorId));
                    }
                    break;
                default:
                    documents = null;
                    
                    if (response.IsException)
                    {
                        throw new ArangoException(
                            response.StatusCode,
                            response.Document.GetField<string>("driverErrorMessage"),
                            response.Document.GetField<string>("driverExceptionMessage"),
                            response.Document.GetField<Exception>("driverInnerException")
                        );
                    }
                    break;
            }
            
            return documents;
        }
        
        #endregion
    }
}
