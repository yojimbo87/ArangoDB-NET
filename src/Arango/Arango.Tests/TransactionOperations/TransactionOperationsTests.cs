using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
