using System.Collections.Generic;
using System.Linq;

namespace Arango.Client
{
    public static class ArangoClient
    {
        #region Properties

        public static string DriverName
        {
            get { return "Arango-NET"; }
        }

        public static string DriverVersion
        {
            get { return "Alpha 1.0"; }
        }

        public static List<ArangoNode> Nodes { get; set; }

        #endregion

        static ArangoClient()
        {
            Nodes = new List<ArangoNode>();
        }

        internal static ArangoNode GetNode(string alias)
        {
            return Nodes.Where(node => node.Alias == alias).FirstOrDefault();
        }
    }
}
