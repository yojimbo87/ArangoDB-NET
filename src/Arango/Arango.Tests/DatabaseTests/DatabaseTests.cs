using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.DatabaseTests
{
    [TestFixture()]
    public class DatabaseTests : IDisposable
    {
        [Test()]
        public void Should_create_database_and_return_list_of_all_databases()
        {
            var created = ArangoClient.CreateDatabase(
                Database.Hostname,
                Database.Port,
                Database.IsSecured,
                Database.TestDatabaseOneTime,
                Database.UserName,
                Database.Password
            );
            
            Assert.AreEqual(true, created);
            
            var databases = ArangoClient.ListDatabases(
                Database.Hostname,
                Database.Port,
                Database.IsSecured,
                Database.UserName,
                Database.Password
            );
            
            Assert.AreEqual(true, databases.Contains(Database.TestDatabaseOneTime));
        }
        
        [Test()]
        public void Should_create_and_delete_database()
        {
            var created = ArangoClient.CreateDatabase(
                Database.Hostname,
                Database.Port,
                Database.IsSecured,
                Database.TestDatabaseGeneral,
                Database.UserName,
                Database.Password
            );
            
            Assert.AreEqual(true, created);
            
            var deleted = ArangoClient.DeleteDatabase(
                Database.Hostname,
                Database.Port,
                Database.IsSecured,
                Database.TestDatabaseGeneral,
                Database.UserName,
                Database.Password
            );
            
            Assert.AreEqual(true, deleted);
        }
        
        public void Dispose()
        {
            var databases = ArangoClient.ListDatabases(
                Database.Hostname,
                Database.Port,
                Database.IsSecured,
                Database.UserName,
                Database.Password
            );
            
            if (databases.Contains(Database.TestDatabaseOneTime))
            {
                ArangoClient.DeleteDatabase(
                    Database.Hostname,
                    Database.Port,
                    Database.IsSecured,
                    Database.TestDatabaseOneTime,
                    Database.UserName,
                    Database.Password
                );
            }
            
            if (databases.Contains(Database.TestDatabaseGeneral))
            {
                ArangoClient.DeleteDatabase(
                    Database.Hostname,
                    Database.Port,
                    Database.IsSecured,
                    Database.TestDatabaseGeneral,
                    Database.UserName,
                    Database.Password
                );
            }
        }
    }
}
