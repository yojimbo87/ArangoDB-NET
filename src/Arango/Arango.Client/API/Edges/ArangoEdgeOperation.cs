using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoEdgeOperation
    {
        private EdgeOperation _edgeOperation;

        internal ArangoEdgeOperation(EdgeOperation edgeOperation)
        {
            _edgeOperation = edgeOperation;
        }

        #region Get
        
        public Document Get(string id)
        {
            return _edgeOperation.Get(id);
        }
        
        public T Get<T>(string id) where T : class, new()
        {
            var edge = Get(id);
            var obj = (T)edge.ToObject<T>();
            
            edge.MapAttributesTo(obj);
            
            return obj;
        }
        
        public List<Document> Get(string collection, string vertexId, ArangoEdgeDirection direction = ArangoEdgeDirection.Any)
        {
            return _edgeOperation.Get(collection, vertexId, direction);
        }
        
        #endregion
        
        #region Create
        
        public void Create(string collection, Document edge, bool waitForSync = false, bool createCollection = false)
        {
            _edgeOperation.Post(collection, edge, waitForSync, createCollection);
        }
        
        public void Create<T>(string collection, T genericObject, bool waitForSync = false, bool createCollection = false)
        {
            var edge = Document.ToDocument(genericObject);
            
            edge.MapAttributesFrom(genericObject);
            
            Create(collection, edge, waitForSync, createCollection);
            
            edge.MapAttributesTo(genericObject);
        }
        
        public Document Create(string collection, string from, string to, bool waitForSync = false, bool createCollection = false)
        {
            var edge = new Document();
            edge.String("_from", from);
            edge.String("_to", to);
            
            Create(collection, edge, waitForSync, createCollection);
            
            return edge;
        }
        
        #endregion
        
        public bool Delete(string id)
        {
            return _edgeOperation.Delete(id);
        }
        
        #region Replace
        
        public bool Replace(Document edge, bool waitForSync = false, string revision = null)
        {
            return _edgeOperation.Put(edge, waitForSync, revision);
        }
        
        public bool Replace<T>(T genericObject, bool waitForSync = false, string revision = null)
        {
            var edge = Document.ToDocument(genericObject);
            
            edge.MapAttributesFrom(genericObject);
            
            var isReplaced = Replace(edge, waitForSync, revision);
            
            edge.MapAttributesTo(genericObject);
            
            return isReplaced;
        }
        
        #endregion
        
        #region Update
        
        public bool Update(Document edge, bool waitForSync = false, string revision = null)
        {
            return _edgeOperation.Patch(edge, waitForSync, revision);
        }
        
        public bool Update<T>(T genericObject, bool waitForSync = false, string revision = null)
        {
            var edge = Document.ToDocument(genericObject);
            
            edge.MapAttributesFrom(genericObject);
            
            var isUpdated = Update(edge, waitForSync, revision);
            
            edge.MapAttributesTo(genericObject);
            
            return isUpdated;
        }
        
        #endregion
        
        public bool Exists(string id)
        {
            return _edgeOperation.Head(id);
        }
    }
}
