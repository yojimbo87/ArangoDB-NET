using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary> 
    /// Expose edge management functionality.
    /// </summary>
    public class ArangoEdgeOperation
    {
        private EdgeOperation _edgeOperation;

        internal ArangoEdgeOperation(EdgeOperation edgeOperation)
        {
            _edgeOperation = edgeOperation;
        }

        #region Get
        
        /// <summary> 
        /// Retrieves edge document with specified identifier.
        /// </summary>
        /// <param name="id">Edge identifier.</param>
        public Document Get(string id)
        {
            return _edgeOperation.Get(id);
        }
        
        /// <summary> 
        /// Retrieves edge with specified identifier mapped to object.
        /// </summary>
        /// <param name="id">Edge identifier.</param>
        public T Get<T>(string id) where T : class, new()
        {
            var edge = Get(id);
            var obj = (T)edge.ToObject<T>();
            
            edge.MapAttributesTo(obj);
            
            return obj;
        }
        
        /// <summary> 
        /// Retrieves list of edge documents from collection with specified direction.
        /// </summary>
        /// <param name="collection">Collection from which will be edges retrieved.</param>
        /// <param name="vertexId">Identifier of the start vertex.</param>
        /// <param name="direction">Direction of edges to retrieve.</param>
        public List<Document> Get(string collection, string vertexId, ArangoEdgeDirection direction = ArangoEdgeDirection.Any)
        {
            return _edgeOperation.Get(collection, vertexId, direction);
        }
        
        #endregion
        
        #region Create
        
        /// <summary> 
        /// Creates edge in specified collection. Passed edge parameter will be updated with _id, _key and _rev fields.
        /// </summary>
        /// <param name="collection">Collection in which will be edge created.</param>
        /// <param name="edge">Document with data which will be saved to newly created edge. Document have to contain _from and _to fields.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="createCollection">Determines if previously specified collection should be created if it not exists.</param>
        public void Create(string collection, Document edge, bool waitForSync = false, bool createCollection = false)
        {
            _edgeOperation.Post(collection, edge, waitForSync, createCollection);
        }
        
        /// <summary> 
        /// Creates edge in specified collection. Passed genericObject parameter will be updated with Identity, Key and Revision properties if they were mapped through ArangoProperty class.
        /// </summary>
        /// <param name="collection">Collection in which will be edge created.</param>
        /// <param name="genericObject">Object with data which will be saved to newly created edge. Object have to contains From and To properties mapped through ArangoProperty class.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="createCollection">Determines if previously specified collection should be created if it not exists.</param>
        public void Create<T>(string collection, T genericObject, bool waitForSync = false, bool createCollection = false)
        {
            var edge = Document.ToDocument(genericObject);
            
            edge.MapAttributesFrom(genericObject);
            
            Create(collection, edge, waitForSync, createCollection);
            
            edge.MapAttributesTo(genericObject);
        }
        
        /// <summary> 
        /// Creates edge in specified collection.
        /// </summary>
        /// <param name="collection">Collection in which will be edge created.</param>
        /// <param name="from">Identifier of document/vertex from which will edge be heading (incoming relation).</param>
        /// <param name="to">Identifier of document/vertex to which will edge be heading (outgoing relation).</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="createCollection">Determines if previously specified collection should be created if it not exists.</param>
        public Document Create(string collection, string from, string to, bool waitForSync = false, bool createCollection = false)
        {
            var edge = new Document();
            edge.String("_from", from);
            edge.String("_to", to);
            
            Create(collection, edge, waitForSync, createCollection);
            
            return edge;
        }
        
        #endregion
        
        /// <summary> 
        /// Deletes edge with specified identifier.
        /// </summary>
        /// <param name="id">Identifier of the edge to be deleted.</param>
        public bool Delete(string id)
        {
            return _edgeOperation.Delete(id);
        }
        
        #region Replace
        
        /// <summary> 
        /// Replaces specified edge. Passed edge parameter will be updated with _id, _key and _rev fields.
        /// </summary>
        /// <param name="edge">Document with data that will replace existing edge data. Document have to contain _from and _to fields.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="revision">Replaces edge with specific revision.</param>
        public bool Replace(Document edge, bool waitForSync = false, string revision = null)
        {
            return _edgeOperation.Put(edge, waitForSync, revision);
        }
        
        /// <summary> 
        /// Replaces specified edge. Passed genericObject parameter will be updated with Identity, Key and Revision properties if they were mapped through ArangoProperty class.
        /// </summary>
        /// <param name="genericObject">Object with data that will replace existing edge data. Object have to contain From and To properties mapped through ArangoProperty class.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="revision">Replaces edge with specific revision.</param>
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
        
        /// <summary> 
        /// Updates specified edge. Passed edge parameter will be updated with _id, _key and _rev fields.
        /// </summary>
        /// <param name="edge">Document with data that will update existing edge data. Document have to contain _from and _to fields.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="revision">Updates edge with specific revision.</param>
        public bool Update(Document edge, bool waitForSync = false, string revision = null)
        {
            return _edgeOperation.Patch(edge, waitForSync, revision);
        }
        
        /// <summary> 
        /// Updates specified edge. Passed genericObject parameter will be updated with Identity, Key and Revision properties if they were mapped through ArangoProperty class.
        /// </summary>
        /// <param name="genericObject">Object with data that will update existing edge data. Object have to contain From and To properties mapped through ArangoProperty class.</param>
        /// <param name="waitForSync">Determines if the response should wait until document has been synced to disk.</param>
        /// <param name="revision">Updates edge with specific revision.</param>
        public bool Update<T>(T genericObject, bool waitForSync = false, string revision = null)
        {
            var edge = Document.ToDocument(genericObject);
            
            edge.MapAttributesFrom(genericObject);
            
            var isUpdated = Update(edge, waitForSync, revision);
            
            edge.MapAttributesTo(genericObject);
            
            return isUpdated;
        }
        
        #endregion
        
        /// <summary> 
        /// Determines if the edge with specified identifier is present in database.
        /// </summary>
        /// <param name="id">Identifier of the edge to be checked for existence.</param>
        public bool Exists(string id)
        {
            return _edgeOperation.Head(id);
        }
    }
}
