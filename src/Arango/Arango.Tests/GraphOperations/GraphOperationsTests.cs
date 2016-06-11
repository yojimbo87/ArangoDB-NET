using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.GraphOperations
{
    [TestFixture()]
    public class GraphOperationsTests : IDisposable
    {
        public GraphOperationsTests()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
        }

        #region Create operations

        [Test()]
        public void Should_create_graph()
        {
            // given
            var collectionName = "testGraphCollection001";
            var fromList = new List<string> { "fromFoo" };
            var toList = new List<string> { "toBar" };
            var graphName = "testGraph001";
            var db = new ADatabase(Database.Alias);

            // when
            var createResult = db.Graph
                .AddEdgeDefinition(collectionName, fromList, toList)
                .Create(graphName);

            // then
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.IsTrue(createResult.Success);
            Assert.IsTrue(createResult.HasValue);
            Assert.IsTrue(ADocument.IsID(createResult.Value.String("_id")));
            Assert.IsTrue(ADocument.IsRev(createResult.Value.String("_rev")));
            Assert.AreEqual(graphName, createResult.Value.String("name"));
            Assert.AreEqual(1, createResult.Value.Size("edgeDefinitions"));
            Assert.AreEqual(collectionName, createResult.Value.String("edgeDefinitions[0].collection"));
            Assert.AreEqual(fromList.Count, createResult.Value.Size("edgeDefinitions[0].from"));
            Assert.AreEqual(fromList[0], createResult.Value.String("edgeDefinitions[0].from[0]"));
            Assert.AreEqual(toList.Count, createResult.Value.Size("edgeDefinitions[0].to"));
            Assert.AreEqual(toList[0], createResult.Value.String("edgeDefinitions[0].to[0]"));
            Assert.AreEqual(0, createResult.Value.Size("orphanCollections"));
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

        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
