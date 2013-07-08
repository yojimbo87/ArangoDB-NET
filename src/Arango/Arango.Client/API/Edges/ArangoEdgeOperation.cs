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
        
        public ArangoEdge Get(string id)
        {
            return _edgeOperation.Get(id);
        }
        
        public T Get<T>(string id) where T : class, new()
        {
            var arangoEdge = Get(id);
            var obj = (T)arangoEdge.Document.To<T>();
            arangoEdge.MapAttributesTo(obj);
            
            return obj;
        }
        
        public List<ArangoEdge> Get(string collection, string vertexId, ArangoEdgeDirection direction = ArangoEdgeDirection.Any)
        {
            return _edgeOperation.Get(collection, vertexId, direction);
        }
        
        #endregion
        
        #region Create
        
        public void Create(string collection, ArangoEdge arangoEdge, bool waitForSync = false, bool createCollection = false)
        {
            _edgeOperation.Post(collection, arangoEdge, waitForSync, createCollection);
        }
        
        public void Create<T>(string collection, T genericObject, bool waitForSync = false, bool createCollection = false)
        {
            var arangoEdge = new ArangoEdge();
            arangoEdge.MapAttributesFrom(genericObject);
            arangoEdge.Document.From(genericObject);
            
            Create(collection, arangoEdge, waitForSync, createCollection);
            
            arangoEdge.MapAttributesTo(genericObject);
        }
        
        #endregion
        
        public bool Delete(string id)
        {
            return _edgeOperation.Delete(id);
        }
        
        #region Replace
        
        public bool Replace(ArangoEdge arangoEdge, bool waitForSync = false, string revision = null)
        {
            return _edgeOperation.Put(arangoEdge, waitForSync, revision);
        }
        
        public bool Replace<T>(T genericObject, bool waitForSync = false, string revision = null)
        {
            var arangoEdge = new ArangoEdge();
            arangoEdge.MapAttributesFrom(genericObject);
            arangoEdge.Document.From(genericObject);
            
            var isReplaced = Replace(arangoEdge, waitForSync, revision);
            arangoEdge.MapAttributesTo(genericObject);
            
            return isReplaced;
        }
        
        #endregion
        
        #region Update
        
        public bool Update(ArangoEdge arangoEdge, bool waitForSync = false, string revision = null)
        {
            return _edgeOperation.Patch(arangoEdge, waitForSync, revision);
        }
        
        public bool Update<T>(T genericObject, bool waitForSync = false, string revision = null)
        {
            var arangoEdge = new ArangoEdge();
            arangoEdge.MapAttributesFrom(genericObject);
            arangoEdge.Document.From(genericObject);
            
            var isUpdated = Update(arangoEdge, waitForSync, revision);
            arangoEdge.MapAttributesTo(genericObject);
            
            return isUpdated;
        }
        
        #endregion
        
        public bool Exists(string id)
        {
            return _edgeOperation.Head(id);
        }
    }
}
