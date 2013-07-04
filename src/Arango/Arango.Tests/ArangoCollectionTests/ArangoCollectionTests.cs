using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.ArangoCollectionTests
{
    [TestFixture()]
    public class ArangoCollectionTests
    {
        [Test()]
        public void Should_create_and_delete_collection()
        {
            ArangoDatabase db = Database.GetTestDatabase();
            
            // set collection data
            ArangoCollection collection = new ArangoCollection();
            collection.Name = Database.TestCollectionName;
            collection.Type = ArangoCollectionType.Edge;
            collection.WaitForSync = true;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // check collection data retrieved from server
            Assert.AreEqual(false, string.IsNullOrEmpty(collection.Id));
            Assert.AreEqual(Database.TestCollectionName, collection.Name);
            Assert.AreEqual(ArangoCollectionType.Edge, collection.Type);
            Assert.AreEqual(ArangoCollectionStatus.Loaded, collection.Status);
            Assert.AreEqual(true, collection.WaitForSync);
            Assert.AreEqual(false, collection.IsSystem);
            Assert.AreEqual(false, collection.IsVolatile);
            
            // delete collection from database
            bool isDeleted = db.Collection.Delete(collection.Name);
            
            // check if the collection was deleted from database
            Assert.AreEqual(true, isDeleted);
        }
    }
}
