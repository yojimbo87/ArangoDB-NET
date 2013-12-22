
namespace Arango.Client.Protocol
{
    internal static class AQL
    {
        internal const string None = "NONE";

        // used to indicate nested AQL expression, e.g. brackets expression in LET x = (FOR ... RETURN ...)
        internal const string Nesting = "NESTING";
        
        // high level operations
        internal const string COLLECT = "COLLECT";
        internal const string FILTER = "FILTER";
        internal const string FOR = "FOR";
        internal const string IN = "IN";
        internal const string INTO = "INTO";
        internal const string LET = "LET";
        internal const string LIMIT = "LIMIT";
        internal const string RETURN = "RETURN";
        internal const string SORT = "SORT";
        
        // symbols
        internal const string And = "&&";
        internal const string DoubleEquals = "==";
        internal new const string Equals = "=";
        internal const string From = ":";
    }
}
