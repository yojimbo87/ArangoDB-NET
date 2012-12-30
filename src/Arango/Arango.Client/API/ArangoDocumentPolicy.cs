
namespace Arango.Client
{
    public enum ArangoDocumentPolicy
    {
        Default = 0,
        Error = 1, // replacements will fail if the revision id found in the database does not match the target revision id specified in the request
        Last = 2 // replacement will succeed, even if the revision id found in the database does not match the target revision id specified in the request
    }
}
