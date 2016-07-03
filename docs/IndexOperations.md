# Index operations

- [Create index](#create-index)
- [Retrieve index](#retrieve-index)
- [Delete index](#delete-index)
- [More examples](#more-examples)

Index operations are focused on management of indexes in specific collection which are accessible through `Index` property in database context object.

## Create index

Creates index within specified collection in current database context.

Applicable optional parameters available through fluent API:

- `Fields(params string[] values)` - Determines an array of attribute paths in the collection with hash, fulltext, geo or skiplist indexes.
- `GeoJson(bool value)` - Determines if the order within the array is longitude followed by latitude in geo index.
- `MinLength(int value)` - Determines minimum character length of words for fulltext index. Will default to a server-defined value if unspecified. It is thus recommended to set this value explicitly when creating the index.
- `Sparse(bool value)` - Determines if the hash or skiplist index should be sparse.
- `Type(AIndexType value)` - Determines type of the index.
- `Unique(bool value)` - Determines if the hash or skiplist index should be unique.

```csharp
var db = new ADatabase("myDatabaseAlias");

var createIndexResult = db.Index
    .Type(AIndexType.Hash)
    .Fields("foo")
    .Create(Database.TestDocumentCollectionName);
    
if (createIndexResult.Success)
{
    var id = createIndexResult.Value.String("id");
    var type = createIndexResult.Value.Enum<AIndexType>("type");
    var fields = createIndexResult.Value.List<string>("fields");
    var isUnique = createIndexResult.Value.Bool("unique");
    var isSparse = createIndexResult.Value.Bool("sparse");
    var selectivityEstimate = createIndexResult.Value.Int("selectivityEstimate");
    var isNewlyCreated = createIndexResult.Value.Bool("isNewlyCreated");
}
```

## Retrieve index

Retrieves specified index.

```csharp
var db = new ADatabase("myDatabaseAlias");

var retrieveIndexResult = db.Index
    .Get("someCollection/0");
    
if (createIndexResult.Success)
{
    var id = retrieveIndexResult.Value.String("id");
    var type = retrieveIndexResult.Value.Enum<AIndexType>("type");
    var fields = retrieveIndexResult.Value.List<string>("fields");
    var isUnique = retrieveIndexResult.Value.Bool("unique");
    var isSparse = retrieveIndexResult.Value.Bool("sparse");
    var selectivityEstimate = retrieveIndexResult.Value.Int("selectivityEstimate");
}
```

## Delete index

Deletes specified index.

```csharp
var db = new ADatabase("myDatabaseAlias");

var deleteIndexResult = db.Index
    .Delete("someCollection/123");
    
if (deleteIndexResult.Success)
{
    var id = deleteIndexResult.Value.String("id");
}
```

## More examples

More examples regarding index operations can be found in [unit tests](../src/Arango/Arango.Tests/IndexOperations/IndexOperationsTests.cs).