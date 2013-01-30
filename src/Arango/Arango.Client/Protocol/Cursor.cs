using System.Collections.Generic;
using System.Dynamic;
using System.Net;

namespace Arango.Client.Protocol
{
    internal class Cursor
    {
        private string _apiUri { get { return "_api/cursor/"; } }
        private JsonParser _parser = new JsonParser();
        private ArangoNode _node;

        internal Cursor(ArangoNode node)
        {
            _node = node;
        }

        #region POST

        internal List<ArangoDocument> Post(string query, bool count, int batchSize, Dictionary<string, string> bindVars)
        {
            dynamic bodyObject = new ExpandoObject();
            bodyObject.query = query;

            if (count)
            {
                bodyObject.count = count;
            }

            if (batchSize > 0)
            {
                bodyObject.batchSize = batchSize;
            }

            if ((bindVars != null) && (bindVars.Count > 0))
            {
                bodyObject.bindVars = bindVars;
            }

            var request = new Request();
            request.RelativeUri = _apiUri;
            request.Method = RequestMethod.POST.ToString();
            request.Body = _parser.Serialize(bodyObject);

            var response = _node.Process(request);

            var documents = new List<ArangoDocument>();

            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    foreach (var jsonDocument in response.JsonObject.result)
                    {
                        ArangoDocument document = new ArangoDocument();
                        document.ID = jsonDocument._id;
                        document.Revision = jsonDocument._rev.ToString();
                        document.JsonObject = jsonDocument;

                        documents.Add(document);
                    }

                    if (response.JsonObject.hasMore)
                    {
                        documents.AddRange(Put((long)response.JsonObject.id));
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
                    foreach (var jsonDocument in response.JsonObject.result)
                    {
                        ArangoDocument document = new ArangoDocument();
                        document.ID = jsonDocument._id;
                        document.Revision = jsonDocument._rev.ToString();
                        document.JsonObject = jsonDocument;

                        documents.Add(document);
                    }

                    if (response.JsonObject.hasMore)
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
