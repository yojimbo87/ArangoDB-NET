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
            var doc1 = new ArangoDocument()
                .SetField("foo", "foo string value 1")
                .SetField("bar", 12345);
            
            var doc2 = new ArangoDocument()
                .SetField("foo", "foo string value 2")
                .SetField("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge = new ArangoEdge()
                .SetField("edgeFoo", "foo string value")
                .SetField("edgeBar", 12345);
            
            edge.From = doc1.Id;
            edge.To = doc2.Id;
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // check if it contains data after creation
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.Id));
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.Key));
            Assert.AreEqual(false, string.IsNullOrEmpty(edge.Revision));
            Assert.AreEqual(true, edge.HasField("edgeFoo"));
            Assert.AreEqual(true, edge.HasField("edgeBar"));
            
            // delete that document
            var isDeleted = db.Edge.Delete(edge.Id);
            
            Assert.AreEqual(true, isDeleted);
        }
        
        [Test()]
        public void Should_create_and_get_edge()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new ArangoDocument()
                .SetField("foo", "foo string value 1")
                .SetField("bar", 12345);
            
            var doc2 = new ArangoDocument()
                .SetField("foo", "foo string value 2")
                .SetField("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge = new ArangoEdge()
                .SetField("edgeFoo", "foo string value")
                .SetField("edgeBar", 12345);
            
            edge.From = doc1.Id;
            edge.To = doc2.Id;
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // get the very same edge from database
            var returnedEdge = db.Edge.Get(edge.Id);
            
            // check if created and returned edge data are equal
            Assert.AreEqual(edge.Id, returnedEdge.Id);
            Assert.AreEqual(edge.Key, returnedEdge.Key);
            Assert.AreEqual(edge.Revision, returnedEdge.Revision);
            Assert.AreEqual(edge.From, returnedEdge.From);
            Assert.AreEqual(edge.To, returnedEdge.To);
            Assert.AreEqual(edge.HasField("edgeFoo"), returnedEdge.HasField("edgeFoo"));
            Assert.AreEqual(edge.GetField<string>("edgeFoo"), returnedEdge.GetField<string>("edgeFoo"));
            Assert.AreEqual(edge.HasField("edgeBar"), returnedEdge.HasField("edgeBar"));
            Assert.AreEqual(edge.GetField<int>("edgeBar"), returnedEdge.GetField<int>("edgeBar"));
        }
        
        [Test()]
        public void Should_create_and_replace_and_get_edge()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new ArangoDocument()
                .SetField("foo", "foo string value 1")
                .SetField("bar", 12345);
            
            var doc2 = new ArangoDocument()
                .SetField("foo", "foo string value 2")
                .SetField("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge = new ArangoEdge()
                .SetField("edgeFoo", "foo string value")
                .SetField("edgeBar", 12345);
            
            edge.From = doc1.Id;
            edge.To = doc2.Id;
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // change data in that edge and replaced it in database
            ArangoEdge newEdge = new ArangoEdge()
                .SetField("baz.foo", "bar string value");
            
            var isReplaced = db.Edge.Replace(edge.Id, newEdge);
            
            Assert.AreEqual(true, isReplaced);
            
            // get the very same edge from database
            var returnedEdge = db.Edge.Get(edge.Id);
            
            // check if the data of replaced and returned edge are equal
            Assert.AreEqual(newEdge.Id, returnedEdge.Id);
            Assert.AreEqual(newEdge.Key, returnedEdge.Key);
            Assert.AreEqual(newEdge.Revision, returnedEdge.Revision);
            Assert.AreEqual(newEdge.HasField("baz.foo"), returnedEdge.HasField("baz.foo"));
            Assert.AreEqual(newEdge.GetField<string>("baz.foo"), returnedEdge.GetField<string>("baz.foo"));
            
            // check if the original data doesn't exist anymore
            Assert.AreEqual(false, newEdge.HasField("foo"));
            Assert.AreEqual(false, newEdge.HasField("bar"));
            Assert.AreEqual(false, returnedEdge.HasField("foo"));
            Assert.AreEqual(false, returnedEdge.HasField("bar"));
        }
        
        [Test()]
        public void Should_create_and_check_for_edge_existence()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new ArangoDocument()
                .SetField("foo", "foo string value 1")
                .SetField("bar", 12345);
            
            var doc2 = new ArangoDocument()
                .SetField("foo", "foo string value 2")
                .SetField("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge = new ArangoEdge()
                .SetField("edgeFoo", "foo string value")
                .SetField("edgeBar", 12345);
            
            edge.From = doc1.Id;
            edge.To = doc2.Id;
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge);
            
            // check if the created edge exists in database        
            var exists = db.Edge.Exists(edge.Id);
            
            Assert.AreEqual(true, exists);
            
            // delete edge
            db.Edge.Delete(edge.Id);
            
            // check if the edge was deleted
            exists = db.Edge.Exists(edge.Id);
            
            Assert.AreEqual(false, exists);
        }
        
        [Test()]
        public void Should_create_and_get_any_direction_edges()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new ArangoDocument()
                .SetField("foo", "foo string value 1")
                .SetField("bar", 12345);
            
            var doc2 = new ArangoDocument()
                .SetField("foo", "foo string value 2")
                .SetField("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge1 = new ArangoEdge()
                .SetField("edgeFoo", "foo string value 1")
                .SetField("edgeBar", 12345);
            
            edge1.From = doc1.Id;
            edge1.To = doc2.Id;
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge1);
            
            // create another test edge
            var edge2 = new ArangoEdge()
                .SetField("edgeFoo", "foo string value 2")
                .SetField("edgeBar", 54321);
            
            edge2.From = doc2.Id;
            edge2.To = doc1.Id;
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge2);
            
            // get both edges
            List<ArangoEdge> anyEdges = db.Edge.Get(Database.TestEdgeCollectionName, doc1.Id);
            
            Assert.AreEqual(2, anyEdges.Count);
            
            ArangoEdge returnedEdge1 = anyEdges.Where(x => x.Id == edge1.Id).First();
            
            Assert.AreEqual(edge1.Id, returnedEdge1.Id);
            Assert.AreEqual(edge1.Key, returnedEdge1.Key);
            Assert.AreEqual(edge1.Revision, returnedEdge1.Revision);
            Assert.AreEqual(edge1.From, returnedEdge1.From);
            Assert.AreEqual(edge1.To, returnedEdge1.To);
            Assert.AreEqual(edge1.HasField("edgeFoo"), returnedEdge1.HasField("edgeFoo"));
            Assert.AreEqual(edge1.GetField<string>("edgeFoo"), returnedEdge1.GetField<string>("edgeFoo"));
            Assert.AreEqual(edge1.HasField("edgeBar"), returnedEdge1.HasField("edgeBar"));
            Assert.AreEqual(edge1.GetField<string>("edgeBar"), returnedEdge1.GetField<string>("edgeBar"));
            
            ArangoEdge returnedEdge2 = anyEdges.Where(x => x.Id == edge2.Id).First();
            
            Assert.AreEqual(edge2.Id, returnedEdge2.Id);
            Assert.AreEqual(edge2.Key, returnedEdge2.Key);
            Assert.AreEqual(edge2.Revision, returnedEdge2.Revision);
            Assert.AreEqual(edge2.From, returnedEdge2.From);
            Assert.AreEqual(edge2.To, returnedEdge2.To);
            Assert.AreEqual(edge2.HasField("edgeFoo"), returnedEdge2.HasField("edgeFoo"));
            Assert.AreEqual(edge2.GetField<string>("edgeFoo"), returnedEdge2.GetField<string>("edgeFoo"));
            Assert.AreEqual(edge2.HasField("edgeBar"), returnedEdge2.HasField("edgeBar"));
            Assert.AreEqual(edge2.GetField<string>("edgeBar"), returnedEdge2.GetField<string>("edgeBar"));
        }
        
        [Test()]
        public void Should_create_and_get_in_direction_edges()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new ArangoDocument()
                .SetField("foo", "foo string value 1")
                .SetField("bar", 12345);
            
            var doc2 = new ArangoDocument()
                .SetField("foo", "foo string value 2")
                .SetField("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge1 = new ArangoEdge()
                .SetField("edgeFoo", "foo string value 1")
                .SetField("edgeBar", 12345);
            
            edge1.From = doc1.Id;
            edge1.To = doc2.Id;
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge1);
            
            // create another test edge
            var edge2 = new ArangoEdge()
                .SetField("edgeFoo", "foo string value 2")
                .SetField("edgeBar", 54321);
            
            edge2.From = doc2.Id;
            edge2.To = doc1.Id;
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge2);
            
            // get both edges
            List<ArangoEdge> anyEdges = db.Edge.Get(Database.TestEdgeCollectionName, doc2.Id, ArangoEdgeDirection.In);
            
            Assert.AreEqual(1, anyEdges.Count);
            
            ArangoEdge returnedEdge1 = anyEdges.Where(x => x.Id == edge1.Id).First();
            
            Assert.AreEqual(edge1.Id, returnedEdge1.Id);
            Assert.AreEqual(edge1.Key, returnedEdge1.Key);
            Assert.AreEqual(edge1.Revision, returnedEdge1.Revision);
            Assert.AreEqual(edge1.From, returnedEdge1.From);
            Assert.AreEqual(edge1.To, returnedEdge1.To);
            Assert.AreEqual(edge1.HasField("edgeFoo"), returnedEdge1.HasField("edgeFoo"));
            Assert.AreEqual(edge1.GetField<string>("edgeFoo"), returnedEdge1.GetField<string>("edgeFoo"));
            Assert.AreEqual(edge1.HasField("edgeBar"), returnedEdge1.HasField("edgeBar"));
            Assert.AreEqual(edge1.GetField<string>("edgeBar"), returnedEdge1.GetField<string>("edgeBar"));
        }
        
        [Test()]
        public void Should_create_and_get_out_direction_edges()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            Database.CreateTestCollection(Database.TestEdgeCollectionName, ArangoCollectionType.Edge);
            var db = Database.GetTestDatabase();
            
            // create test documents
            var doc1 = new ArangoDocument()
                .SetField("foo", "foo string value 1")
                .SetField("bar", 12345);
            
            var doc2 = new ArangoDocument()
                .SetField("foo", "foo string value 2")
                .SetField("bar", 54321);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            
            // create test edge
            var edge1 = new ArangoEdge()
                .SetField("edgeFoo", "foo string value 1")
                .SetField("edgeBar", 12345);
            
            edge1.From = doc1.Id;
            edge1.To = doc2.Id;
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge1);
            
            // create another test edge
            var edge2 = new ArangoEdge()
                .SetField("edgeFoo", "foo string value 2")
                .SetField("edgeBar", 54321);
            
            edge2.From = doc2.Id;
            edge2.To = doc1.Id;
            
            db.Edge.Create(Database.TestEdgeCollectionName, edge2);
            
            // get both edges
            List<ArangoEdge> anyEdges = db.Edge.Get(Database.TestEdgeCollectionName, doc1.Id, ArangoEdgeDirection.Out);
            
            Assert.AreEqual(1, anyEdges.Count);
            
            ArangoEdge returnedEdge1 = anyEdges.Where(x => x.Id == edge1.Id).First();
            
            Assert.AreEqual(edge1.Id, returnedEdge1.Id);
            Assert.AreEqual(edge1.Key, returnedEdge1.Key);
            Assert.AreEqual(edge1.Revision, returnedEdge1.Revision);
            Assert.AreEqual(edge1.From, returnedEdge1.From);
            Assert.AreEqual(edge1.To, returnedEdge1.To);
            Assert.AreEqual(edge1.HasField("edgeFoo"), returnedEdge1.HasField("edgeFoo"));
            Assert.AreEqual(edge1.GetField<string>("edgeFoo"), returnedEdge1.GetField<string>("edgeFoo"));
            Assert.AreEqual(edge1.HasField("edgeBar"), returnedEdge1.HasField("edgeBar"));
            Assert.AreEqual(edge1.GetField<string>("edgeBar"), returnedEdge1.GetField<string>("edgeBar"));
        }
        
        public void Dispose()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            Database.DeleteTestCollection(Database.TestEdgeCollectionName);
        }
    }
}
