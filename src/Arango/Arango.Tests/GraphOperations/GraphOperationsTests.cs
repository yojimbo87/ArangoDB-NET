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

        #region Get operations

        [Test()]
        public void Should_get_all_graphs()
        {
            var db = new ADatabase(Database.Alias);

            var getResult = db.Graph
                .GetAllGraphs();

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
