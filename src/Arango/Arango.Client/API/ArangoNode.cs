using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using ServiceStack.Text;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoNode
    {
        private string _userAgent = "Arango-NET/alpha";

        #region Properties

        public string Server { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Alias { get; set; }

        public Uri BaseUri { get; set; }

        public CredentialCache Credentials { get; set; }

        #endregion

        public ArangoNode(string server, int port, string userName, string password, string alias)
        {
            Server = server;
            Port = port;
            Username = userName;
            Password = password;
            Alias = alias;

            BaseUri = new Uri("http://" + server + ":" + port + "/");
            
            Credentials = new CredentialCache();
            Credentials.Add(BaseUri, "Basic", new NetworkCredential(userName, password));
        }

        internal Response Process(Request request)
        {
            var httpRequest = (HttpWebRequest)HttpWebRequest.Create(BaseUri + request.RelativeUri);
            httpRequest.KeepAlive = true;
            httpRequest.Method = request.Method;
            httpRequest.UserAgent = _userAgent;
            httpRequest.Credentials = Credentials;

            if ((request.Headers.Count > 0))
            {
                httpRequest.Headers = request.Headers;
            }

            /*Stream stream1 = request.GetRequestStream();
            stream1.Write(data, 0, data.Length);
            stream1.Close();*/

            var response = new Response();

            try
            {
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                var reader = new StreamReader(httpResponse.GetResponseStream());

                response.StatusCode = httpResponse.StatusCode;
                response.Headers = httpResponse.Headers;
                response.Content = reader.ReadToEnd();
                response.Data = new JsonParser().Deserialize(response.Content);
            }
            catch (WebException webException)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                var reader = new StreamReader(httpResponse.GetResponseStream());

                if (httpResponse.StatusCode != HttpStatusCode.NotModified)
                {
                    throw new ArangoException(
                        httpResponse.StatusCode,
                        reader.ReadToEnd(),
                        webException.Message,
                        webException.InnerException
                    );
                }
                else
                {
                    response.StatusCode = httpResponse.StatusCode;
                    response.Headers = httpResponse.Headers;
                    response.Content = reader.ReadToEnd();
                    response.Data = new JsonParser().Deserialize(response.Content);
                }
            }

            return response;
        }
    }
}
