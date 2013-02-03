using System.Collections.Generic;
using System.Dynamic;
using System.Net;

namespace Arango.Client.Protocol
{
    internal class Cursor
    {
        private string _apiUri { get { return "_api/cursor/"; } }
        private ArangoNode _node;

        internal Cursor(ArangoNode node)
        {
            _node = node;
        }

        #region POST

        internal List<ArangoDocument> Post(string query, bool count, int batchSize, Dictionary<string, string> bindVars)
        {
            Json bodyObject = new Json();
            bodyObject.Set("query", query);

            if (count)
            {
                bodyObject.Set("count", count);
            }

            if (batchSize > 0)
            {
                bodyObject.Set("batchSize", batchSize);
            }

            if ((bindVars != null) && (bindVars.Count > 0))
            {
                bodyObject.Set("bindVars", bindVars);
            }

            var request = new Request();
            request.RelativeUri = _apiUri;
            request.Method = RequestMethod.POST.ToString();
            request.Body = bodyObject.Stringify();

            var response = _node.Process(request);

            var documents = new List<ArangoDocument>();

            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    foreach (var jsonDocument in response.JsonObject.Get<List<Json>>("result"))
                    {
                        ArangoDocument document = new ArangoDocument();
                        document.ID = jsonDocument.Get("_id");
                        document.Revision = jsonDocument.Get("_rev");
                        document.JsonObject = jsonDocument;

                        documents.Add(document);
                    }

                    if (response.JsonObject.Get<bool>("hasMore"))
                    {
                        documents.AddRange(Put(response.JsonObject.Get<long>("id")));
                    }
                    break;
                default:
                    break;
            }

            return documents;
        }

        #endregion

        #region PUT

        internal List<ArangoDocument> Put(long cursor)
        {
            var request = new Request();
            request.RelativeUri = _apiUri + cursor;
            request.Method = RequestMethod.PUT.ToString();

            var response = _node.Process(request);

            List<ArangoDocument> documents = new List<ArangoDocument>();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    foreach (var jsonDocument in response.JsonObject.Get<List<Json>>("result"))
                    {
                        ArangoDocument document = new ArangoDocument();
                        document.ID = jsonDocument.Get("_id");
                        document.Revision = jsonDocument.Get("_rev");
                        document.JsonObject = jsonDocument;

                        documents.Add(document);
                    }

                    if (response.JsonObject.Get<bool>("hasMore"))
                    {
                        documents.AddRange(Put(cursor));
                    }
                    break;
                default:
                    break;
            }

            return documents;
        }

        #endregion
    }
}
