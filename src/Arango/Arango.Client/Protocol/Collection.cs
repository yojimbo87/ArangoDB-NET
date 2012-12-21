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
            var requestData = new RequestData();
            requestData.RelativeUri = ApiUri + id;
            requestData.Method = RequestMethod.GET.ToString();

            var responseData = Node.Process(requestData);

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
