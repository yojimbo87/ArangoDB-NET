using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Arango.Client.Protocol
{
    internal class Request
    {
        private string _relativeUri;

        internal string RelativeUri 
        {
            get
            {
                if (QueryString.Count == 0)
                {
                    return _relativeUri;
                }
                else
                {
                    var uri = new StringBuilder(_relativeUri + "?");
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

                    return uri.ToString();
                }
            }

            set 
            {
                _relativeUri = value;
            }

        }
        
        internal RequestType Type { get; set; }
        internal string Method { get; set; }
        internal WebHeaderCollection Headers = new WebHeaderCollection();
        internal Dictionary<string, string> QueryString = new Dictionary<string, string>();
        internal string Body { get; set; }

        internal Request(RequestType requestType, string method)
        {
            Type = requestType;
            Method = method;
        }
    }
}

