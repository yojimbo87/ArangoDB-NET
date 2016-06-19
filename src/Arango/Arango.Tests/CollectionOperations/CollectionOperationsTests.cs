using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class CollectionOperationsTests : IDisposable
    {
        #region Create operations
    	
        [Test()]
        public void Should_create_document_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            Assert.AreEqual(200, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.AreEqual(true, createResult.Value.IsString("id"));
            Assert.AreEqual(Database.TestDocumentCollectionName, createResult.Value.String("name"));
            Assert.AreEqual(false, createResult.Value.Bool("waitForSync"));
            Assert.AreEqual(false, createResult.Value.Bool("isVolatile"));
            Assert.AreEqual(false, createResult.Value.Bool("isSystem"));
            Assert.AreEqual(ACollectionStatus.Loaded, createResult.Value.Enum<ACollectionStatus>("status"));
            Assert.AreEqual(ACollectionType.Document, createResult.Value.Enum<ACollectionType>("type"));
        }
        
        [Test()]
        public void Should_create_edge_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Type(ACollectionType.Edge)
                .Create(Database.TestEdgeCollectionName);

            Assert.AreEqual(200, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.AreEqual(true, createResult.Value.IsString("id"));
            Assert.AreEqual(Database.TestEdgeCollectionName, createResult.Value.String("name"));
            Assert.AreEqual(false, createResult.Value.Bool("waitForSync"));
            Assert.AreEqual(false, createResult.Value.Bool("isVolatile"));
            Assert.AreEqual(false, createResult.Value.Bool("isSystem"));
            Assert.AreEqual(ACollectionStatus.Loaded, createResult.Value.Enum<ACollectionStatus>("status"));
            Assert.AreEqual(ACollectionType.Edge, createResult.Value.Enum<ACollectionType>("type"));
        }
        
        [Test()]
        public void Should_create_autoincrement_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            
            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .KeyGeneratorType(AKeyGeneratorType.Autoincrement)
                .Create(Database.TestDocumentCollectionName);
            
            Assert.AreEqual(200, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.AreEqual(true, createResult.Value.IsString("id"));
            Assert.AreEqual(Database.TestDocumentCollectionName, createResult.Value.String("name"));
            Assert.AreEqual(false, createResult.Value.Bool("waitForSync"));
            Assert.AreEqual(false, createResult.Value.Bool("isVolatile"));
            Assert.AreEqual(false, createResult.Value.Bool("isSystem"));
            Assert.AreEqual(ACollectionStatus.Loaded, createResult.Value.Enum<ACollectionStatus>("status"));
            Assert.AreEqual(ACollectionType.Document, createResult.Value.Enum<ACollectionType>("type"));


			// create documents and test if their key are incremented accordingly
			
            var newDocument = new Dictionary<string, object>()
                .String("foo", "some string")
                .Document("bar", new Dictionary<string, object>().String("foo", "string value"));
            
            var doc1Result = db.Document
                .Create(Database.TestDocumentCollectionName, newDocument);
            
            Assert.AreEqual(202, doc1Result.StatusCode);
            Assert.IsTrue(doc1Result.Success);
            Assert.IsTrue(doc1Result.HasValue);
            Assert.AreEqual(Database.TestDocumentCollectionName + "/" + 1, doc1Result.Value.String("_id"));
            Assert.AreEqual("1", doc1Result.Value.String("_key"));
            Assert.IsFalse(string.IsNullOrEmpty(doc1Result.Value.String("_rev")));
            
            var doc2Result = db.Document
                .Create(Database.TestDocumentCollectionName, newDocument);
            
            Assert.AreEqual(202, doc2Result.StatusCode);
            Assert.IsTrue(doc2Result.Success);
            Assert.IsTrue(doc2Result.HasValue);
            Assert.AreEqual(Database.TestDocumentCollectionName + "/" + 2, doc2Result.Value.String("_id"));
            Assert.AreEqual("2", doc2Result.Value.String("_key"));
            Assert.IsFalse(string.IsNullOrEmpty(doc2Result.Value.String("_rev")));
        }
        
        #endregion
        
        #region Get operations
        
        [Test()]
        public void Should_get_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .Get(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
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

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .GetProperties(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Bool("isVolatile"), getResult.Value.Bool("isVolatile"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.AreEqual(createResult.Value.Bool("waitForSync"), getResult.Value.Bool("waitForSync"));
            Assert.IsTrue(getResult.Value.Bool("doCompact"));
            Assert.IsTrue(getResult.Value.Long("journalSize") > 1);
            Assert.AreEqual(AKeyGeneratorType.Traditional, getResult.Value.Enum<AKeyGeneratorType>("keyOptions.type"));
            Assert.AreEqual(true, getResult.Value.Bool("keyOptions.allowUserKeys"));
        }
        
        [Test()]
        public void Should_get_collection_count()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .GetCount(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Bool("isVolatile"), getResult.Value.Bool("isVolatile"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.AreEqual(createResult.Value.Bool("waitForSync"), getResult.Value.Bool("waitForSync"));
            Assert.IsTrue(getResult.Value.Bool("doCompact"));
            Assert.IsTrue(getResult.Value.Long("journalSize") > 1);
            Assert.AreEqual(AKeyGeneratorType.Traditional, getResult.Value.Enum<AKeyGeneratorType>("keyOptions.type"));
            Assert.AreEqual(true, getResult.Value.Bool("keyOptions.allowUserKeys"));
            Assert.AreEqual(0, getResult.Value.Long("count"));
        }
        
        [Test()]
        public void Should_get_collection_figures()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .GetFigures(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Bool("isVolatile"), getResult.Value.Bool("isVolatile"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.AreEqual(createResult.Value.Bool("waitForSync"), getResult.Value.Bool("waitForSync"));
            Assert.IsTrue(getResult.Value.Bool("doCompact"));
            Assert.IsTrue(getResult.Value.Long("journalSize") > 0);
            Assert.AreEqual(AKeyGeneratorType.Traditional, getResult.Value.Enum<AKeyGeneratorType>("keyOptions.type"));
            Assert.AreEqual(true, getResult.Value.Bool("keyOptions.allowUserKeys"));
            Assert.AreEqual(0, getResult.Value.Long("count"));
            Assert.IsTrue(getResult.Value.Document("figures").Count > 0);
        }
        
        [Test()]
        public void Should_get_collection_revision()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .GetRevision(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
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

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var getResult = db.Collection
                .WithData(true)
                .WithRevisions(true)
                .GetChecksum(createResult.Value.String("name"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), getResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), getResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), getResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), getResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), getResult.Value.Int("type"));
            Assert.IsTrue(getResult.Value.IsString("revision"));
            Assert.IsTrue(getResult.Value.IsString("checksum"));
        }
        
        [Test()]
        public void Should_get_all_indexes_in_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            Database.CreateTestCollection(Database.TestDocumentCollectionName, ACollectionType.Document);
            var db = new ADatabase(Database.Alias);
            
            var operationResult = db.Collection
                .GetAllIndexes(Database.TestDocumentCollectionName);
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.IsTrue(operationResult.Value.Size("indexes") > 0);
            Assert.IsTrue(operationResult.Value.IsDocument("identifiers"));
        }
        
        #endregion
        
        #region Update/change operations
        
        [Test()]
        public void Should_truncate_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var clearResult = db.Collection
                .Truncate(createResult.Value.String("name"));
            
            Assert.AreEqual(200, clearResult.StatusCode);
            Assert.IsTrue(clearResult.Success);
            Assert.IsTrue(clearResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), clearResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), clearResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), clearResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), clearResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), clearResult.Value.Int("type"));
        }
        
        [Test()]
        public void Should_load_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var operationResult = db.Collection
                .Load(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
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

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var operationResult = db.Collection
                .Count(false)
                .Load(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.AreEqual(ACollectionStatus.Loaded, operationResult.Value.Enum<ACollectionStatus>("status"));
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
            Assert.IsFalse(operationResult.Value.Has("count"));
        }
        
        [Test()]
        public void Should_unload_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var operationResult = db.Collection
                .Unload(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.IsTrue(operationResult.Value.Enum<ACollectionStatus>("status") == ACollectionStatus.Unloaded || operationResult.Value.Enum<ACollectionStatus>("status") == ACollectionStatus.Unloading);
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
        }
        
        [Test()]
        public void Should_change_collection_properties()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            const long journalSize = 199999999;
            
            var operationResult = db.Collection
                .WaitForSync(true)
                .JournalSize(journalSize)
                .ChangeProperties(createResult.Value.String("name"));
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), operationResult.Value.String("id"));
            Assert.AreEqual(createResult.Value.String("name"), operationResult.Value.String("name"));
            Assert.AreEqual(createResult.Value.Bool("isSystem"), operationResult.Value.Bool("isSystem"));
            Assert.AreEqual(createResult.Value.Int("status"), operationResult.Value.Int("status"));
            Assert.AreEqual(createResult.Value.Int("type"), operationResult.Value.Int("type"));
            Assert.IsFalse(operationResult.Value.Bool("isVolatile"));
            Assert.IsTrue(operationResult.Value.Bool("doCompact"));
            Assert.AreEqual(AKeyGeneratorType.Traditional, operationResult.Value.Enum<AKeyGeneratorType>("keyOptions.type"));
            Assert.IsTrue(operationResult.Value.Bool("keyOptions.allowUserKeys"));
            Assert.IsTrue(operationResult.Value.Bool("waitForSync"));
            Assert.IsTrue(operationResult.Value.Long("journalSize") == journalSize);
        }
        
        [Test()]
        public void Should_rename_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var operationResult = db.Collection
                .Rename(createResult.Value.String("name"), Database.TestEdgeCollectionName);
            
            Assert.AreEqual(200, operationResult.StatusCode);
            Assert.IsTrue(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
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

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);

            var operationResult = db.Collection
                .RotateJournal(createResult.Value.String("name"));
            
            Assert.AreEqual(400, operationResult.StatusCode);
            Assert.IsFalse(operationResult.Success);
            Assert.IsTrue(operationResult.HasValue);
            Assert.IsFalse(operationResult.Value);
        }
        
        #endregion
        
        #region Delete operations
        
        [Test()]
        public void Should_delete_collection()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);

            var db = new ADatabase(Database.Alias);

            var createResult = db.Collection
                .Create(Database.TestDocumentCollectionName);
            
            var deleteResult = db.Collection
                .Delete(createResult.Value.String("name"));
            
            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(createResult.Value.String("id"), deleteResult.Value.String("id"));
        }
        
        #endregion
        
        public void Dispose()
        {
            Database.CleanupTestDatabases();
        }
    }
}
