using System.Collections.Generic;
using System.Net;

namespace Arango.Client.Protocol
{
    internal class Document
    {
        private string _apiUri { get { return "_api/document"; } }
        private ArangoNode _node { get; set; }

        internal Document(ArangoNode node)
        {
            _node = node;
        }

        #region POST

        internal ArangoDocument Post(long collectionID, Json jsonObject, bool waitForSync)
        {
            var request = new Request();
            request.RelativeUri = _apiUri;
            request.QueryString.Add("collection", collectionID.ToString());
            request.Method = RequestMethod.POST.ToString();
            request.Body = jsonObject.Stringify();

            if (waitForSync)
            {
                request.QueryString.Add("waitForSync", "true");
            }

            return Post(request);
        }

        internal ArangoDocument Post(string collectionName, bool createCollection, Json jsonObject, bool waitForSync)
        {
            var request = new Request();
            request.RelativeUri = _apiUri;
            request.QueryString.Add("collection", collectionName);
            request.Method = RequestMethod.POST.ToString();
            request.Body = jsonObject.Stringify();

            if (createCollection)
            {
                request.QueryString.Add("createCollection", "true");
            }

            if (waitForSync)
            {
                request.QueryString.Add("waitForSync", "true");
            }

            return Post(request);
        }

        private ArangoDocument Post(Request request)
        {
            var response = _node.Process(request);

            var document = new ArangoDocument();

            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                    document.ID = response.JsonObject.GetValue("_id");
                    document.Revision = response.JsonObject.GetValue("_rev");
                    break;
                default:
                    break;
            }

            return document;
        }

        #endregion

        #region PUT

        internal string Put(string documentID, string revision, DocumentUpdatePolicy policy, Json jsonObject, bool waitForSync)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + "/" + documentID;
            request.Method = RequestMethod.PUT.ToString();
            request.Body = jsonObject.Stringify();

            if (!string.IsNullOrEmpty(revision))
            {
                request.QueryString.Add("_rev", revision);
            }

            switch (policy)
            {
                case DocumentUpdatePolicy.Error:
                    request.QueryString.Add("policy", "error");
                    break;
                case DocumentUpdatePolicy.Last:
                    request.QueryString.Add("policy", "last");
                    break;
                default:
                    break;
            }

            if (waitForSync)
            {
                request.QueryString.Add("waitForSync", "true");
            }

            var response = _node.Process(request);
            var newRevision = "";

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    newRevision = response.JsonObject.GetValue("_rev");
                    break;
                default:
                    break;
            }

            return newRevision;
        }

        #endregion

        #region PATCH

        internal string Patch(string documentID, string revision, DocumentUpdatePolicy policy, Json jsonObject, bool keepNullFields, bool waitForSync)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + "/" + documentID;
            request.Method = RequestMethod.PATCH.ToString();
            request.Body = jsonObject.Stringify();

            if (!string.IsNullOrEmpty(revision))
            {
                request.QueryString.Add("_rev", revision);
            }

            switch (policy)
            {
                case DocumentUpdatePolicy.Error:
                    request.QueryString.Add("policy", "error");
                    break;
                case DocumentUpdatePolicy.Last:
                    request.QueryString.Add("policy", "last");
                    break;
                default:
                    break;
            }

            if (!keepNullFields)
            {
                request.QueryString.Add("keepNull", "false");
            }

            if (waitForSync)
            {
                request.QueryString.Add("waitForSync", "true");
            }

            var response = _node.Process(request);
            var newRevision = "";

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    newRevision = response.JsonObject.GetValue("_rev");
                    break;
                default:
                    break;
            }

            return newRevision;
        }

        #endregion

        #region DELETE

        internal string Delete(string documentID, string revision, DocumentUpdatePolicy policy, bool waitForSync)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + "/" + documentID;
            request.Method = RequestMethod.DELETE.ToString();

            if (!string.IsNullOrEmpty(revision))
            {
                request.QueryString.Add("_rev", revision);
            }

            switch (policy)
            {
                case DocumentUpdatePolicy.Error:
                    request.QueryString.Add("policy", "error");
                    break;
                case DocumentUpdatePolicy.Last:
                    request.QueryString.Add("policy", "last");
                    break;
                default:
                    break;
            }

            if (waitForSync)
            {
                request.QueryString.Add("waitForSync", "true");
            }

            var response = _node.Process(request);
            var deletedDocumentID = "";

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    deletedDocumentID = response.JsonObject.GetValue("_id");
                    break;
                default:
                    break;
            }

            return deletedDocumentID;
        }

        #endregion

        #region HEAD

        internal ArangoDocument Head(string documentID)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + "/" + documentID;
            request.Method = RequestMethod.HEAD.ToString();

            var response = _node.Process(request);
            var document = new ArangoDocument();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    document.ID = documentID;
                    document.Revision = response.Headers.Get("etag").Replace("\"", "");
                    break;
                default:
                    break;
            }

            return document;
        }

        #endregion

        #region GET

        #region Get

        internal ArangoDocument Get(string id)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + "/" + id;
            request.Method = RequestMethod.GET.ToString();

            return Get(request);
        }

        internal ArangoDocument Get(string id, string revision)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + "/" + id;
            request.Method = RequestMethod.GET.ToString();

            if (!string.IsNullOrEmpty(revision))
            {
                request.Headers.Add("If-None-Match", "\"" + revision + "\"");
            }

            return Get(request);
        }

        private ArangoDocument Get(Request request)
        {
            var response = _node.Process(request);

            var document = new ArangoDocument();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    document.ID = response.JsonObject.GetValue("_id");
                    document.Revision = response.Headers.Get("etag").Replace("\"", "");
                    document.JsonObject = response.JsonObject;
                    break;
                case HttpStatusCode.NotModified:
                    document.Revision = response.Headers.Get("etag").Replace("\"", "");
                    break;
                default:
                    break;
            }

            return document;
        }

        #endregion

        #region GetAll

        internal List<ArangoDocument> GetAll(long collectionID)
        {
            var request = new Request();
            request.RelativeUri = _apiUri;
            request.Method = RequestMethod.GET.ToString();

            request.QueryString.Add("collection", collectionID.ToString());

            return GetAll(request);
        }

        internal List<ArangoDocument> GetAll(string collectionName)
        {
            var request = new Request();
            request.RelativeUri = _apiUri;
            request.Method = RequestMethod.GET.ToString();

            request.QueryString.Add("collection", collectionName);

            return GetAll(request);
        }

        private List<ArangoDocument> GetAll(Request request)
        {
            var response = _node.Process(request);

            var documents = new List<ArangoDocument>();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    foreach (string item in response.JsonObject.GetValue<List<string>>("documents"))
                    {
                        var document = new ArangoDocument();
                        var lastSlashIndex = item.LastIndexOf('/') - 1;
                        document.ID = item.Substring(item.LastIndexOf('/', lastSlashIndex) + 1);

                        documents.Add(document);
                    }
                    break;
                default:
                    break;
            }

            return documents;
        }

        #endregion

        #endregion
    }
}
