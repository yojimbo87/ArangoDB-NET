using System;
using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class CollectionOperationsTests : IDisposable
    {
        #region Create collections
    	
        [Test()]
        public void Should_create_document_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            Assert.AreEqual(200, createResult.StatusCode);
            Assert.AreEqual(true, createResult.Success);
            Assert.AreEqual(true, createResult.Value.IsString("id"));
            Assert.AreEqual(Database.TestDocumentCollectionName, createResult.Value.String("name"));
            Assert.AreEqual(false, createResult.Value.Bool("waitForSync"));
            Assert.AreEqual(false, createResult.Value.Bool("isVolatile"));
            Assert.AreEqual(false, createResult.Value.Bool("isSystem"));
            Assert.AreEqual(ArangoCollectionStatus.Loaded, createResult.Value.Enum<ArangoCollectionStatus>("status"));
            Assert.AreEqual(ArangoCollectionType.Document, createResult.Value.Enum<ArangoCollectionType>("type"));
        }
        
        [Test()]
        public void Should_create_edge_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Type(ArangoCollectionType.Edge)
                .Create(Database.TestEdgeCollectionName);

            Assert.AreEqual(200, createResult.StatusCode);
            Assert.AreEqual(true, createResult.Success);
            Assert.AreEqual(true, createResult.Value.IsString("id"));
            Assert.AreEqual(Database.TestEdgeCollectionName, createResult.Value.String("name"));
            Assert.AreEqual(false, createResult.Value.Bool("waitForSync"));
            Assert.AreEqual(false, createResult.Value.Bool("isVolatile"));
            Assert.AreEqual(false, createResult.Value.Bool("isSystem"));
            Assert.AreEqual(ArangoCollectionStatus.Loaded, createResult.Value.Enum<ArangoCollectionStatus>("status"));
            Assert.AreEqual(ArangoCollectionType.Edge, createResult.Value.Enum<ArangoCollectionType>("type"));
        }
        
        [Test()]
        public void Should_create_autoincrement_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            
            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .KeyGeneratorType(ArangoKeyGeneratorType.Autoincrement)
                .Create(Database.TestDocumentCollectionName);
            
            Assert.AreEqual(200, createResult.StatusCode);
            Assert.AreEqual(true, createResult.Success);
            Assert.AreEqual(true, createResult.Value.IsString("id"));
            Assert.AreEqual(Database.TestDocumentCollectionName, createResult.Value.String("name"));
            Assert.AreEqual(false, createResult.Value.Bool("waitForSync"));
            Assert.AreEqual(false, createResult.Value.Bool("isVolatile"));
            Assert.AreEqual(false, createResult.Value.Bool("isSystem"));
            Assert.AreEqual(ArangoCollectionStatus.Loaded, createResult.Value.Enum<ArangoCollectionStatus>("status"));
            Assert.AreEqual(ArangoCollectionType.Document, createResult.Value.Enum<ArangoCollectionType>("type"));

            // TODO:
			// create documents and test if their key are incremented accordingly
            
//            var document1 = new JObject();
//            document1.Add("foo", "foo string value");
//
//            var createDocument1Result = db.Document.Create(Database.TestDocumentCollectionName, document1);
//            
//            Assert.AreEqual(202, createDocument1Result.StatusCode);
//            Assert.AreEqual(true, createDocument1Result.Success);
//            Assert.AreEqual(true, Handle.IsValid(createDocument1Result.Value.ID));
//            Assert.AreEqual(Database.TestDocumentCollectionName + "/" + 1, createDocument1Result.Value.ID);
//            Assert.AreEqual(Database.TestDocumentCollectionName, createDocument1Result.Value.Collection);
//            Assert.AreEqual(1, createDocument1Result.Value.Key);
//            Assert.AreEqual(false, string.IsNullOrEmpty(createDocument1Result.Value.Revision));
//            
//            var document2 = new JObject();
//            document2.Add("foo", "foo string value");
//            
//            var createDocument2Result = db.Document.Create(Database.TestDocumentCollectionName, document2);
//            
//            Assert.AreEqual(202, createDocument2Result.StatusCode);
//            Assert.AreEqual(true, createDocument2Result.Success);
//            Assert.AreEqual(true, Handle.IsValid(createDocument2Result.Value.ID));
//            Assert.AreEqual(Database.TestDocumentCollectionName + "/" + 2, createDocument2Result.Value.ID);
//            Assert.AreEqual(Database.TestDocumentCollectionName, createDocument2Result.Value.Collection);
//            Assert.AreEqual(2, createDocument2Result.Value.Key);
//            Assert.AreEqual(false, string.IsNullOrEmpty(createDocument2Result.Value.Revision));
        }
        
        #endregion
        
        [Test()]
        public void Should_delete_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);
            
            var deleteResult = db.Collection
                .Delete(createResult.Value.String("name"));
            
            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.AreEqual(true, deleteResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), deleteResult.Value.String("id"));
        }
        
        public void Dispose()
        {
            Database.CleanupTestDatabases();
        }
    }
}
