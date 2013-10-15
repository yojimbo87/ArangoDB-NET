using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Arango.Client.Protocol
{
    internal class Connection
    {
        #region Properties

        public string Hostname { get; set; }

        public int Port { get; set; }

        public bool IsSecured { get; set; }
        
        public string DatabaseName { get; set; }

        public string Alias { get; set; }
        
        public string Username { get; set; }

        public string Password { get; set; }

        internal Uri BaseUri { get; set; }

        internal CredentialCache Credentials { get; set; }

        #endregion

        internal Connection(string hostname, int port, bool isSecured, string userName = "", string password = "")
        {
            Hostname = hostname;
            Port = port;
            IsSecured = isSecured;
            Username = userName;
            Password = password;

            BaseUri = new Uri((isSecured ? "https" : "http") + "://" + hostname + ":" + port + "/");

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                Credentials = new CredentialCache();
                Credentials.Add(BaseUri, "Basic", new NetworkCredential(userName, password));
            }
        }
        
        internal Connection(string hostname, int port, bool isSecured, string databaseName, string alias, string userName = "", string password = "")
        {
            Hostname = hostname;
            Port = port;
            IsSecured = isSecured;
            DatabaseName = databaseName;
            Alias = alias;
            Username = userName;
            Password = password;

            BaseUri = new Uri((isSecured ? "https" : "http") + "://" + hostname + ":" + port + "/_db/" + databaseName + "/");

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
                using (var httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    var responseStream = httpResponse.GetResponseStream();
                    var reader = new StreamReader(responseStream);
    
                    response.StatusCode = httpResponse.StatusCode;
                    response.Headers = httpResponse.Headers;
                    response.JsonString = reader.ReadToEnd();
                    
                    reader.Close();
                    reader.Dispose();
                    responseStream.Close();
                    responseStream.Dispose();
                }
                
                if (!string.IsNullOrEmpty(response.JsonString))
                {
                    response.Document.Parse(response.JsonString);
                }
            }
            catch (WebException webException)
            {
                if ((webException.Status == WebExceptionStatus.ProtocolError) && 
                    (webException.Response != null))
                {
                    var errorMessage = "";
                    var exceptionHttpResponse = (HttpWebResponse)webException.Response;
                    
                    response.IsException = true;
                    response.StatusCode = exceptionHttpResponse.StatusCode;
                    
                    if (exceptionHttpResponse.Headers.Count > 0)
                    {
                        response.Headers = exceptionHttpResponse.Headers;
                    }
                    
                    if (exceptionHttpResponse.ContentLength > 0)
                    {
                        var exceptionResponseStream = exceptionHttpResponse.GetResponseStream();
                        var exceptionReader = new StreamReader(exceptionResponseStream);
        
                        response.JsonString = exceptionReader.ReadToEnd();
                        
                        exceptionReader.Close();
                        exceptionReader.Dispose();
                        exceptionResponseStream.Close();
                        exceptionResponseStream.Dispose();
                    }
                    
                    exceptionHttpResponse.Close();
                    exceptionHttpResponse.Dispose();
                    
                    if (!string.IsNullOrEmpty(response.JsonString))
                    {
                        response.Document.Parse(response.JsonString);
                        
                        errorMessage = string.Format(
                                "ArangoDB responded with error code {0}:\n{1} [error number {2}]",
                                response.Document.Enum<HttpStatusCode>("code"),
                                response.Document.String("errorMessage"),
                                response.Document.Int("errorNum")
                            );
                    }
                    else
                    {
                        errorMessage = "ArangoDB response was empty.";
                    }
                    
                    response.Document.String("driverErrorMessage", errorMessage);
                    response.Document.String("driverExceptionMessage", webException.Message);
                    response.Document.Object("driverInnerException", webException.InnerException);
                }
                else
                {
                    throw;
                }
            }

            return response;
        }
    }
}

