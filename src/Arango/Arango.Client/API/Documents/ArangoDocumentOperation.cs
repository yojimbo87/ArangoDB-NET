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

        public ArangoDocument Get(string id)
        {
            return _documentOperation.Get(id);
        }
        
        public void Create(string collection, ArangoDocument arangoDocument, bool waitForSync = false, bool createCollection = false)
        {
            _documentOperation.Post(collection, arangoDocument, waitForSync, createCollection);
        }
        
        public bool Delete(string id)
        {
            return _documentOperation.Delete(id);
        }
    }
}

