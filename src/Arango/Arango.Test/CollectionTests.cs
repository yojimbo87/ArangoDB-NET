using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arango.Client;

namespace Arango.Test
{
    [TestClass]
    public class CollectionTests
    {
        private ArangoDatabase _database;

        public CollectionTests()
        {
            string alias = "test";
            string[] connectionString = File.ReadAllText(@"..\..\..\..\..\ConnectionString.txt").Split(';');

            ArangoNode node = new ArangoNode(
                connectionString[0],
                int.Parse(connectionString[1]),
                connectionString[2],
                connectionString[3],
                alias
            );
            ArangoClient.Nodes.Add(node);

            _database = new ArangoDatabase(alias);
        }

        [TestMethod]
        public void CreateCollectionAndDeleteItByID()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollection001xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            Assert.AreEqual(testCollection.Name, newCollection.Name);
            Assert.AreEqual(testCollection.Type, newCollection.Type);
            Assert.AreEqual(testCollection.WaitForSync, newCollection.WaitForSync);
            Assert.AreEqual(testCollection.JournalSize, newCollection.JournalSize);

            long deletedCollectionID = _database.DeleteCollection(newCollection.ID);

            Assert.AreEqual(newCollection.ID, deletedCollectionID);
        }

        [TestMethod]
        public void CreateCollectionAndDeleteItByName()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollection002xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            Assert.AreEqual(testCollection.Name, newCollection.Name);
            Assert.AreEqual(testCollection.Type, newCollection.Type);
            Assert.AreEqual(testCollection.WaitForSync, newCollection.WaitForSync);
            Assert.AreEqual(testCollection.JournalSize, newCollection.JournalSize);

            long deletedCollectionID = _database.DeleteCollection(newCollection.Name);

            Assert.AreEqual(newCollection.ID, deletedCollectionID);
        }
    }
}
