using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Arango.Client;
using Arango.fastJSON;
using RestSharp;

namespace Arango.ConsoleTests
{
    public class Performance : IDisposable
    {
        public Performance()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            Database.CreateTestCollection(Database.TestDocumentCollectionName, ACollectionType.Document);
        }
        
        public void TestSimpleParallelHttpPostRequests()
        {
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
            
            var threadCount = 10;
            var tasks = new Task[threadCount];
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {    
                    int messages_sent_by_one_task = 0;
                    
                    while(messages_sent_by_one_task < 100)
                    {
                        SimpleHttpPostCreateDocument(
                            "http://localhost:8529/_db/" + Database.TestDatabaseGeneral + "/_api/document?collection=" + Database.TestDocumentCollectionName, 
                            jsonEntity
                        );
                        
                        messages_sent_by_one_task++;
                    }
                });
            }
            
            while (tasks.Any(t => !t.IsCompleted)) { }
            
            Console.WriteLine("Elapsed time [s]: {0}", stopwatch.Elapsed.TotalMilliseconds / 1000);
        }
        
        public void TestSimpleSequentialHttpPostRequests(int iterationCount)
        {
            Console.WriteLine("Operation start: TestSimpleSequentialHttpPostRequests\n");
            
            var db = new ADatabase(Database.Alias);
            
            //ServicePointManager.DefaultConnectionLimit = 40;
            //ServicePointManager.Expect100Continue = false;
            //ServicePointManager.UseNagleAlgorithm = false;
            
            ExecuteTimedTest(iterationCount, () => {
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

                var response = SimpleHttpPostCreateDocument(
                    "http://localhost:8529/_db/" + Database.TestDatabaseGeneral + "/_api/document?collection=" + Database.TestDocumentCollectionName, 
                    jsonEntity
                );

                var deserializedResponse = JSON.ToObject<Dictionary<string, object>>(response);
            });

            Console.WriteLine("\nOperation end: TestSimpleSequentialHttpPostRequests");
        }
        
        public void TestRestSharpHttpPostRequests()
        {
            Console.WriteLine("Operation start: TestRestSharpHttpPostRequests");
            
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
            
            ServicePointManager.DefaultConnectionLimit = 40;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
            
            var client = new RestClient("http://localhost:8529");
            var request = new RestRequest("_db/" + Database.TestDatabaseGeneral + "/_api/document", Method.POST);
            request.AddParameter("collection", Database.TestDocumentCollectionName);
            request.AddBody(jsonEntity);
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < 10000; i++)
            {                
                var response = client.Execute(request);
            }
            
            Console.WriteLine("Elapsed time [s]: {0}", stopwatch.Elapsed.TotalMilliseconds / 1000);
                
            Console.WriteLine("Operation end: TestRestSharpHttpPostRequests");
        }
        
        public void TestArangoClientSequentialInsertion(int iterationCount)
        {
            Console.WriteLine("Operation start: TestArangoClientSequentialInsertion\n");
            
            var db = new ADatabase(Database.Alias);
            
            ExecuteTimedTest(iterationCount, () => {
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
                
                var createResult = db.Document.Create(Database.TestDocumentCollectionName, entity);
            });
            
            Console.WriteLine("\nOperation end: TestArangoClientSequentialInsertion");
        }
        
        public string SimpleHttpPostCreateDocument(string uriString, string body, string username = "test", string password = "test")
        {
            //var stopwatch = Stopwatch.StartNew();
            int statusCode = 0;
            WebHeaderCollection headers = null;

            var responseBody = "";
            //var httpRequest = HttpWebRequest.CreateHttp(uri);
            var httpRequest = HttpWebRequest.CreateHttp(new Uri(uriString));

            httpRequest.KeepAlive = true;
            httpRequest.SendChunked = false;
            httpRequest.Proxy = null;
            httpRequest.Method = "POST";
            httpRequest.UserAgent = ASettings.DriverName + "/" + ASettings.DriverVersion;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                httpRequest.Headers.Add(
                    "Authorization", 
                    "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password))
                );
            }

            if (!string.IsNullOrEmpty(body))
            {
                httpRequest.ContentType = "application/json; charset=utf-8";

                var data = Encoding.UTF8.GetBytes(body);
                
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

            try
            {
                using (var httpResponse = (HttpWebResponse)httpRequest.GetResponse())
                using (var responseStream = httpResponse.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    statusCode = (int)httpResponse.StatusCode;
                    headers = httpResponse.Headers;
                    responseBody = reader.ReadToEnd();
                        
                    reader.Close();
                    responseStream.Close();
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
                                
                                exceptionReader.Close();
                                exceptionResponseStream.Close();
                            }
                        }
                    }
                }
                else
                {
                    throw;
                }
            }

            //Console.WriteLine("{0}", stopwatch.Elapsed.TotalMilliseconds);

            return responseBody;
        }
        
        static void ExecuteTimedTest(int iterations, Action test)
        {
            double jit = Execute(test); //disregard jit pass
            Console.WriteLine("JIT: {0:F2}ms.", jit);
        
            double optimize = Execute(test); //disregard optimize pass
            Console.WriteLine("Optimize: {0:F2}ms.", optimize);
        
            double totalElapsed = 0;
            
            for (int i = 0; i < iterations; i++)
            {
                totalElapsed += Execute(test);
            }
        
            double averageMs = (totalElapsed / iterations);
            
            Console.WriteLine("Total: {0:F2}ms.", totalElapsed);
            Console.WriteLine("Average: {0:F2}ms.", averageMs);
        }
        
        static double Execute(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            
            action();
            
            return stopwatch.Elapsed.TotalMilliseconds;
        }
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
