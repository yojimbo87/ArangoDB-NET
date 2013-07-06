using System;
using System.Collections.Generic;
using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client.Protocol
{
    internal class EdgeOperation
    {
        private string _apiUri { get { return "_api/edge"; } }
        private Connection _connection { get; set; }

        internal EdgeOperation(Connection connection)
        {
            _connection = connection;
        }
        
        #region GET

        internal ArangoEdge Get(string id)
        {
            var request = new Request(RequestType.Edge, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, id);
            
            var response = _connection.Process(request);
            ArangoEdge edge = new ArangoEdge();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    edge.Id = response.Document.GetField<string>("_id");
                    edge.Key = response.Document.GetField<string>("_key");
                    edge.Revision = response.Document.GetField<string>("_rev");
                    edge.From = response.Document.GetField<string>("_from");
                    edge.To = response.Document.GetField<string>("_to");
                    edge.Document = response.Document;
                    break;
                case HttpStatusCode.NotModified:
                    edge.Revision = response.Headers.Get("etag").Replace("\"", "");
                    break;
                case HttpStatusCode.NotFound:
                    edge = null;
                    break;
                default:
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

            return edge;
        }
        
        internal List<ArangoEdge> Get(string collection, string vertexId, ArangoEdgeDirection direction)
        {
            var request = new Request(RequestType.Edge, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri + "s", collection);
            
            request.QueryString.Add("vertex", vertexId);
            
            request.QueryString.Add("direction", direction.ToString().ToLower());
            
            var response = _connection.Process(request);
            List<ArangoEdge> edges = new List<ArangoEdge>();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    List<Document> edgeDocuments = response.Document.GetField<List<Document>>("edges");
                    
                    foreach (Document document in edgeDocuments)
                    {
                        ArangoEdge edge = new ArangoEdge();
                        edge.Id = document.GetField<string>("_id");
                        edge.Key = document.GetField<string>("_key");
                        edge.Revision = document.GetField<string>("_rev");
                        edge.From = document.GetField<string>("_from");
                        edge.To = document.GetField<string>("_to");
                        
                        // TODO: remove arango specific fields
                        edge.Document = document;
                        
                        edges.Add(edge);
                    }
                    break;
                default:
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

            return edges;
        }

        #endregion
        
        #region POST
        
        internal void Post(string collection, ArangoEdge arangoEdge, bool waitForSync, bool createCollection)
        {
            var request = new Request(RequestType.Document, HttpMethod.Post);
            request.RelativeUri = _apiUri;
            request.Body = arangoEdge.Serialize();
            
            // set collection name where the document will be created
            request.QueryString.Add("collection", collection);
            
            // set from document handle
            request.QueryString.Add("from", arangoEdge.From);
            
            // set to document handle
            request.QueryString.Add("to", arangoEdge.To);
            
            // (optional)
            if (createCollection)
            {
                request.QueryString.Add("createCollection", "true");
            }
            
            // (optional)
            if (waitForSync)
            {
                request.QueryString.Add("waitForSync", "true");
            }
            
            var response = _connection.Process(request);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                    arangoEdge.Id = response.Document.GetField<string>("_id");
                    arangoEdge.Key = response.Document.GetField<string>("_key");
                    arangoEdge.Revision = response.Document.GetField<string>("_rev");
                    break;
                default:
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
        }
        
        #endregion
        
        #region DELETE
        
        internal bool Delete(string id)
        {
            var request = new Request(RequestType.Edge, HttpMethod.Delete);
            request.RelativeUri = string.Join("/", _apiUri, id);
            
            var response = _connection.Process(request);
            var isRemoved = false;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Accepted:
                    isRemoved = true;
                    break;
                default:
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
            
            return isRemoved;
        }
        
        #endregion
        
        #region PUT
        
        internal bool Put(string id, ArangoEdge arangoEdge, bool waitForSync, string revision)
        {
            var request = new Request(RequestType.Edge, HttpMethod.Put);
            request.RelativeUri = string.Join("/", _apiUri, id);
            request.Body = arangoEdge.Serialize();
            
            // (optional)
            if (waitForSync)
            {
                request.QueryString.Add("waitForSync", "true");
            }
            
            // (optional)
            if (!string.IsNullOrEmpty(revision))
            {
                request.QueryString.Add("rev", revision);
            }
            
            var response = _connection.Process(request);
            var isReplaced = false;
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                    isReplaced = true;
                    arangoEdge.Id = response.Document.GetField<string>("_id");
                    arangoEdge.Key = response.Document.GetField<string>("_key");
                    arangoEdge.Revision = response.Document.GetField<string>("_rev");
                    break;
                default:
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
            
            return isReplaced;
        }
        
        #endregion
        
        #region HEAD
        
        internal bool Head(string id)
        {
            var request = new Request(RequestType.Edge, HttpMethod.Head);
            request.RelativeUri = string.Join("/", _apiUri, id);
            
            var response = _connection.Process(request);
            var exists = false;
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    exists = true;
                    break;
                case HttpStatusCode.NotFound:
                    break;
                default:
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
            
            return exists;
        }
        
        #endregion
    }
}
