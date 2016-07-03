# Document operations

- [Create document](#create-document)
- [Create document with user defined key](#create-document-with-user-defined-key)
- [Create edge](#create-edge)
- [Check document existence](#check-document-existence)
- [Retrieve document](#retrieve-document)
- [Retrieve vertex edges](#retrieve-vertex-edges)
- [Update document](#update-document)
- [Replace document](#replace-document)
- [Replace edge](#replace-edge)
- [Delete document](#delete-document)
- [More examples](#more-examples)

## Retrieve vertex edges

Retrieves list of edges from specified edge type collection to specified document vertex with given direction.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getEdgesResult = db.Edge
    .Get("MyEdgeCollection", "MyDocumentCollection/123", ADirection.In);
    
if (getEdgesResult.Success)
{
    foreach (var edge in getEdgesResult.Value)
    {
        var id = edge.String("_id");
        var key = edge.String("_key");
        var revision = edge.String("_rev");
        var fromVertex = edge.String("_from");
        var toVertex = edge.String("_to");
    }
}
```

Document and edge operations are focused on management of documents in document and edge type collections. These operations are accessible through `Document` property in database context object. The API for documents and edges have been unified in ArangoDB 3.0. For CRUD operations there is no distinction anymore between documents and edges API-wise.

## Create document

Creates new document within specified collection in current database context.

Applicable optional parameters available through fluent API:

- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.
- `ReturnNew()` - Determines whether to return additionally the complete new document under the attribute 'new' in the result.

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var document = new Dictionary<string, object>()
    .String("foo", "foo string value")
    .Int("bar", 12345);

var createDocumentResult = db.Document
    .WaitForSync(true)
    .Create("MyDocumentCollection", document);
    
if (createDocumentResult.Success)
{
    var id = createDocumentResult.Value.String("_id");
    var key = createDocumentResult.Value.String("_key");
    var revision = createDocumentResult.Value.String("_rev");
}
```

Generic version:

```csharp
var db = new ADatabase("myDatabaseAlias");

var dummy = new Dummy();
dummy.Foo = "foo string value";
dummy.Bar = 12345;

var createDocumentResult = db.Document
    .WaitForSync(true)
    .Create("MyDocumentCollection", dummy);
    
if (createDocumentResult.Success)
{
    var id = createDocumentResult.Value.String("_id");
    var key = createDocumentResult.Value.String("_key");
    var revision = createDocumentResult.Value.String("_rev");
}
```

## Create document with user defined key

Documents can be created with custom `_key` field value within the collection which is set to allow user defined keys. Key value must have [valid format](https://docs.arangodb.com/NamingConventions/DocumentKeys.html).

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var document = new Dictionary<string, object>()
    .String("_key", "1234-5678")
    .String("foo", "foo string value")
    .Int("bar", 12345);

var createDocumentResult = db.Document
    .WaitForSync(true)
    .Create("MyDocumentCollection", document);
    
if (createDocumentResult.Success)
{
    // string value "MyDocumentCollection/1234-5678"
    var id = createDocumentResult.Value.String("_id");
    // string value "1234-5678"
    var key = createDocumentResult.Value.String("_key");
    var revision = createDocumentResult.Value.String("_rev");
}
```

## Create edge

Creates new edge within specified collection between two document vertices in current database context.

```csharp
var db = new ADatabase("myDatabaseAlias");

var edgeData = new Dictionary<string, object>()
    .String("foo", "foo string value")
    .Int("bar", 12345);

var createEdgeResult = db.Document
    .WaitForSync(true)
    .CreateEdge("MyEdgeCollection", "MyDocumentCollection/123", "MyDocumentCollection/456", edgeData);
    
if (createEdgeResult.Success)
{
    var id = createEdgeResult.Value.String("_id");
    var key = createEdgeResult.Value.String("_key");
    var revision = createEdgeResult.Value.String("_rev");
}
```

Generic version:

```csharp
var db = new ADatabase("myDatabaseAlias");

var dummy = new Dummy();
dummy.Foo = "foo string value";
dummy.Bar = 12345;

// creates new edge
var createEdgeResult = db.Document
    .WaitForSync(true)
    .CreateEdge("MyEdgeCollection", "MyDocumentCollection/123", "MyDocumentCollection/456", dummy);
    
if (createEdgeResult.Success)
{
    var id = createEdgeResult.Value.String("_id");
    var key = createEdgeResult.Value.String("_key");
    var revision = createEdgeResult.Value.String("_rev");
}
```

## Check document existence

Checks for existence of specified document.

Applicable optional parameters available through fluent API:

- `IfMatch(string revision)` - Conditionally operate on document with specified revision.
- `IfNoneMatch(string revision)` - Conditionally operate on document which current revision does not match specified revision.

```csharp
var db = new ADatabase("myDatabaseAlias");

var checkDocumentResult = db.Document
    .Check("MyDocumentCollection/123");
    
if (checkDocumentResult.Success)
{
    var revision = checkDocumentResult.Value;
}
```

## Retrieve document

Retrieves specified document.

Applicable optional parameters available through fluent API:

- `IfMatch(string revision)` - Conditionally operate on document with specified revision.
- `IfNoneMatch(string revision)` - Conditionally operate on document which current revision does not match specified revision.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getDocumentResult = db.Document
    .Get("MyDocumentCollection/123");
    
if (getDocumentResult.Success)
{
    // standard document descriptors
    var id = getDocumentResult.Value.String("_id");
    var key = getDocumentResult.Value.String("_key");
    var revision = getDocumentResult.Value.String("_rev");
    // document data
    var foo = getDocumentResult.Value.String("foo");
    var bar = getDocumentResult.Value.Int("bar");
}
```

Generic version:

```csharp
var db = new ADatabase("myDatabaseAlias");

var getDocumentResult = db.Document
    .Get<Dummy>("MyDocumentCollection/123");
    
if (getDocumentResult.Success)
{
    var foo = getDocumentResult.Value.Foo;
    var bar = getDocumentResult.Value.Bar;
}
```

## Retrieve vertex edges

Retrieves list of edges from specified edge type collection to specified document vertex with given direction.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getEdgesResult = db.Document
    .GetEdges("MyEdgeCollection", "MyDocumentCollection/123", ADirection.In);
    
if (getEdgesResult.Success)
{
    foreach (var edge in getEdgesResult.Value)
    {
        var id = edge.String("_id");
        var key = edge.String("_key");
        var revision = edge.String("_rev");
        var fromVertex = edge.String("_from");
        var toVertex = edge.String("_to");
    }
}
```

## Update document

Updates existing document identified by its handle with new document data.

Applicable optional parameters available through fluent API:

- `IfMatch(string revision)` - Conditionally operate on document with specified revision.
- `KeepNull(bool value)` - Determines whether to keep any attributes from existing document that are contained in the patch document which contains null value. Default value: true.
- `MergeObjects(bool value)` - Determines whether the value in the patch document will overwrite the existing document's value. Default value: true.
- `IgnoreRevs()` - Determines whether to '_rev' field in the given document is ignored. If this is set to false, then the '_rev' attribute given in the body document is taken as a precondition. The document is only replaced if the current revision is the one specified.
- `ReturnNew()` - Determines whether to return additionally the complete new document under the attribute 'new' in the result.
- `ReturnOld()` - Determines whether to return additionally the complete previous revision of the changed document under the attribute 'old' in the result.

```csharp
var db = new ADatabase("myDatabaseAlias");

var document = new Dictionary<string, object>()
    .String("foo", "new foo string value")
    .Int("baz", 123);

var updateDocumentResult = db.Document
    .Update("MyDocumentCollection/123", document);
    
if (updateDocumentResult.Success)
{
    var id = updateDocumentResult.Value.String("_id");
    var key = updateDocumentResult.Value.String("_key");
    var revision = updateDocumentResult.Value.String("_rev");
}
```

Generic version:

```csharp
var db = new ADatabase("myDatabaseAlias");

var dummy = new Dummy();
dummy.Foo = "some other new string";
dummy.Baz = 123;

var updateDocumentResult = db.Document
    .Update("MyDocumentCollection/123", dummy);
    
if (updateDocumentResult.Success)
{
    var id = updateDocumentResult.Value.String("_id");
    var key = updateDocumentResult.Value.String("_key");
    var revision = updateDocumentResult.Value.String("_rev");
}
```

## Replace document

Completely replaces existing document identified by its handle with new document data.

Applicable optional parameters available through fluent API:

- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.
- `IfMatch(string revision)` - Conditionally operate on document with specified revision.
- `IgnoreRevs()` - Determines whether to '_rev' field in the given document is ignored. If this is set to false, then the '_rev' attribute given in the body document is taken as a precondition. The document is only replaced if the current revision is the one specified.
- `ReturnNew()` - Determines whether to return additionally the complete new document under the attribute 'new' in the result.
- `ReturnOld()` - Determines whether to return additionally the complete previous revision of the changed document under the attribute 'old' in the result.

```csharp
var db = new ADatabase("myDatabaseAlias");

var document = new Dictionary<string, object>()
    .String("foo", "other foo string value")
    .Int("baz", 123);

var replaceDocumentResult = db.Document
    .Replace("MyDocumentCollection/123", document);
    
if (replaceDocumentResult.Success)
{
    var id = replaceDocumentResult.Value.String("_id");
    var key = replaceDocumentResult.Value.String("_key");
    var revision = replaceDocumentResult.Value.String("_rev");
}
```

Generic version:

```csharp
var db = new ADatabase("myDatabaseAlias");

var dummy = new Dummy();
dummy.Foo = "some other new string";
dummy.Baz = 123;

var replaceDocumentResult = db.Document
    .Replace("MyDocumentCollection/123", dummy);
    
if (replaceDocumentResult.Success)
{
    var id = replaceDocumentResult.Value.String("_id");
    var key = replaceDocumentResult.Value.String("_key");
    var revision = replaceDocumentResult.Value.String("_rev");
}
```

## Replace edge

Completely replaces existing edge identified by its handle with new edge data. This helper method injects 'fromID' and 'toID' fields into given document to construct valid edge document. 

```csharp
var db = new ADatabase("myDatabaseAlias");

var edgeData = new Dictionary<string, object>()
    .String("foo", "other foo string value")
    .Int("baz", 123);

var replaceEdgeResult = db.Document
    .ReplaceEdge("MyEdgeCollection/123", "MyDocumentCollection/456", "MyDocumentCollection/789", edgeData);
    
if (replaceEdgeResult.Success)
{
    var id = replaceEdgeResult.Value.String("_id");
    var key = replaceEdgeResult.Value.String("_key");
    var revision = replaceEdgeResult.Value.String("_rev");
}
```

Generic version:

```csharp
var db = new ADatabase("myDatabaseAlias");

var dummy = new Dummy();
dummy.Foo = "some other new string";
dummy.Baz = 123;

var replaceEdgeResult = db.Edge
    .ReplaceEdge("MyEdgeCollection/123", "MyDocumentCollection/456", "MyDocumentCollection/789", dummy);
    
if (replaceEdgeResult.Success)
{
    var id = replaceEdgeResult.Value.String("_id");
    var key = replaceEdgeResult.Value.String("_key");
    var revision = replaceEdgeResult.Value.String("_rev");
}
```

## Delete document

Deletes specified document.

- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.
- `IfMatch(string revision)` - Conditionally operate on document with specified revision.
- `ReturnOld()` - Determines whether to return additionally the complete previous revision of the changed document under the attribute 'old' in the result.

```csharp
var db = new ADatabase("myDatabaseAlias");

var deleteDocumentResult = db.Document
    .Delete("MyDocumentCollection/123");
    
if (deleteDocumentResult.Success)
{
    var id = deleteDocumentResult.Value.String("_id");
    var key = deleteDocumentResult.Value.String("_key");
    var revision = deleteDocumentResult.Value.String("_rev");
}
```

## More examples

More examples regarding document operations can be found in [unit tests](../src/Arango/Arango.Tests/DocumentOperations/DocumentOperationsTests.cs).