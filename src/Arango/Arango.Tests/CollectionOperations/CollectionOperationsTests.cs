﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        
        [Test()]
        public void Should_truncate_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var clearResult = db.Collection
                .Truncate(createResult.Value.String("name"));
            
            Assert.AreEqual(200, clearResult.StatusCode);
            Assert.AreEqual(true, clearResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), clearResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), clearResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), clearResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), clearResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), clearResult.Value.Int("type"));
        }
        
        [Test()]
        public void Should_get_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .Get(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.AreEqual(true, getResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
        }
        
        [Test()]
        public void Should_get_collection_properties()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .GetProperties(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.AreEqual(true, getResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Bool("isVolatile"), getResult.Value.Bool("isVolatile"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.AreEqual(createResult.Value.Bool("waitForSync"), getResult.Value.Bool("waitForSync"));
            Assert.IsTrue(getResult.Value.Bool("doCompact"));
            Assert.IsTrue(getResult.Value.Long("journalSize") > 1);
            Assert.AreEqual(ArangoKeyGeneratorType.Traditional, getResult.Value.Enum<ArangoKeyGeneratorType>("keyOptions.type"));
            Assert.AreEqual(true, getResult.Value.Bool("keyOptions.allowUserKeys"));
        }
        
        [Test()]
        public void Should_get_collection_count()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .GetCount(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.AreEqual(true, getResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Bool("isVolatile"), getResult.Value.Bool("isVolatile"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.AreEqual(createResult.Value.Bool("waitForSync"), getResult.Value.Bool("waitForSync"));
            Assert.IsTrue(getResult.Value.Bool("doCompact"));
            Assert.IsTrue(getResult.Value.Long("journalSize") > 1);
            Assert.AreEqual(ArangoKeyGeneratorType.Traditional, getResult.Value.Enum<ArangoKeyGeneratorType>("keyOptions.type"));
            Assert.AreEqual(true, getResult.Value.Bool("keyOptions.allowUserKeys"));
            Assert.AreEqual(0, getResult.Value.Long("count"));
        }
        
        [Test()]
        public void Should_get_collection_figures()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .GetFigures(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.AreEqual(true, getResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Bool("isVolatile"), getResult.Value.Bool("isVolatile"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.AreEqual(createResult.Value.Bool("waitForSync"), getResult.Value.Bool("waitForSync"));
            Assert.IsTrue(getResult.Value.Bool("doCompact"));
            Assert.IsTrue(getResult.Value.Long("journalSize") > 0);
            Assert.AreEqual(ArangoKeyGeneratorType.Traditional, getResult.Value.Enum<ArangoKeyGeneratorType>("keyOptions.type"));
            Assert.AreEqual(true, getResult.Value.Bool("keyOptions.allowUserKeys"));
            Assert.AreEqual(0, getResult.Value.Long("count"));
            Assert.IsTrue(getResult.Value.Document("figures").Count > 0);
        }
        
        [Test()]
        public void Should_get_collection_revision()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .GetRevision(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.AreEqual(true, getResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.IsTrue(getResult.Value.IsString("revision"));
        }
        
        [Test()]
        public void Should_get_collection_cehcksum()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .WithData(true)
                .WithRevisions(true)
                .GetChecksum(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.AreEqual(true, getResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.IsTrue(getResult.Value.IsString("revision"));
            Assert.IsTrue(getResult.Value.IsLong("checksum"));
        }
        
        [Test()]
        public void Should_all_collections()
        {
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
        public void Should_load_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var operationResult = db.Collection
                .Load(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.AreEqual(true, operationResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), operationResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
            Assert.IsTrue(operationResult.Value.Long("count") == 0);
        }
        
        [Test()]
        public void Should_load_collection_without_count()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var operationResult = db.Collection
                .Count(false)
                .Load(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.AreEqual(true, operationResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.AreEqual(ArangoCollectionStatus.Loaded, operationResult.Value.Enum<ArangoCollectionStatus>("status"));
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
            Assert.IsFalse(operationResult.Value.Has("count"));
        }
        
        [Test()]
        public void Should_unload_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var operationResult = db.Collection
                .Unload(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.AreEqual(true, operationResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.IsTrue(operationResult.Value.Enum<ArangoCollectionStatus>("status") == ArangoCollectionStatus.Unloaded || operationResult.Value.Enum<ArangoCollectionStatus>("status") == ArangoCollectionStatus.Unloading);
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
        }
        
        [Test()]
        public void Should_change_collection_properties()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            const long journalSize = 199999999;
            
            var operationResult = db.Collection
                .WaitForSync(true)
                .JournalSize(journalSize)
                .ChangeProperties(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.AreEqual(true, operationResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), operationResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
            Assert.IsFalse(operationResult.Value.Bool("isVolatile"));
            Assert.IsTrue(operationResult.Value.Bool("doCompact"));
            Assert.AreEqual(ArangoKeyGeneratorType.Traditional, operationResult.Value.Enum<ArangoKeyGeneratorType>("keyOptions.type"));
            Assert.IsTrue(operationResult.Value.Bool("keyOptions.allowUserKeys"));
            Assert.IsTrue(operationResult.Value.Bool("waitForSync"));
            Assert.IsTrue(operationResult.Value.Long("journalSize") == journalSize);
        }
        
        [Test()]
        public void Should_rename_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var operationResult = db.Collection
                .Rename(createResult.Value.String("name"), Database.TestEdgeCollectionName);
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.AreEqual(true, operationResult.Success);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(Database.TestEdgeCollectionName, operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), operationResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
        }
        
        [Test()]
        public void Should_fail_to_rotate_collection_journal()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var operationResult = db.Collection
                .RotateJournal(createResult.Value.String("name"));
            
            Assert.AreEqual(400, operationResult.StatusCode);
            Assert.IsFalse(operationResult.Success);
        }
        
        public void Dispose()
        {
            Database.CleanupTestDatabases();
        }
    }
}