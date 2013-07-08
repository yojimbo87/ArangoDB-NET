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
            arangoDocument.MapAttributesTo(obj);
            
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
            
            Create(collection, arangoDocument, waitForSync, createCollection);
            
            arangoDocument.MapAttributesTo(genericObject);
        }
        
        #endregion
        
        public bool Delete(string id)
        {
            return _documentOperation.Delete(id);
        }
        
        #region Replace
        
        public bool Replace(ArangoDocument arangoDocument, bool waitForSync = false, string revision = null)
        {
            return _documentOperation.Put(arangoDocument, waitForSync, revision);
        }
        
        public bool Replace<T>(T genericObject, bool waitForSync = false, string revision = null)
        {
            var arangoDocument = new ArangoDocument();
            arangoDocument.MapAttributesFrom(genericObject);
            arangoDocument.Document.From(genericObject);
            
            var isReplaced = Replace(arangoDocument, waitForSync, revision);
            arangoDocument.MapAttributesTo(genericObject);
            
            return isReplaced;
        }
        
        #endregion
        
        #region Update
        
        public bool Update(ArangoDocument arangoDocument, bool waitForSync = false, string revision = null)
        {
            return _documentOperation.Patch(arangoDocument, waitForSync, revision);
        }
        
        public bool Update<T>(T genericObject, bool waitForSync = false, string revision = null)
        {
            var arangoDocument = new ArangoDocument();
            arangoDocument.MapAttributesFrom(genericObject);
            arangoDocument.Document.From(genericObject);
            
            var isUpdated = Update(arangoDocument, waitForSync, revision);
            arangoDocument.MapAttributesTo(genericObject);
            
            return isUpdated;
        }
        
        #endregion
        
        public bool Exists(string id)
        {
            return _documentOperation.Head(id);
        }
    }
}

