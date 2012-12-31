
namespace Arango.Client
{
    public enum DocumentUpdatePolicy
    {
        /// <summary>
        /// If a target revision id is provided in the request, ArangoDB will check that the revision id of the document found in the database is equal to the target revision id provided in the request. If there is a mismatch between the revision id, then by default a HTTP 409 conflict is returned and no replacement is performed. ArangoDB will return an HTTP 412 precondition failed response then.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Replacement will fail if the revision id found in the database does not match the target revision id specified in the request.
        /// </summary>
        Error = 1,
        /// <summary>
        /// Replacement will succeed, even if the revision id found in the database does not match the target revision id specified in the request.
        /// </summary>
        Last = 2
    }
}
