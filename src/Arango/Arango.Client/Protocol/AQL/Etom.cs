using System.Collections.Generic;
using System.Linq;

namespace Arango.Client.Protocol
{
    internal class Etom
    {
        internal string Type { get; set; }
        internal object Value { get; set; }
        internal List<Etom> Children { get; set; }

        internal Etom()
        {
            Children = new List<Etom>();
        }
    }
}

