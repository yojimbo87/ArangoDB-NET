using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arango.Client;

namespace Arango.Test
{
    [TestClass]
    public class DocumentTests : IDisposable
    {
        private ArangoDatabase _database;
        private ArangoCollection _collection;

        public DocumentTests()
        {
            string alias = "test";
            string[] connectionString = File.ReadAllText(@"..\..\..\..\..\ConnectionString.txt").Split(';');

            // create aliased arango node
            ArangoNode node = new ArangoNode(
                connectionString[0],
                int.Parse(connectionString[1]),
                connectionString[2],
                connectionString[3],
                alias
            );
            ArangoClient.Nodes.Add(node);

            _database = new ArangoDatabase(alias);

            // create test collection
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = "tempUnitTestCollectionForDocumentTests001xyz";
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            _collection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);
        }

        #region Create, get

        [TestMethod]
        public void CreateDocumentUsingCollectionID_AND_GetDocumentByID()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.ID, document.ID);
            Assert.AreEqual(loadedDocument.Revision, document.Revision);
            Assert.AreEqual(loadedDocument.JsonObject.foo, jsonObject.foo);
            Assert.AreEqual(loadedDocument.JsonObject.Bar, jsonObject.Bar);
        }

        [TestMethod]
        public void CreateDocumentUsingCollectionID_AND_GetDocumentByIDAndOutdatedRevision()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument loadedDocument = _database.GetDocument(document.ID, "whoa");
            Assert.AreEqual(loadedDocument.ID, document.ID);
            Assert.AreEqual(loadedDocument.Revision, document.Revision);
            Assert.AreEqual(loadedDocument.JsonObject.foo, jsonObject.foo);
            Assert.AreEqual(loadedDocument.JsonObject.Bar, jsonObject.Bar);
        }

        [TestMethod]
        public void CreateDocumentUsingCollectionID_AND_GetDocumentByIDAndActualRevision()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument loadedDocument = _database.GetDocument(document.ID, document.Revision);
            Assert.IsTrue(string.IsNullOrEmpty(loadedDocument.ID));
            Assert.AreEqual(loadedDocument.Revision, document.Revision);
        }

        [TestMethod]
        public void CreateDocumentUsingCollectionNameWithoutCreateProcess_AND_GetDocumentByID()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document = _database.CreateDocument(_collection.Name, false, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.ID, document.ID);
            Assert.AreEqual(loadedDocument.Revision, document.Revision);
            Assert.AreEqual(loadedDocument.JsonObject.foo, jsonObject.foo);
            Assert.AreEqual(loadedDocument.JsonObject.Bar, jsonObject.Bar);
        }

        [TestMethod]
        public void CreateDocumentUsingCollectionNameWithCreateProcess_AND_GetDocumentByID()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            string tempCollection = "tempUnitTestCollectionForDocumentTests002xyz";

            ArangoDocument document = _database.CreateDocument(tempCollection, true, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoCollection collection = _database.GetCollection(tempCollection);
            Assert.AreEqual(collection.Name, tempCollection);

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.ID, document.ID);
            Assert.AreEqual(loadedDocument.Revision, document.Revision);
            Assert.AreEqual(loadedDocument.JsonObject.foo, jsonObject.foo);
            Assert.AreEqual(loadedDocument.JsonObject.Bar, jsonObject.Bar);

            _database.DeleteCollection(tempCollection);
        }

        #endregion

        public void Dispose()
        {
            _database.DeleteCollection(_collection.ID);
        }
    }
}
