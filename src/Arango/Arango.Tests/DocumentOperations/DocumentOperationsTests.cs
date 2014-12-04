using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class DocumentOperationsTests : IDisposable
    {
        public DocumentOperationsTests()
		{
			Database.CreateTestDatabase(Database.TestDatabaseGeneral);
			Database.CreateTestCollection(Database.TestDocumentCollectionName, ArangoCollectionType.Document);
		}
        
        #region Get
        
        [Test()]
        public void Should_get_document()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ArangoDatabase(Database.Alias);
        	
            var getResult = db.Document
                .Get(documents[0].String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(getResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), documents[0].String("_rev"));
            Assert.AreEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("bar"), documents[0].String("bar"));
        }
        
        [Test()]
        public void Should_get_document_with_ifMatch()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ArangoDatabase(Database.Alias);
        	
            var getResult = db.Document
                .IfMatch(documents[0].String("_rev"))
                .Get(documents[0].String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(getResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), documents[0].String("_rev"));
            Assert.AreEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("bar"), documents[0].String("bar"));
        }
        
        [Test()]
        public void Should_get_document_with_ifMatch_and_return_return_412()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ArangoDatabase(Database.Alias);
        	
            var getResult = db.Document
                .IfMatch("123456789")
                .Get(documents[0].String("_id"));
            
            Assert.AreEqual(412, getResult.StatusCode);
            Assert.IsFalse(getResult.Success);
            Assert.AreEqual(getResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), documents[0].String("_rev"));
        }
        
        [Test()]
        public void Should_get_document_with_ifNoneMatch()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ArangoDatabase(Database.Alias);
        	
            var getResult = db.Document
                .IfNoneMatch("123456789")
                .Get(documents[0].String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(getResult.Value.String("_id"), documents[0].String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), documents[0].String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), documents[0].String("_rev"));
            Assert.AreEqual(getResult.Value.String("foo"), documents[0].String("foo"));
            Assert.AreEqual(getResult.Value.String("bar"), documents[0].String("bar"));
        }
        
        [Test()]
        public void Should_get_document_with_ifNoneMatch_and_return_304()
        {
        	var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        	var db = new ArangoDatabase(Database.Alias);
        	
            var getResult = db.Document
                .IfNoneMatch(documents[0].String("_rev"))
                .Get(documents[0].String("_id"));
            
            Assert.AreEqual(304, getResult.StatusCode);
            Assert.IsFalse(getResult.Success);
        }
        
        #endregion
        
        #region Create
        
        [Test()]
        public void Should_create_document()
        {
        	Database.ClearTestCollection(Database.TestDocumentCollectionName);

            var db = new ArangoDatabase(Database.Alias);

            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Document
                .Create(Database.TestDocumentCollectionName, document);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }
        
        [Test()]
        public void Should_create_document_with_waitForSync()
        {
        	Database.ClearTestCollection(Database.TestDocumentCollectionName);

            var db = new ArangoDatabase(Database.Alias);

            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Document
                .WaitForSync(true)
                .Create(Database.TestDocumentCollectionName, document);
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }
        
        #endregion
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
