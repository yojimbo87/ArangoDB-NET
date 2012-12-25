using System.Dynamic;
using System.Net;

namespace Arango.Client.Protocol
{
    internal class Collection
    {
        private string _apiUri { get { return "_api/collection/"; } }
        private JsonParser _parser = new JsonParser();
        private ArangoNode _node;

        internal Collection(ArangoNode node)
        {
            _node = node;
        }

        #region POST

        internal ArangoCollection Post(string name, ArangoCollectionType type, bool waitForSync, long journalSize)
        {
            dynamic bodyObject = new ExpandoObject();
            bodyObject.name = name;
            bodyObject.type = (int)type;
            bodyObject.waitForSync = waitForSync;
            bodyObject.journalSize = journalSize;
            bodyObject.isSystem = false;

            var request = new Request();
            request.RelativeUri = _apiUri;
            request.Method = RequestMethod.POST.ToString();
            request.Body = _parser.Serialize(bodyObject);

            var response = _node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = (long)response.JsonObject.id;
                    collection.Name = response.JsonObject.name;
                    collection.WaitForSync = response.JsonObject.waitForSync;
                    collection.JournalSize = journalSize;
                    collection.Status = (ArangoCollectionStatus)response.JsonObject.status;
                    collection.Type = (ArangoCollectionType)response.JsonObject.type;
                    break;
                default:
                    break;
            }

            return collection;
        }

        #endregion

        #region PUT

        internal bool TruncateCollection(long id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + id + "/truncate";
            request.Method = RequestMethod.PUT.ToString();

            return TruncateCollection(request);
        }

        internal bool TruncateCollection(string name)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + name + "/truncate";
            request.Method = RequestMethod.PUT.ToString();

            return TruncateCollection(request);
        }

        private bool TruncateCollection(Request request)
        {
            var response = _node.Process(request);

            bool isTruncated = false;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    isTruncated = true;
                    break;
                default:
                    break;
            }

            return isTruncated;
        }

        #endregion

        #region DELETE

        internal long Delete(long id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + id;
            request.Method = RequestMethod.DELETE.ToString();

            return Delete(request);
        }

        internal long Delete(string name)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + name;
            request.Method = RequestMethod.DELETE.ToString();

            return Delete(request);
        }

        private long Delete(Request request)
        {
            var response = _node.Process(request);

            long collectionID = 0;

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    if (response.JsonObject.error == false)
                    {
                        collectionID = (long)response.JsonObject.id;
                    }
                    break;
                default:
                    break;
            }

            return collectionID;
        }

        #endregion

        #region GET

        // returns collection id, name, status, type
        #region Get

        internal ArangoCollection Get(long id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + id;
            request.Method = RequestMethod.GET.ToString();

            var response = _node.Process(request);

            return Get(request);
        }

        internal ArangoCollection Get(string collectionName)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + collectionName;
            request.Method = RequestMethod.GET.ToString();

            var response = _node.Process(request);

            return Get(request);
        }

        private ArangoCollection Get(Request request)
        {
            var response = _node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = (long)response.JsonObject.id;
                    collection.Name = response.JsonObject.name;
                    collection.Status = (ArangoCollectionStatus)response.JsonObject.status;
                    collection.Type = (ArangoCollectionType)response.JsonObject.type;
                    break;
                default:
                    break;
            }

            return collection;
        }

        #endregion

        // returns collection id, name, status, type, waitForSync, journalSize
        #region GetProperties

        internal ArangoCollection GetProperties(long id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + id + "/properties";
            request.Method = RequestMethod.GET.ToString();

            var response = _node.Process(request);

            return GetProperties(request);
        }

        internal ArangoCollection GetProperties(string collectionName)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + collectionName + "/properties";
            request.Method = RequestMethod.GET.ToString();

            var response = _node.Process(request);

            return GetProperties(request);
        }

        private ArangoCollection GetProperties(Request request)
        {
            var response = _node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = (long)response.JsonObject.id;
                    collection.Name = response.JsonObject.name;
                    collection.WaitForSync = response.JsonObject.waitForSync;
                    collection.JournalSize = (long)response.JsonObject.journalSize;
                    collection.Status = (ArangoCollectionStatus)response.JsonObject.status;
                    collection.Type = (ArangoCollectionType)response.JsonObject.type;
                    break;
                default:
                    break;
            }

            return collection;
        }

        #endregion

        #endregion
    }
}
