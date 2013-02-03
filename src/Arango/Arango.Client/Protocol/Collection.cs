using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using ServiceStack.Text;

namespace Arango.Client.Protocol
{
    internal class Collection
    {
        private string _apiUri { get { return "_api/collection/"; } }
        private ArangoNode _node;

        internal Collection(ArangoNode node)
        {
            _node = node;
        }

        #region POST

        internal ArangoCollection Post(string name, ArangoCollectionType type, bool waitForSync, long journalSize)
        {
            Json bodyObject = new Json();
            bodyObject.Set("name", name);
            bodyObject.Set("type", (int)type);
            bodyObject.Set("waitForSync", waitForSync);
            bodyObject.Set("journalSize", journalSize);
            bodyObject.Set("isSystem", false);

            var request = new Request();
            request.RelativeUri = _apiUri;
            request.Method = RequestMethod.POST.ToString();
            request.Body = bodyObject.Stringify();

            var response = _node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = response.JsonObject.Get<long>("id");
                    collection.Name = response.JsonObject.Get("name");
                    collection.WaitForSync = response.JsonObject.Get<bool>("waitForSync");
                    collection.JournalSize = journalSize;
                    collection.Status = response.JsonObject.Get<ArangoCollectionStatus>("status");
                    collection.Type = response.JsonObject.Get<ArangoCollectionType>("type");
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
                    collection.ID = response.JsonObject.Get<long>("id");
                    collection.Name = response.JsonObject.Get("name");
                    collection.Status = response.JsonObject.Get<ArangoCollectionStatus>("status");
                    collection.Type = response.JsonObject.Get<ArangoCollectionType>("type");
                    collection.DocumentsCount = response.JsonObject.Get<long>("count");
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
                    collection.ID = response.JsonObject.Get<long>("id");
                    collection.Name = response.JsonObject.Get("name");
                    collection.Status = response.JsonObject.Get<ArangoCollectionStatus>("status");
                    collection.Type = response.JsonObject.Get<ArangoCollectionType>("type");
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
            Json bodyObject = new Json();
            bodyObject.Set("waitForSync", waitForSync);

            var request = new Request();
            request.RelativeUri = _apiUri + id + "/properties";
            request.Method = RequestMethod.PUT.ToString();
            request.Body = bodyObject.Stringify();

            return PutProperties(request);
        }

        internal ArangoCollection PutProperties(string name, bool waitForSync)
        {
            Json bodyObject = new Json();
            bodyObject.Set("waitForSync", waitForSync);

            var request = new Request();
            request.RelativeUri = _apiUri + name + "/properties";
            request.Method = RequestMethod.PUT.ToString();
            request.Body = bodyObject.Stringify();

            return PutProperties(request);
        }

        internal ArangoCollection PutProperties(Request request)
        {
            var response = _node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = response.JsonObject.Get<long>("id");
                    collection.Name = response.JsonObject.Get("name");
                    collection.Status = response.JsonObject.Get<ArangoCollectionStatus>("status");
                    collection.Type = response.JsonObject.Get<ArangoCollectionType>("type");
                    collection.JournalSize = response.JsonObject.Get<long>("journalSize");
                    collection.WaitForSync = response.JsonObject.Get<bool>("waitForSync");
                    break;
                default:
                    break;
            }

            return collection;
        }

        #endregion

        #region PutRename

        internal ArangoCollection PutRename(long id, string newName)
        {
            Json bodyObject = new Json();
            bodyObject.Set("name", newName);

            var request = new Request();
            request.RelativeUri = _apiUri + id + "/rename";
            request.Method = RequestMethod.PUT.ToString();
            request.Body = bodyObject.Stringify();

            return PutRename(request);
        }

        internal ArangoCollection PutRename(string name, string newName)
        {
            Json bodyObject = new Json();
            bodyObject.Set("name", newName);

            var request = new Request();
            request.RelativeUri = _apiUri + name + "/rename";
            request.Method = RequestMethod.PUT.ToString();
            request.Body = bodyObject.Stringify();

            return PutRename(request);
        }

        internal ArangoCollection PutRename(Request request)
        {
            var response = _node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = response.JsonObject.Get<long>("id");
                    collection.Name = response.JsonObject.Get("name");
                    collection.Status = response.JsonObject.Get<ArangoCollectionStatus>("status");
                    collection.Type = response.JsonObject.Get<ArangoCollectionType>("type");
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
                    if (response.JsonObject.Get<bool>("error") == false)
                    {
                        collectionID = response.JsonObject.Get<long>("id");
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
                    collection.ID = response.JsonObject.Get<long>("id");
                    collection.Name = response.JsonObject.Get("name");
                    collection.Status = response.JsonObject.Get<ArangoCollectionStatus>("status");
                    collection.Type = response.JsonObject.Get<ArangoCollectionType>("type");
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
                    collection.ID = response.JsonObject.Get<long>("id");
                    collection.Name = response.JsonObject.Get("name");
                    collection.Status = response.JsonObject.Get<ArangoCollectionStatus>("status");
                    collection.Type = response.JsonObject.Get<ArangoCollectionType>("type");
                    collection.WaitForSync = response.JsonObject.Get<bool>("waitForSync");
                    collection.JournalSize = response.JsonObject.Get<long>("journalSize");
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
                    collection.ID = response.JsonObject.Get<long>("id");
                    collection.Name = response.JsonObject.Get("name");
                    collection.Status = response.JsonObject.Get<ArangoCollectionStatus>("status");
                    collection.Type = response.JsonObject.Get<ArangoCollectionType>("type");
                    collection.WaitForSync = response.JsonObject.Get<bool>("waitForSync");
                    collection.JournalSize = response.JsonObject.Get<long>("journalSize");
                    collection.DocumentsCount = response.JsonObject.Get<long>("count");
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
                    collection.ID = response.JsonObject.Get<long>("id");
                    collection.Name = response.JsonObject.Get("name");
                    collection.Status = response.JsonObject.Get<ArangoCollectionStatus>("status");
                    collection.Type = response.JsonObject.Get<ArangoCollectionType>("type");
                    collection.WaitForSync = response.JsonObject.Get<bool>("waitForSync");
                    collection.JournalSize = response.JsonObject.Get<long>("journalSize");
                    collection.DocumentsCount = response.JsonObject.Get<long>("count");

                    collection.AliveDocumentsCount = response.JsonObject.Get<long>("figures.alive.count");
                    collection.AliveDocumentsSize = response.JsonObject.Get<long>("figures.alive.size");
                    collection.DeadDocumentsCount = response.JsonObject.Get<long>("figures.dead.count");
                    collection.DeadDocumentsSize = response.JsonObject.Get<long>("figures.dead.size");
                    collection.DeadDeletetionCount = response.JsonObject.Get<long>("figures.dead.deletion");
                    collection.DataFilesCount = response.JsonObject.Get<long>("figures.datafiles.count");
                    collection.DataFilesSize = response.JsonObject.Get<long>("figures.datafiles.fileSize");
                    collection.JournalsCount = response.JsonObject.Get<long>("figures.journals.count");
                    collection.JournalsFileSize = response.JsonObject.Get<long>("figures.journals.fileSize");
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
                    List<Json> foo = response.JsonObject.Get<List<Json>>("collections");
                    foreach (var item in foo)
                    {
                        ArangoCollection collection = new ArangoCollection();
                        collection.ID = item.Get<long>("id");
                        collection.Name = item.Get("name");
                        collection.Status = item.Get<ArangoCollectionStatus>("status");
                        collection.Type = item.Get<ArangoCollectionType>("type");

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
