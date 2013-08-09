
namespace Arango.Client.Protocol
{
    internal static class AQL
    {
        // high level operations
        internal static string Collect { get { return "COLLECT"; } }
        internal static string Filter { get { return "FILTER"; } }
        internal static string For { get { return "FOR"; } }
        internal static string In { get { return "IN"; } }
        internal static string Into { get { return "INTO"; } }
        internal static string Let { get { return "LET"; } }
        internal static string Limit { get { return "LIMIT"; } }
        internal static string Return { get { return "RETURN"; } }
        internal static string Sort { get { return "SORT"; } }
        
        // symbols
        internal static string And { get { return "&&"; } }
        internal new static string Equals { get { return "=="; } }
        internal static string From { get { return ":"; } }
    }
}
