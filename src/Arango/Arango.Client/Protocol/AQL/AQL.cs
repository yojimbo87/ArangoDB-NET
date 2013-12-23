
namespace Arango.Client.Protocol
{
    internal static class AQL
    {
        // standard high level operations
        internal const string COLLECT = "COLLECT";
        internal const string FILTER = "FILTER";
        internal const string FOR = "FOR";
        internal const string IN = "IN";
        internal const string INTO = "INTO";
        internal const string LET = "LET";
        internal const string LIMIT = "LIMIT";
        internal const string RETURN = "RETURN";
        internal const string SORT = "SORT";

        // standard functions
        internal const string DOCUMENT = "DOCUMENT";
        internal const string EDGES = "EDGES";

        // symbols
        internal const string And = "&&";
        internal new const string Equals = "==";

        // internal operations
        internal const string Collection = "COLLECTION";
        internal const string Field = "FIELD";
        internal const string List = "LIST";
        internal const string ListExpression = "LISTEXPRESSION";
        internal const string Object = "OBJECT";
        internal const string String = "STRING";
        internal const string Value = "VALUE";
        internal const string Variable = "VARIABLE";
    }
}
