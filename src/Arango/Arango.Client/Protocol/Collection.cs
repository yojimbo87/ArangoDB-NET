using System.Dynamic;
using System.Collections.Generic;
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

        #region PutTruncate

        internal bool PutTruncate(long id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + id + "/truncate";
            request.Method = RequestMethod.PUT.ToString();

            return PutTruncate(request);
        }

        internal bool PutTruncate(string name)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + name + "/truncate";
            request.Method = RequestMethod.PUT.ToString();

            return PutTruncate(request);
        }

        private bool PutTruncate(Request request)
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

        #region PutLoad

        internal ArangoCollection PutLoad(long id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + id + "/load";
            request.Method = RequestMethod.PUT.ToString();

            return PutLoad(request);
        }

        internal ArangoCollection PutLoad(string name)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + name + "/load";
            request.Method = RequestMethod.PUT.ToString();

            return PutLoad(request);
        }

        internal ArangoCollection PutLoad(Request request)
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
                    collection.DocumentsCount = (long)response.JsonObject.count;
                    break;
                default:
                    break;
            }

            return collection;
        }

        #endregion

        #region PutUnload

        internal ArangoCollection PutUnload(long id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + id + "/unload";
            request.Method = RequestMethod.PUT.ToString();

            return PutUnload(request);
        }

        internal ArangoCollection PutUnload(string name)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + name + "/unload";
            request.Method = RequestMethod.PUT.ToString();

            return PutUnload(request);
        }

        internal ArangoCollection PutUnload(Request request)
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

        #region PutProperties

        internal ArangoCollection PutProperties(long id, bool waitForSync)
        {
            dynamic bodyObject = new ExpandoObject();
            bodyObject.waitForSync = waitForSync;

            var request = new Request();
            request.RelativeUri = _apiUri + id + "/properties";
            request.Method = RequestMethod.PUT.ToString();
            request.Body = _parser.Serialize(bodyObject);

            return PutProperties(request);
        }

        internal ArangoCollection PutProperties(string name, bool waitForSync)
        {
            dynamic bodyObject = new ExpandoObject();
            bodyObject.waitForSync = waitForSync;

            var request = new Request();
            request.RelativeUri = _apiUri + name + "/properties";
            request.Method = RequestMethod.PUT.ToString();
            request.Body = _parser.Serialize(bodyObject);

            return PutProperties(request);
        }

        internal ArangoCollection PutProperties(Request request)
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
                    collection.JournalSize = (long)response.JsonObject.journalSize;
                    collection.WaitForSync = response.JsonObject.waitForSync;
                    break;
                default:
                    break;
            }

            return collection;
        }

        #endregion

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

            return Get(request);
        }

        internal ArangoCollection Get(string collectionName)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + collectionName;
            request.Method = RequestMethod.GET.ToString();

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

            return GetProperties(request);
        }

        internal ArangoCollection GetProperties(string collectionName)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + collectionName + "/properties";
            request.Method = RequestMethod.GET.ToString();

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

        // returns collection id, name, status, type, waitForSync, journalSize, count
        #region GetCount

        internal ArangoCollection GetCount(long id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + id + "/count";
            request.Method = RequestMethod.GET.ToString();

            return GetCount(request);
        }

        internal ArangoCollection GetCount(string name)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + name + "/count";
            request.Method = RequestMethod.GET.ToString();

            return GetCount(request);
        }

        private ArangoCollection GetCount(Request request)
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
                    collection.DocumentsCount = (long)response.JsonObject.count;
                    break;
                default:
                    break;
            }

            return collection;
        }

        #endregion

        // returns collection id, name, status, type, waitForSync, journalSize, count, figures
        #region GetFigures

        internal ArangoCollection GetFigures(long id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + id + "/figures";
            request.Method = RequestMethod.GET.ToString();

            return GetFigures(request);
        }

        internal ArangoCollection GetFigures(string name)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + name + "/figures";
            request.Method = RequestMethod.GET.ToString();

            return GetFigures(request);
        }

        private ArangoCollection GetFigures(Request request)
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
                    collection.DocumentsCount = (long)response.JsonObject.count;
                    collection.AliveDocumentsCount = (long)response.JsonObject.figures.alive.count;
                    collection.AliveDocumentsSize = (long)response.JsonObject.figures.alive.size;
                    collection.DeadDocumentsCount = (long)response.JsonObject.figures.dead.count;
                    collection.DeadDocumentsSize = (long)response.JsonObject.figures.dead.size;
                    collection.DeadDeletetionCount = (long)response.JsonObject.figures.dead.deletion;
                    collection.DataFilesCount = (long)response.JsonObject.figures.datafiles.count;
                    collection.DataFilesSize = (long)response.JsonObject.figures.datafiles.fileSize;
                    collection.JournalsCount = (long)response.JsonObject.figures.journals.count;
                    collection.JournalsFileSize = (long)response.JsonObject.figures.journals.fileSize;
                    break;
                default:
                    break;
            }

            return collection;
        }

        #endregion

        // returns list of collections where each item consists of id, name, status, type
        internal List<ArangoCollection> GetAll()
        {
            var request = new Request();
            request.RelativeUri = _apiUri;
            request.Method = RequestMethod.GET.ToString();

            var response = _node.Process(request);

            var collections = new List<ArangoCollection>();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    foreach (var item in response.JsonObject.collections)
                    {
                        ArangoCollection collection = new ArangoCollection();
                        collection.ID = (long)item.id;
                        collection.Name = item.name;
                        collection.Status = (ArangoCollectionStatus)item.status;
                        collection.Type = (ArangoCollectionType)item.type;

                        collections.Add(collection);
                    }
                    break;
                default:
                    break;
            }

            return collections;
        }

        #endregion
    }
}
