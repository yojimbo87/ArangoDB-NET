using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoDocumentOperation
    {
        private DocumentOperation _documentOperation;

        internal ArangoDocumentOperation(DocumentOperation documentOperation)
        {
            _documentOperation = documentOperation;
        }

        #region Get
        
        /// <summary>
        /// Retrieves document object from database identified by ID.
        /// </summary>
        public Document Get(string id)
        {
            return _documentOperation.Get(id);
        }
        
        /// <summary>
        /// Retrieves document from database identified by ID and maps it to specified generic object.
        /// </summary>
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
        /// Creates document in database collection and assigns additional data to referenced object.
        /// </summary>
        public void Create(string collection, Document document, bool waitForSync = false, bool createCollection = false)
        {
            _documentOperation.Post(collection, document, waitForSync, createCollection);
        }
        
        /// <summary>
        /// Creates document in database collection and assigns additional data to referenced object.
        /// </summary>
        public void Create<T>(string collection, T genericObject, bool waitForSync = false, bool createCollection = false)
        {
            var document = Document.ToDocument(genericObject);
            
            Create(collection, document, waitForSync, createCollection);
            
            document.MapAttributesTo(genericObject);
        }
        
        #endregion
        
        /// <summary>
        /// Deletes specified document from database collection and returnes boolean value which indicates if the operation was successful.
        /// </summary>
        public bool Delete(string id)
        {
            return _documentOperation.Delete(id);
        }
        
        #region Replace
        
        /// <summary>
        /// Replace existing document in database collection with another document object and retrieves boolean value indicating if the operation was successful.
        /// </summary>
        public bool Replace(Document document, bool waitForSync = false, string revision = null)
        {
            return _documentOperation.Put(document, waitForSync, revision);
        }
        
        /// <summary>
        /// Replace existing document in database collection with another document object and retrieves boolean value indicating if the operation was successful.
        /// </summary>
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
        /// Updates existing document in database collection and retrieves boolean value indicating if the operation was successful.
        /// </summary>
        public bool Update(Document document, bool waitForSync = false, string revision = null)
        {
            return _documentOperation.Patch(document, waitForSync, revision);
        }
        
        /// <summary>
        /// Updates existing document in database collection and retrieves boolean value indicating if the operation was successful.
        /// </summary>
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
        /// Check for existence of specific document.
        /// </summary>
        public bool Exists(string id)
        {
            return _documentOperation.Head(id);
        }
    }
}

