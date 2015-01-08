# Edge operations

- [Create edge](#create-edge)
- [Check edge existence](#check-edge-existence)
- [Retrieve edge](#retrieve-edge)
- [Retrieve vertex edges](#retrieve-vertex-edges)
- [Update edge](#update-edge)
- [Replace edge](#replace-edge)
- [Delete edge](#delete-edge)
- [More examples](#more-examples)

Edge operations are focused on management of documents in edge type collections. These operations are accessible through `Edge` property in database context object.

## Create edge

Creates new edge within specified collection between two document vertices in current database context.

Applicable optional parameters available through fluent API:

- `CreateCollection(bool value)` - Determines whether collection should be created if it does not exist. Default value: false.
- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.

```csharp
var db = new ADatabase("myDatabaseAlias");

var edgeData = new Dictionary<string, object>()
    .String("foo", "foo string value")
    .Int("bar", 12345);

var createEdgeResult = db.Edge
    .WaitForSync(true)
    .Create("MyEdgeCollection", "MyDocumentCollection/123", "MyDocumentCollection/456", edgeData);
    
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
var createEdgeResult = db.Edge
    .WaitForSync(true)
    .Create("MyEdgeCollection", "MyDocumentCollection/123", "MyDocumentCollection/456", dummy);
    
if (createEdgeResult.Success)
{
    var id = createEdgeResult.Value.String("_id");
    var key = createEdgeResult.Value.String("_key");
    var revision = createEdgeResult.Value.String("_rev");
}
```

## Check edge existence

Checks for existence of specified edge.

Applicable optional parameters available through fluent API:

- `IfMatch(string revision)` - Conditionally operate on edge with specified revision.
- `IfMatch(string revision, AUpdatePolicy updatePolicy)` - Conditionally operate on edge with specified revision and update policy.
- `IfNoneMatch(string revision)` - Conditionally operate on edge which current revision does not match specified revision.

```csharp
var db = new ADatabase("myDatabaseAlias");

var checkEdgeResult = db.Edge
    .Check("MyEdgeCollection/123");
    
if (checkEdgeResult.Success)
{
    var revision = checkEdgeResult.Value;
}
```

## Retrieve edge

Retrieves specified edge.

Applicable optional parameters available through fluent API:

- `IfMatch(string revision)` - Conditionally operate on edge with specified revision.
- `IfNoneMatch(string revision)` - Conditionally operate on edge which current revision does not match specified revision.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getEdgeResult = db.Edge
    .Get("MyEdgeCollection/123");
    
if (getEdgeResult.Success)
{
    // standard edge descriptors
    var id = getEdgeResult.Value.String("_id");
    var key = getEdgeResult.Value.String("_key");
    var revision = getEdgeResult.Value.String("_rev");
    var fromVertex = getEdgeResult.Value.String("_from");
    var toVertex = getEdgeResult.Value.String("_to");
    // edge document data
    var foo = getEdgeResult.Value.String("foo");
    var bar = getEdgeResult.Value.Int("bar");
}
```

Generic version:

```csharp
var db = new ADatabase("myDatabaseAlias");

var getEdgeResult = db.Edge
    .Get<Dummy>("MyEdgeCollection/123");
    
if (getEdgeResult.Success)
{
    var foo = getEdgeResult.Value.Foo;
    var bar = getEdgeResult.Value.Bar;
}
```

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

## Update edge

Updates existing edge identified by its handle with new edge data.

Applicable optional parameters available through fluent API:

- `IfMatch(string revision)` - Conditionally operate on edge with specified revision.
- `IfMatch(string revision, AUpdatePolicy updatePolicy)` - Conditionally operate on edge with specified revision and update policy.
- `KeepNull(bool value)` - Determines whether to keep any attributes from existing edge that are contained in the patch edge which contains null value. Default value: true.
- `MergeArrays(bool value)` - Determines whether the value in the patch edge will overwrite the existing edge's value. Default value: true.

```csharp
var db = new ADatabase("myDatabaseAlias");

var edgeData = new Dictionary<string, object>()
    .String("foo", "new foo string value")
    .Int("baz", 123);

var updateEdgeResult = db.Edge
    .Update("MyEdgeCollection/123", edgeData);
    
if (updateEdgeResult.Success)
{
    var id = updateEdgeResult.Value.String("_id");
    var key = updateEdgeResult.Value.String("_key");
    var revision = updateEdgeResult.Value.String("_rev");
}
```

Generic version:

```csharp
var db = new ADatabase("myDatabaseAlias");

var dummy = new Dummy();
dummy.Foo = "some other new string";
dummy.Baz = 123;

var updateEdgeResult = db.Edge
    .Update("MyEdgeCollection/123", dummy);
    
if (updateEdgeResult.Success)
{
    var id = updateEdgeResult.Value.String("_id");
    var key = updateEdgeResult.Value.String("_key");
    var revision = updateEdgeResult.Value.String("_rev");
}
```

## Replace edge

Completely replaces existing edge identified by its handle with new edge data.

Applicable optional parameters available through fluent API:

- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.
- `IfMatch(string revision)` - Conditionally operate on edge with specified revision.
- `IfMatch(string revision, AUpdatePolicy updatePolicy)` - Conditionally operate on edge with specified revision and update policy.

```csharp
var db = new ADatabase("myDatabaseAlias");

var edgeData = new Dictionary<string, object>()
    .String("foo", "other foo string value")
    .Int("baz", 123);

var replaceEdgeResult = db.Edge
    .Replace("MyEdgeCollection/123", edgeData);
    
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
    .Replace("MyEdgeCollection/123", dummy);
    
if (replaceEdgeResult.Success)
{
    var id = replaceEdgeResult.Value.String("_id");
    var key = replaceEdgeResult.Value.String("_key");
    var revision = replaceEdgeResult.Value.String("_rev");
}
```

## Delete edge

Deletes specified edge.

- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.
- `IfMatch(string revision)` - Conditionally operate on edge with specified revision.
- `IfMatch(string revision, AUpdatePolicy updatePolicy)` - Conditionally operate on edge with specified revision and update policy.

```csharp
var db = new ADatabase("myDatabaseAlias");

var deleteEdgeResult = db.Edge
    .Delete("MyEdgeCollection/123");
    
if (deleteEdgeResult.Success)
{
    var id = deleteEdgeResult.Value.String("_id");
    var key = deleteEdgeResult.Value.String("_key");
    var revision = deleteEdgeResult.Value.String("_rev");
}
```

## More examples

More examples regarding edge operations can be found in [unit tests](../src/Arango/Arango.Tests/EdgeOperations/EdgeOperationsTests.cs).