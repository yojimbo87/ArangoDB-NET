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
            var request = new Request();
            request.RelativeUri = string.Join("/", _apiUri, name);
            request.Method = HttpMethod.Get;

            return Get(request);
        }
        
        private ArangoCollection Get(Request request)
        {
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
                default:
                    break;
            }

            return collection;
        }
        
        #endregion
        
        #region POST
        
        internal void Post(ArangoCollection collection)
        {
            var request = new Request();
            request.RelativeUri = _apiUri;
            request.Method = HttpMethod.Post;
            
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
                    break;
            }
        }
        
        #endregion
        
        #region DELETE
        
        internal bool Delete(string name)
        {
            var request = new Request();
            request.RelativeUri = string.Join("/", _apiUri, name);
            request.Method = HttpMethod.Delete;
            
            var response = _connection.Process(request);
            var collectionId = "";

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collectionId = response.Document.GetField<string>("id");
                    break;
                default:
                    break;
            }
            
            if (string.IsNullOrEmpty(collectionId))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        #endregion
    }
}
