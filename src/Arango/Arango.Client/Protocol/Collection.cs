using System.Dynamic;
using System.Net;

namespace Arango.Client.Protocol
{
    internal class Collection
    {
        private string ApiUri { get { return "_api/collection/"; } }
        private ArangoNode Node { get; set; }

        internal Collection(ArangoNode node)
        {
            Node = node;
        }

        internal ArangoCollection Post(string name, ArangoCollectionType type, bool waitForSync, int journalSize)
        {
            dynamic body = new ExpandoObject();
            body.name = name;
            body.type = type;
            body.waitForSync = waitForSync;
            body.journalSize = journalSize;

            var request = new Request();
            request.RelativeUri = ApiUri;
            request.Method = RequestMethod.POST.ToString();
            request.Body = "";

            var response = Node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = (int)response.Object.id;
                    collection.Name = response.Object.name;
                    collection.WaitForSync = response.Object.waitForSync;
                    collection.JournalSize = (long)response.Object.journalSize;
                    collection.Status = (ArangoCollectionStatus)response.Object.status;
                    collection.Type = (ArangoCollectionType)response.Object.type;
                    break;
                default:
                    break;
            }

            return collection;
        }

        internal ArangoCollection Get(int id)
        {
            var request = new Request();
            request.RelativeUri = ApiUri + id + "/properties";
            request.Method = RequestMethod.GET.ToString();

            var response = Node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = (int)response.Object.id;
                    collection.Name = response.Object.name;
                    collection.WaitForSync = response.Object.waitForSync;
                    collection.JournalSize = (long)response.Object.journalSize;
                    collection.Status = (ArangoCollectionStatus)response.Object.status;
                    collection.Type = (ArangoCollectionType)response.Object.type;
                    break;
                default:
                    break;
            }

            return collection;
        }

        // returns only ID, Name, Status and Type
        internal ArangoCollection Get(string collectionName)
        {
            var request = new Request();
            request.RelativeUri = ApiUri + collectionName + "/properties";
            request.Method = RequestMethod.GET.ToString();

            var response = Node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = (int)response.Object.id;
                    collection.Name = response.Object.name;
                    collection.WaitForSync = response.Object.waitForSync;
                    collection.JournalSize = (long)response.Object.journalSize;
                    collection.Status = (ArangoCollectionStatus)response.Object.status;
                    collection.Type = (ArangoCollectionType)response.Object.type;
                    break;
                default:
                    break;
            }

            return collection;
        }
    }
}
