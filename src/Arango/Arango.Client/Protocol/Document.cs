using System.Net;

namespace Arango.Client.Protocol
{
    internal class Document
    {
        private string _apiUri { get { return "_api/document"; } }
        private JsonParser _parser = new JsonParser();
        private ArangoNode _node { get; set; }

        internal Document(ArangoNode node)
        {
            _node = node;
        }

        #region POST

        internal ArangoDocument Post(long collectionID, dynamic jsonObject, bool waitForSync)
        {
            var request = new Request();
            request.RelativeUri = _apiUri;
            request.QueryString.Add("collection", collectionID.ToString());
            request.Method = RequestMethod.POST.ToString();
            request.Body = _parser.Serialize(jsonObject);

            if (waitForSync)
            {
                request.QueryString.Add("waitForSync", "true");
            }

            return Post(request);
        }

        internal ArangoDocument Post(string collectionName, bool createCollection,  dynamic jsonObject, bool waitForSync)
        {
            var request = new Request();
            request.RelativeUri = _apiUri;
            request.QueryString.Add("collection", collectionName);
            request.Method = RequestMethod.POST.ToString();
            request.Body = _parser.Serialize(jsonObject);

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
                    document.ID = response.JsonObject._id;
                    document.Revision = ((long)response.JsonObject._rev).ToString();
                    break;
                default:
                    break;
            }

            return document;
        }

        #endregion

        #region PUT

        internal string Put(string documentID, string revision, ArangoDocumentPolicy policy, dynamic jsonObject, bool waitForSync)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + "/" + documentID;
            request.Method = RequestMethod.PUT.ToString();
            request.Body = _parser.Serialize(jsonObject);

            if (!string.IsNullOrEmpty(revision))
            {
                request.QueryString.Add("_rev", revision);
            }

            switch (policy)
            {
                case ArangoDocumentPolicy.Error:
                    request.QueryString.Add("policy", "error");
                    break;
                case ArangoDocumentPolicy.Last:
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
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                    newRevision = ((long)response.JsonObject._rev).ToString();
                    break;
                default:
                    break;
            }

            return newRevision;
        }

        #endregion

        #region GET

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
                    document.JsonObject = _parser.Deserialize(response.JsonString);
                    document.ID = document.JsonObject._id;
                    document.Revision = response.Headers.Get("etag").Replace("\"", "");
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
    }
}
