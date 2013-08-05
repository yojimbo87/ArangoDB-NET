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
            
            List<Document> documents = db.Query
                .AQL(aql)
                .Execute();
            
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
            
            List<Document> documents = db.Query
                .AQL(aql)
                .BatchSize(2)
                .Execute();
            
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
            
            List<Document> documents = db.Query
                .AQL(aql)
                .Execute(out count);
            
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

            List<Document> documents = db.Query
                .AQL(aql)
                .Execute(out count);
            
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
        
        [Test()]
        public void Should_return_generic_list_through_AQL()
        {
            List<Person> people = CreateDummyPeople();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "RETURN x";
            
            List<Person> returnedPeople = db.Query
                .AQL(aql)
                .Execute<Person>();
            
            Assert.AreEqual(5, returnedPeople.Count);
            
            foreach (Person person in returnedPeople)
            {
                Person per = people.Where(x => x.ThisIsId == person.ThisIsId).First();
                
                Assert.AreEqual(per.ThisIsId, person.ThisIsId);
                Assert.AreEqual(per.ThisIsKey, person.ThisIsKey);
                Assert.AreEqual(per.ThisIsRevision, person.ThisIsRevision);
                Assert.AreEqual(per.FirstName, person.FirstName);
                Assert.AreEqual(per.LastName, person.LastName);
            }
        }
        
        [Test()]
        public void Should_return_list_through_AQL_with_bindVars()
        {
            List<ArangoDocument> docs = CreateDummyDocuments().Where(q => q.GetField<int>("bar") == 54321).ToList();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "FILTER x.bar == @bar " +
                "RETURN x";
            
            List<Document> documents = db.Query
                .AQL(aql)
                .AddVar("bar", 54321)
                .Execute();
            
            Assert.AreEqual(4, documents.Count);
            
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
        
        private List<Person> CreateDummyPeople()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            var people = new List<Person>();
            
            // create test documents
            var p1 = new Person();
            p1.FirstName = "Johny";
            p1.LastName = "Bravo";
            
            var p2 = new Person();
            p2.FirstName = "Larry";
            p2.LastName = "Bravo";
            
            var p3 = new Person();
            p3.FirstName = "Sergei";
            p3.LastName = "Fitt";
            
            var p4 = new Person();
            p4.FirstName = "Lucy";
            p4.LastName = "Fox";
            
            var p5 = new Person();
            p5.FirstName = "Tom";
            p5.LastName = "Tall";
            
            people.Add(p1);
            people.Add(p2);
            people.Add(p3);
            people.Add(p4);
            people.Add(p5);
            
            db.Document.Create(Database.TestDocumentCollectionName, p1);
            db.Document.Create(Database.TestDocumentCollectionName, p2);
            db.Document.Create(Database.TestDocumentCollectionName, p3);
            db.Document.Create(Database.TestDocumentCollectionName, p4);
            db.Document.Create(Database.TestDocumentCollectionName, p5);
            
            return people;
        }
        
        public void Dispose()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
        }
    }
}
