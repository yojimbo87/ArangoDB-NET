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

        internal ArangoDocument Get(string id)
        {
            var request = new Request(RequestType.Document, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, id);
            
            var response = _connection.Process(request);
            ArangoDocument document = new ArangoDocument();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    document.Id = response.Document.GetField<string>("_id");
                    document.Key = response.Document.GetField<string>("_key");
                    document.Revision = response.Document.GetField<string>("_rev");
                    document.Document = response.Document;
                    break;
                case HttpStatusCode.NotModified:
                    document.Revision = response.Headers.Get("etag").Replace("\"", "");
                    break;
                case HttpStatusCode.NotFound:
                    document = null;
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

            return document;
        }

        #endregion
        
        #region POST
        
        internal void Post(string collection, ArangoDocument arangoDocument, bool waitForSync, bool createCollection)
        {
            var request = new Request(RequestType.Document, HttpMethod.Post);
            request.RelativeUri = _apiUri;
            request.Body = arangoDocument.Serialize();
            
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
                    arangoDocument.Id = response.Document.GetField<string>("_id");
                    arangoDocument.Key = response.Document.GetField<string>("_key");
                    arangoDocument.Revision = response.Document.GetField<string>("_rev");
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
        
        internal bool Put(string id, ArangoDocument arangoDocument, bool waitForSync, string revision)
        {
            var request = new Request(RequestType.Document, HttpMethod.Put);
            request.RelativeUri = string.Join("/", _apiUri, id);
            request.Body = arangoDocument.Serialize();
            
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
                    arangoDocument.Id = response.Document.GetField<string>("_id");
                    arangoDocument.Key = response.Document.GetField<string>("_key");
                    arangoDocument.Revision = response.Document.GetField<string>("_rev");
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
        
        #region PATCH
        
        internal bool Patch(ArangoDocument arangoDocument, bool waitForSync, string revision)
        {
            var request = new Request(RequestType.Document, HttpMethod.Put);
            request.RelativeUri = string.Join("/", _apiUri, arangoDocument.Id);
            request.Body = arangoDocument.Serialize();
            
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
                    arangoDocument.Id = response.Document.GetField<string>("_id");
                    arangoDocument.Key = response.Document.GetField<string>("_key");
                    arangoDocument.Revision = response.Document.GetField<string>("_rev");
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
            
            return isUpdated;
        }
        
        #endregion
    }
}

