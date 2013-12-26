using System.Collections.Generic;
using System.Linq;

namespace Arango.Client.Protocol
{
    internal class Etom
    {
        internal string Type { get; set; }
        internal object Value { get; set; }
        internal List<Etom> Children { get; set; }
        // might be used for example in cases of CONCAT(AQL, AQL, AQL, ...)
        internal List<List<Etom>> ChildrenList { get; set; }

        internal Etom()
        {
            Children = new List<Etom>();
        }
    }
}

