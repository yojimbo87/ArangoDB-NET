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
        
        public ArangoDocument Get(string id)
        {
            return _documentOperation.Get(id);
        }
        
        public T Get<T>(string id) where T : class, new()
        {
            var arangoDocument = Get(id);
            var obj = (T)arangoDocument.Document.To<T>();
            arangoDocument.MapAttributes(obj);
            
            return obj;
        }
        
        #endregion
        
        #region Create
        
        public void Create(string collection, ArangoDocument arangoDocument, bool waitForSync = false, bool createCollection = false)
        {
            _documentOperation.Post(collection, arangoDocument, waitForSync, createCollection);
        }
        
        public void Create<T>(string collection, T genericObject, bool waitForSync = false, bool createCollection = false)
        {
            var arangoDocument = new ArangoDocument();
            arangoDocument.Document.From(genericObject);
            
            _documentOperation.Post(collection, arangoDocument, waitForSync, createCollection);
            
            arangoDocument.MapAttributes(genericObject);
        }
        
        #endregion
        
        public bool Delete(string id)
        {
            return _documentOperation.Delete(id);
        }
        
        #region Replace
        
        public bool Replace(string id, ArangoDocument arangoDocument, bool waitForSync = false, string revision = null)
        {
            return _documentOperation.Put(id, arangoDocument, waitForSync, revision);
        }
        
        public bool Replace<T>(string id, T genericObject, bool waitForSync = false, string revision = null)
        {
            var arangoDocument = new ArangoDocument();
            arangoDocument.Document.From(genericObject);
            
            var isReplaced = _documentOperation.Put(id, arangoDocument, waitForSync, revision);
            arangoDocument.MapAttributes(genericObject);
            
            return isReplaced;
        }
        
        #endregion
        
        #region Update
        
        public bool Update(string id, ArangoDocument arangoDocument, bool waitForSync = false, string revision = null)
        {
            return _documentOperation.Patch(id, arangoDocument, waitForSync, revision);
        }
        
        public bool Update<T>(string id, T genericObject, bool waitForSync = false, string revision = null)
        {
            var arangoDocument = new ArangoDocument();
            arangoDocument.Document.From(genericObject);
            
            var isUpdated = _documentOperation.Patch(id, arangoDocument, waitForSync, revision);
            arangoDocument.MapAttributes(genericObject);
            
            return isUpdated;
        }
        
        #endregion
        
        public bool Exists(string id)
        {
            return _documentOperation.Head(id);
        }
    }
}

