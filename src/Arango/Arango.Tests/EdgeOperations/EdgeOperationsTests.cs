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
			Database.CreateTestCollection(Database.TestDocumentCollectionName, ACollectionType.Document);
			Database.CreateTestCollection(Database.TestEdgeCollectionName, ACollectionType.Edge);
			
			_documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        }
        
        #region Create operations
        
        [Test()]
        public void Should_create_empty_edge()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"));
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }
        
        [Test()]
        public void Should_create_empty_edge_with_waitForSync()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var createResult = db.Edge
                .WaitForSync(true)
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"));
            
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
        }
        
        [Test()]
        public void Should_create_edge()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .From(_documents[0].String("_id"))
                .To(_documents[1].String("_id"))
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, document);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
            
            var getResult = db.Edge
                .Get(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
            Assert.IsTrue(getResult.Value.IsString("_from"));
            Assert.IsTrue(getResult.Value.IsString("_to"));
            Assert.AreEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.Int("bar"), document.Int("bar"));
        }
        
        [Test()]
        public void Should_create_edge_from_generic_object()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var dummy = new Dummy();
            dummy.Foo = "foo string value";
            dummy.Bar = 12345;

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), dummy);
            
            Assert.AreEqual(202, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(createResult.Value.IsString("_id"));
            Assert.IsTrue(createResult.Value.IsString("_key"));
            Assert.IsTrue(createResult.Value.IsString("_rev"));
            
            var getResult = db.Edge
                .Get(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
            Assert.IsTrue(getResult.Value.IsString("_from"));
            Assert.IsTrue(getResult.Value.IsString("_to"));
            Assert.AreEqual(getResult.Value.String("foo"), dummy.Foo);
            Assert.AreEqual(getResult.Value.Int("bar"), dummy.Bar);
            Assert.AreEqual(0, dummy.Baz);
        }
        
        #endregion
        
        #region Check operations
        
        [Test()]
        public void Should_check_edge()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var checkResult = db.Edge
                .Check(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, checkResult.StatusCode);
            Assert.IsTrue(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_check_edge_with_ifMatch()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var checkResult = db.Edge
                .IfMatch(createResult.Value.String("_rev"))
                .Check(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, checkResult.StatusCode);
            Assert.IsTrue(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_check_edge_with_ifMatch_and_return_412()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var checkResult = db.Edge
                .IfMatch("123456789")
                .Check(createResult.Value.String("_id"));
            
            Assert.AreEqual(412, checkResult.StatusCode);
            Assert.IsFalse(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_check_edge_with_ifNoneMatch()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var checkResult = db.Edge
                .IfNoneMatch("123456789")
                .Check(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, checkResult.StatusCode);
            Assert.IsTrue(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_check_edge_with_ifNoneMatch_and_return_304()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var checkResult = db.Edge
                .IfNoneMatch(createResult.Value.String("_rev"))
                .Check(createResult.Value.String("_id"));
            
            Assert.AreEqual(304, checkResult.StatusCode);
            Assert.IsFalse(checkResult.Success);
            Assert.IsTrue(checkResult.HasValue);
            Assert.AreEqual(checkResult.Value, createResult.Value.String("_rev"));
        }
        
        #endregion
        
        #region Get operations
        
        [Test()]
        public void Should_get_edge()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var getResult = db.Edge
                .Get(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
            Assert.IsTrue(getResult.Value.IsString("_from"));
            Assert.IsTrue(getResult.Value.IsString("_to"));
            Assert.IsTrue(getResult.Value.IsString("foo"));
            // integers are by default parsed as long type
            Assert.IsTrue(getResult.Value.IsLong("bar"));
        }
        
        [Test()]
        public void Should_get_edge_with_ifMatch()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
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
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
            Assert.IsTrue(getResult.Value.IsString("_from"));
            Assert.IsTrue(getResult.Value.IsString("_to"));
            Assert.IsTrue(getResult.Value.IsString("foo"));
            // integers are by default parsed as long type
            Assert.IsTrue(getResult.Value.IsLong("bar"));
        }
        
        [Test()]
        public void Should_get_edge_with_ifMatch_and_return_412()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
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
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_get_edge_with_ifNoneMatch()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);
        	
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
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), createResult.Value.String("_rev"));
            Assert.IsTrue(getResult.Value.IsString("_from"));
            Assert.IsTrue(getResult.Value.IsString("_to"));
            Assert.IsTrue(getResult.Value.IsString("foo"));
            // integers are by default parsed as long type
            Assert.IsTrue(getResult.Value.IsLong("bar"));
        }
        
        [Test()]
        public void Should_get_edge_with_ifNoneMatch_and_return_304()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
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
            Assert.IsFalse(getResult.HasValue);
        }
        
        [Test()]
        public void Should_get_edge_as_generic_object()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var getResult = db.Edge
                .Get<Dummy>(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(document.String("foo"), getResult.Value.Foo);
            Assert.AreEqual(document.Int("bar"), getResult.Value.Bar);
            Assert.AreEqual(0, getResult.Value.Baz);
        }
        
        #endregion
        
        #region Get in/out/any
        
        [Test()]
        public void Should_get_edges_in()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[1].String("_id"), _documents[0].String("_id"), document);
            
            var getResult = db.Edge
                .Get(Database.TestEdgeCollectionName, _documents[0].String("_id"), ADirection.In);
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.Count, 1);
            
            Assert.AreEqual(getResult.Value[0].String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value[0].String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value[0].String("_rev"), createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_get_edges_out()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var getResult = db.Edge
                .Get(Database.TestEdgeCollectionName, _documents[0].String("_id"), ADirection.Out);
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.Count, 1);
            
            Assert.AreEqual(getResult.Value[0].String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value[0].String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value[0].String("_rev"), createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_get_edges_any()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
        	var db = new ADatabase(Database.Alias);
        	
            var document = new Dictionary<string, object>()
        		.String("foo", "foo string value")
        		.Int("bar", 12345);

            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var getResult = db.Edge
                .Get(Database.TestEdgeCollectionName, _documents[0].String("_id"), ADirection.Any);
            
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.Count, 1);
            
            Assert.AreEqual(getResult.Value[0].String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value[0].String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value[0].String("_rev"), createResult.Value.String("_rev"));
        }
        
        #endregion
        
        #region Update operations
        
        [Test()]
        public void Should_update_edge()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var newDocument = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("bar", 54321)
                .Int("baz", 12345);
            
            var updateResult = db.Edge
                .Update(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), createResult.Value.String("_rev"));
            
            var getResult = db.Edge
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            
            Assert.AreNotEqual(getResult.Value.Int("bar"), document.Int("bar"));
            Assert.AreEqual(getResult.Value.Int("bar"), newDocument.Int("bar"));
            
            // by default JSON integers are deserialized to long type
            Assert.IsTrue(getResult.Value.IsLong("baz"));
        }
        
        [Test()]
        public void Should_update_edge_with_waitForSync()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var newDocument = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("bar", 54321)
                .Int("baz", 12345);
            
            var updateResult = db.Edge
                .WaitForSync(true)
                .Update(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(201, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), createResult.Value.String("_rev"));
            
            var getResult = db.Edge
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            
            Assert.AreNotEqual(getResult.Value.Int("bar"), document.Int("bar"));
            Assert.AreEqual(getResult.Value.Int("bar"), newDocument.Int("bar"));
            
            // by default JSON integers are deserialized to long type
            Assert.IsTrue(getResult.Value.IsLong("baz"));
        }
        
        [Test()]
        public void Should_update_edge_with_ifMatch()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var newDocument = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("bar", 54321)
                .Int("baz", 12345);
            
            var updateResult = db.Edge
                .IfMatch(createResult.Value.String("_rev"))
                .Update(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), createResult.Value.String("_rev"));
            
            var getResult = db.Edge
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            
            Assert.AreNotEqual(getResult.Value.Int("bar"), document.Int("bar"));
            Assert.AreEqual(getResult.Value.Int("bar"), newDocument.Int("bar"));
            
            // by default JSON integers are deserialized to long type
            Assert.IsTrue(getResult.Value.IsLong("baz"));
        }
        
        [Test()]
        public void Should_update_edge_with_ifMatch_and_return_412()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var newDocument = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Int("bar", 54321)
                .Int("baz", 12345);
            
            var updateResult = db.Edge
                .IfMatch("123456789")
                .Update(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(412, updateResult.StatusCode);
            Assert.IsFalse(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(updateResult.Value.String("_rev"), createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_update_edge_with_keepNull()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Object("bar", null);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            document.Merge(createResult.Value);
            
            var newDocument = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Object("baz", null);
            
            var updateResult = db.Edge
                .KeepNull(false)
                .Update(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), document.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), document.String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), document.String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            
            Assert.IsTrue(getResult.Value.Has("bar"));
            
            Assert.IsFalse(getResult.Value.Has("baz"));
        }
        
        [Test()]
        public void Should_update_edge_with_mergeArrays_set_to_true()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Document("bar", new Dictionary<string, object>().String("foo", "string value"));
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            document.Merge(createResult.Value);
            
            var newDocument = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Document("bar", new Dictionary<string, object>().String("bar", "other string value"));
            
            var updateResult = db.Edge
                .MergeObjects(true) // this is also default behavior
                .Update(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), document.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), document.String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), document.String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            
            Assert.IsTrue(getResult.Value.Has("bar.foo"));
            
            Assert.IsTrue(getResult.Value.Has("bar.bar"));
        }
        
        [Test()]
        public void Should_update_edge_with_mergeArrays_set_to_false()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Document("bar", new Dictionary<string, object>().String("foo", "string value"));
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            document.Merge(createResult.Value);
            
            var newDocument = new Dictionary<string, object>()
                .String("foo", "some other new string")
                .Document("bar", new Dictionary<string, object>().String("bar", "other string value"));
            
            var updateResult = db.Edge
                .MergeObjects(false)
                .Update(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), document.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), document.String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), document.String("_rev"));
            
            var getResult = db.Document
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            
            Assert.IsFalse(getResult.Value.Has("bar.foo"));
            
            Assert.IsTrue(getResult.Value.Has("bar.bar"));
        }
        
        [Test()]
        public void Should_update_edge_with_generic_object()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var dummy = new Dummy();
            dummy.Foo = "some other new string";
            dummy.Bar = 54321;
            dummy.Baz = 12345;
            
            var updateResult = db.Edge
                .Update(createResult.Value.String("_id"), dummy);
            
            Assert.AreEqual(202, updateResult.StatusCode);
            Assert.IsTrue(updateResult.Success);
            Assert.IsTrue(updateResult.HasValue);
            Assert.AreEqual(updateResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(updateResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreNotEqual(updateResult.Value.String("_rev"), createResult.Value.String("_rev"));
            
            var getResult = db.Edge
                .Get(updateResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), updateResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), updateResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), updateResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), dummy.Foo);
            
            Assert.AreNotEqual(getResult.Value.Int("bar"), document.Int("bar"));
            Assert.AreEqual(getResult.Value.Int("bar"), dummy.Bar);
            Assert.AreEqual(getResult.Value.Int("baz"), dummy.Baz);
        }
        
        #endregion
        
        #region Replace operations
        
        [Test()]
        public void Should_replace_edge()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var newDocument = new Dictionary<string, object>()
                .From(_documents[0].String("_id"))
                .To(_documents[1].String("_id"))
                .String("foo", "some other new string")
                .Int("baz", 54321);
            
            var replaceResult = db.Edge
                .Replace(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreNotEqual(replaceResult.Value.String("_rev"), createResult.Value.String("_rev"));
            
            var getResult = db.Edge
                .Get(replaceResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), replaceResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), replaceResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), replaceResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            
            Assert.AreEqual(getResult.Value.Int("baz"), newDocument.Int("baz"));

            Assert.IsFalse(getResult.Value.Has("bar"));
        }
        
        [Test()]
        public void Should_replace_edge_with_waitForSync()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var newDocument = new Dictionary<string, object>()
                .From(_documents[0].String("_id"))
                .To(_documents[1].String("_id"))
                .String("foo", "some other new string")
                .Int("baz", 54321);
            
            var replaceResult = db.Edge
                .WaitForSync(true)
                .Replace(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(201, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreNotEqual(replaceResult.Value.String("_rev"), createResult.Value.String("_rev"));
            
            var getResult = db.Edge
                .Get(replaceResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), replaceResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), replaceResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), replaceResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            
            Assert.AreEqual(getResult.Value.Int("baz"), newDocument.Int("baz"));

            Assert.IsFalse(getResult.Value.Has("bar"));
        }
        
        [Test()]
        public void Should_replace_edge_with_ifMatch()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            document.Merge(createResult.Value);
            
            var newDocument = new Dictionary<string, object>()
                .From(_documents[0].String("_id"))
                .To(_documents[1].String("_id"))
                .String("foo", "some other new string")
                .Int("baz", 54321);
            
            var replaceResult = db.Edge
                .IfMatch(document.String("_rev"))
                .Replace(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), document.String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), document.String("_key"));
            Assert.AreNotEqual(replaceResult.Value.String("_rev"), document.String("_rev"));
            
            var getResult = db.Edge
                .Get(replaceResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), replaceResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), replaceResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), replaceResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), newDocument.String("foo"));
            
            Assert.AreEqual(getResult.Value.Int("baz"), newDocument.Int("baz"));
            
            Assert.IsFalse(getResult.Value.Has("bar"));
        }
        
        [Test()]
        public void Should_replace_edge_with_ifMatch_and_return_412()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            document.Merge(createResult.Value);
            
            var newDocument = new Dictionary<string, object>()
                .From(_documents[0].String("_id"))
                .To(_documents[1].String("_id"))
                .String("foo", "some other new string")
                .Int("baz", 54321);
            
            var replaceResult = db.Edge
                .IfMatch("123456789")
                .Replace(createResult.Value.String("_id"), newDocument);
            
            Assert.AreEqual(412, replaceResult.StatusCode);
            Assert.IsFalse(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), document.String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), document.String("_key"));
            Assert.AreEqual(replaceResult.Value.String("_rev"), document.String("_rev"));
        }
        
        [Test()]
        public void Should_replace_edge_with_generic_object()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var dummy = new Dummy();
            dummy.Foo = "some other new string";
            dummy.Baz = 54321;
            
            var replaceResult = db.Edge
                .Replace(createResult.Value.String("_id"), _documents[0].String("_id"), _documents[1].String("_id"), dummy);
            
            Assert.AreEqual(202, replaceResult.StatusCode);
            Assert.IsTrue(replaceResult.Success);
            Assert.IsTrue(replaceResult.HasValue);
            Assert.AreEqual(replaceResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(replaceResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreNotEqual(replaceResult.Value.String("_rev"), createResult.Value.String("_rev"));
            
            var getResult = db.Edge
                .Get(replaceResult.Value.String("_id"));
            
            Assert.AreEqual(getResult.Value.String("_id"), replaceResult.Value.String("_id"));
            Assert.AreEqual(getResult.Value.String("_key"), replaceResult.Value.String("_key"));
            Assert.AreEqual(getResult.Value.String("_rev"), replaceResult.Value.String("_rev"));
            
            Assert.AreNotEqual(getResult.Value.String("foo"), document.String("foo"));
            Assert.AreEqual(getResult.Value.String("foo"), dummy.Foo);
            
            Assert.AreEqual(getResult.Value.Int("baz"), dummy.Baz);

            Assert.AreEqual(0, getResult.Value.Int("bar"));
        }
        
        #endregion
        
        #region Delete operations
        
        [Test()]
        public void Should_delete_edge()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var deleteResult = db.Document
                .Delete(createResult.Value.String("_id"));
            
            Assert.AreEqual(202, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(deleteResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(deleteResult.Value.String("_rev"), createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_delete_edge_with_waitForSync()
        {
        	Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var deleteResult = db.Document
                .WaitForSync(true)
                .Delete(createResult.Value.String("_id"));
            
            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(deleteResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(deleteResult.Value.String("_rev"), createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_delete_edge_with_ifMatch()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var deleteResult = db.Document
                .IfMatch(createResult.Value.String("_rev"))
                .Delete(createResult.Value.String("_id"));
            
            Assert.AreEqual(202, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(deleteResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(deleteResult.Value.String("_rev"), createResult.Value.String("_rev"));
        }
        
        [Test()]
        public void Should_delete_edge_with_ifMatch_and_return_412()
        {
            Database.ClearTestCollection(Database.TestEdgeCollectionName);
            var db = new ADatabase(Database.Alias);

            var document = new Dictionary<string, object>()
                .String("foo", "some string")
                .Int("bar", 12345);
            
            var createResult = db.Edge
                .Create(Database.TestEdgeCollectionName, _documents[0].String("_id"), _documents[1].String("_id"), document);
            
            var deleteResult = db.Document
                .IfMatch("123456789")
                .Delete(createResult.Value.String("_id"));
            
            Assert.AreEqual(412, deleteResult.StatusCode);
            Assert.IsFalse(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.AreEqual(deleteResult.Value.String("_id"), createResult.Value.String("_id"));
            Assert.AreEqual(deleteResult.Value.String("_key"), createResult.Value.String("_key"));
            Assert.AreEqual(deleteResult.Value.String("_rev"), createResult.Value.String("_rev"));
        }
        
        #endregion
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
