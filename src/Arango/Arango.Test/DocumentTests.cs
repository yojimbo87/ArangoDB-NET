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

        #region Create, get all

        [TestMethod]
        public void CreateDocument_AND_GetAllDocumentsByCollectionID()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document1 = _database.CreateDocument(_collection.ID, jsonObject, false);
            ArangoDocument document2 = _database.CreateDocument(_collection.ID, jsonObject, false);

            List<ArangoDocument> documents = _database.GetAllDocuments(_collection.ID);
            Assert.IsTrue(documents.Count == 2);
            Assert.AreEqual(documents.Where(doc => doc.ID == document1.ID).First().ID, document1.ID);
            Assert.AreEqual(documents.Where(doc => doc.ID == document2.ID).First().ID, document2.ID);
        }

        [TestMethod]
        public void CreateDocument_AND_GetAllDocumentsByCollectionName()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document1 = _database.CreateDocument(_collection.ID, jsonObject, false);
            ArangoDocument document2 = _database.CreateDocument(_collection.ID, jsonObject, false);

            List<ArangoDocument> documents = _database.GetAllDocuments(_collection.Name);
            Assert.IsTrue(documents.Count == 2);
            Assert.AreEqual(documents.Where(doc => doc.ID == document1.ID).First().ID, document1.ID);
            Assert.AreEqual(documents.Where(doc => doc.ID == document2.ID).First().ID, document2.ID);
        }

        #endregion

        #region Create, replace

        [TestMethod]
        public void CreateDocument_AND_ReplaceWithDefaultPolicy()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;
            jsonObject.baz = "test";

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            dynamic newJsonObject = new ExpandoObject();
            newJsonObject.Foo = "Johny";
            newJsonObject.Bar = 54321;

            string revision = _database.ReplaceDocument(document.ID, document.Revision, DocumentUpdatePolicy.Default, newJsonObject, false);
            Assert.AreNotEqual(revision, document.Revision);

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.Revision, revision);
            Assert.AreEqual(loadedDocument.JsonObject.Foo, newJsonObject.Foo);
            Assert.AreEqual(loadedDocument.JsonObject.Bar, newJsonObject.Bar);
        }

        #endregion

        #region Create, update

        [TestMethod]
        public void CreateDocument_AND_UpdateWithKeepNullValues()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            dynamic update = new ExpandoObject();
            update.baz = "new field";

            string revision = _database.UpdateDocument(document.ID, document.Revision, DocumentUpdatePolicy.Default, update, true, false);
            Assert.AreNotEqual(revision, document.Revision);

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.Revision, revision);
            Assert.AreEqual(loadedDocument.JsonObject.foo, jsonObject.foo);
            Assert.AreEqual(loadedDocument.JsonObject.Bar, jsonObject.Bar);
            Assert.AreEqual(loadedDocument.JsonObject.baz, update.baz);
        }

        [TestMethod]
        public void CreateDocument_AND_UpdateWithoutKeepNullValues()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            dynamic update = new ExpandoObject();
            update.Bar = null;
            update.baz = "new field";

            string revision = _database.UpdateDocument(document.ID, document.Revision, DocumentUpdatePolicy.Default, update, false, false);
            Assert.AreNotEqual(revision, document.Revision);

            ArangoDocument loadedDocument = _database.GetDocument(document.ID);
            Assert.AreEqual(loadedDocument.Revision, revision);
            Assert.AreEqual(loadedDocument.JsonObject.foo, jsonObject.foo);
            Assert.AreEqual(loadedDocument.Has("Bar"), false);
            Assert.AreEqual(loadedDocument.JsonObject.baz, update.baz);
        }

        #endregion

        #region Create, delete

        [TestMethod]
        public void CreateDocument_AND_Delete()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            string deleteDocumentID = _database.DeleteDocument(document.ID, document.Revision, DocumentUpdatePolicy.Default, false);
            Assert.IsTrue(!string.IsNullOrEmpty(deleteDocumentID));
        }

        #endregion

        #region Create, check

        [TestMethod]
        public void CreateDocument_AND_CheckExisting()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument checkedDocument = _database.CheckDocument(document.ID);
            Assert.AreEqual(checkedDocument.ID, document.ID);
            Assert.AreEqual(checkedDocument.Revision, document.Revision);
        }

        [TestMethod]
        public void CreateDocument_AND_CheckNotExisting()
        {
            dynamic jsonObject = new ExpandoObject();
            jsonObject.foo = "bravo";
            jsonObject.Bar = 12345;

            ArangoDocument document = _database.CreateDocument(_collection.ID, jsonObject, false);
            Assert.IsTrue(!string.IsNullOrEmpty(document.ID));
            Assert.IsTrue(!string.IsNullOrEmpty(document.Revision));

            ArangoDocument checkedDocument = _database.CheckDocument("whoa" + document.ID);
            Assert.IsTrue(string.IsNullOrEmpty(checkedDocument.ID));
            Assert.IsTrue(string.IsNullOrEmpty(checkedDocument.Revision));
        }

        #endregion

        public void Dispose()
        {
            _database.DeleteCollection(_collection.ID);
        }
    }
}
