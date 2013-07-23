using System;
using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.ArangoDocumentTests
{
    [TestFixture()]
    public class ArangoDocumentTests : IDisposable
    {
        [Test()]
        public void Should_create_and_delete_document()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            // create some test document
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
            db.Document.Create(Database.TestDocumentCollectionName, arangoDocument);
            
            // check if it contains data after creation
            Assert.AreEqual(false, string.IsNullOrEmpty(arangoDocument.Id));
            Assert.AreEqual(false, string.IsNullOrEmpty(arangoDocument.Key));
            Assert.AreEqual(false, string.IsNullOrEmpty(arangoDocument.Revision));
            Assert.AreEqual(true, arangoDocument.HasField("foo"));
            Assert.AreEqual(true, arangoDocument.HasField("bar"));
            
            // delete that document
            var isDeleted = db.Document.Delete(arangoDocument.Id);
            
            Assert.AreEqual(true, isDeleted);
        }
        
        [Test()]
        public void Should_create_and_get_document()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            // create some test document
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
            db.Document.Create(Database.TestDocumentCollectionName, arangoDocument);
            
            // get the very same document from database
            var returnedArangoDocument = db.Document.Get(arangoDocument.Id);
            
            // check if created and returned document data are equal
            Assert.AreEqual(arangoDocument.Id, returnedArangoDocument.Id);
            Assert.AreEqual(arangoDocument.Key, returnedArangoDocument.Key);
            Assert.AreEqual(arangoDocument.Revision, returnedArangoDocument.Revision);
            Assert.AreEqual(arangoDocument.HasField("foo"), returnedArangoDocument.HasField("foo"));
            Assert.AreEqual(arangoDocument.GetField<string>("foo"), returnedArangoDocument.GetField<string>("foo"));
            Assert.AreEqual(arangoDocument.HasField("bar"), returnedArangoDocument.HasField("bar"));
            Assert.AreEqual(arangoDocument.GetField<int>("bar"), returnedArangoDocument.GetField<int>("bar"));
        }
        
        [Test()]
        public void Should_create_and_replace_and_get_document()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            // create some test document
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
            db.Document.Create(Database.TestDocumentCollectionName, arangoDocument);
            
            // change data in that document and replaced it in database
            ArangoDocument newArangoDocument = new ArangoDocument();
            newArangoDocument.Id = arangoDocument.Id;
            newArangoDocument.SetField("baz.foo", "bar string value");
            
            var isReplaced = db.Document.Replace(newArangoDocument);
            
            Assert.AreEqual(true, isReplaced);
            
            // get the very same document from database
            var returnedArangoDocument = db.Document.Get(arangoDocument.Id);
            
            // check if the data of replaced and returned document are equal
            Assert.AreEqual(newArangoDocument.Id, returnedArangoDocument.Id);
            Assert.AreEqual(newArangoDocument.Key, returnedArangoDocument.Key);
            Assert.AreEqual(newArangoDocument.Revision, returnedArangoDocument.Revision);
            Assert.AreEqual(newArangoDocument.HasField("baz.foo"), returnedArangoDocument.HasField("baz.foo"));
            Assert.AreEqual(newArangoDocument.GetField<string>("baz.foo"), returnedArangoDocument.GetField<string>("baz.foo"));
            
            // check if the original data doesn't exist anymore
            Assert.AreEqual(false, newArangoDocument.HasField("foo"));
            Assert.AreEqual(false, newArangoDocument.HasField("bar"));
            Assert.AreEqual(false, returnedArangoDocument.HasField("foo"));
            Assert.AreEqual(false, returnedArangoDocument.HasField("bar"));
        }
        
        [Test()]
        public void Should_create_and_update_and_get_document()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            // create some test document
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
            db.Document.Create(Database.TestDocumentCollectionName, arangoDocument);
            
            // update data in that document and update it in database
            arangoDocument.SetField("baz.foo", "bar string value");
            
            var isUpdated = db.Document.Update(arangoDocument);
            
            Assert.AreEqual(true, isUpdated);
            
            // get the very same document from database
            var returnedArangoDocument = db.Document.Get(arangoDocument.Id);
            
            // check if the data of updated and returned document are equal
            Assert.AreEqual(arangoDocument.Id, returnedArangoDocument.Id);
            Assert.AreEqual(arangoDocument.Key, returnedArangoDocument.Key);
            Assert.AreEqual(arangoDocument.Revision, returnedArangoDocument.Revision);
            Assert.AreEqual(arangoDocument.HasField("foo"), returnedArangoDocument.HasField("foo"));
            Assert.AreEqual(arangoDocument.GetField<string>("foo"), returnedArangoDocument.GetField<string>("foo"));
            Assert.AreEqual(arangoDocument.HasField("bar"), returnedArangoDocument.HasField("bar"));
            Assert.AreEqual(arangoDocument.GetField<int>("bar"), returnedArangoDocument.GetField<int>("bar"));
            Assert.AreEqual(arangoDocument.HasField("baz.foo"), returnedArangoDocument.HasField("baz.foo"));
            Assert.AreEqual(arangoDocument.GetField<string>("baz.foo"), returnedArangoDocument.GetField<string>("baz.foo"));
        }
        
        [Test()]
        public void Should_create_and_check_for_document_existence()
        {
            Database.CreateTestCollection(Database.TestDocumentCollectionName);
            var db = Database.GetTestDatabase();
            
            // create some test document
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
            db.Document.Create(Database.TestDocumentCollectionName, arangoDocument);
            
            // check if the created document exists in database        
            var exists = db.Document.Exists(arangoDocument.Id);
            
            Assert.AreEqual(true, exists);
            
            // delete document
            db.Document.Delete(arangoDocument.Id);
            
            // check if the document was deleted
            exists = db.Document.Exists(arangoDocument.Id);
            
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
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
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
        }
    }
}
