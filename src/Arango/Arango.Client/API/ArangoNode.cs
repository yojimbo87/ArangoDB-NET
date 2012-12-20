using System;
using System.IO;
using System.Net;
using System.Net.Http;
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

        internal ResponseData Process(RequestData requestData)
        {
            var request = (HttpWebRequest)HttpWebRequest.Create(BaseUri + requestData.RelativeUri);
            request.KeepAlive = true;
            request.Method = requestData.Method;
            request.UserAgent = _userAgent;
            request.Credentials = Credentials;

            if ((requestData.Headers.Count > 0))
            {
                request.Headers = requestData.Headers;
            }

            /*Stream stream1 = request.GetRequestStream();
            stream1.Write(data, 0, data.Length);
            stream1.Close();*/

            var responseData = new ResponseData();

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());

                responseData.StatusCode = response.StatusCode;
                responseData.Headers = response.Headers;
                responseData.Content = reader.ReadToEnd();
            }
            catch (WebException webException)
            {
                var response = (HttpWebResponse)webException.Response;
                var reader = new StreamReader(response.GetResponseStream());

                if (response.StatusCode != HttpStatusCode.NotModified)
                {
                    throw new ArangoException(
                        response.StatusCode,
                        reader.ReadToEnd(),
                        webException.Message,
                        webException.InnerException
                    );
                }
                else
                {
                    responseData.StatusCode = response.StatusCode;
                    responseData.Headers = response.Headers;
                    responseData.Content = reader.ReadToEnd();
                }
            }

            return responseData;
        }
    }
}
