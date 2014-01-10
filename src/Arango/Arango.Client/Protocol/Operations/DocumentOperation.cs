using System;
using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client.Protocol
{
    internal class DocumentOperation
    {
        private string _apiUri { get { return "_api/document"; } }
        private Connection _connection { get; set; }

        internal DocumentOperation(Connection connection)
        {
            _connection = connection;
        }

        #region GET

        internal Document Get(string id)
        {
            var request = new Request(RequestType.Document, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, id);
            
            var response = _connection.Process(request);
            var document = new Document();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    document = response.Document;
                    break;
                case HttpStatusCode.NotModified:
                    document.String("_rev", response.Headers.Get("etag").Replace("\"", ""));
                    break;
                case HttpStatusCode.NotFound:
                    document = null;
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

            return document;
        }

        #endregion
        
        #region POST
        
        internal void Post(string collection, Document document, bool waitForSync, bool createCollection)
        {
            var request = new Request(RequestType.Document, HttpMethod.Post);
            request.RelativeUri = _apiUri;
	    request.Body = document.Serialize();
            if (
                request.Body.Contains("_id")
                || request.Body.Contains("_key")
                || request.Body.Contains("_rev")
            ) {
                request.Body = document.Except("_id", "_key", "_rev").Serialize();
	    }
            
            // set collection name where the document will be created
            request.QueryString.Add("collection", collection);
            
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
                    document.String("_id", response.Document.String("_id"));
                    document.String("_key", response.Document.String("_key"));
                    document.String("_rev", response.Document.String("_rev"));
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
            var request = new Request(RequestType.Document, HttpMethod.Delete);
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
        
        internal bool Put(Document document, bool waitForSync, string revision)
        {
            var request = new Request(RequestType.Document, HttpMethod.Put);
            request.RelativeUri = string.Join("/", _apiUri, document.String("_id"));
            request.Body =  document.Except("_id", "_key", "_rev").Serialize();
            
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
                    document.String("_id", response.Document.String("_id"));
                    document.String("_key", response.Document.String("_key"));
                    document.String("_rev", response.Document.String("_rev"));
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
        
        internal bool Patch(Document document, bool waitForSync, string revision)
        {
            var request = new Request(RequestType.Document, HttpMethod.Patch);
            request.RelativeUri = string.Join("/", _apiUri, document.String("_id"));
            request.Body = document.Except("_id", "_key", "_rev").Serialize();
            
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
                    document.String("_id", response.Document.String("_id"));
                    document.String("_key", response.Document.String("_key"));
                    document.String("_rev", response.Document.String("_rev"));
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
            var request = new Request(RequestType.Document, HttpMethod.Head);
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

