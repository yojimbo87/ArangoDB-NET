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

        internal ArangoCollection Get(int id)
        {
            var request = new Request();
            request.RelativeUri = ApiUri + id;
            request.Method = RequestMethod.GET.ToString();

            var response = Node.Process(request);

            var collection = new ArangoCollection();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.ID = (int)response.Object.id;
                    collection.Name = response.Object.name;
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
