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
        internal Dictionary<string, string> QueryString { get; set; }
        internal string Method { get; set; }
        internal WebHeaderCollection Headers { get; set; }
        internal string Body { get; set; }

        internal Request()
        {
            QueryString = new Dictionary<string, string>();
            Headers = new WebHeaderCollection();
        }


    }
}
