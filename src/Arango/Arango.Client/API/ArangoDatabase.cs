using System.Collections.Generic;
using System.Net;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoDatabase
    {
        #region Properties

        private ArangoNode _node;

        #endregion

        /// <summary>
        /// Creates instance of database object which provides public API to work with ArangoDB node.
        /// </summary>
        /// <param name="alias">Alias which was given to previously specified database connection.</param>
        public ArangoDatabase(string alias)
        {
            _node = ArangoClient.GetNode(alias);
        }

        #region Collection

        #region Create

        /// <summary>
        /// Creates new collection with given parameters.
        /// </summary>
        /// <param name="name">Name of the collection.</param>
        /// <param name="type">Type of the collection.</param>
        /// <param name="waitForSync">Determines if creating or changing a document will wait until the data has been synchronised to disk.</param>
        /// <param name="journalSize">Maximum size of a journal or datafile in bytes which also limits the maximum size of a single object. Must be at least 1MB.</param>
        /// <returns>Newly created ArangoCollection object.</returns>
        public ArangoCollection CreateCollection(string name, ArangoCollectionType type, bool waitForSync, long journalSize)
        {
            var collection = new Collection(_node);

            return collection.Post(name, type, waitForSync, journalSize);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes specified collection.
        /// </summary>
        /// <param name="id">ID of collection to be deleted.</param>
        /// <returns>ID of deleted collection.</returns>
        public long DeleteCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.Delete(id);
        }

        /// <summary>
        /// Deletes specified collection.
        /// </summary>
        /// <param name="name">Name of collection to be deleted.</param>
        /// <returns>ID of deleted collection.</returns>
        public long DeleteCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.Delete(name);
        }

        #endregion

        #region Get

        /// <summary>
        /// Retrieves specified collection with ID, Name, Status and Type properties. Collection will not be loaded into memory.
        /// </summary>
        /// <param name="id">ID of collection to be retrieved.</param>
        /// <returns>Arango collection object with ID, Name, Status and Type properties.</returns>
        public ArangoCollection GetCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.Get(id);
        }

        /// <summary>
        /// Retrieves specified collection with ID, Name, Status and Type properties. Collection will not be loaded into memory.
        /// </summary>
        /// <param name="name">Name of collection to be retrieved.</param>
        /// <returns>Arango collection object with ID, Name, Status and Type properties.</returns>
        public ArangoCollection GetCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.Get(name);
        }

        /// <summary>
        /// Retrieves specified collection with ID, Name, Status, Type, WaitForSync and JournalSize properties. Collection will be loaded into memory.
        /// </summary>
        /// <param name="id">ID of collection to be retrieved.</param>
        /// <returns>Arango collection object with ID, Name, Status, Type, WaitForSync and JournalSize properties.</returns>
        public ArangoCollection GetCollectionProperties(long id)
        {
            var collection = new Collection(_node);

            return collection.GetProperties(id);
        }

        /// <summary>
        /// Retrieves specified collection with ID, Name, Status, Type, WaitForSync and JournalSize properties. Collection will not be loaded into memory.
        /// </summary>
        /// <param name="name">Name of collection to be retrieved.</param>
        /// <returns>Arango collection object with ID, Name, Status, Type, WaitForSync and JournalSize properties.</returns>
        public ArangoCollection GetCollectionProperties(string name)
        {
            var collection = new Collection(_node);

            return collection.GetProperties(name);
        }

        /// <summary>
        /// Retrieves specified collection with ID, Name, Status, Type, WaitForSync, JournalSize and DocumentsCount properties. Collection will not be loaded into memory.
        /// </summary>
        /// <param name="id">ID of collection to be retrieved.</param>
        /// <returns>Arango collection object with ID, Name, Status, Type, WaitForSync, JournalSize and DocumentsCount properties.</returns>
        public ArangoCollection GetCollectionCount(long id)
        {
            var collection = new Collection(_node);

            return collection.GetCount(id);
        }

        /// <summary>
        /// Retrieves specified collection with ID, Name, Status, Type, WaitForSync, JournalSize and DocumentsCount properties. Collection will not be loaded into memory.
        /// </summary>
        /// <param name="name">Name of collection to be retrieved.</param>
        /// <returns>Arango collection object with ID, Name, Status, Type, WaitForSync, JournalSize and DocumentsCount properties.</returns>
        public ArangoCollection GetCollectionCount(string name)
        {
            var collection = new Collection(_node);

            return collection.GetCount(name);
        }

        /// <summary>
        /// Retrieves specified collection with entire properties. Collection will not be loaded into memory.
        /// </summary>
        /// <param name="id">ID of collection to be retrieved.</param>
        /// <returns>Arango collection object with entire properties.</returns>
        public ArangoCollection GetCollectionFigures(long id)
        {
            var collection = new Collection(_node);

            return collection.GetFigures(id);
        }

        /// <summary>
        /// Retrieves specified collection with entire properties. Collection will not be loaded into memory.
        /// </summary>
        /// <param name="name">Name of collection to be retrieved.</param>
        /// <returns>Arango collection object with entire properties.</returns>
        public ArangoCollection GetCollectionFigures(string name)
        {
            var collection = new Collection(_node);

            return collection.GetFigures(name);
        }

        /// <summary>
        /// Retrieves list of collections where each item consists of ID, Name, Status and Type properties. Collections will not be loaded into memory.
        /// </summary>
        /// <returns>List of Arango collection objects where each item consists of ID, Name, Status and Type properties.</returns>
        public List<ArangoCollection> GetCollections()
        {
            var collection = new Collection(_node);

            return collection.GetAll();
        }

        #endregion

        #region Truncate

        /// <summary>
        /// Removes all documents from the collection, but leaves the indexes intact.
        /// </summary>
        /// <param name="id">ID of collection to be truncated.</param>
        /// <returns>Boolean indicating if the collection was truncated.</returns>
        public bool TruncateCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.PutTruncate(id);
        }

        /// <summary>
        /// Removes all documents from the collection, but leaves the indexes intact.
        /// </summary>
        /// <param name="name">Name of collection to be truncated.</param>
        /// <returns>Boolean indicating if the collection was truncated.</returns>
        public bool TruncateCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.PutTruncate(name);
        }

        #endregion

        #region Load

        /// <summary>
        /// Loads the collection into memory.
        /// </summary>
        /// <param name="id">ID of collection to be loaded.</param>
        /// <returns>Arango collection object with ID, Name, Status, Type and DocumentsCount properties.</returns>
        public ArangoCollection LoadCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.PutLoad(id);
        }

        /// <summary>
        /// Loads the collection into memory.
        /// </summary>
        /// <param name="id">ID of collection to be loaded.</param>
        /// <returns>Arango collection object with ID, Name, Status, Type and DocumentsCount properties.</returns>
        public ArangoCollection LoadCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.PutLoad(name);
        }

        #endregion

        #region Unload

        /// <summary>
        /// Unloads the collection from memory.
        /// </summary>
        /// <param name="id">ID of collection to be unloaded.</param>
        /// <returns>Arango collection object with ID, Name, Status and Type properties.</returns>
        public ArangoCollection UnloadCollection(long id)
        {
            var collection = new Collection(_node);

            return collection.PutUnload(id);
        }

        /// <summary>
        /// Unloads the collection from memory.
        /// </summary>
        /// <param name="name">Name of collection to be unloaded.</param>
        /// <returns>Arango collection object with ID, Name, Status and Type properties.</returns>
        public ArangoCollection UnloadCollection(string name)
        {
            var collection = new Collection(_node);

            return collection.PutUnload(name);
        }

        #endregion

        #region Update properties

        /// <summary>
        /// Updates the collection properties.
        /// </summary>
        /// <param name="id">ID of collection to be updated.</param>
        /// <param name="waitForSync">Determines if creating or changing a document will wait until the data has been synchronised to disk.</param>
        /// <returns>Arango collection object with ID, Name, Status, Type, JournalSize and WaitForSync properties.</returns>
        public ArangoCollection UpdateCollectionProperties(long id, bool waitForSync)
        {
            var collection = new Collection(_node);

            return collection.PutProperties(id, waitForSync);
        }

        /// <summary>
        /// Updates the collection properties.
        /// </summary>
        /// <param name="name">Name of collection to be updated.</param>
        /// <param name="waitForSync">Determines if creating or changing a document will wait until the data has been synchronised to disk.</param>
        /// <returns>Arango collection object with ID, Name, Status, Type, JournalSize and WaitForSync properties.</returns>
        public ArangoCollection UpdateCollectionProperties(string name, bool waitForSync)
        {
            var collection = new Collection(_node);

            return collection.PutProperties(name, waitForSync);
        }

        #endregion

        #region Update name

        /// <summary>
        /// Updates the collection name.
        /// </summary>
        /// <param name="id">ID of collection to be updated.</param>
        /// <param name="newName">New name of the collection.</param>
        /// <returns>Arango collection object with ID, Name, Status adn Type properties.</returns>
        public ArangoCollection UpdateCollectionName(long id, string newName)
        {
            var collection = new Collection(_node);

            return collection.PutRename(id, newName);
        }

        /// <summary>
        /// Updates the collection name.
        /// </summary>
        /// <param name="name">Name of collection to be updated.</param>
        /// <param name="newName">New name of the collection.</param>
        /// <returns>Arango collection object with ID, Name, Status adn Type properties.</returns>
        public ArangoCollection UpdateCollectionName(string name, string newName)
        {
            var collection = new Collection(_node);

            return collection.PutRename(name, newName);
        }

        #endregion

        #endregion

        #region Document

        #region Create

        /// <summary>
        /// Creates new document in specified collection.
        /// </summary>
        /// <param name="collectionID">Identifier of the collection.</param>
        /// <param name="jsonObject">JSON object which will be created in specified collection.</param>
        /// <param name="waitForSync">If true forces synchronisation.</param>
        /// <returns>ArangoDocument object with assigned document ID and revision.</returns>
        public ArangoDocument CreateDocument(long collectionID, dynamic jsonObject, bool waitForSync)
        {
            var document = new Document(_node);

            return document.Post(collectionID, jsonObject, waitForSync);
        }

        /// <summary>
        /// Creates new document in specified collection.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="createCollection">Determines if the collection should be created if it does not exist.</param>
        /// <param name="jsonObject">JSON object which will be created in specified collection.</param>
        /// <param name="waitForSync">If true forces synchronisation.</param>
        /// <returns>ArangoDocument object with assigned handle and revision.</returns>
        public ArangoDocument CreateDocument(string collectionName, bool createCollection, dynamic jsonObject, bool waitForSync)
        {
            var document = new Document(_node);

            return document.Post(collectionName, createCollection, jsonObject, waitForSync);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes specified document.
        /// </summary>
        /// <param name="documentID">Identifier of the document to be deleted.</param>
        /// <param name="revision">Document revision string.</param>
        /// <param name="policy">Document update policy to be used.</param>
        /// <param name="waitForSync">If true forces synchronisation.</param>
        /// <returns>Identified of the document which was deleted.</returns>
        public string DeleteDocument(string documentID, string revision, DocumentUpdatePolicy policy, bool waitForSync)
        {
            var document = new Document(_node);

            return document.Delete(documentID, revision, policy, waitForSync);
        }

        #endregion

        #region Replace

        /// <summary>
        /// Replaces specified document with new data.
        /// </summary>
        /// <param name="documentID">Identifier of the document to be replaced.</param>
        /// <param name="revision">Document revision string.</param>
        /// <param name="policy">Document update policy to be used.</param>
        /// <param name="jsonObject">JSON object which holds new data.</param>
        /// <param name="waitForSync">If true forces synchronisation.</param>
        /// <returns>New revision string of the document.</returns>
        public string ReplaceDocument(string documentID, string revision, DocumentUpdatePolicy policy, dynamic jsonObject, bool waitForSync)
        {
            var document = new Document(_node);

            return document.Put(documentID, revision, policy, jsonObject, waitForSync);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates specified document data.
        /// </summary>
        /// <param name="documentID">Identifier of the document to be updated.</param>
        /// <param name="revision">Document revision string.</param>
        /// <param name="policy">Document update policy to be used.</param>
        /// <param name="jsonObject">JSON object which holds new data.</param>
        /// <param name="keepNullFields">If the intention is to delete existing fields this parameter can be used with a value of false.</param>
        /// <param name="waitForSync">If true forces synchronisation.</param>
        /// <returns>New revision string of the document.</returns>
        public string UpdateDocument(string documentID, string revision, DocumentUpdatePolicy policy, dynamic jsonObject, bool keepNullFields, bool waitForSync)
        {
            var document = new Document(_node);

            return document.Patch(documentID, revision, policy, jsonObject, keepNullFields, waitForSync);
        }

        #endregion

        #region Check

        /// <summary>
        /// Checks existence of specified document.
        /// </summary>
        /// <param name="documentID">Identifier of the document.</param>
        /// <returns>ArangoDocument object with assigned document ID and revision.</returns>
        public ArangoDocument CheckDocument(string documentID)
        {
            var document = new Document(_node);

            return document.Head(documentID);
        }

        #endregion

        #region Get

        public ArangoDocument GetDocument(string handle)
        {
            var document = new Document(_node);

            return document.Get(handle);
        }

        public ArangoDocument GetDocument(string handle, string revision)
        {
            var document = new Document(_node);

            return document.Get(handle, revision);
        }

        #endregion

        #endregion
    }
}
