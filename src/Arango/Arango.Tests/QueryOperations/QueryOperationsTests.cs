using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class QueryOperationsTests : IDisposable
    {
        readonly List<Dictionary<string, object>> _documents;
        
        public QueryOperationsTests()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
			Database.CreateTestCollection(Database.TestDocumentCollectionName, ArangoCollectionType.Document);
			
			_documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_list_result()
        {
            var db = new ArangoDatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToList();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.AreEqual(queryResult.Value.Count, 2);
        }
        
        // TODO: ArangoResult must have field for additional data object
        [Test()]
        public void Should_execute_AQL_query_with_list_count()
        {
            var db = new ArangoDatabase(Database.Alias);

            var queryResult = db.Query
                .Count(true)
                .Aql(string.Format(@"
                FOR item IN {0}
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToList();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.AreEqual(queryResult.Value.Count, 2);
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_batchSize()
        {
            var db = new ArangoDatabase(Database.Alias);

            var doc3 = new Dictionary<string, object>()
                .String("foo", "foo string 3");
            
            db.Document
                .Create(Database.TestDocumentCollectionName, doc3);
            
            var doc4 = new Dictionary<string, object>()
                .String("foo", "foo string 4");
            
            db.Document
                .Create(Database.TestDocumentCollectionName, doc4);
            
            var queryResult = db.Query
                .BatchSize(1)
                .Aql(string.Format(@"
                FOR item IN {0}
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToList();

            Assert.AreEqual(200, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.AreEqual(queryResult.Value.Count, 4);
        }
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
