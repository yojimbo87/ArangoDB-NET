
namespace Arango.Client.Protocol
{
    internal static class AQL
    {
        internal const string None = "NONE";
        
        // high level operations
        internal const string Collect = "COLLECT";
        internal const string Filter = "FILTER";
        internal const string For = "FOR";
        internal const string In = "IN";
        internal const string Into = "INTO";
        internal const string Let = "LET";
        internal const string Limit = "LIMIT";
        internal const string Return = "RETURN";
        internal const string Sort = "SORT";
        
        // symbols
        internal const string And = "&&";
        internal const string DoubleEquals = "==";
        internal new const string Equals = "=";
        internal const string From = ":";
    }
}
