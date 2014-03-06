﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.ArangoCollectionTests
{
    [TestFixture()]
    public class ArangoCollectionTests : IDisposable
    {
        public ArangoCollectionTests()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
        }
        
        [Test()]
        public void Should_create_and_delete_collection()
        {
            Database.DeleteTestCollection(Database.TestEdgeCollectionName);
            
            ArangoDatabase db = Database.GetTestDatabase();
            
            // set collection data
            ArangoCollection collection = new ArangoCollection();
            collection.Name = Database.TestEdgeCollectionName;
            collection.Type = ArangoCollectionType.Edge;
            collection.WaitForSync = true;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // check collection data retrieved from server
            Assert.AreEqual(false, string.IsNullOrEmpty(collection.Id));
            Assert.AreEqual(Database.TestEdgeCollectionName, collection.Name);
            Assert.AreEqual(ArangoCollectionType.Edge, collection.Type);
            Assert.AreEqual(ArangoCollectionStatus.Loaded, collection.Status);
            Assert.AreEqual(true, collection.WaitForSync);
            Assert.AreEqual(false, collection.IsSystem);
            Assert.AreEqual(false, collection.IsVolatile);
            
            // delete collection from database
            bool isDeleted = db.Collection.Delete(collection.Name);
            
            // check if the collection was deleted from database
            Assert.AreEqual(true, isDeleted);
        }
        
        [Test()]
        public void Should_create_and_get_and_delete_collection()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            
            ArangoDatabase db = Database.GetTestDatabase();
            
            // set collection data
            ArangoCollection collection = new ArangoCollection();
            collection.Name = Database.TestDocumentCollectionName;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // get collection from database
            ArangoCollection returnedCollection = db.Collection.Get(Database.TestDocumentCollectionName);
            
            // check collection data retrieved from server
            Assert.AreEqual(collection.Id, returnedCollection.Id);
            Assert.AreEqual(collection.Name, returnedCollection.Name);
            Assert.AreEqual(collection.Type, returnedCollection.Type);
            Assert.AreEqual(collection.Status, returnedCollection.Status);
            
            // delete collection from database
            bool isDeleted = db.Collection.Delete(collection.Name);
            
            // check if the collection was deleted from database
            Assert.AreEqual(true, isDeleted);
        }

        [Test()]
        public void Should_create_and_clear_and_delete_collection()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            
            ArangoDatabase db = Database.GetTestDatabase();
            
            // set collection data
            ArangoCollection collection = new ArangoCollection();
            collection.Name = Database.TestDocumentCollectionName;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // clear collection data
            var isCleared = db.Collection.Clear(Database.TestDocumentCollectionName);
            Assert.AreEqual(true, isCleared);
            
            // delete collection from database
            bool isDeleted = db.Collection.Delete(collection.Name);
            
            // check if the collection was deleted from database
            Assert.AreEqual(true, isDeleted);
        }
        
        [Test()]
        public void Should_create_autoincrement_collection()
        {            
            var db = Database.GetTestDatabase();
            
            if (db.Server.Role().IsCluster())
            {
            	// do not execute this test on a coordinator
            	return;
            }
        	
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            
            // set collection data
            var collection = new ArangoCollection();
            collection.Name = Database.TestDocumentCollectionName;
            collection.KeyOptions = new ArangoCollectionKeyOptions();
            collection.KeyOptions.GeneratorType = ArangoKeyGeneratorType.Autoincrement;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // create document
            var document1 = new Document()
                .String("foo", "foo string value");

            db.Document.Create(Database.TestDocumentCollectionName, document1);
            
            // check if the created document key starts with number 1
            Assert.AreEqual("1", document1.String("_key"));
            
            // create another document
            var document2 = new Document()
                .String("foo", "foo string value");
            
            db.Document.Create(Database.TestDocumentCollectionName, document2);
            
            // check if the created document key is autoincremented to 2
            Assert.AreEqual("2", document2.String("_key"));
        }
        
        [Test()]
        public void Should_create_volatile_collection()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            
            var db = Database.GetTestDatabase();
            
            // set collection data
            var collection = new ArangoCollection();
            collection.Name = Database.TestDocumentCollectionName;
            collection.IsVolatile = true;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // get collection from database
            ArangoCollection returnedCollection = db.Collection.Properties(Database.TestDocumentCollectionName);
            
            // get collection properties from database
            Assert.AreEqual(collection.Id, returnedCollection.Id);
            Assert.AreEqual(collection.Name, returnedCollection.Name);            
            Assert.AreEqual(false, returnedCollection.WaitForSync);
            Assert.AreEqual(true, returnedCollection.IsVolatile);
        }          

        [Test()]
        public void Should_create_synchronous_collection()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            
            var db = Database.GetTestDatabase();
            
            // set collection data
            var collection = new ArangoCollection();
            collection.Name = Database.TestDocumentCollectionName;
            collection.WaitForSync = true;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // get collection from database
            ArangoCollection returnedCollection = db.Collection.Properties(Database.TestDocumentCollectionName);
            
            // get collection properties from database
            Assert.AreEqual(collection.Id, returnedCollection.Id);
            Assert.AreEqual(collection.Name, returnedCollection.Name);            
            Assert.AreEqual(true, returnedCollection.WaitForSync);
            Assert.AreEqual(false, returnedCollection.IsVolatile);
        }
        
        [Test()]
        public void Should_create_sharded_collection()
        {            
            var db = Database.GetTestDatabase();
            
            if (! db.Server.Role().IsCluster())
            {
            	// do not execute this test in non-cluster mode
            	return;
            }

            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            	
            // set collection data
            var collection = new ArangoCollection();
            collection.Name = Database.TestDocumentCollectionName;
            collection.NumberOfShards = 5;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // get collection from database
            ArangoCollection returnedCollection = db.Collection.Properties(Database.TestDocumentCollectionName);
            
            // get collection properties from database
            Assert.AreEqual(collection.Id, returnedCollection.Id);
            Assert.AreEqual(collection.Name, returnedCollection.Name);            
            Assert.AreEqual(false, returnedCollection.IsVolatile);
            Assert.AreEqual(false, returnedCollection.IsSystem);
            Assert.AreEqual(5, returnedCollection.NumberOfShards);
        }        
        
        
        [Test()]
        public void Should_create_collection_with_shard_keys()
        {            
            var db = Database.GetTestDatabase();
            
            if (! db.Server.Role().IsCluster())
            {
            	// do not execute this test in non-cluster mode
            	return;
            }

            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            	
            // set collection data
            var collection = new ArangoCollection();
            collection.Name = Database.TestDocumentCollectionName;
            collection.NumberOfShards = 2;
            List<string> shardKeys = new List<string>();
            shardKeys.Add("a");
            shardKeys.Add("b");
            collection.ShardKeys = shardKeys;
            
            // create collection in database
            db.Collection.Create(collection);
            
            // get collection from database
            ArangoCollection returnedCollection = db.Collection.Properties(Database.TestDocumentCollectionName);
            
            // get collection properties from database
            Assert.AreEqual(collection.Id, returnedCollection.Id);
            Assert.AreEqual(collection.Name, returnedCollection.Name);            
            Assert.AreEqual(false, returnedCollection.IsVolatile);
            Assert.AreEqual(false, returnedCollection.IsSystem);
            Assert.AreEqual(2, returnedCollection.NumberOfShards);
            Assert.AreEqual(shardKeys, returnedCollection.ShardKeys);
        }                
        
        public void Dispose()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
