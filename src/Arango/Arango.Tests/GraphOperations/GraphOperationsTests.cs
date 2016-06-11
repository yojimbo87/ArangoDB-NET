using System;
using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.GraphOperations
{
    [TestFixture()]
    public class GraphOperationsTests : IDisposable
    {
        private const string _collectionName = "testGraphCollection001";
        private const string _graphName = "testGraph001";
        private List<string> _fromList = new List<string> { "fromFoo" };
        private List<string> _toList = new List<string> { "toBar" };

        public GraphOperationsTests()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
        }

        #region Create operations

        [Test()]
        public void Should_create_graph()
        {
            // given
            var db = new ADatabase(Database.Alias);

            // when
            var createResult = db.Graph
                .AddEdgeDefinition(_collectionName, _fromList, _toList)
                .Create(_graphName);

            // then
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(ADocument.IsID(createResult.Value.String("_id")));
            Assert.IsTrue(ADocument.IsRev(createResult.Value.String("_rev")));
            Assert.AreEqual(_graphName, createResult.Value.String("name"));
            Assert.AreEqual(1, createResult.Value.Size("edgeDefinitions"));
            Assert.AreEqual(_collectionName, createResult.Value.String("edgeDefinitions[0].collection"));
            Assert.AreEqual(_fromList.Count, createResult.Value.Size("edgeDefinitions[0].from"));
            Assert.AreEqual(_fromList[0], createResult.Value.String("edgeDefinitions[0].from[0]"));
            Assert.AreEqual(_toList.Count, createResult.Value.Size("edgeDefinitions[0].to"));
            Assert.AreEqual(_toList[0], createResult.Value.String("edgeDefinitions[0].to[0]"));
            Assert.AreEqual(0, createResult.Value.Size("orphanCollections"));

            ClearGraph(_graphName);
        }

        #endregion

        #region Get operations

        [Test()]
        public void Should_get_all_graphs()
        {
            // given
            var db = new ADatabase(Database.Alias);

            // when
            var getResult = db.Graph
                .GetAllGraphs();

            // then
            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
        }

        #endregion

        #region Delete operations

        [Test()]
        public void Should_delete_graph()
        {
            // given
            var db = new ADatabase(Database.Alias);

            var createResult = db.Graph
                .AddEdgeDefinition(_collectionName, _fromList, _toList)
                .Create(_graphName);

            Assert.AreEqual(201, createResult.StatusCode);

            // when
            var deleteResult = db.Graph
                .Delete(_graphName);

            // then
            Assert.AreEqual(200, deleteResult.StatusCode);
            Assert.IsTrue(deleteResult.Success);
            Assert.IsTrue(deleteResult.HasValue);
            Assert.IsTrue(deleteResult.Value);
        }

        #endregion

        private void ClearGraph(string graphName)
        {

        }

        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
