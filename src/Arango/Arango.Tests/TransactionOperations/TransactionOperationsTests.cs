using System;
using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class TransactionOperationsTests : IDisposable
    {
        public TransactionOperationsTests()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
			Database.CreateTestCollection(Database.TestDocumentCollectionName, ACollectionType.Document);
        }
        
        [Test()]
        public void Should_execute_transaction_and_return_int_value()
        {
            Database.ClearTestCollection(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var transactionResult = db.Transaction
                .WriteCollection(Database.TestDocumentCollectionName)
                .Execute<int>(@"
                function () { 
                    var db = require('internal').db; 

                    db." + Database.TestDocumentCollectionName + @".save({ });

                    return db." + Database.TestDocumentCollectionName + @".count(); 
                }
                ");

            Assert.AreEqual(200, transactionResult.StatusCode);
            Assert.IsTrue(transactionResult.Success);
            Assert.IsTrue(transactionResult.HasValue);
            Assert.AreEqual(1, transactionResult.Value);
        }
        
        [Test()]
        public void Should_execute_transaction_and_return_document()
        {
            Database.ClearTestCollection(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var transactionResult = db.Transaction
                .WriteCollection(Database.TestDocumentCollectionName)
                .Execute<Dictionary<string, object>>(@"
                function () { 
                    var db = require('internal').db; 

                    db." + Database.TestDocumentCollectionName + @".save({ });

                    return { 'Executed': true };
                }
                ");

            Assert.AreEqual(200, transactionResult.StatusCode);
            Assert.IsTrue(transactionResult.Success);
            Assert.IsTrue(transactionResult.HasValue);
            Assert.IsTrue(transactionResult.Value.Bool("Executed"));
        }
        
        [Test()]
        public void Should_execute_transaction_and_return_generic_object()
        {
            Database.ClearTestCollection(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var transactionResult = db.Transaction
                .WriteCollection(Database.TestDocumentCollectionName)
                .Execute<TransactionEntity>(@"
                function () { 
                    var db = require('internal').db; 

                    db." + Database.TestDocumentCollectionName + @".save({ });

                    return { 'Executed': true };
                }
                ");

            Assert.AreEqual(200, transactionResult.StatusCode);
            Assert.IsTrue(transactionResult.Success);
            Assert.IsTrue(transactionResult.HasValue);
            Assert.IsTrue(transactionResult.Value.Executed);
        }

        [Test()]
        public void Should_execute_transaction_with_single_value_parameter_and_return_document_ID()
        {
            Database.ClearTestCollection(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var transactionResult = db.Transaction
                .WriteCollection(Database.TestDocumentCollectionName)
                .Param("data", "some string value")
                .Execute<string>(@"
                function (params) { 
                    var db = require('internal').db; 

                    return db." + Database.TestDocumentCollectionName + @".save({ foo: params.data })._id;
                }
                ");

            Assert.AreEqual(200, transactionResult.StatusCode);
            Assert.IsTrue(transactionResult.Success);
            Assert.IsTrue(transactionResult.HasValue);
            Assert.IsFalse(string.IsNullOrEmpty(transactionResult.Value));

            var docId = transactionResult.Value;

            var getResult = db.Document
                .Get(docId);

            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.ID(), docId);
        }

        [Test()]
        public void Should_execute_transaction_with_object_parameter_and_return_document_ID()
        {
            Database.ClearTestCollection(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var transactionData = new TransactionEntity
            {
                Foo = "Some string value"
            };

            var transactionResult = db.Transaction
                .WriteCollection(Database.TestDocumentCollectionName)
                .Param("data", transactionData)
                .Execute<string>(@"
                function (params) { 
                    var db = require('internal').db; 

                    return db." + Database.TestDocumentCollectionName + @".save(params.data)._id;
                }
                ");

            Assert.AreEqual(200, transactionResult.StatusCode);
            Assert.IsTrue(transactionResult.Success);
            Assert.IsTrue(transactionResult.HasValue);
            Assert.IsFalse(string.IsNullOrEmpty(transactionResult.Value));

            var docId = transactionResult.Value;

            var getResult = db.Document
                .Get(docId);

            Assert.AreEqual(200, getResult.StatusCode);
            Assert.IsTrue(getResult.Success);
            Assert.IsTrue(getResult.HasValue);
            Assert.AreEqual(getResult.Value.ID(), docId);
            Assert.AreEqual(transactionData.Foo, getResult.Value.String("Foo"));
        }

        [Test()]
        public void Should_execute_transaction_with_list_parameter_and_return_document_IDs()
        {
            Database.ClearTestCollection(Database.TestDocumentCollectionName);
            var db = new ADatabase(Database.Alias);

            var transactionData = new List<TransactionEntity>
            {
                new TransactionEntity
                {
                    Foo = "string1"
                },
                new TransactionEntity
                {
                    Foo = "string2"
                },
                new TransactionEntity
                {
                    Foo = "string3"
                }
            };

            var transactionResult = db.Transaction
                .WriteCollection(Database.TestDocumentCollectionName)
                .Param("data", transactionData)
                .Execute<List<TransactionEntity>>(@"
                function (params) { 
                    var db = require('internal').db;

                    for (var i = 0; i < params.data.length; i++) {
                        db." + Database.TestDocumentCollectionName + @".save(params.data[i]);
                    }

                    return db._query('FOR doc IN " + Database.TestDocumentCollectionName + @" SORT TO_NUMBER(doc._key) RETURN doc').toArray();
                }
                ");

            Assert.AreEqual(200, transactionResult.StatusCode);
            Assert.IsTrue(transactionResult.Success);
            Assert.IsTrue(transactionResult.HasValue);
            Assert.AreEqual(3, transactionResult.Value.Count);

            for (int i = 0; i < transactionResult.Value.Count; i++)
            {
                Assert.AreEqual(transactionData[i].Foo, transactionResult.Value[i].Foo);
            }
        }

        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
