using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arango.Client;

namespace Arango.Test
{
    [TestClass]
    public class CursorTests : IDisposable
    {
        private ArangoDatabase _database;
        private ArangoCollection _collection;

        public CursorTests()
        {
            string alias = "test";
            string testCollectionName = "tempUnitTestCollectionForDocumentTests001xyz";
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

            // delete test collection if it already exist
            /*ArangoCollection col = _database.GetCollection(testCollectionName);

            if ((col != null) && !string.IsNullOrEmpty(col.Name))
            {
                _database.DeleteCollection(testCollectionName);
            }*/

            // create test collection
            ArangoCollection testCollection = new ArangoCollection();
            testCollection.Name = testCollectionName;
            testCollection.Type = ArangoCollectionType.Document;
            testCollection.WaitForSync = false;
            testCollection.JournalSize = 1024 * 1024; // 1 MB

            _collection = _database.CreateCollection(testCollection.Name, testCollection.Type, testCollection.WaitForSync, testCollection.JournalSize);

            Json doc = new Json();
            doc.Set("Name", "Johny");
            doc.Set("Surname", "Bravo");
            doc.Set("Age", 18);
            _database.CreateDocument(_collection.ID, doc, false);

            doc = new Json();
            doc.Set("Name", "Frankie");
            doc.Set("Surname", "Hoover");
            doc.Set("Age", 25);
            _database.CreateDocument(_collection.ID, doc, false);

            doc = new Json();
            doc.Set("Name", "Maria");
            doc.Set("Surname", "Cool");
            doc.Set("Age", 32);
            _database.CreateDocument(_collection.ID, doc, false);

            doc = new Json();
            doc.Set("Name", "Mario");
            doc.Set("Surname", "Bro");
            doc.Set("Age", 43);
            _database.CreateDocument(_collection.ID, doc, false);
        }

        [TestMethod]
        public void BacisCursorQuery()
        {
            List<ArangoDocument> documents = _database.Query("FOR u IN " + _collection.Name + " RETURN u");

            Assert.AreEqual(documents.Count, 4);
        }

        public void Dispose()
        {
            _database.DeleteCollection(_collection.ID);
        }
    }
}
