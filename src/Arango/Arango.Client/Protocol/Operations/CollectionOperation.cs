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
            request.Body = Document.Serialize(collection);
            
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
    }
}
