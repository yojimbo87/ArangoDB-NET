using Arango.Client;

namespace Arango.Tests
{
    public static class Database
    {
        public static string Hostname { get; set; }
        public static int Port { get; set; }
        public static bool IsSecured { get; set; }
        public static string DatabaseName { get; set; }
        public static string Alias { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
        
        public static string TestDatabaseOneTime { get; set; }
        public static string TestDatabaseGeneral { get; set; }
        
        public static string TestDocumentCollectionName { get; set; }
        public static string TestEdgeCollectionName { get; set; }
        
        static Database()
        {
            Hostname = "localhost";
            Port = 8529;
            IsSecured = false;
            DatabaseName = "test";
            Alias = "test";
            UserName = "";
            Password = "";
            
            TestDatabaseOneTime = "testOneTimeDatabase001xyzLatif";
            TestDatabaseGeneral = "testDatabaseGeneral001xyzLatif";
            
            TestDocumentCollectionName = "testDocumentCollection001xyzLatif";
            TestEdgeCollectionName = "testEdgeCollection001xyzLatif";
            
            ArangoClient.AddConnection(
                Hostname,
                Port,
                IsSecured,
                DatabaseName,
                Alias,
                UserName,
                Password
            );
        }
        
        public static ArangoDatabase GetTestDatabase()
        {
            return new ArangoDatabase(Alias);
        }
        
        public static void CreateTestCollection(string collectionName, ArangoCollectionType collectionType = ArangoCollectionType.Document)
        {
            var db = GetTestDatabase();
            
            if (db.Collection.Get(collectionName) != null)
            {
                // delet collection if it exists
                db.Collection.Delete(collectionName);
            }
            
            // create new test collection
            var collection = new ArangoCollection();
            collection.Name = collectionName;
            collection.Type = collectionType;
            
            db.Collection.Create(collection);
        }
        
        public static void DeleteTestCollection(string collectionName)
        {
            var db = GetTestDatabase();
            
            if (db.Collection.Get(collectionName) != null)
            {
                db.Collection.Delete(collectionName);
            }
        }
    }
}
