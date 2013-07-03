using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
//using ServiceStack.Text;
using Newtonsoft.Json;

namespace Arango.Client.Protocol
{
    internal class Connection
    {
        #region Properties

        public string Server { get; set; }

        public int Port { get; set; }

        public bool IsSecured { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Alias { get; set; }

        internal Uri BaseUri { get; set; }

        internal CredentialCache Credentials { get; set; }

        #endregion

        internal Connection(string server, int port, bool isSecured, string userName, string password, string alias)
        {
            Server = server;
            Port = port;
            IsSecured = isSecured;
            Username = userName;
            Password = password;
            Alias = alias;

            BaseUri = new Uri((isSecured ? "https" : "http") + "://" + server + ":" + port + "/");

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                Credentials = new CredentialCache();
                Credentials.Add(BaseUri, "Basic", new NetworkCredential(userName, password));
            }
        }

        internal Response Process(Request request)
        {
            var httpRequest = (HttpWebRequest)HttpWebRequest.Create(BaseUri + request.RelativeUri);
            httpRequest.KeepAlive = true;
            httpRequest.Method = request.Method;
            httpRequest.UserAgent = ArangoClient.DriverName + "/" + ArangoClient.DriverVersion;

            if (Credentials != null)
            {
                httpRequest.Credentials = Credentials;
            }

            if ((request.Headers.Count > 0))
            {
                httpRequest.Headers = request.Headers;
            }

            if (!string.IsNullOrEmpty(request.Body))
            {
                byte[] data = Encoding.UTF8.GetBytes(request.Body);

                var stream = httpRequest.GetRequestStream();
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

                if (!string.IsNullOrEmpty(response.JsonString))
                {
                    response.Document.Deserialize(response.JsonString);
                }
            }
            catch (WebException webException)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                var reader = new StreamReader(httpResponse.GetResponseStream());

                if ((httpResponse.StatusCode == HttpStatusCode.NotModified) ||
                    ((httpResponse.StatusCode == HttpStatusCode.NotFound) && (request.Method == HttpMethod.Head)))
                {
                    response.StatusCode = httpResponse.StatusCode;
                    response.Headers = httpResponse.Headers;
                    response.JsonString = reader.ReadToEnd();

                    if (!string.IsNullOrEmpty(response.JsonString))
                    {
                        response.Document.Deserialize(response.JsonString);
                    }
                }
                else
                {
                    var jsonString = reader.ReadToEnd();
                    var errorMessage = "";

                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var document = new Document(jsonString);
                        
                        errorMessage = string.Format(
                            "ArangoDB responded with error code {0}:\n{1} [error number {2}]",
                            document.GetField<string>("code"),
                            document.GetField<string>("errorMessage"),
                            document.GetField<string>("errorNum")
                        );
                    }

                    throw new ArangoException(
                        httpResponse.StatusCode,
                        errorMessage,
                        webException.Message,
                        webException.InnerException
                    );
                }
            }

            return response;
        }
    }
}

