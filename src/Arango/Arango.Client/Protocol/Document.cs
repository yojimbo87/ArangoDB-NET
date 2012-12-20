
namespace Arango.Client.Protocol
{
    internal class Document
    {
        private string ApiUri { get { return "_api/document/"; } }

        internal RequestMethod RequestMethod { get; set; }
        internal string Handle { get; set; }
        internal string Revision { get; set; }

        internal ResponseData Request(ArangoNode node)
        {
            RequestData requestData = new RequestData();
            requestData.RelativeUri = ApiUri + Handle;
            requestData.Method = RequestMethod.ToString();

            if (!string.IsNullOrEmpty(Revision))
            {
                requestData.Headers.Add("If-None-Match", "\"" + Revision + "\"");
            }

            return node.Process(requestData);
        }
    }
}
