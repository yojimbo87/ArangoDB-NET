using System;
using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client.Protocol
{
    public class CollectionOperation
    {
        private string _apiUri { get { return "_api/collection"; } }
        private Connection _connection { get; set; }

        internal CollectionOperation(Connection connection)
        {
            _connection = connection;
        }
        
        #region GET
        
        internal ArangoCollection Get(string name)
        {
            var request = new Request(RequestType.Collection, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, name);
            
            var response = _connection.Process(request);
            ArangoCollection collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.Id = response.Document.GetField<string>("id");
                    collection.Name = response.Document.GetField<string>("name");
                    collection.Status = response.Document.GetField<ArangoCollectionStatus>("status");
                    collection.Type = response.Document.GetField<ArangoCollectionType>("type");
                    break;
                case HttpStatusCode.NotFound:
                    collection = null;
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

            return collection;
        }
        
        #endregion
        
        #region POST
        
        internal void Post(ArangoCollection collection)
        {
            var request = new Request(RequestType.Collection, HttpMethod.Post);
            request.RelativeUri = _apiUri;
            
            var document = new Document();
            
            // set collection name
            document.SetField("name", collection.Name);
            
            // (optional, default: 2) set type
            if (collection.Type != 0)
            {
                document.SetField("type", collection.Type);
            }
            
            // (optional, default: false) set waitForSync
            if (collection.WaitForSync)
            {
                document.SetField("waitForSync", collection.WaitForSync);
            }
            
            // (optional, default: arangodb config) set journalSize
            if (collection.JournalSize > 0)
            {
                document.SetField("journalSize", collection.JournalSize);
            }
            
            // (optional, default: false) set isSystem
            if (collection.IsSystem)
            {
                document.SetField("isSystem", collection.IsSystem);
            }
            
            // (optional, default: false) set isVolatile
            if (collection.IsVolatile)
            {
                document.SetField("isVolatile", collection.IsVolatile);
            }
            
            // (optional) set keyOptions
            if (collection.KeyOptions != null)
            {
                if (collection.KeyOptions.GeneratorType != 0)
                {
                    document.SetField("keyOptions.type", collection.KeyOptions.GeneratorType);
                    
                    if (collection.KeyOptions.GeneratorType == ArangoKeyGeneratorType.Autoincrement)
                    {
                        if (collection.KeyOptions.Increment > 0)
                        {
                            document.SetField("keyOptions.increment", collection.KeyOptions.Increment);
                        }
                        
                        if (collection.KeyOptions.Offset > 0)
                        {
                            document.SetField("keyOptions.offset", collection.KeyOptions.Offset);
                        }
                    }
                }
                
                if (collection.KeyOptions.AllowUserKeys)
                {
                    document.SetField("keyOptions.allowUserKeys", collection.KeyOptions.AllowUserKeys);
                }
            }
            
            request.Body = Document.Serialize(document);
            
            var response = _connection.Process(request);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.Id = response.Document.GetField<string>("id");
                    collection.Name = response.Document.GetField<string>("name");
                    collection.Status = response.Document.GetField<ArangoCollectionStatus>("status");
                    collection.Type = response.Document.GetField<ArangoCollectionType>("type");
                    collection.WaitForSync = response.Document.GetField<bool>("waitForSync");
                    collection.IsVolatile = response.Document.GetField<bool>("isVolatile");
                    collection.IsSystem = response.Document.GetField<bool>("isSystem");
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
        
        internal bool Delete(string name)
        {
            var request = new Request(RequestType.Collection, HttpMethod.Delete);
            request.RelativeUri = string.Join("/", _apiUri, name);
            
            var response = _connection.Process(request);
            bool isDeleted = false;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NotFound:
                    isDeleted = true;
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
            
            return isDeleted;
        }
        
        #endregion
        
        #region PUT
        
        internal bool PutTruncate(string name)
        {
            var request = new Request(RequestType.Collection, HttpMethod.Put);
            request.RelativeUri = string.Join("/", _apiUri, name, "truncate");
            
            var response = _connection.Process(request);
            var isTruncated = false;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    isTruncated = true;
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
            
            return isTruncated;
        }
        
        #endregion
    }
}
