# AQL query operations

- [Query operation parameters](#query-operation-parameters)
- [Executing simple query](#executing-simple-query)
- [Executing query with bind variables](#executing-query-with-bind-variables)
- [Result format options](#result-format-options)
- [Parse query](#parse-query)
- [Minify query](#minify-query)
- [Delete cursor](#delete-cursor)
- [More examples](#more-examples)

Query operations are focused on executing and analyzing AQL queries. These operations are accessible through `Query` property in database context object.

## Query operation parameters

Applicable parameters available through fluent API:

- `Aql(string query)` - Sets AQL query code.
- `BindVar(string key, object value)` - Maps key/value bind parameter.
- `Count(bool value)` - Determines whether the number of retrieved documents should be returned in `Extra` property of `ArangoResult` instance. Default value: false.
- `Ttl(int value)` - Determines whether the number of documents in the result set should be returned. Default value: false.
- `BatchSize(int value)` - Determines maximum number of result documents to be transferred from the server to the client in one roundtrip. If not set this value is server-controlled.

## Executing simple query

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var queryResult = db.Query
    .Aql(@"
    FOR item IN MyDocumentCollection 
        RETURN item
    ")
    .ToDocuments();
    
if (queryResult.Success)
{
    foreach (var document in queryResult.Value)
    {
        var foo = document.String("foo");
        var bar = document.Int("bar");
    }
}
```

## Executing query with bind variables

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var queryResult = db.Query
    .BindVar("bar", 123)
    .Aql(@"
    FOR item IN MyDocumentCollection 
        FILTER item.bar == @bar
        RETURN item
    ")
    .ToDocuments();
    
if (queryResult.Success)
{
    foreach (var document in queryResult.Value)
    {
        var foo = document.String("foo");
        var bar = document.Int("bar");
    }
}
```

## Result format options

Query result can be retrieved through following methods with different value type:

- `ToDocuments()` - Retrieves result value as list of documents.
- `ToList<T>()` - Retrieves result value as list of generic objects.
- `ToList()` - Retrieves result value as list of objects.
- `ToDocument()` - Retrieves result value as single document.
- `ToObject<T>()` - Retrieves result value as single generic object.
- `ToObject()` - Retrieves result value as single object.

## Parse query

Analyzes specified AQL query.

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var parseQueryResult = db.Query.Parse("FOR item IN MyDocumentCollection RETURN item");
    
if (parseQueryResult.Success)
{
    var analyzedQueryDocument = parseQueryResult.Value;
}
```

## Minify query

Transforms specified query into minified version with removed leading and trailing whitespaces except new line characters.

```csharp
var singleLineQuery = ArangoQuery.Minify(@"
FOR item IN MyDocumentCollection
    RETURN item
");
```

## Delete cursor

Deletes specified AQL query cursor.

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var deleteCursorResult = db.Query
    .DeleteCursor("someCursorID");
    
if (deleteCursorResult.Success)
{
    var isCursorDeleted = deleteCursorResult.Value;
}
```

## More examples

More examples regarding AQL query operations can be found in [unit tests](../src/Arango/Arango.Tests/QueryOperations/QueryOperationsTests.cs).