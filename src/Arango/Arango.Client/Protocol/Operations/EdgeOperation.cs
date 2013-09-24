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

        internal Document Get(string id)
        {
            var request = new Request(RequestType.Edge, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, id);
            
            var response = _connection.Process(request);
            var edge = new Document();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    edge = response.Document;
                    break;
                case HttpStatusCode.NotModified:
                    edge.String("_rev", response.Headers.Get("etag").Replace("\"", ""));
                    break;
                case HttpStatusCode.NotFound:
                    edge = null;
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

            return edge;
        }
        
        internal List<Document> Get(string collection, string vertexId, ArangoEdgeDirection direction)
        {
            var request = new Request(RequestType.Edge, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri + "s", collection);
            
            request.QueryString.Add("vertex", vertexId);
            
            request.QueryString.Add("direction", direction.ToString().ToLower());
            
            var response = _connection.Process(request);
            var edges = new List<Document>();
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    edges = response.Document.List<Document>("edges");
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

            return edges;
        }

        #endregion
        
        #region POST
        
        internal void Post(string collection, Document edge, bool waitForSync, bool createCollection)
        {
            var request = new Request(RequestType.Document, HttpMethod.Post);
            request.RelativeUri = _apiUri;
            request.Body = edge.Except("_id", "_key", "_rev", "_from", "_to").Serialize();
            
            // set collection name where the document will be created
            request.QueryString.Add("collection", collection);
            
            // set from document handle
            request.QueryString.Add("from", edge.String("_from"));
            
            // set to document handle
            request.QueryString.Add("to", edge.String("_to"));
            
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
                    edge.String("_id", response.Document.String("_id"));
                    edge.String("_key", response.Document.String("_key"));
                    edge.String("_rev", response.Document.String("_rev"));
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
        
        #region PUT
        
        internal bool Put(Document edge, bool waitForSync, string revision)
        {
            var request = new Request(RequestType.Edge, HttpMethod.Put);
            request.RelativeUri = string.Join("/", _apiUri, edge.String("_id"));
            request.Body = edge.Except("_id", "_key", "_rev", "_from", "_to").Serialize();
            
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
                    edge.String("_id", response.Document.String("_id"));
                    edge.String("_key", response.Document.String("_key"));
                    edge.String("_rev", response.Document.String("_rev"));
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
            
            return isReplaced;
        }
        
        #endregion
        
        #region PATCH
        
        internal bool Patch(Document edge, bool waitForSync, string revision)
        {
            var request = new Request(RequestType.Edge, HttpMethod.Patch);
            request.RelativeUri = string.Join("/", _apiUri, edge.String("_id"));
            request.Body = edge.Except("_id", "_key", "_rev", "_from", "_to").Serialize();
            
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
            var isUpdated = false;
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                    isUpdated = true;
                    edge.String("_id", response.Document.String("_id"));
                    edge.String("_key", response.Document.String("_key"));
                    edge.String("_rev", response.Document.String("_rev"));
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
            
            return isUpdated;
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
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }
            
            return exists;
        }
        
        #endregion
    }
}
