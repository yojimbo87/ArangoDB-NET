using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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
            var parser = new JsonParser();
            var httpRequest = (HttpWebRequest)HttpWebRequest.Create(BaseUri + request.RelativeUri);
            httpRequest.KeepAlive = true;
            httpRequest.Method = request.Method;
            httpRequest.UserAgent = _userAgent;
            httpRequest.Credentials = Credentials;

            if ((request.Headers.Count > 0))
            {
                httpRequest.Headers = request.Headers;
            }

            if (!string.IsNullOrEmpty(request.Body))
            {
                byte[] data = Encoding.UTF8.GetBytes(request.Body);

                Stream stream = httpRequest.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
            }
            else
            {
                httpRequest.ContentLength = 0;
            }

            var response = new Response();

            try
            {
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                var reader = new StreamReader(httpResponse.GetResponseStream());

                response.StatusCode = httpResponse.StatusCode;
                response.Headers = httpResponse.Headers;
                response.JsonString = reader.ReadToEnd();
                response.JsonObject = parser.Deserialize(response.JsonString);
            }
            catch (WebException webException)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                var reader = new StreamReader(httpResponse.GetResponseStream());

                if (httpResponse.StatusCode != HttpStatusCode.NotModified)
                {
                    dynamic jsonObject = parser.Deserialize(reader.ReadToEnd());
                    string errorMessage = string.Format("ArangoDB responded with error code {0}:\n{1} [error number {2}]", jsonObject.code, jsonObject.errorMessage, jsonObject.errorNum);

                    throw new ArangoException(
                        httpResponse.StatusCode,
                        errorMessage,
                        webException.Message,
                        webException.InnerException
                    );
                }
                else
                {
                    response.StatusCode = httpResponse.StatusCode;
                    response.Headers = httpResponse.Headers;
                    response.JsonString = reader.ReadToEnd();
                    response.JsonObject = parser.Deserialize(response.JsonString);
                }
            }

            return response;
        }
    }
}
