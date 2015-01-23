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
        public QueryOperationsTests()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
			Database.CreateTestCollection(Database.TestDocumentCollectionName, ACollectionType.Document);
        }
        
        #region ToDocument(s)
        
        [Test()]
        public void Should_execute_AQL_query_with_document_result()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    LIMIT 1
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToDocument();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.IsTrue(queryResult.Value.IsString("foo"));
            Assert.IsTrue(queryResult.Value.IsLong("bar"));
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_document_list_result()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToDocuments();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(2, queryResult.Value.Count);
            Assert.IsTrue(queryResult.Value[0].IsString("foo"));
            Assert.IsTrue(queryResult.Value[0].IsLong("bar"));
            Assert.IsTrue(queryResult.Value[1].IsString("foo"));
            Assert.IsTrue(queryResult.Value[1].IsLong("bar"));
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_empty_document_result()
        {
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(@"
                LET items = []
                FOR item IN items
                    RETURN item
                ")
                .ToDocument();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(0, queryResult.Value.Count);
        }
        
        #endregion
        
        #region ToObject
        
        [Test()]
        public void Should_execute_AQL_query_with_single_object_result()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    LIMIT 1
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToObject();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.IsTrue(queryResult.Value != null);
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_single_primitive_object_result()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    SORT item.bar
                    LIMIT 1
                    RETURN item.bar
                ", Database.TestDocumentCollectionName))
                .ToObject<int>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(1, queryResult.Value);
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_single_dictionary_object_result()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    SORT item.bar
                    LIMIT 1
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToObject<Dictionary<string, object>>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.IsTrue(queryResult.Value.IsString("foo"));
            Assert.IsTrue(queryResult.Value.IsLong("bar"));
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_single_strongly_typed_object_result()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    SORT item.bar
                    LIMIT 1
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToObject<Dummy>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.IsTrue(documents.First(q => q.String("foo") == queryResult.Value.Foo) != null);
            Assert.IsTrue(documents.First(q => q.Int("bar") == queryResult.Value.Bar) != null);
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_empty_single_object_result()
        {
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(@"
                LET items = []
                FOR item IN items
                    RETURN item
                ")
                .ToObject();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.IsTrue(queryResult.Value != null);
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_empty_single_strongly_typed_object_result()
        {
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(@"
                LET items = []
                FOR item IN items
                    RETURN item
                ")
                .ToObject<Dummy>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.IsTrue(queryResult.Value != null);
            Assert.AreEqual(null, queryResult.Value.Foo);
            Assert.AreEqual(0, queryResult.Value.Bar);
        }
        
        #endregion
        
        #region ToList
        
        [Test()]
        public void Should_execute_AQL_query_with_list_result()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToList();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(2, queryResult.Value.Count);
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_primitive_list_result()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    SORT item.bar
                    RETURN item.bar
                ", Database.TestDocumentCollectionName))
                .ToList<int>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(2, queryResult.Value.Count);
            Assert.AreEqual(1, queryResult.Value[0]);
            Assert.AreEqual(2, queryResult.Value[1]);
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_dictionary_list_result()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToList<Dictionary<string, object>>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(2, queryResult.Value.Count);
            Assert.IsTrue(queryResult.Value[0].IsString("foo"));
            Assert.IsTrue(queryResult.Value[0].IsLong("bar"));
            Assert.IsTrue(queryResult.Value[1].IsString("foo"));
            Assert.IsTrue(queryResult.Value[1].IsLong("bar"));
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_strongly_typed_list_result()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    SORT item.bar
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToList<Dummy>();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(2, queryResult.Value.Count);
            Assert.AreEqual(documents[0].String("foo"), queryResult.Value[0].Foo);
            Assert.AreEqual(documents[0].Int("bar"), queryResult.Value[0].Bar);
            Assert.AreEqual(documents[1].String("foo"), queryResult.Value[1].Foo);
            Assert.AreEqual(documents[1].Int("bar"), queryResult.Value[1].Bar);
        }
        
        #endregion
        
        [Test()]
        public void Should_execute_AQL_query_with_count()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .Count(true)
                .Aql(string.Format(@"
                FOR item IN {0}
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToList();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(queryResult.Value.Count, 2);
            Assert.AreEqual(queryResult.Extra.Long("count"), 2);
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_batchSize()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

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
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(queryResult.Value.Count, 4);
        }
        
        [Test()]
        public void Should_execute_AQL_query_with_bindVar()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .BindVar("barNumber", 1)
                .Aql(string.Format(@"
                FOR item IN {0}
                    FILTER item.bar == @barNumber
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToList();

            Assert.AreEqual(201, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(queryResult.Value.Count, 1);
        }
        
        [Test()]
        public void Should_execute_AQL_query_fluent()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);
            var useCount = true;
            var useBatchSize = true;
            
            var queryOperation = db.Query
                .Aql(string.Format(@"
                FOR item IN {0}
                    RETURN item
                ", Database.TestDocumentCollectionName));
            
            if (useCount)
            {
                queryOperation.Count(true);
            }
            
            if (useBatchSize)
            {
                queryOperation.BatchSize(1);
            }
            
            var queryResult = queryOperation.ToList();

            Assert.AreEqual(200, queryResult.StatusCode);
            Assert.IsTrue(queryResult.Success);
            Assert.IsTrue(queryResult.HasValue);
            Assert.AreEqual(queryResult.Value.Count, 2);
            Assert.AreEqual(queryResult.Extra.Long("count"), 2);
        }
        
        [Test()]
        public void Should_parse_query()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var parseResult = db.Query
                .Parse(string.Format(@"
                FOR item IN {0}
                    RETURN item
                ", Database.TestDocumentCollectionName));
            
            Assert.AreEqual(200, parseResult.StatusCode);
            Assert.IsTrue(parseResult.Success);
            Assert.IsTrue(parseResult.HasValue);
            Assert.IsTrue(parseResult.Value.IsList("bindVars"));
            Assert.IsTrue(parseResult.Value.IsList("collections"));
            Assert.IsTrue(parseResult.Value.IsList("ast"));
        }
        
        [Test()]
        public void Should_minify_query()
        {
            var singleLineQuery = AQuery.Minify(@"
            FOR item IN MyDocumentCollection
                RETURN item
            ");
            
            Assert.AreEqual("FOR item IN MyDocumentCollection\nRETURN item\n", singleLineQuery);
        }
        
        [Test()]
        public void Should_return_404_with_deleteCursor_operation()
        {
            var documents = Database.ClearCollectionAndFetchTestDocumentData(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var queryResult = db.Query
                .BatchSize(1)
                .Aql(string.Format(@"
                FOR item IN {0}
                    RETURN item
                ", Database.TestDocumentCollectionName))
                .ToList();

            Assert.IsTrue(queryResult.Extra.IsString("id"));
            
            var deleteCursorResult = db.Query
                .DeleteCursor(queryResult.Extra.String("id"));
            
            Assert.AreEqual(404, deleteCursorResult.StatusCode);
            Assert.IsFalse(deleteCursorResult.Success);
            Assert.IsTrue(deleteCursorResult.HasValue);
            Assert.IsFalse(deleteCursorResult.Value);
        }
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
