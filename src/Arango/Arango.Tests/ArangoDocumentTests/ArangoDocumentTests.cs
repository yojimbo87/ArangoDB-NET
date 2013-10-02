using System;
using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.ArangoDocumentTests
{
    [TestFixture()]
    public class ArangoDocumentTests : IDisposable
    {
        public ArangoDocumentTests()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
        }
        
        [Test()]
        public void Should_create_and_delete_document()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            // create document object
            var document = new Document()
                .String("foo", "foo string value")
                .Int("bar", 12345);
            
            // save it to database collection 
            db.Document.Create(Database.TestDocumentCollectionName, document);
            
            // check if it contains data after creation
            Assert.AreEqual(false, document.IsNull("_id"));
            Assert.AreEqual(false, document.IsNull("_key"));
            Assert.AreEqual(false, document.IsNull("_rev"));
            Assert.AreEqual(false, document.IsNull("foo"));
            Assert.AreEqual(false, document.IsNull("bar"));
            
            Assert.AreEqual(true, document.Has("foo"));
            Assert.AreEqual(true, document.Has("bar"));
            
            // delete created document
            var isDeleted = db.Document.Delete(document.String("_id"));
            
            Assert.AreEqual(true, isDeleted);
        }
        
        [Test()]
        public void Should_create_and_get_document()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            var dateUtcNow = DateTime.UtcNow;
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan span = (dateUtcNow.ToUniversalTime() - unixEpoch);
            
            // create document object
            var document = new Document()
                .String("foo", "foo string value")
                .Int("bar", 12345)
                .DateTime("dateTime", dateUtcNow);
            
            // save it to database collection 
            db.Document.Create(Database.TestDocumentCollectionName, document);
            
            // get the very same document from database
            var returnedDocument = db.Document.Get(document.String("_id"));
            
            // check if created and returned document data are equal
            
            Assert.AreEqual(false, document.IsNull("_id"));
            Assert.AreEqual(false, document.IsNull("_key"));
            Assert.AreEqual(false, document.IsNull("_rev"));
            Assert.AreEqual(false, document.IsNull("foo"));
            Assert.AreEqual(false, document.IsNull("bar"));
            Assert.AreEqual(false, document.IsNull("dateTime"));
            Assert.AreEqual(false, returnedDocument.IsNull("_id"));
            Assert.AreEqual(false, returnedDocument.IsNull("_key"));
            Assert.AreEqual(false, returnedDocument.IsNull("_rev"));
            Assert.AreEqual(false, returnedDocument.IsNull("foo"));
            Assert.AreEqual(false, returnedDocument.IsNull("bar"));
            Assert.AreEqual(false, returnedDocument.IsNull("dateTime"));
            
            Assert.AreEqual(document.String("_id"), returnedDocument.String("_id"));
            Assert.AreEqual(document.String("_key"), returnedDocument.String("_key"));
            Assert.AreEqual(document.String("_rev"), returnedDocument.String("_rev"));
            Assert.AreEqual(document.Has("foo"), returnedDocument.Has("foo"));
            Assert.AreEqual(document.String("foo"), returnedDocument.String("foo"));
            Assert.AreEqual(document.Has("bar"), returnedDocument.Has("bar"));
            Assert.AreEqual(document.Int("bar"), returnedDocument.Int("bar"));
            Assert.AreEqual(typeof(long), returnedDocument.Type("dateTime"));
            Assert.AreEqual(typeof(DateTime), returnedDocument.DateTime("dateTime").GetType());
            Assert.AreEqual((long)span.TotalSeconds, document.Long("dateTime"));
            Assert.AreEqual(dateUtcNow.ToString("yyyy-MM-dd HH:mm:ss"), document.DateTime("dateTime").ToString("yyyy-MM-dd HH:mm:ss"));
        }
        
        [Test()]
        public void Should_create_and_replace_and_get_document()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            // create document object
            var document = new Document()
                .String("foo", "foo string value")
                .Int("bar", 12345);
            
            // save it to database collection 
            db.Document.Create(Database.TestDocumentCollectionName, document);
            
            // create new document object
            var newDocument = new Document()
                .String("_id", document.String("_id"))
                .String("baz.foo", "bar string value");
            
            // replace previously created document with new one
            var isReplaced = db.Document.Replace(newDocument);
            
            Assert.AreEqual(true, isReplaced);
            
            // get the very same document from database
            var returnedDocument = db.Document.Get(document.String("_id"));
            
            // check if the data of replaced and returned document are equal
            Assert.AreEqual(false, document.IsNull("_id"));
            Assert.AreEqual(false, document.IsNull("_key"));
            Assert.AreEqual(false, document.IsNull("_rev"));
            Assert.AreEqual(false, document.IsNull("foo"));
            Assert.AreEqual(false, document.IsNull("bar"));
            Assert.AreEqual(false, newDocument.IsNull("_id"));
            Assert.AreEqual(false, newDocument.IsNull("_key"));
            Assert.AreEqual(false, newDocument.IsNull("_rev"));
            Assert.AreEqual(false, newDocument.IsNull("baz.foo"));
            Assert.AreEqual(false, returnedDocument.IsNull("_id"));
            Assert.AreEqual(false, returnedDocument.IsNull("_key"));
            Assert.AreEqual(false, returnedDocument.IsNull("_rev"));
            Assert.AreEqual(false, returnedDocument.IsNull("baz.foo"));
            
            Assert.AreEqual(newDocument.String("_id"), returnedDocument.String("_id"));
            Assert.AreEqual(newDocument.String("_key"), returnedDocument.String("_key"));
            Assert.AreEqual(newDocument.String("_rev"), returnedDocument.String("_rev"));
            Assert.AreEqual(newDocument.Has("baz.foo"), returnedDocument.Has("baz.foo"));
            Assert.AreEqual(newDocument.String("baz.foo"), returnedDocument.String("baz.foo"));
            
            // check if the original data doesn't exist anymore
            Assert.AreEqual(false, newDocument.Has("foo"));
            Assert.AreEqual(false, newDocument.Has("bar"));
            Assert.AreEqual(false, returnedDocument.Has("foo"));
            Assert.AreEqual(false, returnedDocument.Has("bar"));
        }
        
        [Test()]
        public void Should_create_and_update_and_get_document()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            // create document object
            var document = new Document()
                .String("foo", "foo string value")
                .Int("bar", 12345);
            
            // save it to database collection
            db.Document.Create(Database.TestDocumentCollectionName, document);
            
            // update data in that document
            document.String("baz.foo", "bar string value");
            
            // update document in database
            var isUpdated = db.Document.Update(document);
            
            Assert.AreEqual(true, isUpdated);
            
            // get the very same document from database
            var returnedDocument = db.Document.Get(document.String("_id"));
            
            // check if the data of updated and returned document are equal
            Assert.AreEqual(false, document.IsNull("_id"));
            Assert.AreEqual(false, document.IsNull("_key"));
            Assert.AreEqual(false, document.IsNull("_rev"));
            Assert.AreEqual(false, document.IsNull("foo"));
            Assert.AreEqual(false, document.IsNull("bar"));
            Assert.AreEqual(false, document.IsNull("baz.foo"));
            Assert.AreEqual(false, returnedDocument.IsNull("_id"));
            Assert.AreEqual(false, returnedDocument.IsNull("_key"));
            Assert.AreEqual(false, returnedDocument.IsNull("_rev"));
            Assert.AreEqual(false, returnedDocument.IsNull("foo"));
            Assert.AreEqual(false, returnedDocument.IsNull("bar"));
            Assert.AreEqual(false, returnedDocument.IsNull("baz.foo"));
            
            Assert.AreEqual(document.String("_id"), returnedDocument.String("_id"));
            Assert.AreEqual(document.String("_key"), returnedDocument.String("_key"));
            Assert.AreEqual(document.String("_rev"), returnedDocument.String("_rev"));
            Assert.AreEqual(document.Has("foo"), returnedDocument.Has("foo"));
            Assert.AreEqual(document.String("foo"), returnedDocument.String("foo"));
            Assert.AreEqual(document.Has("bar"), returnedDocument.Has("bar"));
            Assert.AreEqual(document.Int("bar"), returnedDocument.Int("bar"));
            Assert.AreEqual(document.Has("baz.foo"), returnedDocument.Has("baz.foo"));
            Assert.AreEqual(document.String("baz.foo"), returnedDocument.String("baz.foo"));
        }
        
        [Test()]
        public void Should_create_and_check_for_document_existence()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            // create document object
            var document = new Document()
                .String("foo", "foo string value")
                .Int("bar", 12345);
            
            // save it to database collection
            db.Document.Create(Database.TestDocumentCollectionName, document);
            
            // check if the created document exists in database        
            var exists = db.Document.Exists(document.String("_id"));
            
            Assert.AreEqual(true, exists);
            
            // delete document
            db.Document.Delete(document.String("_id"));
            
            // check if the document was deleted
            exists = db.Document.Exists(document.String("_id"));
            
            Assert.AreEqual(false, exists);
        }
        
        [Test()]
        public void Should_create_document_from_generic_object_and_get_it_back()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            var person = new Person();
            person.FirstName = "Johny";
            person.LastName = "Bravo";
            person.Age = 25;
            person.ShouldNotBeSerializedOrDeserialized = "shouldn't be seen";
            person.Aliased = "aliased string";
            person.Interests = new List<string> {"programming", "hacking", "coding"};
            person.Father = new Person { FirstName = "Larry", LastName = "Bravo", Age = 45, Interests = new List<string> { "couching", "tennis" } };
            
            var follower1 = new Person { FirstName = "Mike", LastName = "Tall", Interests = new List<string> { "biking", "skiing"} };
            var follower2 = new Person { FirstName = "Lucy", LastName = "Fox", Interests = new List<string> { "cooking", "gardening"} };
            
            person.Followers = new List<Person> { follower1, follower2 };
            
            db.Document.Create(Database.TestDocumentCollectionName, person);
            
            // check if the created document exists in database        
            var exists = db.Document.Exists(person.ThisIsId);
            
            Assert.AreEqual(true, exists);
            
            // retrieve the very same document from database
            var returnedPerson = db.Document.Get<Person>(person.ThisIsId);
            
            // check if the data from created and returned document are equal
            Assert.AreEqual(false, string.IsNullOrEmpty(person.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(person.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(person.ThisIsRevision));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedPerson.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedPerson.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedPerson.ThisIsRevision));
            Assert.AreEqual(person.ThisIsId, returnedPerson.ThisIsId);
            Assert.AreEqual(person.ThisIsKey, returnedPerson.ThisIsKey);
            Assert.AreEqual(person.ThisIsRevision, returnedPerson.ThisIsRevision);
            
            Assert.AreEqual(person.FirstName, returnedPerson.FirstName);
            Assert.AreEqual(person.LastName, returnedPerson.LastName);
            Assert.AreEqual(person.Age, returnedPerson.Age);
            Assert.AreEqual(null, returnedPerson.ShouldNotBeSerializedOrDeserialized);
            Assert.AreEqual(person.Aliased, returnedPerson.Aliased);
            
            Assert.AreEqual(person.Interests.Count, returnedPerson.Interests.Count);
            Assert.AreEqual(person.Interests[0], returnedPerson.Interests[0]);
            Assert.AreEqual(person.Interests[1], returnedPerson.Interests[1]);
            Assert.AreEqual(person.Interests[2], returnedPerson.Interests[2]);
            
            Assert.AreEqual(person.Father.FirstName, returnedPerson.Father.FirstName);
            Assert.AreEqual(person.Father.LastName, returnedPerson.Father.LastName);
            Assert.AreEqual(person.Father.Age, returnedPerson.Father.Age);
            Assert.AreEqual(person.Father.Interests.Count, returnedPerson.Father.Interests.Count);
            Assert.AreEqual(person.Father.Interests[0], returnedPerson.Father.Interests[0]);
            Assert.AreEqual(person.Father.Interests[1], returnedPerson.Father.Interests[1]);
            
            Assert.AreEqual(person.Followers.Count, returnedPerson.Followers.Count);
            Assert.AreEqual(person.Followers[0].FirstName, returnedPerson.Followers[0].FirstName);
            Assert.AreEqual(person.Followers[0].LastName, returnedPerson.Followers[0].LastName);
            Assert.AreEqual(person.Followers[0].Interests.Count, returnedPerson.Followers[0].Interests.Count);
            Assert.AreEqual(person.Followers[0].Interests[0], returnedPerson.Followers[0].Interests[0]);
            Assert.AreEqual(person.Followers[0].Interests[1], returnedPerson.Followers[0].Interests[1]);
            Assert.AreEqual(person.Followers[1].FirstName, returnedPerson.Followers[1].FirstName);
            Assert.AreEqual(person.Followers[1].LastName, returnedPerson.Followers[1].LastName);
            Assert.AreEqual(person.Followers[1].Interests.Count, returnedPerson.Followers[1].Interests.Count);
            Assert.AreEqual(person.Followers[1].Interests[0], returnedPerson.Followers[1].Interests[0]);
            Assert.AreEqual(person.Followers[1].Interests[1], returnedPerson.Followers[1].Interests[1]);
        }
        
        [Test()]
        public void Should_create_document_from_generic_object_and_replace_it_and_get_it_back()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            // create some test document
            var document = new Document()
                .String("foo", "foo string value")
                .Int("bar", 12345);
            
            var person = new Person();
            person.FirstName = "Johny";
            person.LastName = "Bravo";
            person.Age = 25;
            person.Aliased = "aliased string";
            
            db.Document.Create(Database.TestDocumentCollectionName, person);
            
            // check if the created document exists in database        
            var exists = db.Document.Exists(person.ThisIsId);
            
            Assert.AreEqual(true, exists);
            
            var replacedPerson = new Person();
            replacedPerson.ThisIsId = person.ThisIsId;
            replacedPerson.FirstName = "Robert";
            replacedPerson.LastName = "Pizza";
            replacedPerson.Age = 14;
            
            // replace original document with new one
            var isReplaced = db.Document.Replace(replacedPerson);
            
            Assert.AreEqual(true, isReplaced);
            
            // retrieve the very same document from database
            var returnedPerson = db.Document.Get<Person>(person.ThisIsId);
            
            // check if the data from created and returned document are equal
            Assert.AreEqual(false, string.IsNullOrEmpty(replacedPerson.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(replacedPerson.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(replacedPerson.ThisIsRevision));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedPerson.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedPerson.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedPerson.ThisIsRevision));
            Assert.AreEqual(replacedPerson.ThisIsId, returnedPerson.ThisIsId);
            Assert.AreEqual(replacedPerson.ThisIsKey, returnedPerson.ThisIsKey);
            Assert.AreEqual(replacedPerson.ThisIsRevision, returnedPerson.ThisIsRevision);
            
            Assert.AreEqual(replacedPerson.FirstName, returnedPerson.FirstName);
            Assert.AreEqual(replacedPerson.LastName, returnedPerson.LastName);
            Assert.AreEqual(replacedPerson.Age, returnedPerson.Age);
            
            Assert.AreEqual(true, string.IsNullOrEmpty(returnedPerson.Aliased));
        }
        
        [Test()]
        public void Should_create_document_from_generic_object_and_update_it_and_get_it_back()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            var person = new Person();
            person.FirstName = "Johny";
            person.LastName = "Bravo";
            person.Age = 25;
            
            db.Document.Create(Database.TestDocumentCollectionName, person);
            
            // update data in that document and update it in database
            person.FirstName = "Robert";
            person.Aliased = "aliased string";
            
            var isUpdated = db.Document.Update(person);
            
            Assert.AreEqual(true, isUpdated);
            
            /// retrieve the very same document from database
            var returnedPerson = db.Document.Get<Person>(person.ThisIsId);
            
            // check if the data from created and returned document are equal
            Assert.AreEqual(false, string.IsNullOrEmpty(person.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(person.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(person.ThisIsRevision));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedPerson.ThisIsId));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedPerson.ThisIsKey));
            Assert.AreEqual(false, string.IsNullOrEmpty(returnedPerson.ThisIsRevision));
            Assert.AreEqual(person.ThisIsId, returnedPerson.ThisIsId);
            Assert.AreEqual(person.ThisIsKey, returnedPerson.ThisIsKey);
            Assert.AreEqual(person.ThisIsRevision, returnedPerson.ThisIsRevision);
            
            Assert.AreEqual(person.FirstName, returnedPerson.FirstName);
            Assert.AreEqual(person.LastName, returnedPerson.LastName);
            Assert.AreEqual(person.Age, returnedPerson.Age);
            Assert.AreEqual(person.Aliased, returnedPerson.Aliased);
        }
        
        public void Dispose()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
