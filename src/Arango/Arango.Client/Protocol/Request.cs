using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Arango.Client.Protocol
{
    internal class Request
    {
        internal HttpMethod HttpMethod { get; set; }
        internal string OperationUri { get; set; }
        internal WebHeaderCollection Headers = new WebHeaderCollection();
        internal Dictionary<string, string> QueryString = new Dictionary<string, string>();
        internal string Body { get; set; }

        internal Request(HttpMethod httpMethod, string apiUri)
        {
            HttpMethod = httpMethod;
            OperationUri = apiUri;
        }

        internal Request(HttpMethod httpMethod, string apiUri, string operationUri) : this(httpMethod, apiUri + operationUri) {}
        
        internal string GetRelativeUri()
        {
            var uri = new StringBuilder(OperationUri);
            
            if (QueryString.Count > 0)
            {
                uri.Append("?");
                
                var index = 0;

                foreach (var item in QueryString)
                {
                    uri.Append(item.Key + "=" + item.Value);

                    index++;

                    if (index != QueryString.Count)
                    {
                        uri.Append("&");
                    }
                }
            }
            
            return uri.ToString();
        }
        
        internal void TrySetHeaderParameter(string parameterName, Dictionary<string, object> parameters)
        {
            if (parameters.ContainsKey(parameterName))
            {
                Headers.Add(parameterName, parameters.String(parameterName));
            }
        }
        
        internal void TrySetQueryStringParameter(string parameterName, Dictionary<string, object> parameters)
        {
            if (parameters.ContainsKey(parameterName))
            {
                QueryString.Add(parameterName, parameters.String(parameterName));
            }
        }
        
        internal static void TrySetBodyParameter(string parameterName, Dictionary<string, object> source, Dictionary<string, object> destination)
        {
            if (source.Has(parameterName))
            {
                destination.Object(parameterName, source.Object(parameterName));
            }
        }
    }
}
