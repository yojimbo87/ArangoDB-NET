using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoDatabase
    {
        private Connection _connection;

        public ArangoCollectionOperation Collection
        {
            get
            {
                return new ArangoCollectionOperation(new CollectionOperation(_connection));
            }
        }
        
        public ArangoDocumentOperation Document
        {
            get
            {
                return new ArangoDocumentOperation(new DocumentOperation(_connection));
            }
        }
        
        public ArangoEdgeOperation Edge
        {
            get
            {
                return new ArangoEdgeOperation(new EdgeOperation(_connection));
            }
        }

        public ArangoDatabase(string alias)
        {
            _connection = ArangoClient.GetConnection(alias);
        }
        
        public List<Document> Query(string aql, bool count = false, int batchSize = 0)
        {
            CursorOperation cursorOperation = new CursorOperation(_connection);
            
            return cursorOperation.Post(aql, count, batchSize);
        }
    }
}

