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
            
            var arangoDocument = new ArangoDocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);
            
            db.Document.Create(Database.TestCollectionName, arangoDocument);
            
            Assert.AreEqual(false, string.IsNullOrEmpty(arangoDocument.Id));
            Assert.AreEqual(false, string.IsNullOrEmpty(arangoDocument.Key));
            Assert.AreEqual(false, string.IsNullOrEmpty(arangoDocument.Revision));
            Assert.AreEqual(true, arangoDocument.HasField("foo"));
            Assert.AreEqual(true, arangoDocument.HasField("bar"));
            
            var isDeleted = db.Document.Delete(arangoDocument.Id);
            
            Assert.AreEqual(true, isDeleted);
            
            Database.DeleteTestCollection();
        }
    }
}
