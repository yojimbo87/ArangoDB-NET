using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.ArangoEdgeTests
{
    [TestFixture()]
    public class ArangoEdgeTests : IDisposable
    {
        [Test()]
        public void Should_create_and_delete_edge()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);
            
            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge = new Document()
                .String("edgeFoo", "foo string value")
                .Int("edgeBar", 12345);
            
            edge.String("_from", doc1.String("_id"));
            edge.String("_to", doc2.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // check if it contains data after creation
            Assert.AreEqual(false, edge.IsNull("_id"));
            Assert.AreEqual(false, edge.IsNull("_key"));
            Assert.AreEqual(false, edge.IsNull("_rev"));
            
            Assert.AreEqual(true, edge.Has("edgeFoo"));
            Assert.AreEqual(true, edge.Has("edgeBar"));
            
            // delete that document
            var isDeleted = db.Edge.Delete(edge.String("_id"));
            
            Assert.AreEqual(true, isDeleted);
        }
        
        [Test()]
        public void Should_create_and_get_edge()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);
            
            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge = new Document()
                .String("edgeFoo", "foo string value")
                .Int("edgeBar", 12345);
            
            edge.String("_from", doc1.String("_id"));
            edge.String("_to", doc2.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // get the very same edge from database
            var returnedEdge = db.Edge.Get(edge.String("_id"));
            
            // check if created and returned edge data are equal
            Assert.AreEqual(false, edge.IsNull("_id"));
            Assert.AreEqual(false, edge.IsNull("_key"));
            Assert.AreEqual(false, edge.IsNull("_rev"));
            Assert.AreEqual(false, edge.IsNull("_from"));
            Assert.AreEqual(false, edge.IsNull("_to"));
            Assert.AreEqual(false, edge.IsNull("edgeFoo"));
            Assert.AreEqual(false, edge.IsNull("edgeBar"));
            Assert.AreEqual(false, returnedEdge.IsNull("_id"));
            Assert.AreEqual(false, returnedEdge.IsNull("_key"));
            Assert.AreEqual(false, returnedEdge.IsNull("_rev"));
            Assert.AreEqual(false, returnedEdge.IsNull("_from"));
            Assert.AreEqual(false, returnedEdge.IsNull("_to"));
            Assert.AreEqual(false, returnedEdge.IsNull("edgeFoo"));
            Assert.AreEqual(false, returnedEdge.IsNull("edgeBar"));
            
            Assert.AreEqual(edge.String("_id"), returnedEdge.String("_id"));
            Assert.AreEqual(edge.String("_key"), returnedEdge.String("_key"));
            Assert.AreEqual(edge.String("_rev"), returnedEdge.String("_rev"));
            Assert.AreEqual(edge.String("_from"), returnedEdge.String("_from"));
            Assert.AreEqual(edge.String("_to"), returnedEdge.String("_to"));
            Assert.AreEqual(edge.Has("edgeFoo"), returnedEdge.Has("edgeFoo"));
            Assert.AreEqual(edge.String("edgeFoo"), returnedEdge.String("edgeFoo"));
            Assert.AreEqual(edge.Has("edgeBar"), returnedEdge.Has("edgeBar"));
            Assert.AreEqual(edge.Int("edgeBar"), returnedEdge.Int("edgeBar"));
        }
        
        [Test()]
        public void Should_create_edge_without_fields_and_get_it_back()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);
            
            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge = db.Edge.Create(Database.TestEdgeCollectionName, doc1.String("_id"), doc2.String("_id"));
            
            // get the very same edge from database
            var returnedEdge = db.Edge.Get(edge.String("_id"));
            
            // check if created and returned edge data are equal
            Assert.AreEqual(false, edge.IsNull("_id"));
            Assert.AreEqual(false, edge.IsNull("_key"));
            Assert.AreEqual(false, edge.IsNull("_rev"));
            Assert.AreEqual(false, edge.IsNull("_from"));
            Assert.AreEqual(false, edge.IsNull("_to"));
            Assert.AreEqual(false, returnedEdge.IsNull("_id"));
            Assert.AreEqual(false, returnedEdge.IsNull("_key"));
            Assert.AreEqual(false, returnedEdge.IsNull("_rev"));
            Assert.AreEqual(false, returnedEdge.IsNull("_from"));
            Assert.AreEqual(false, returnedEdge.IsNull("_to"));
            
            Assert.AreEqual(edge.String("_id"), returnedEdge.String("_id"));
            Assert.AreEqual(edge.String("_key"), returnedEdge.String("_key"));
            Assert.AreEqual(edge.String("_rev"), returnedEdge.String("_rev"));
            Assert.AreEqual(edge.String("_from"), returnedEdge.String("_from"));
            Assert.AreEqual(edge.String("_to"), returnedEdge.String("_to"));
        }
        
        [Test()]
        public void Should_create_and_replace_and_get_edge()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);
            
            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge = new Document()
                .String("edgeFoo", "foo string value")
                .Int("edgeBar", 12345);
            
            edge.String("_from", doc1.String("_id"));
            edge.String("_to", doc2.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // change data in that edge and replaced it in database
            var newEdge = new Document();
            newEdge.String("_id", edge.String("_id"));
            newEdge.String("baz.foo", "bar string value");
            
            var isReplaced = db.Edge.Replace(newEdge);
            
            Assert.AreEqual(true, isReplaced);
            
            // get the very same edge from database
            var returnedEdge = db.Edge.Get(edge.String("_id"));
            
            // check if the data of replaced and returned edge are equal
            Assert.AreEqual(false, newEdge.IsNull("_id"));
            Assert.AreEqual(false, newEdge.IsNull("_key"));
            Assert.AreEqual(false, newEdge.IsNull("_rev"));
            Assert.AreEqual(false, newEdge.IsNull("_from"));
            Assert.AreEqual(false, newEdge.IsNull("_to"));
            Assert.AreEqual(false, newEdge.IsNull("baz.foo"));
            Assert.AreEqual(false, returnedEdge.IsNull("_id"));
            Assert.AreEqual(false, returnedEdge.IsNull("_key"));
            Assert.AreEqual(false, returnedEdge.IsNull("_rev"));
            Assert.AreEqual(false, returnedEdge.IsNull("_from"));
            Assert.AreEqual(false, returnedEdge.IsNull("_to"));
            Assert.AreEqual(false, returnedEdge.IsNull("baz.foo"));
            
            Assert.AreEqual(newEdge.String("_id"), returnedEdge.String("_id"));
            Assert.AreEqual(newEdge.String("_key"), returnedEdge.String("_key"));
            Assert.AreEqual(newEdge.String("_rev"), returnedEdge.String("_rev"));
            Assert.AreEqual(newEdge.Has("baz.foo"), returnedEdge.Has("baz.foo"));
            Assert.AreEqual(newEdge.String("baz.foo"), returnedEdge.String("baz.foo"));
            
            // check if the original data doesn't exist anymore
            Assert.AreEqual(false, newEdge.Has("foo"));
            Assert.AreEqual(false, newEdge.Has("bar"));
            Assert.AreEqual(false, returnedEdge.Has("foo"));
            Assert.AreEqual(false, returnedEdge.Has("bar"));
        }
        
        [Test()]
        public void Should_create_and_update_and_get_edge()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
             // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);
            
            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge = new Document()
                .String("edgeFoo", "foo string value")
                .Int("edgeBar", 12345);
            
            edge.String("_from", doc1.String("_id"));
            edge.String("_to", doc2.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // update data in that document and update it in database
            edge.String("baz.foo", "bar string value");
            
            var isUpdated = db.Edge.Update(edge);
            
            Assert.AreEqual(true, isUpdated);
            
            // get the very same document from database
            var returnedEdge = db.Edge.Get(edge.String("_id"));
            
            // check if the data of updated and returned document are equal
            Assert.AreEqual(false, edge.IsNull("_id"));
            Assert.AreEqual(false, edge.IsNull("_key"));
            Assert.AreEqual(false, edge.IsNull("_rev"));
            Assert.AreEqual(false, edge.IsNull("_from"));
            Assert.AreEqual(false, edge.IsNull("_to"));
            Assert.AreEqual(false, edge.IsNull("edgeFoo"));
            Assert.AreEqual(false, edge.IsNull("edgeBar"));
            Assert.AreEqual(false, edge.IsNull("baz.foo"));
            Assert.AreEqual(false, returnedEdge.IsNull("_id"));
            Assert.AreEqual(false, returnedEdge.IsNull("_key"));
            Assert.AreEqual(false, returnedEdge.IsNull("_rev"));
            Assert.AreEqual(false, returnedEdge.IsNull("_from"));
            Assert.AreEqual(false, returnedEdge.IsNull("_to"));
            Assert.AreEqual(false, returnedEdge.IsNull("edgeFoo"));
            Assert.AreEqual(false, returnedEdge.IsNull("edgeBar"));
            Assert.AreEqual(false, returnedEdge.IsNull("baz.foo"));
            
            Assert.AreEqual(edge.String("_id"), returnedEdge.String("_id"));
            Assert.AreEqual(edge.String("_key"), returnedEdge.String("_key"));
            Assert.AreEqual(edge.String("_rev"), returnedEdge.String("_rev"));
            Assert.AreEqual(edge.Has("edgeFoo"), returnedEdge.Has("edgeFoo"));
            Assert.AreEqual(edge.String("edgeFoo"), returnedEdge.String("edgeFoo"));
            Assert.AreEqual(edge.Has("edgeBar"), returnedEdge.Has("edgeBar"));
            Assert.AreEqual(edge.Int("edgeBar"), returnedEdge.Int("edgeBar"));
            Assert.AreEqual(edge.Has("baz.foo"), returnedEdge.Has("baz.foo"));
            Assert.AreEqual(edge.String("baz.foo"), returnedEdge.String("baz.foo"));
        }
        
        [Test()]
        public void Should_create_and_check_for_edge_existence()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);
            
            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge = new Document()
                .String("edgeFoo", "foo string value")
                .Int("edgeBar", 12345);
            
            edge.String("_from", doc1.String("_id"));
            edge.String("_to", doc2.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // check if the created edge exists in database        
            var exists = db.Edge.Exists(edge.String("_id"));
            
            Assert.AreEqual(true, exists);
            
            // delete edge
            db.Edge.Delete(edge.String("_id"));
            
            // check if the edge was deleted
            exists = db.Edge.Exists(edge.String("_id"));
            
            Assert.AreEqual(false, exists);
        }
        
        [Test()]
        public void Should_create_and_get_any_direction_edges()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);
            
            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge1 = new Document()
                .String("edgeFoo", "foo string value 1")
                .Int("edgeBar", 12345);
            
            edge1.String("_from", doc1.String("_id"));
            edge1.String("_to", doc2.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge1);
            
            // create another test edge
            var edge2 = new Document()
                .String("edgeFoo", "foo string value 2")
                .Int("edgeBar", 54321);
            
            edge2.String("_from", doc2.String("_id"));
            edge2.String("_to", doc1.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge2);
            
            // get both edges
            var anyEdges = db.Edge.Get(Database.TestEdgeCollectionName, doc1.String("_id"));
            
            Assert.AreEqual(2, anyEdges.Count);
            
            var returnedEdge1 = anyEdges.Where(x => x.String("_id") == edge1.String("_id")).First();
            
            Assert.AreEqual(false, edge1.IsNull("_id"));
            Assert.AreEqual(false, edge1.IsNull("_key"));
            Assert.AreEqual(false, edge1.IsNull("_rev"));
            Assert.AreEqual(false, edge1.IsNull("_from"));
            Assert.AreEqual(false, edge1.IsNull("_to"));
            Assert.AreEqual(false, edge1.IsNull("edgeFoo"));
            Assert.AreEqual(false, edge1.IsNull("edgeBar"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_id"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_key"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_rev"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_from"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_to"));
            Assert.AreEqual(false, returnedEdge1.IsNull("edgeFoo"));
            Assert.AreEqual(false, returnedEdge1.IsNull("edgeBar"));
            
            Assert.AreEqual(edge1.String("_id"), returnedEdge1.String("_id"));
            Assert.AreEqual(edge1.String("_key"), returnedEdge1.String("_key"));
            Assert.AreEqual(edge1.String("_rev"), returnedEdge1.String("_rev"));
            Assert.AreEqual(edge1.String("_from"), returnedEdge1.String("_from"));
            Assert.AreEqual(edge1.String("_to"), returnedEdge1.String("_to"));
            Assert.AreEqual(edge1.Has("edgeFoo"), returnedEdge1.Has("edgeFoo"));
            Assert.AreEqual(edge1.String("edgeFoo"), returnedEdge1.String("edgeFoo"));
            Assert.AreEqual(edge1.Has("edgeBar"), returnedEdge1.Has("edgeBar"));
            Assert.AreEqual(edge1.Int("edgeBar"), returnedEdge1.Int("edgeBar"));
            
            var returnedEdge2 = anyEdges.Where(x => x.String("_id") == edge2.String("_id")).First();
            
            Assert.AreEqual(false, edge2.IsNull("_id"));
            Assert.AreEqual(false, edge2.IsNull("_key"));
            Assert.AreEqual(false, edge2.IsNull("_rev"));
            Assert.AreEqual(false, edge2.IsNull("_from"));
            Assert.AreEqual(false, edge2.IsNull("_to"));
            Assert.AreEqual(false, edge2.IsNull("edgeFoo"));
            Assert.AreEqual(false, edge2.IsNull("edgeBar"));
            Assert.AreEqual(false, returnedEdge2.IsNull("_id"));
            Assert.AreEqual(false, returnedEdge2.IsNull("_key"));
            Assert.AreEqual(false, returnedEdge2.IsNull("_rev"));
            Assert.AreEqual(false, returnedEdge2.IsNull("_from"));
            Assert.AreEqual(false, returnedEdge2.IsNull("_to"));
            Assert.AreEqual(false, returnedEdge2.IsNull("edgeFoo"));
            Assert.AreEqual(false, returnedEdge2.IsNull("edgeBar"));
            
            Assert.AreEqual(edge2.String("_id"), returnedEdge2.String("_id"));
            Assert.AreEqual(edge2.String("_key"), returnedEdge2.String("_key"));
            Assert.AreEqual(edge2.String("_rev"), returnedEdge2.String("_rev"));
            Assert.AreEqual(edge2.String("_from"), returnedEdge2.String("_from"));
            Assert.AreEqual(edge2.String("_to"), returnedEdge2.String("_to"));
            Assert.AreEqual(edge2.Has("edgeFoo"), returnedEdge2.Has("edgeFoo"));
            Assert.AreEqual(edge2.String("edgeFoo"), returnedEdge2.String("edgeFoo"));
            Assert.AreEqual(edge2.Has("edgeBar"), returnedEdge2.Has("edgeBar"));
            Assert.AreEqual(edge2.Int("edgeBar"), returnedEdge2.Int("edgeBar"));
        }
        
        [Test()]
        public void Should_create_and_get_in_direction_edges()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);
            
            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge1 = new Document()
                .String("edgeFoo", "foo string value 1")
                .Int("edgeBar", 12345);
            
            edge1.String("_from", doc1.String("_id"));
            edge1.String("_to", doc2.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge1);
            
            // create another test edge
            var edge2 = new Document()
                .String("edgeFoo", "foo string value 2")
                .Int("edgeBar", 54321);
            
            edge2.String("_from", doc2.String("_id"));
            edge2.String("_to", doc1.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge2);
            
            // get both edges
            var anyEdges = db.Edge.Get(Database.TestEdgeCollectionName, doc2.String("_id"), ArangoEdgeDirection.In);
            
            Assert.AreEqual(1, anyEdges.Count);
            
            var returnedEdge1 = anyEdges.Where(x => x.String("_id") == edge1.String("_id")).First();
            
            Assert.AreEqual(false, edge1.IsNull("_id"));
            Assert.AreEqual(false, edge1.IsNull("_key"));
            Assert.AreEqual(false, edge1.IsNull("_rev"));
            Assert.AreEqual(false, edge1.IsNull("_from"));
            Assert.AreEqual(false, edge1.IsNull("_to"));
            Assert.AreEqual(false, edge1.IsNull("edgeFoo"));
            Assert.AreEqual(false, edge1.IsNull("edgeBar"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_id"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_key"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_rev"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_from"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_to"));
            Assert.AreEqual(false, returnedEdge1.IsNull("edgeFoo"));
            Assert.AreEqual(false, returnedEdge1.IsNull("edgeBar"));
            
            Assert.AreEqual(edge1.String("_id"), returnedEdge1.String("_id"));
            Assert.AreEqual(edge1.String("_key"), returnedEdge1.String("_key"));
            Assert.AreEqual(edge1.String("_rev"), returnedEdge1.String("_rev"));
            Assert.AreEqual(edge1.String("_from"), returnedEdge1.String("_from"));
            Assert.AreEqual(edge1.String("_to"), returnedEdge1.String("_to"));
            Assert.AreEqual(edge1.Has("edgeFoo"), returnedEdge1.Has("edgeFoo"));
            Assert.AreEqual(edge1.String("edgeFoo"), returnedEdge1.String("edgeFoo"));
            Assert.AreEqual(edge1.Has("edgeBar"), returnedEdge1.Has("edgeBar"));
            Assert.AreEqual(edge1.Int("edgeBar"), returnedEdge1.Int("edgeBar"));
        }
        
        [Test()]
        public void Should_create_and_get_out_direction_edges()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);
            
            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge1 = new Document()
                .String("edgeFoo", "foo string value 1")
                .Int("edgeBar", 12345);
            
            edge1.String("_from", doc1.String("_id"));
            edge1.String("_to", doc2.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge1);
            
            // create another test edge
            var edge2 = new Document()
                .String("edgeFoo", "foo string value 2")
                .Int("edgeBar", 54321);
            
            edge2.String("_from", doc2.String("_id"));
            edge2.String("_to", doc1.String("_id"));
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge2);
            
            // get both edges
            var anyEdges = db.Edge.Get(Database.TestEdgeCollectionName, doc1.String("_id"), ArangoEdgeDirection.Out);
            
            Assert.AreEqual(1, anyEdges.Count);
            
            var returnedEdge1 = anyEdges.Where(x => x.String("_id") == edge1.String("_id")).First();
            
            Assert.AreEqual(false, edge1.IsNull("_id"));
            Assert.AreEqual(false, edge1.IsNull("_key"));
            Assert.AreEqual(false, edge1.IsNull("_rev"));
            Assert.AreEqual(false, edge1.IsNull("_from"));
            Assert.AreEqual(false, edge1.IsNull("_to"));
            Assert.AreEqual(false, edge1.IsNull("edgeFoo"));
            Assert.AreEqual(false, edge1.IsNull("edgeBar"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_id"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_key"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_rev"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_from"));
            Assert.AreEqual(false, returnedEdge1.IsNull("_to"));
            Assert.AreEqual(false, returnedEdge1.IsNull("edgeFoo"));
            Assert.AreEqual(false, returnedEdge1.IsNull("edgeBar"));
            
            Assert.AreEqual(edge1.String("_id"), returnedEdge1.String("_id"));
            Assert.AreEqual(edge1.String("_key"), returnedEdge1.String("_key"));
            Assert.AreEqual(edge1.String("_rev"), returnedEdge1.String("_rev"));
            Assert.AreEqual(edge1.String("_from"), returnedEdge1.String("_from"));
            Assert.AreEqual(edge1.String("_to"), returnedEdge1.String("_to"));
            Assert.AreEqual(edge1.Has("edgeFoo"), returnedEdge1.Has("edgeFoo"));
            Assert.AreEqual(edge1.String("edgeFoo"), returnedEdge1.String("edgeFoo"));
            Assert.AreEqual(edge1.Has("edgeBar"), returnedEdge1.Has("edgeBar"));
            Assert.AreEqual(edge1.Int("edgeBar"), returnedEdge1.Int("edgeBar"));
        }
        
        [Test()]
        public void Should_create_edge_from_generic_object_and_get_it_back()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName);
            var db = Database.GetTestDatabase();
            
            var person1 = new Person();
            person1.FirstName = "Johny";
            person1.LastName = "Bravo";
            
            var person2 = new Person();
            person2.FirstName = "Larry";
            person2.LastName = "Page";
            
            db.Document.Create(Database.TestDocumentCollectionName, person1);
            db.Document.Create(Database.TestDocumentCollectionName, person2);
            
            var edge = new Person();
            edge.ThisIsFrom = person1.ThisIsId;
            edge.ThisIsTo = person2.ThisIsId;
            edge.Aliased = "edge alias string";
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // retrieve the very same document from database
            var returnedEdge = db.Edge.Get<Person>(edge.ThisIsId);
            
            // check if the data from created and returned document are equal
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.ThisIsRevision));
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.ThisIsFrom));
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.ThisIsTo));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsRevision));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsFrom));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsTo));
            Assert.AreEqual(edge.ThisIsId, returnedEdge.ThisIsId);
            Assert.AreEqual(edge.ThisIsKey, returnedEdge.ThisIsKey);
            Assert.AreEqual(edge.ThisIsRevision, returnedEdge.ThisIsRevision);
            Assert.AreEqual(edge.ThisIsFrom, returnedEdge.ThisIsFrom);
            Assert.AreEqual(edge.ThisIsTo, returnedEdge.ThisIsTo);
            
            Assert.AreEqual(edge.Aliased, returnedEdge.Aliased);
        }
        
        [Test()]
        public void Should_create_edge_from_generic_object_and_replace_it_and_get_it_back()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName);
            var db = Database.GetTestDatabase();
            
            var person1 = new Person();
            person1.FirstName = "Johny";
            person1.LastName = "Bravo";
            
            var person2 = new Person();
            person2.FirstName = "Larry";
            person2.LastName = "Page";
            
            db.Document.Create(Database.TestDocumentCollectionName, person1);
            db.Document.Create(Database.TestDocumentCollectionName, person2);
            
            var edge = new Person();
            edge.ThisIsFrom = person1.ThisIsId;
            edge.ThisIsTo = person2.ThisIsId;
            edge.Aliased = "edge alias string";
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            var replacedEdge = new Person();
            replacedEdge.ThisIsId = edge.ThisIsId;
            replacedEdge.FirstName = "Robert";
            
            // replace original document with new one
            var isReplaced = db.Edge.Replace(replacedEdge);
            
            Assert.AreEqual(true, isReplaced);
            
            // retrieve the very same document from database
            var returnedEdge = db.Edge.Get<Person>(replacedEdge.ThisIsId);
            
            // check if the data from created and returned document are equal
            Assert.AreEqual(false, string.IsNullOrEmpty(replacedEdge.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(replacedEdge.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(replacedEdge.ThisIsRevision));
            //Assert.AreEqual(false, string.IsNullOrEmpty(replacedEdge.ThisIsFrom));
            //Assert.AreEqual(false, string.IsNullOrEmpty(replacedEdge.ThisIsTo));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsRevision));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsFrom));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsTo));
            Assert.AreEqual(replacedEdge.ThisIsId, returnedEdge.ThisIsId);
            Assert.AreEqual(replacedEdge.ThisIsKey, returnedEdge.ThisIsKey);
            Assert.AreEqual(replacedEdge.ThisIsRevision, returnedEdge.ThisIsRevision);
            //Assert.AreEqual(replacedEdge.ThisIsFrom, returnedEdge.ThisIsFrom);
            //Assert.AreEqual(replacedEdge.ThisIsTo, returnedEdge.ThisIsTo);
            
            Assert.AreEqual(replacedEdge.FirstName, returnedEdge.FirstName);
            Assert.AreEqual(true, string.IsNullOrEmpty(returnedEdge.Aliased));
        }
        
        [Test()]
        public void Should_create_edge_from_generic_object_and_update_it_and_get_it_back()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName);
            var db = Database.GetTestDatabase();
            
            var person1 = new Person();
            person1.FirstName = "Johny";
            person1.LastName = "Bravo";
            
            var person2 = new Person();
            person2.FirstName = "Larry";
            person2.LastName = "Page";
            
            db.Document.Create(Database.TestDocumentCollectionName, person1);
            db.Document.Create(Database.TestDocumentCollectionName, person2);
            
            var edge = new Person();
            edge.ThisIsFrom = person1.ThisIsId;
            edge.ThisIsTo = person2.ThisIsId;
            edge.LastName = "Bravo";
            edge.Aliased = "edge alias string";
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // update data in edge and update it in database
            edge.FirstName = "Robert";
            edge.Aliased = "aliased string";
            
            var isUpdated = db.Edge.Update(edge);
            
            Assert.AreEqual(true, isUpdated);
            
           // retrieve the very same document from database
            var returnedEdge = db.Edge.Get<Person>(edge.ThisIsId);
            
            // check if the data from created and returned document are equal
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.ThisIsRevision));
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.ThisIsFrom));
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.ThisIsTo));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsRevision));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsFrom));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedEdge.ThisIsTo));
            Assert.AreEqual(edge.ThisIsId, returnedEdge.ThisIsId);
            Assert.AreEqual(edge.ThisIsKey, returnedEdge.ThisIsKey);
            Assert.AreEqual(edge.ThisIsRevision, returnedEdge.ThisIsRevision);
            Assert.AreEqual(edge.ThisIsFrom, returnedEdge.ThisIsFrom);
            Assert.AreEqual(edge.ThisIsTo, returnedEdge.ThisIsTo);
            
            Assert.AreEqual(edge.FirstName, returnedEdge.FirstName);
            Assert.AreEqual(edge.LastName, returnedEdge.LastName);
            Assert.AreEqual(edge.Aliased, returnedEdge.Aliased);
        }
        
        public void Dispose()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            Database.DeleteTestCollection(Database.TestEdgeCollectionName);
        }
    }
}
