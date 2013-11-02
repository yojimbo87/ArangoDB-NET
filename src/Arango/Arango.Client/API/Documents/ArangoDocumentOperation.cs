using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary> 
    /// Expose document management functionality.
    /// </summary>
    public class ArangoDocumentOperation
    {
        private DocumentOperation _documentOperation;

        internal ArangoDocumentOperation(DocumentOperation documentOperation)
        {
            _documentOperation = documentOperation;
        }

        #region Get
        
        /// <summary>
        /// Retrieves document with specified identifier.
        /// </summary>
        /// <param name="id">Document identifier.</param>
        public Document Get(string id)
        {
            return _documentOperation.Get(id);
        }
        
        /// <summary>
        /// Retrieves document with specified identifier mapped to object.
        /// </summary>
        /// <param name="id">Document identifier.</param>
        public T Get<T>(string id) where T : class, new()
        {
            var document = Get(id);
            var obj = (T)document.ToObject<T>();
            
            document.MapAttributesTo(obj);
            
            return obj;
        }
        
        #endregion
        
        #region Create
        
        /// <summary> 
        /// Creates document in specified collection. Passed document parameter will be updated with _id, _key and _rev fields.
        /// </summary>
        /// <param name="collection">Collection in which will be document created.</param>
        /// <param name="document">Document with data which will be saved to newly created document. Document have to contain _id field.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="createCollection">Determines if previously specified collection should be created if it not exists.</param>
        public void Create(string collection, Document document, bool waitForSync = false, bool createCollection = false)
        {
            _documentOperation.Post(collection, document, waitForSync, createCollection);
        }
        
        /// <summary> 
        /// Creates document in specified collection. Passed genericObject parameter will be updated with Identity, Key and Revision properties if they were mapped through ArangoProperty class.
        /// </summary>
        /// <param name="collection">Collection in which will be document created.</param>
        /// <param name="genericObject">Object with data which will be saved to newly created document. Object have to contain Identity property mapped through ArangoProperty class.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="createCollection">Determines if previously specified collection should be created if it not exists.</param>
        public void Create<T>(string collection, T genericObject, bool waitForSync = false, bool createCollection = false)
        {
            var document = Document.ToDocument(genericObject);
            
            Create(collection, document, waitForSync, createCollection);
            
            document.MapAttributesTo(genericObject);
        }
        
        #endregion
        
        /// <summary> 
        /// Deletes document with specified identifier.
        /// </summary>
        /// <param name="id">Identifier of the document to be deleted.</param>
        public bool Delete(string id)
        {
            return _documentOperation.Delete(id);
        }
        
        #region Replace
        
        /// <summary> 
        /// Replaces specified document. Passed document parameter will be updated with _id, _key and _rev fields.
        /// </summary>
        /// <param name="document">Document with data that will replace existing document data. Document have to contain _id field.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="revision">Replaces edge with specific revision.</param>
        public bool Replace(Document document, bool waitForSync = false, string revision = null)
        {
            return _documentOperation.Put(document, waitForSync, revision);
        }
        
        /// <summary> 
        /// Replaces specified document. Passed genericObject parameter will be updated with Identity, Key and Revision properties if they were mapped through ArangoProperty class.
        /// </summary>
        /// <param name="genericObject">Object with data that will replace existing edge data. Object have to contain Identity property mapped through ArangoProperty class.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="revision">Replaces edge with specific revision.</param>
        public bool Replace<T>(T genericObject, bool waitForSync = false, string revision = null)
        {
            var document = Document.ToDocument(genericObject);
            
            document.MapAttributesFrom(genericObject);
            
            var isReplaced = Replace(document, waitForSync, revision);
            
            document.MapAttributesTo(genericObject);
            
            return isReplaced;
        }
        
        #endregion
        
        #region Update
        
        /// <summary> 
        /// Updates specified document. Passed document parameter will be updated with _id, _key and _rev fields.
        /// </summary>
        /// <param name="edge">Document with data that will update existing document data. Document have to contain _id field.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="revision">Updates edge with specific revision.</param>
        public bool Update(Document document, bool waitForSync = false, string revision = null)
        {
            return _documentOperation.Patch(document, waitForSync, revision);
        }
        
        /// <summary> 
        /// Updates specified document. Passed genericObject parameter will be updated with Identifier, Key and Revision properties if they were mapped through ArangoProperty class.
        /// </summary>
        /// <param name="genericObject">Object with data that will update existing document data. Object have to contain Identity property mapped through ArangoProperty class.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="revision">Updates edge with specific revision.</param>
        public bool Update<T>(T genericObject, bool waitForSync = false, string revision = null)
        {
            var document = Document.ToDocument(genericObject);
            
            document.MapAttributesFrom(genericObject);
            
            var isUpdated = Update(document, waitForSync, revision);
            
            document.MapAttributesTo(genericObject);
            
            return isUpdated;
        }
        
        #endregion
        
        /// <summary> 
        /// Determines if the document with specified identifier is present in database.
        /// </summary>
        /// <param name="id">Identifier of the document to be checked for existence.</param>
        public bool Exists(string id)
        {
            return _documentOperation.Head(id);
        }
    }
}

