using System;
using System.Collections.Generic;
using System.Linq;
using Arango.Client.ExternalLibraries.dictator;
using Arango.Client.Public;
using NUnit.Framework;

namespace Arango.Tests.DatabaseOperations
{
    [TestFixture()]
    public class DatabaseOperationsTests : IDisposable
    {
        [Test()]
        public void Should_get_list_of_accessible_databases()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ADatabase(Database.SystemAlias);

            var resultCreate = db.Create(Database.TestDatabaseOneTime);

            var resultList = db.GetAccessibleDatabases();

            Assert.AreEqual(200, resultList.StatusCode);
            Assert.IsTrue(resultList.Success);
            Assert.IsTrue(resultList.HasValue);
            Assert.IsTrue(resultList.Value.Contains(Database.TestDatabaseOneTime));
        }
        
        [Test()]
        public void Should_get_list_of_all_databases()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ADatabase(Database.SystemAlias);

            var resultCreate = db.Create(Database.TestDatabaseOneTime);

            var resultList = db.GetAllDatabases();

            Assert.AreEqual(200, resultList.StatusCode);
            Assert.IsTrue(resultList.Success);
            Assert.IsTrue(resultList.HasValue);
            Assert.IsTrue(resultList.Value.Contains(Database.TestDatabaseOneTime));
            Assert.IsTrue(resultList.Value.Contains("_system"));
        }
        
        [Test()]
        public void Should_get_current_database()
        {
            Database.CleanupTestDatabases();

            var db = new ADatabase(Database.SystemAlias);

            var resultCurrent = db.GetCurrent();

            Assert.AreEqual(200, resultCurrent.StatusCode);
            Assert.IsTrue(resultCurrent.Success);
            Assert.IsTrue(resultCurrent.HasValue);
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

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db
                .ExcludeSystem(true)
                .GetAllCollections();
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            
            var foundCreatedCollection = getResult.Value.FirstOrDefault(col => col.String("name") == createResult.Value.String("name"));
            
            Assert.IsNotNull(foundCreatedCollection);
            
            var foundSystemCollection = getResult.Value.FirstOrDefault(col => col.String("name") == "_system");
            
            Assert.IsNull(foundSystemCollection);
        }
        
        [Test()]
        public void Should_create_database()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ADatabase(Database.SystemAlias);

            var createResult = db.Create(Database.TestDatabaseOneTime);

            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value);
        }
        
        [Test()]
        public void Should_create_database_with_users()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ADatabase(Database.SystemAlias);

            var users = new List<AUser>()
            {
                new AUser { Username = "admin", Password = "secret", Active = true },
                new AUser { Username = "tester001", Password = "test001", Active = false } 
            };
            
            var createResult = db.Create(Database.TestDatabaseOneTime, users);

            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value);
        }
        
        [Test()]
        public void Should_fail_create_already_existing_database()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ADatabase(Database.SystemAlias);

            var createResult = db.Create(Database.TestDatabaseGeneral);
            
            var createResult2 = db.Create(Database.TestDatabaseGeneral);

            Assert.AreEqual(409, createResult2.StatusCode);
            Assert.IsFalse(createResult2.Success);
            Assert.IsTrue(createResult2.HasValue);
            Assert.IsFalse(createResult2.Value);
            Assert.IsNotNull(createResult2.Error);
        }
        
        [Test()]
        public void Should_fail_create_database_from_non_system_database()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ADatabase(Database.SystemAlias);

            var createResult = db.Create(Database.TestDatabaseGeneral);

            var nonSystemDatabase = new ADatabase(Database.Alias);
            
            var createResult2 = nonSystemDatabase.Create(Database.TestDatabaseOneTime);

            Assert.AreEqual(403, createResult2.StatusCode);
            Assert.IsFalse(createResult2.Success);
            Assert.IsTrue(createResult2.HasValue);
            Assert.IsFalse(createResult2.Value);
            Assert.IsNotNull(createResult2.Error);
        }
        
        [Test()]
        public void Should_delete_database()
        {
            Database.CleanupTestDatabases();
        	
            var db = new ADatabase(Database.SystemAlias);

            var createResult = db.Create(Database.TestDatabaseGeneral);
            
            var deleteResult = db.Drop(Database.TestDatabaseGeneral);

            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.IsTrue(deleteResult.Value);
        }
        
        public void Dispose()
        {
            Database.CleanupTestDatabases();
        }
    }
}
