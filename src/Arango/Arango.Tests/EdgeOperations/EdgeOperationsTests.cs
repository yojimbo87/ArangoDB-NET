using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class EdgeOperationsTests : IDisposable
    {
        readonly List<Dictionary<string, object>> _documents;
        
        public EdgeOperationsTests()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
			Database.CreateTestCollection(Database.TestDocumentCollectionName, ArangoCollectionType.Document);
			Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
			
			_documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        }
        
        #region Get
        
        [Test()]
        public void Should_get_edge()
        {
        	var db = new ArangoDatabase(Database.Alias);
        	
            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var getResult = db.Edge
                .Get(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
            Assert.IsTrue(getResult.Value.IsString("foo"));
            // integers are by default parsed as long type
            Assert.IsTrue(getResult.Value.IsLong("bar"));
        }
        
        [Test()]
        public void Should_get_edge_with_ifMatch()
        {
        	var db = new ArangoDatabase(Database.Alias);
        	
            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var getResult = db.Edge
                .IfMatch(createResult.Value.String("_rev"))
                .Get(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
            Assert.IsTrue(getResult.Value.IsString("foo"));
            // integers are by default parsed as long type
            Assert.IsTrue(getResult.Value.IsLong("bar"));
        }
        
        [Test()]
        public void Should_get_edge_with_ifMatch_and_return_412()
        {
        	var db = new ArangoDatabase(Database.Alias);
        	
            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var getResult = db.Edge
                .IfMatch("123456789")
                .Get(createResult.Value.String("_id"));
            
            Assert.AreEqual(412, getResult.StatusCode);
            Assert.IsFalse(getResult.Success);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_get_edge_with_ifNoneMatch()
        {   
            var db = new ArangoDatabase(Database.Alias);
        	
            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var getResult = db.Edge
                .IfNoneMatch("123456789")
                .Get(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
            Assert.IsTrue(getResult.Value.IsString("foo"));
            // integers are by default parsed as long type
            Assert.IsTrue(getResult.Value.IsLong("bar"));
        }
        
        [Test()]
        public void Should_get_edge_with_ifNoneMatch_and_return_304()
        {
        	var db = new ArangoDatabase(Database.Alias);
        	
            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var getResult = db.Edge
                .IfNoneMatch(createResult.Value.String("_rev"))
                .Get(createResult.Value.String("_id"));
            
            Assert.AreEqual(304, getResult.StatusCode);
            Assert.IsFalse(getResult.Success);
        }
        
        #endregion
        
        #region Create
        
        [Test()]
        public void Should_create_empty_edge()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"));
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }
        
        [Test()]
        public void Should_create_empty_edge_with_waitForSync()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);

            var db = new ArangoDatabase(Database.Alias);

            var createResult = db.Edge
                .WaitForSync(true)
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"));
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }
        
        [Test()]
        public void Should_create_edge()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);

            var db = new ArangoDatabase(Database.Alias);

            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
            
            var getResult = db.Edge
                .Get(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
            Assert.IsTrue(getResult.Value.IsString("foo"));
            // integers are by default parsed as long type
            Assert.IsTrue(getResult.Value.IsLong("bar"));
        }
        
        #endregion
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
