using System.Collections.Generic;
using Arango.Client;

namespace Arango.Tests
{
    public static class Database
    {
        public static string TestDatabaseOneTime { get; set; }
        public static string TestDatabaseGeneral { get; set; }

        public static string TestDocumentCollectionName { get; set; }
        public static string TestEdgeCollectionName { get; set; }

        public static string Alias { get; set; }
        public static string SystemAlias { get; set; }

        public static string Endpoint { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        
        static Database()
        {
            TestDatabaseOneTime = "testOneTimeDatabase001xyzLatif";
            TestDatabaseGeneral = "testDatabaseGeneral001xyzLatif";

            TestDocumentCollectionName = "testDocumentCollection001xyzLatif";
            TestEdgeCollectionName = "testEdgeCollection001xyzLatif";
            
            Alias = "testAlias";
            SystemAlias = "systemAlias";
            Endpoint = "http://127.0.0.1:8529";
            Username = "";
            Password = "";

            ASettings.AddConnection(
                SystemAlias,
                Endpoint,
                "_system",
                Username,
                Password
            );

            ASettings.AddConnection(
                Alias,
                Endpoint,
                TestDatabaseGeneral,
                Username,
                Password
            );
        }
        
        public static void CreateTestDatabase(string databaseName)
        {	
            DeleteTestDatabase(databaseName);

            var db = new ADatabase(SystemAlias);
            
            var resultList = db.GetAccessibleDatabases();

            if (resultList.Success && resultList.Value.Contains(databaseName))
            {
            	db.Drop(databaseName);
            }
            
            db.Create(databaseName);
        }

        public static void DeleteTestDatabase(string databaseName)
        {
            var db = new ADatabase(SystemAlias);
            
            var resultList = db.GetAccessibleDatabases();

            if (resultList.Success && resultList.Value.Contains(databaseName))
            {
            	db.Drop(databaseName);
            }
        }

        public static void CleanupTestDatabases()
        {
            DeleteTestDatabase(TestDatabaseGeneral);
            DeleteTestDatabase(TestDatabaseOneTime);
        }
        
        public static void CreateTestCollection(string collectionName, ACollectionType collectionType)
        {
        	DeleteTestCollection(collectionName);
        	
            var db = new ADatabase(Alias);

            var createResult = db.Collection
                .Type(collectionType)
                .Create(collectionName);
        }
        
        public static void ClearTestCollection(string collectionName)
        {
            var db = new ADatabase(Alias);

            var createResult = db.Collection
                .Truncate(collectionName);
        }
        
        public static List<Dictionary<string, object>> ClearCollectionAndFetchTestDocumentData(string collectionName)
        {
            ClearTestCollection(collectionName);
            
            var documents = new List<Dictionary<string, object>>();
        	var db = new ADatabase(Alias);
        	
        	var document1 = new Dictionary<string, object>()
        		.String("foo", "string value one")
        		.Int("bar", 1);
        	
        	var document2 = new Dictionary<string, object>()
        		.String("foo", "string value two")
        		.Int("bar", 2);
        	
        	var createResult1 = db.Document.Create(TestDocumentCollectionName, document1);
        	
        	document1.Merge(createResult1.Value);
        	
        	var createResult2 = db.Document.Create(TestDocumentCollectionName, document2);
        	
        	document2.Merge(createResult2.Value);
        	
        	documents.Add(document1);
        	documents.Add(document2);
        	
        	return documents;
        }

        public static void DeleteTestCollection(string collectionName)
        {
            var db = new ADatabase(Alias);

            var resultGet = db.Collection.Get(collectionName);
            
            if (resultGet.Success && (resultGet.Value.String("name") == collectionName))
            {
                db.Collection.Delete(collectionName);
            }
        }
    }
}
