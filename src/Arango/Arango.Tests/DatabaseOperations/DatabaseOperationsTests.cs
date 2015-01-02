using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class DatabaseOperationsTests : IDisposable
    {
        [Test()]
        public void Should_get_list_of_accessible_databases()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ArangoDatabase(Database.SystemAlias);

            var resultCreate = db.Create(Database.TestDatabaseOneTime);

            var resultList = db.GetAccessibleDatabases();

            Assert.AreEqual(200, resultList.StatusCode);
            Assert.AreEqual(true, resultList.Success);
            Assert.AreEqual(true, resultList.Value.Contains(Database.TestDatabaseOneTime));
        }
        
        [Test()]
        public void Should_get_list_of_all_databases()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ArangoDatabase(Database.SystemAlias);

            var resultCreate = db.Create(Database.TestDatabaseOneTime);

            var resultList = db.GetAllDatabases();

            Assert.AreEqual(200, resultList.StatusCode);
            Assert.AreEqual(true, resultList.Success);
            Assert.AreEqual(true, resultList.Value.Contains(Database.TestDatabaseOneTime));
            Assert.AreEqual(true, resultList.Value.Contains("_system"));
        }
        
        [Test()]
        public void Should_get_current_database()
        {
            Database.CleanupTestDatabases();

            var db = new ArangoDatabase(Database.SystemAlias);

            var resultCurrent = db.GetCurrent();

            Assert.AreEqual(200, resultCurrent.StatusCode);
            Assert.AreEqual(true, resultCurrent.Success);
            Assert.AreEqual("_system", resultCurrent.Value.String("name"));
            Assert.AreEqual(false, string.IsNullOrEmpty(resultCurrent.Value.String("id")));
            Assert.AreEqual(false, string.IsNullOrEmpty(resultCurrent.Value.String("path")));
            Assert.AreEqual(true, resultCurrent.Value.Bool("isSystem"));
        }
        
        [Test()]
        public void Should_get_database_collections()
        {
            Database.CleanupTestDatabases();
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db
                .ExcludeSystem(true)
                .GetAllCollections();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.AreEqual(true, getResult.Success);
            
            var foundCreatedCollection = getResult.Value.FirstOrDefault(col => col.String("name") == createResult.Value.String("name"));
            
            Assert.IsNotNull(foundCreatedCollection);
            
            var foundSystemCollection = getResult.Value.FirstOrDefault(col => col.String("name") == "_system");
            
            Assert.IsNull(foundSystemCollection);
        }
        
        [Test()]
        public void Should_create_database()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ArangoDatabase(Database.SystemAlias);

            var resultCreate = db.Create(Database.TestDatabaseOneTime);

            Assert.AreEqual(201, resultCreate.StatusCode);
            Assert.AreEqual(true, resultCreate.Success);
            Assert.AreEqual(true, resultCreate.Value);
        }
        
        [Test()]
        public void Should_create_database_with_users()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ArangoDatabase(Database.SystemAlias);

            var users = new List<ArangoUser>()
            {
                new ArangoUser { Username = "admin", Password = "secret", Active = true },
                new ArangoUser { Username = "tester001", Password = "test001", Active = false } 
            };
            
            var resultCreate = db.Create(Database.TestDatabaseOneTime, users);

            Assert.AreEqual(201, resultCreate.StatusCode);
            Assert.AreEqual(true, resultCreate.Success);
            Assert.AreEqual(true, resultCreate.Value);
        }
        
        [Test()]
        public void Should_fail_create_already_existing_database()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ArangoDatabase(Database.SystemAlias);

            var resultCreate = db.Create(Database.TestDatabaseGeneral);
            
            var resultCreate2 = db.Create(Database.TestDatabaseGeneral);

            Assert.AreEqual(409, resultCreate2.StatusCode);
            Assert.AreEqual(false, resultCreate2.Success);
            Assert.IsNotNull(resultCreate2.Error);
        }
        
        [Test()]
        public void Should_fail_create_database_from_non_system_database()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ArangoDatabase(Database.SystemAlias);

            var resultCreate = db.Create(Database.TestDatabaseGeneral);

            var nonSystemDatabase = new ArangoDatabase(Database.Alias);
            
            var resultCreate2 = nonSystemDatabase.Create(Database.TestDatabaseOneTime);

            Assert.AreEqual(403, resultCreate2.StatusCode);
            Assert.AreEqual(false, resultCreate2.Success);
            Assert.IsNotNull(resultCreate2.Error);
        }
        
        [Test()]
        public void Should_delete_database()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ArangoDatabase(Database.SystemAlias);

            var resultCreate = db.Create(Database.TestDatabaseGeneral);
            
            var resultDelete = db.Drop(Database.TestDatabaseGeneral);

            Assert.AreEqual(200, resultDelete.StatusCode);
            Assert.AreEqual(true, resultDelete.Success);
            Assert.AreEqual(true, resultDelete.Value);
        }
        
        public void Dispose()
        {
            Database.CleanupTestDatabases();
        }
    }
}
