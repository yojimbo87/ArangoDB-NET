using System;
using System.Collections.Generic;
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
            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.Id = response.Document.String("id");
                    collection.Name = response.Document.String("name");
                    collection.Status = response.Document.Enum<ArangoCollectionStatus>("status");
                    collection.Type = response.Document.Enum<ArangoCollectionType>("type");
                    break;
                case HttpStatusCode.NotFound:
                    collection = null;
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

            return collection;
        }
        
        #endregion
        
        #region PROPERTIES
        
        internal ArangoCollection Properties(string name)
        {
            var request = new Request(RequestType.Collection, HttpMethod.Get);
            request.RelativeUri = string.Join("/", _apiUri, name, "properties");
            
            var response = _connection.Process(request);
            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.Id = response.Document.String("id");
                    collection.Name = response.Document.String("name");
                    collection.Status = response.Document.Enum<ArangoCollectionStatus>("status");
                    collection.Type = response.Document.Enum<ArangoCollectionType>("type");
                    collection.WaitForSync = response.Document.Bool("waitForSync");
                    collection.IsSystem = response.Document.Bool("isSystem");
                    collection.IsVolatile = response.Document.Bool("isVolatile");                    
                    collection.JournalSize = response.Document.Int("journalSize");      
                    if (response.Document.Has("numberOfShards")) {
                    	collection.NumberOfShards = response.Document.Int("numberOfShards");
                    }
                    if (response.Document.Has("shardKeys")) {
                    	collection.ShardKeys = (List<string>) response.Document.List<string>("shardKeys");
                    }
                    break;
                case HttpStatusCode.NotFound:
                    collection = null;
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
            document.String("name", collection.Name);
            
            // (optional, default: 2) set type
            if (collection.Type != 0)
            {
                document.Enum("type", collection.Type);
            }
            
            // (optional, default: false) set waitForSync
            if (collection.WaitForSync)
            {
                document.Bool("waitForSync", collection.WaitForSync);
            }
            
            // (optional, default: arangodb config) set journalSize
            if (collection.JournalSize > 0)
            {
                document.Int("journalSize", collection.JournalSize);
            }
            
            // (optional, default: false) set isSystem
            if (collection.IsSystem)
            {
                document.Bool("isSystem", collection.IsSystem);
            }
            
            // (optional, default: false) set isVolatile
            if (collection.IsVolatile)
            {
                document.Bool("isVolatile", collection.IsVolatile);
            }
            
            if (collection.NumberOfShards != null)
            {
            	document.Int("numberOfShards", (int) collection.NumberOfShards);
            }
            
            if (collection.ShardKeys != null)
            {
            	document.List("shardKeys", collection.ShardKeys);
            }
            
            // (optional) set keyOptions
            if (collection.KeyOptions != null)
            {
                if (collection.KeyOptions.GeneratorType != 0)
                {
                    document.String("keyOptions.type", collection.KeyOptions.GeneratorType.ToString().ToLower());
                    
                    if (collection.KeyOptions.GeneratorType == ArangoKeyGeneratorType.Autoincrement)
                    {
                        if (collection.KeyOptions.Increment > 0)
                        {
                            document.Int("keyOptions.increment", collection.KeyOptions.Increment);
                        }
                        
                        if (collection.KeyOptions.Offset > 0)
                        {
                            document.Int("keyOptions.offset", collection.KeyOptions.Offset);
                        }
                    }
                }
                
                if (collection.KeyOptions.AllowUserKeys)
                {
                    document.Bool("keyOptions.allowUserKeys", collection.KeyOptions.AllowUserKeys);
                }
            }
            
            request.Body = Document.Serialize(document);
            
            var response = _connection.Process(request);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.Id = response.Document.String("id");
                    collection.Name = response.Document.String("name");
                    collection.Status = response.Document.Enum<ArangoCollectionStatus>("status");
                    collection.Type = response.Document.Enum<ArangoCollectionType>("type");
                    collection.WaitForSync = response.Document.Bool("waitForSync");
                    collection.IsVolatile = response.Document.Bool("isVolatile");
                    collection.IsSystem = response.Document.Bool("isSystem");
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
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
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
                            response.Document.String("driverErrorMessage"),
                            response.Document.String("driverExceptionMessage"),
                            response.Document.Object<Exception>("driverInnerException")
                        );
                    }
                    break;
            }
            
            return isTruncated;
        }
        
        #endregion
    }
}
