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
        public void Should_return_simple_values_list_through_Aql()
        {
            var docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();
            
            var aql1 = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "RETURN x.foo";
            
            var items1 = db.Query
                .Aql(aql1)
                .ToList<string>();
            
            Assert.AreEqual(5, items1.Count);
            
            foreach (string item in items1)
            {
                var i = docs.Where(x => x.String("foo") == item).First().String("foo");
                
                Assert.AreEqual(i, item);
            }
            
            var aql2 = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "RETURN x.bar";
            
            var items2 = db.Query
                .Aql(aql2)
                .ToList<int>();
            
            Assert.AreEqual(5, items2.Count);
            
            foreach (int item in items2)
            {
                var i = docs.Where(x => x.Int("bar") == item).First().Int("bar");
                
                Assert.AreEqual(i, item);
            }
        }
        
        [Test()]
        public void Should_return_list_through_Aql()
        {
            var docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "RETURN x";
            
            var documents = db.Query
                .Aql(aql)
                .ToList();
            
            Assert.AreEqual(5, documents.Count);
            
            foreach (Document document in documents)
            {
                var doc = docs.Where(x => x.String("_id") == document.String("_id")).First();
                
                Assert.AreEqual(doc.String("_id"), document.String("_id"));
                Assert.AreEqual(doc.String("_key"), document.String("_key"));
                Assert.AreEqual(doc.String("_rev"), document.String("_rev"));
                Assert.AreEqual(doc.Has("foo"), document.Has("foo"));
                Assert.AreEqual(doc.String("foo"), document.String("foo"));
                Assert.AreEqual(doc.Has("bar"), document.Has("bar"));
                Assert.AreEqual(doc.Int("bar"), document.Int("bar"));
            }
        }
        
        [Test()]
        public void Should_return_list_through_Aql_with_small_batch_size()
        {
            var docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "RETURN x";
            
            var documents = db.Query
                .Aql(aql)
                .BatchSize(2)
                .ToList();
            
            Assert.AreEqual(5, documents.Count);
            
            foreach (Document document in documents)
            {
                var doc = docs.Where(x => x.String("_id") == document.String("_id")).First();
                
                Assert.AreEqual(doc.String("_id"), document.String("_id"));
                Assert.AreEqual(doc.String("_key"), document.String("_key"));
                Assert.AreEqual(doc.String("_rev"), document.String("_rev"));
                Assert.AreEqual(doc.Has("foo"), document.Has("foo"));
                Assert.AreEqual(doc.String("foo"), document.String("foo"));
                Assert.AreEqual(doc.Has("bar"), document.Has("bar"));
                Assert.AreEqual(doc.Int("bar"), document.Int("bar"));
            }
        }
        
        [Test()]
        public void Should_return_list_through_Aql_and_return_count()
        {
            var docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "RETURN x";
            
            var count = 0;
            
            var documents = db.Query
                .Aql(aql)
                .ToList(out count);
                
            Assert.AreEqual(5, documents.Count);
            Assert.AreEqual(5, count);
            
            foreach (Document document in documents)
            {
                var doc = docs.Where(x => x.String("_id") == document.String("_id")).First();
                
                Assert.AreEqual(doc.String("_id"), document.String("_id"));
                Assert.AreEqual(doc.String("_key"), document.String("_key"));
                Assert.AreEqual(doc.String("_rev"), document.String("_rev"));
                Assert.AreEqual(doc.Has("foo"), document.Has("foo"));
                Assert.AreEqual(doc.String("foo"), document.String("foo"));
                Assert.AreEqual(doc.Has("bar"), document.Has("bar"));
                Assert.AreEqual(doc.Int("bar"), document.Int("bar"));
            }
        }
        
        [Test()]
        public void Should_return_list_through_Aql_with_limit_and_return_count()
        {
            var docs = CreateDummyDocuments();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "LIMIT 2 " +
                "RETURN x";
            
            int count = 0;

            var documents = db.Query
                .Aql(aql)
                .ToList(out count);
            
            Assert.AreEqual(2, documents.Count);
            Assert.AreEqual(2, count);
            
            foreach (Document document in documents)
            {
                var doc = docs.Where(x => x.String("_id") == document.String("_id")).First();
                
                Assert.AreEqual(doc.String("_id"), document.String("_id"));
                Assert.AreEqual(doc.String("_key"), document.String("_key"));
                Assert.AreEqual(doc.String("_rev"), document.String("_rev"));
                Assert.AreEqual(doc.Has("foo"), document.Has("foo"));
                Assert.AreEqual(doc.String("foo"), document.String("foo"));
                Assert.AreEqual(doc.Has("bar"), document.Has("bar"));
                Assert.AreEqual(doc.Int("bar"), document.Int("bar"));
            }
        }
        
        [Test()]
        public void Should_return_generic_list_through_Aql()
        {
            var people = CreateDummyPeople();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "RETURN x";
            
            var returnedPeople = db.Query
                .Aql(aql)
                .ToList<Person>();
            
            Assert.AreEqual(5, returnedPeople.Count);
            
            foreach (Person person in returnedPeople)
            {
                var per = people.Where(x => x.ThisIsId == person.ThisIsId).First();
                
                Assert.AreEqual(per.ThisIsId, person.ThisIsId);
                Assert.AreEqual(per.ThisIsKey, person.ThisIsKey);
                Assert.AreEqual(per.ThisIsRevision, person.ThisIsRevision);
                Assert.AreEqual(per.FirstName, person.FirstName);
                Assert.AreEqual(per.LastName, person.LastName);
            }
        }
        
        [Test()]
        public void Should_return_list_through_Aql_with_bindVars()
        {
            var docs = CreateDummyDocuments().Where(q => q.Int("bar") == 54321).ToList();
            var db = Database.GetTestDatabase();
            
            var aql = 
                "FOR x IN " + Database.TestDocumentCollectionName + " " +
                "FILTER x.bar == @bar " +
                "RETURN x";
            
            var documents = db.Query
                .Aql(aql)
                .AddParameter("bar", 54321)
                .ToList();
            
            Assert.AreEqual(4, documents.Count);
            
            foreach (Document document in documents)
            {
                var doc = docs.Where(x => x.String("_id") == document.String("_id")).First();
                
                Assert.AreEqual(doc.String("_id"), document.String("_id"));
                Assert.AreEqual(doc.String("_key"), document.String("_key"));
                Assert.AreEqual(doc.String("_rev"), document.String("_rev"));
                Assert.AreEqual(doc.Has("foo"), document.Has("foo"));
                Assert.AreEqual(doc.String("foo"), document.String("foo"));
                Assert.AreEqual(doc.Has("bar"), document.Has("bar"));
                Assert.AreEqual(doc.Int("bar"), document.Int("bar"));
            }
        }
        
        private List<Document> CreateDummyDocuments()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            var docs = new List<Document>();
            
            // create test documents
            var doc1 = new Document()
                .String("foo", "foo string value 1")
                .Int("bar", 12345);
            
            var doc2 = new Document()
                .String("foo", "foo string value 2")
                .Int("bar", 54321);
            
            var doc3 = new Document()
                .String("foo", "foo string value 3")
                .Int("bar", 54321);
            
            var doc4 = new Document()
                .String("foo", "foo string value 4")
                .Int("bar", 54321);
            
            var doc5 = new Document()
                .String("foo", "foo string value 5")
                .Int("bar", 54321);
            
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
