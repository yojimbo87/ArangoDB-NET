using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        #region Create, delete

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

        #endregion

        #region Create, truncate, delete

        [TestMethod]
        public void CreateCollectionAndTruncateByIdAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeTruncated001xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            bool isTruncated = _database.TruncateCollection(newCollection.ID);

            Assert.AreEqual(isTruncated, true);

            _database.DeleteCollection(newCollection.ID);
        }

        [TestMethod]
        public void CreateCollectionAndTruncateByNameAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeTruncated002xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            bool isTruncated = _database.TruncateCollection(newCollection.Name);

            Assert.AreEqual(isTruncated, true);

            _database.DeleteCollection(newCollection.Name);
        }

        #endregion

        #region Create, load, delete

        [TestMethod]
        public void CreateCollectionAndLoadByIdAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead001xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            ArangoCollection collection = _database.LoadCollection(newCollection.ID);

            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);
            Assert.AreEqual(collection.Type, newCollection.Type);
            Assert.IsTrue(collection.DocumentsCount >= 0);

            _database.DeleteCollection(newCollection.Name);
        }

        [TestMethod]
        public void CreateCollectionAndLoadByNameAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead002xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            ArangoCollection collection = _database.LoadCollection(newCollection.Name);

            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);
            Assert.AreEqual(collection.Type, newCollection.Type);
            Assert.IsTrue(collection.DocumentsCount >= 0);

            _database.DeleteCollection(newCollection.Name);
        }

        #endregion

        #region Create, get, delete

        [TestMethod]
        public void CreateCollectionAndGetByIdAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead001xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            ArangoCollection collection = _database.GetCollection(newCollection.ID);

            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);
            Assert.AreEqual(collection.Type, newCollection.Type);

            _database.DeleteCollection(newCollection.Name);
        }

        [TestMethod]
        public void CreateCollectionAndGetByNameAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead002xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            ArangoCollection collection = _database.GetCollection(newCollection.Name);

            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);
            Assert.AreEqual(collection.Type, newCollection.Type);

            _database.DeleteCollection(newCollection.Name);
        }

        [TestMethod]
        public void CreateCollectionAndGetPropertiesByIdAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead001xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            ArangoCollection collection = _database.GetCollectionProperties(newCollection.ID);

            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);
            Assert.AreEqual(collection.Type, newCollection.Type);
            Assert.AreEqual(collection.WaitForSync, newCollection.WaitForSync);
            Assert.AreEqual(collection.JournalSize, newCollection.JournalSize);

            _database.DeleteCollection(newCollection.Name);
        }

        [TestMethod]
        public void CreateCollectionAndGetPropertiesByNameAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead002xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            ArangoCollection collection = _database.GetCollectionProperties(newCollection.Name);

            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);
            Assert.AreEqual(collection.Type, newCollection.Type);
            Assert.AreEqual(collection.WaitForSync, newCollection.WaitForSync);
            Assert.AreEqual(collection.JournalSize, newCollection.JournalSize);

            _database.DeleteCollection(newCollection.Name);
        }

        [TestMethod]
        public void CreateCollectionAndGetCountByIdAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead001xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            ArangoCollection collection = _database.GetCollectionCount(newCollection.ID);

            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);
            Assert.AreEqual(collection.Type, newCollection.Type);
            Assert.AreEqual(collection.WaitForSync, newCollection.WaitForSync);
            Assert.AreEqual(collection.JournalSize, newCollection.JournalSize);
            Assert.IsTrue(collection.DocumentsCount >= 0);

            _database.DeleteCollection(newCollection.Name);
        }

        [TestMethod]
        public void CreateCollectionAndGetCountByNameAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead002xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            ArangoCollection collection = _database.GetCollectionCount(newCollection.Name);

            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);
            Assert.AreEqual(collection.Type, newCollection.Type);
            Assert.AreEqual(collection.WaitForSync, newCollection.WaitForSync);
            Assert.AreEqual(collection.JournalSize, newCollection.JournalSize);
            Assert.IsTrue(collection.DocumentsCount >= 0);

            _database.DeleteCollection(newCollection.Name);
        }

        [TestMethod]
        public void CreateCollectionAndGeFiguresByIdAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead001xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            ArangoCollection collection = _database.GetCollectionFigures(newCollection.ID);

            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);
            Assert.AreEqual(collection.Type, newCollection.Type);
            Assert.AreEqual(collection.WaitForSync, newCollection.WaitForSync);
            Assert.AreEqual(collection.JournalSize, newCollection.JournalSize);
            Assert.IsTrue(collection.DocumentsCount >= 0);
            Assert.IsTrue(collection.AliveDocumentsCount >= 0);
            Assert.IsTrue(collection.AliveDocumentsSize >= 0);
            Assert.IsTrue(collection.DeadDocumentsCount >= 0);
            Assert.IsTrue(collection.DeadDocumentsSize >= 0);
            Assert.IsTrue(collection.DeadDeletetionCount >= 0);
            Assert.IsTrue(collection.DataFilesCount >= 0);
            Assert.IsTrue(collection.DataFilesSize >= 0);
            Assert.IsTrue(collection.JournalsCount >= 0);
            Assert.IsTrue(collection.JournalsFileSize >= 0);

            _database.DeleteCollection(newCollection.Name);
        }

        [TestMethod]
        public void CreateCollectionAndGeFiguresByNameAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead002xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            ArangoCollection collection = _database.GetCollectionFigures(newCollection.Name);

            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);
            Assert.AreEqual(collection.Type, newCollection.Type);
            Assert.AreEqual(collection.WaitForSync, newCollection.WaitForSync);
            Assert.AreEqual(collection.JournalSize, newCollection.JournalSize);
            Assert.IsTrue(collection.DocumentsCount >= 0);
            Assert.IsTrue(collection.AliveDocumentsCount >= 0);
            Assert.IsTrue(collection.AliveDocumentsSize >= 0);
            Assert.IsTrue(collection.DeadDocumentsCount >= 0);
            Assert.IsTrue(collection.DeadDocumentsSize >= 0);
            Assert.IsTrue(collection.DeadDeletetionCount >= 0);
            Assert.IsTrue(collection.DataFilesCount >= 0);
            Assert.IsTrue(collection.DataFilesSize >= 0);
            Assert.IsTrue(collection.JournalsCount >= 0);
            Assert.IsTrue(collection.JournalsFileSize >= 0);

            _database.DeleteCollection(newCollection.Name);
        }

        [TestMethod]
        public void CreateCollectionAndGetAllAndDeleteIt()
        {
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionToBeRead001xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            ArangoCollection newCollection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            List<ArangoCollection> collections = _database.GetCollections();

            Assert.IsTrue(collections.Count >= 1);

            ArangoCollection collection = collections.Where(col => col.ID == newCollection.ID).FirstOrDefault();
            Assert.AreEqual(collection.ID, newCollection.ID);
            Assert.AreEqual(collection.Name, newCollection.Name);
            Assert.AreEqual(collection.Status, newCollection.Status);

            _database.DeleteCollection(newCollection.Name);
        }

        #endregion
    }
}
