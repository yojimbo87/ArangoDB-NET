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

            var responseData = Node.Process(request);

            var collection = new ArangoCollection();
            collection.ID = id;

            switch (responseData.StatusCode)
            {
                case HttpStatusCode.OK:
                    collection.Name = responseData.Content;
                    break;
                default:
                    break;
            }

            return collection;
        }
    }
}
