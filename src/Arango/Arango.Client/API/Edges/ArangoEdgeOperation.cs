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

        public ArangoEdge Get(string id)
        {
            return _edgeOperation.Get(id);
        }
        
        public List<ArangoEdge> Get(string collection, string vertexId, ArangoEdgeDirection direction = ArangoEdgeDirection.Any)
        {
            return _edgeOperation.Get(collection, vertexId, direction);
        }
        
        public void Create(string collection, ArangoEdge arangoEdge, bool waitForSync = false, bool createCollection = false)
        {
            _edgeOperation.Post(collection, arangoEdge, waitForSync, createCollection);
        }
        
        public bool Delete(string id)
        {
            return _edgeOperation.Delete(id);
        }
        
        public bool Replace(string id, ArangoEdge arangoEdge, bool waitForSync = false, string revision = null)
        {
            return _edgeOperation.Put(id, arangoEdge, waitForSync, revision);
        }
        
        public bool Update(ArangoEdge arangoEdge, bool waitForSync = false, string revision = null)
        {
            return _edgeOperation.Patch(arangoEdge, waitForSync, revision);
        }
        
        public bool Exists(string id)
        {
            return _edgeOperation.Head(id);
        }
    }
}
