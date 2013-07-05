using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.ArangoDocumentTests
{
    [TestFixture()]
    public class ArangoDocumentTests
    {
        [Test()]
        public void Should_create_and_delete_document()
        {
            Database.CreateTestCollection();
            var db = Database.GetTestDatabase();
            
            // create some test document
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
            db.Document.Create(Database.TestCollectionName, arangoDocument);
            
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
            Database.CreateTestCollection();
            var db = Database.GetTestDatabase();
            
            // create some test document
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
            db.Document.Create(Database.TestCollectionName, arangoDocument);
            
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
            Database.CreateTestCollection();
            var db = Database.GetTestDatabase();
            
            // create some test document
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
            db.Document.Create(Database.TestCollectionName, arangoDocument);
            
            // change data in that document and replaced it in database
            ArangoDocument newArangoDocument = new ArangoDocument()
                .SetField("baz.foo", "bar string value");
            
            var isReplaced = db.Document.Replace(arangoDocument.Id, newArangoDocument);
            
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
            Database.CreateTestCollection();
            var db = Database.GetTestDatabase();
            
            // create some test document
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
            db.Document.Create(Database.TestCollectionName, arangoDocument);
            
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
    }
}
