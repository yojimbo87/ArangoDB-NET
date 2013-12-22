using System.Collections.Generic;
using System.Linq;

namespace Arango.Client.Protocol
{
    internal class Etom
    {
        internal string Type { get; set; }
        internal object Value { get { return Values.First(); } }
        internal List<object> Values { get; set; }
        internal List<Etom> Children { get; set; }

        internal Etom(string type)
        {
            Type = type;
            Values = new List<object>();
            Children = new List<Etom>();
        }

        internal void AddValues(params object[] values)
        {
            foreach (object value in values)
            {
                Values.Add(value);
            }
        }
    }
}

