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

        public static string Hostname { get; set; }
        public static int Port { get; set; }
        public static bool IsSecured { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
        
        static Database()
        {
            TestDatabaseOneTime = "testOneTimeDatabase001xyzLatif";
            TestDatabaseGeneral = "testDatabaseGeneral001xyzLatif";

            TestDocumentCollectionName = "testDocumentCollection001xyzLatif";
            TestEdgeCollectionName = "testEdgeCollection001xyzLatif";
            
            Alias = "testAlias";
            SystemAlias = "systemAlias";
            Hostname = "localhost";
            Port = 8529;
            IsSecured = false;
            UserName = "";
            Password = "";

            ArangoSettings.AddConnection(
                SystemAlias,
                Hostname,
                Port,
                IsSecured,
                "_system",
                UserName,
                Password
            );

            ArangoSettings.AddConnection(
                Alias,
                Hostname,
                Port,
                IsSecured,
                TestDatabaseGeneral,
                UserName,
                Password
            );
        }
        
        public static void CreateTestDatabase(string databaseName)
        {	
            DeleteTestDatabase(databaseName);

            var db = new ArangoDatabase(Database.SystemAlias);
            
            var resultList = db.GetAccessibleDatabases();

            if (resultList.Success && resultList.Value.Contains(databaseName))
            {
            	db.Drop(databaseName);
            }
            
            db.Create(databaseName);
        }

        public static void DeleteTestDatabase(string databaseName)
        {
            var db = new ArangoDatabase(Database.SystemAlias);
            
            var resultList = db.GetAccessibleDatabases();

            if (resultList.Success && resultList.Value.Contains(databaseName))
            {
            	db.Drop(databaseName);
            }
        }

        public static void CleanupTestDatabases()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
            Database.DeleteTestDatabase(Database.TestDatabaseOneTime);
        }
    }
}
