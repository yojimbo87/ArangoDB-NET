using System;
using System.Collections.Generic;
using Arango.Client;
using Arango.fastJSON;

namespace Arango.ConsoleTests
{
    class Program
    {
        public static void Main(string[] args)
        {
            InsertTest();
            //PerformanceTests();
            
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
        
        static void InsertTest()
        {
            Database.CleanupTestDatabases();
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            Database.CreateTestCollection("Release", ACollectionType.Document);
            
            var db = new ADatabase(Database.Alias);
            
            var album = new Release { Id = 123 , Status = "some status", Title = "some title", Country = "some country", Released = "released" };
            var releaseKey = CreateDocument(db, "Release", album);
        }
        
        static string CreateDocument(ADatabase db, string collection, object dataType)
        {
            var createDocumentResult = db.Document
                .WaitForSync(false)
                .Create(collection, dataType);
            
            var key = "";
            
            if (createDocumentResult.Success)
            {
                key = createDocumentResult.Value.String("_key");
            }
            
            return key;
        }
        
        static void PerformanceTests()
        {
            var performance = new Performance();
            
            //performance.TestSimpleSequentialHttpPostRequests();
            //performance.TestRestSharpHttpPostRequests();
            //performance.TestSimpleParallelHttpPostRequests();
            
            performance.Dispose();
        }
    }
}