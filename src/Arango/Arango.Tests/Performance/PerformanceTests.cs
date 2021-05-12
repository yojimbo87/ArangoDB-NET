using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Arango.Client.ExternalLibraries.fastJSON;
using Arango.Client.Public;
using Arango.Tests.Entities;
using NUnit.Framework;

namespace Arango.Tests.Performance
{
    [TestFixture()]
    public class PerformanceTests : IDisposable
    {
        public PerformanceTests()
		{
			Database.CreateTestDatabase(Database.TestDatabaseGeneral);
		}
        
        //[Test()]
        public void Insertion_10k()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName, ACollectionType.Document);
            var db = new ADatabase(Database.Alias);
            
            var startTime = DateTime.Now;
            Debug.WriteLine("Start Time: " + startTime.ToLongTimeString());
        
            var entity = new PerformanceEntity();
            entity.Id = "1234567890123456789012345678901234";
            entity.Key = "1234567";
            entity.Revision = "1234567";
            entity.Name = "Mohamad Abu Bakar";
            entity.IcNumber = "1234567-12-3444";
            entity.Department = "IT Department";
            entity.Height = 1234;
            entity.DateOfBirth = new DateTime(2015, 1, 27, 3, 33, 3);
            entity.Salary = 3333;
            
            var jsonEntity = JSON.ToJSON(entity);
            
            for (int i = 0; i < 10000; i++)
            {
//                var entity = new PerformanceEntity();
//                entity.Id = "1234567890123456789012345678901234";
//                entity.Key = "1234567";
//                entity.Revision = "1234567";
//                entity.Name = "Mohamad Abu Bakar";
//                entity.IcNumber = "1234567-12-3444";
//                entity.Department = "IT Department";
//                entity.Height = 1234;
//                entity.DateOfBirth = new DateTime(2015, 1, 27, 3, 33, 3);
//                entity.Salary = 3333;
                
//                var entity = new Dictionary<string, object>()
//                    .String("Id", "1234567890123456789012345678901234")
//                    .String("Key", "1234567")
//                    .String("Revision", "1234567")
//                    .String("Name", "Mohamad Abu Bakar")
//                    .String("IcNumber", "1234567-12-3444")
//                    .String("Department", "IT Department")
//                    .Int("Height", 1234)
//                    .DateTime("DateOfBirth", new DateTime(2015, 1, 27, 3, 33, 3))
//                    .Int("Salary", 3333);

                //var createResult = db.Document.Create(Database.TestDocumentCollectionName, jsonEntity);
                
                SimpleHttpPostCreateDocument(
                    "http://localhost:8529/_db/" + Database.TestDatabaseGeneral + "/_api/document?collection=" + Database.TestDocumentCollectionName, 
                    jsonEntity
                );
        
                //var updateDocument = new Dictionary<string, object>()
                //    .String("DocumentId", "SomeId");
                //db2.Document.Update(result.Value.ID(), updateDocument);
            }
            
            /*var tasks = new Task[10];
            
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {    
                    int messages_sent_by_one_task = 0;
                    
                    while(messages_sent_by_one_task < 1000)
                    {
                        SimpleHttpPostCreateDocument(
                            "http://localhost:8529/_db/" + Database.TestDatabaseGeneral + "/_api/document?collection=" + Database.TestDocumentCollectionName, 
                            jsonEntity
                        );
                        
                        messages_sent_by_one_task++;
                    }
                });
            }
            
            while (tasks.Any(t => !t.IsCompleted)) { }*/
        
            var endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;
            
            Debug.WriteLine("End Time: " + endTime.ToLongTimeString());
            Debug.WriteLine("Total time taken: " + duration.TotalSeconds);
        }
        
        private string SimpleHttpPostCreateDocument(string uri, string body)
        {
            var responseBody = "";
            var httpRequest = HttpWebRequest.CreateHttp(uri);

            httpRequest.KeepAlive = true;
            httpRequest.SendChunked = false;
            httpRequest.Method = "POST";
            httpRequest.UserAgent = ASettings.DriverName + "/" + ASettings.DriverVersion;

            /*if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                httpRequest.Headers.Add(
                    "Authorization", 
                    "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(Username + ":" + Password))
                );
            }*/

            if (!string.IsNullOrEmpty(body))
            {
                httpRequest.ContentType = "application/json; charset=utf-8";

                var data = Encoding.UTF8.GetBytes(body);
                var stream = httpRequest.GetRequestStream();

                stream.Write(data, 0, data.Length);
                stream.Flush();

                stream.Close();
                stream.Dispose();
            }
            else
            {
                httpRequest.ContentLength = 0;
            }

            try
            {
                using (var httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                {
                    var responseStream = httpResponse.GetResponseStream();
                    var reader = new StreamReader(responseStream);

                    responseBody = reader.ReadToEnd();

                    reader.Close();
                    reader.Dispose();
                    responseStream.Close();
                    responseStream.Dispose();
                }
            }
            catch (WebException webException)
            {
                if ((webException.Status == WebExceptionStatus.ProtocolError) && 
                    (webException.Response != null))
                {
                    using (var exceptionHttpResponse = (HttpWebResponse)webException.Response)
                    {

                        if (exceptionHttpResponse.ContentLength > 0)
                        {
                            using (var exceptionResponseStream = exceptionHttpResponse.GetResponseStream())
                            using (var exceptionReader = new StreamReader(exceptionResponseStream))
                            {
                                responseBody = exceptionReader.ReadToEnd();
                            }
                        }
                    }
                }
                else
                {
                    throw;
                }
            }
            
            return responseBody;
        }
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
