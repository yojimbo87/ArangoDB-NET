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
            ArangoDocument arangoDocument = Get(id);
            
            // TODO: convert also ArangoDocument specific properties
            
            return (T)arangoDocument.Document.To<T>();
        }
        
        #endregion
        
        #region Create
        
        public void Create(string collection, ArangoDocument arangoDocument, bool waitForSync = false, bool createCollection = false)
        {
            _documentOperation.Post(collection, arangoDocument, waitForSync, createCollection);
        }
        
        public string Create<T>(string collection, T genericObject, bool waitForSync = false, bool createCollection = false)
        {
            ArangoDocument arangoDocument = new ArangoDocument();
            arangoDocument.Document.From(genericObject);
            
            _documentOperation.Post(collection, arangoDocument, waitForSync, createCollection);
            
            // TODO: convert ArangoDocument specific properties which were added after create process
            return arangoDocument.Id;
        }
        
        #endregion
        
        public bool Delete(string id)
        {
            return _documentOperation.Delete(id);
        }
        
        public bool Replace(string id, ArangoDocument arangoDocument, bool waitForSync = false, string revision = null)
        {
            return _documentOperation.Put(id, arangoDocument, waitForSync, revision);
        }
        
        public bool Update(ArangoDocument arangoDocument, bool waitForSync = false, string revision = null)
        {
            return _documentOperation.Patch(arangoDocument, waitForSync, revision);
        }
        
        public bool Exists(string id)
        {
            return _documentOperation.Head(id);
        }
    }
}

