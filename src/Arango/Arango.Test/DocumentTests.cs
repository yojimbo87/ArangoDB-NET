using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arango.Client;

namespace Arango.Test
{
    [TestClass]
    public class DocumentTests
    {
        private ArangoDatabase _database;

        public DocumentTests()
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

        #region Get

        [TestMethod]
        public void GetDocumentByHandle()
        {
        }

        [TestMethod]
        public void GetDocumentByHandleAndRevision()
        {
        }

        #endregion
    }
}
