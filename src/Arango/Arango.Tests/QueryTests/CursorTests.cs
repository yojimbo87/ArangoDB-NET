using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.QueryTests
{
    [TestFixture()]
    public class CursorTests : IDisposable
    {
        [Test()]
        public void Should_return_list_through_AQL()
        {
            List<ArangoDocument> docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "RETURN x";
            
            List<Document> documents = db.Query(aql);
            
            Assert.AreEqual(5, documents.Count);
            
            foreach (Document document in documents)
            {
                ArangoDocument doc = docs.Where(x => x.Id == document.GetField<string>("_id")).First();
                
                Assert.AreEqual(doc.Id, document.GetField<string>("_id"));
                Assert.AreEqual(doc.Key, document.GetField<string>("_key"));
                Assert.AreEqual(doc.Revision, document.GetField<string>("_rev"));
                Assert.AreEqual(doc.HasField("foo"), document.HasField("foo"));
                Assert.AreEqual(doc.GetField<string>("foo"), document.GetField<string>("foo"));
                Assert.AreEqual(doc.HasField("bar"), document.HasField("bar"));
                Assert.AreEqual(doc.GetField<string>("bar"), document.GetField<string>("bar"));
            }
        }
        
        [Test()]
        public void Should_return_list_through_AQL_with_small_batch_size()
        {
            List<ArangoDocument> docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "RETURN x";
            
            List<Document> documents = db.Query(aql, 2);
            
            Assert.AreEqual(5, documents.Count);
            
            foreach (Document document in documents)
            {
                ArangoDocument doc = docs.Where(x => x.Id == document.GetField<string>("_id")).First();
                
                Assert.AreEqual(doc.Id, document.GetField<string>("_id"));
                Assert.AreEqual(doc.Key, document.GetField<string>("_key"));
                Assert.AreEqual(doc.Revision, document.GetField<string>("_rev"));
                Assert.AreEqual(doc.HasField("foo"), document.HasField("foo"));
                Assert.AreEqual(doc.GetField<string>("foo"), document.GetField<string>("foo"));
                Assert.AreEqual(doc.HasField("bar"), document.HasField("bar"));
                Assert.AreEqual(doc.GetField<string>("bar"), document.GetField<string>("bar"));
            }
        }
        
        [Test()]
        public void Should_return_list_through_AQL_and_return_count()
        {
            List<ArangoDocument> docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "RETURN x";
            
            int count = 0;
            
            List<Document> documents = db.Query(aql, out count);
            
            Assert.AreEqual(5, documents.Count);
            Assert.AreEqual(5, count);
            
            foreach (Document document in documents)
            {
                ArangoDocument doc = docs.Where(x => x.Id == document.GetField<string>("_id")).First();
                
                Assert.AreEqual(doc.Id, document.GetField<string>("_id"));
                Assert.AreEqual(doc.Key, document.GetField<string>("_key"));
                Assert.AreEqual(doc.Revision, document.GetField<string>("_rev"));
                Assert.AreEqual(doc.HasField("foo"), document.HasField("foo"));
                Assert.AreEqual(doc.GetField<string>("foo"), document.GetField<string>("foo"));
                Assert.AreEqual(doc.HasField("bar"), document.HasField("bar"));
                Assert.AreEqual(doc.GetField<string>("bar"), document.GetField<string>("bar"));
            }
        }
        
        [Test()]
        public void Should_return_list_through_AQL_with_limit_and_return_count()
        {
            List<ArangoDocument> docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "LIMIT 2 " +
                "RETURN x";
            
            int count = 0;
            
            List<Document> documents = db.Query(aql, out count);
            
            Assert.AreEqual(2, documents.Count);
            Assert.AreEqual(2, count);
            
            foreach (Document document in documents)
            {
                ArangoDocument doc = docs.Where(x => x.Id == document.GetField<string>("_id")).First();
                
                Assert.AreEqual(doc.Id, document.GetField<string>("_id"));
                Assert.AreEqual(doc.Key, document.GetField<string>("_key"));
                Assert.AreEqual(doc.Revision, document.GetField<string>("_rev"));
                Assert.AreEqual(doc.HasField("foo"), document.HasField("foo"));
                Assert.AreEqual(doc.GetField<string>("foo"), document.GetField<string>("foo"));
                Assert.AreEqual(doc.HasField("bar"), document.HasField("bar"));
                Assert.AreEqual(doc.GetField<string>("bar"), document.GetField<string>("bar"));
            }
        }
        
        private List<ArangoDocument> CreateDummyDocuments()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            var docs = new List<ArangoDocument>();
            
            // create test documents
            var doc1 = new ArangoDocument()
                .SetField("foo", "foo string value 1")
                .SetField("bar", 12345);
            
            var doc2 = new ArangoDocument()
                .SetField("foo", "foo string value 2")
                .SetField("bar", 54321);
            
            var doc3 = new ArangoDocument()
                .SetField("foo", "foo string value 3")
                .SetField("bar", 54321);
            
            var doc4 = new ArangoDocument()
                .SetField("foo", "foo string value 4")
                .SetField("bar", 54321);
            
            var doc5 = new ArangoDocument()
                .SetField("foo", "foo string value 5")
                .SetField("bar", 54321);
            
            docs.Add(doc1);
            docs.Add(doc2);
            docs.Add(doc3);
            docs.Add(doc4);
            docs.Add(doc5);
            
            db.Document.Create(Database.TestDocumentCollectionName, doc1);
            db.Document.Create(Database.TestDocumentCollectionName, doc2);
            db.Document.Create(Database.TestDocumentCollectionName, doc3);
            db.Document.Create(Database.TestDocumentCollectionName, doc4);
            db.Document.Create(Database.TestDocumentCollectionName, doc5);
            
            return docs;
        }
        
        public void Dispose()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
        }
    }
}
