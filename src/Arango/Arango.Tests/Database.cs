using Arango.Client;

namespace Arango.Tests
{
    public static class Database
    {
        public static string Hostname { get; set; }
        public static int Port { get; set; }
        public static bool IsSecured { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
        public static string Alias { get; set; }
        
        public static string TestCollectionName { get; set; }
        
        static Database()
        {
            Hostname = "localhost";
            Port = 8529;
            IsSecured = false;
            UserName = "";
            Password = "";
            Alias = "test";
            
            TestCollectionName = "testCollection001xyzLatif";
            
            ArangoClient.AddDatabase(
                Hostname,
                Port,
                IsSecured,
                UserName,
                Password,
                Alias
            );
        }
        
        public static ArangoDatabase GetTestDatabase()
        {
            return new ArangoDatabase(Alias);
        }
        
        public static void CreateTestCollection()
        {
            ArangoDatabase db = GetTestDatabase();
            
            if (db.Collection.Get(Database.TestCollectionName) != null)
            {
                // delet collection if it exists
                db.Collection.Delete(Database.TestCollectionName);
            }
            
            // create new test collection
            ArangoCollection collection = new ArangoCollection();
            collection.Name = TestCollectionName;
            
            db.Collection.Create(collection);
        }
        
        public static void DeleteTestCollection()
        {
            ArangoDatabase db = GetTestDatabase();
            
            db.Collection.Delete(TestCollectionName);
        }
    }
}
