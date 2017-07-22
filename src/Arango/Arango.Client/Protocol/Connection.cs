using System;
using System.IO;
using System.Net;
using System.Text;

namespace Arango.Client.Protocol
{
    /// <summary>
    /// Represents connection to specific endpoint and sends low level HTTP requests from client to server.
    /// </summary>
    internal class Connection
    {
        internal string Alias { get; set; }

        internal string Endpoint { get; set; }

        internal string DatabaseName { get; set; }

        internal string Username { get; set; }

        internal string Password { get; set; }

        internal bool UseWebProxy { get; set; }
        
        internal Connection(string alias, string endpoint, string username, string password, bool useWebProxy = false)
        {
            Alias = alias;
            Endpoint = endpoint.EndsWith("/") ? endpoint : endpoint + "/";
            Username = username;
            Password = password;
            UseWebProxy = useWebProxy;
        }

        internal Connection(string alias, string Endpoint, string databaseName, string username, string password, bool useWebProxy = false)
            : this(alias, Endpoint, username, password, useWebProxy)
        {
            DatabaseName = databaseName;
        }

        internal Response Send(Request request)
        {
            var requestUriString = Endpoint + (string.IsNullOrEmpty(DatabaseName) ? "" : "_db/" + DatabaseName + "/") + request.GetRelativeUri();
            var httpRequest = WebRequest.CreateHttp(requestUriString);

            if (request.Headers.Count > 0)
            {
                httpRequest.Headers = request.Headers;
            }

            httpRequest.KeepAlive = true;
            if (!UseWebProxy)
            {
                httpRequest.Proxy = null;
            }
            httpRequest.SendChunked = false;
            httpRequest.Method = request.HttpMethod.ToString();
            httpRequest.UserAgent = ASettings.DriverName + "/" + ASettings.DriverVersion;

            if (!string.IsNullOrEmpty(Username) && (Password != null))
            {
                httpRequest.Headers.Add(
                    "Authorization", 
                    "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + Password))
                );
            }

            if (!string.IsNullOrEmpty(request.Body))
            {
                httpRequest.ContentType = "application/json; charset=utf-8";

                var data = Encoding.UTF8.GetBytes(request.Body);

                using (var stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    stream.Close();
                }
            }
            else
            {
                httpRequest.ContentLength = 0;
            }

            var response = new Response();

            try
            {
                using (var httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                using (var responseStream = httpResponse.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    response.StatusCode = (int)httpResponse.StatusCode;
                    response.Headers = httpResponse.Headers;
                    response.Body = reader.ReadToEnd();

                    reader.Close();
                    responseStream.Close();
                }

                response.GetBodyDataType();
            }
            catch (WebException webException)
            {
                if ((webException.Status == WebExceptionStatus.ProtocolError) && 
                    (webException.Response != null))
                {
                    using (var exceptionHttpResponse = (HttpWebResponse)webException.Response)
                    {
                        response.StatusCode = (int)exceptionHttpResponse.StatusCode;

                        if (exceptionHttpResponse.Headers.Count > 0)
                        {
                            response.Headers = exceptionHttpResponse.Headers;
                        }

                        if (exceptionHttpResponse.ContentLength > 0)
                        {
                            using (var exceptionResponseStream = exceptionHttpResponse.GetResponseStream())
                            using (var exceptionReader = new StreamReader(exceptionResponseStream))
                            {
                                response.Body = exceptionReader.ReadToEnd();

                                exceptionReader.Close();
                                exceptionResponseStream.Close();
                            }
                            
                            response.GetBodyDataType();
                        }
                    }

                    response.Error = new AEerror();
                    response.Error.Exception = webException;

                    if (response.BodyType == BodyType.Document)
                    {
                        var body = response.ParseBody<Body<object>>();
                        
                        if ((body != null) && body.Error)
                        {
                            response.Error.StatusCode = body.Code;
                            response.Error.Number = body.ErrorNum;
                            response.Error.Message = "ArangoDB error: " + body.ErrorMessage;
                        }
                    }
                    
                    if (string.IsNullOrEmpty(response.Error.Message))
                    {
                        response.Error.StatusCode = response.StatusCode;
                        response.Error.Number = 0;
                        response.Error.Message = "Protocol error: " + webException.Message;
                    }
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
