# Document operations

- [Create document](#create-document)
- [Check document existence](#check-document-existence)
- [Retrieve document](#retrieve-document)
- [Update document](#update-document)
- [Replace document](#replace-document)
- [Delete document](#delete-document)
- [Static methods](#static-methods)
- [More examples](#more-examples)

Document operations are focused on management of documents in document type collections. These operations are accessible through `Document` property in database context object.

## Create document

Creates new document within specified collection in current database context.

Applicable optional parameters available through fluent API:

- `CreateCollection(bool value)` - Determines whether collection should be created if it does not exist. Default value: false.
- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.

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

## Check document existence

Checks for existence of specified document.

Applicable optional parameters available through fluent API:

- `IfMatch(string revision)` - Conditionally operate on document with specified revision.
- `IfMatch(string revision, AUpdatePolicy updatePolicy)` - Conditionally operate on document with specified revision and update policy.
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

## Update document

Updates existing document identified by its handle with new document data.

Applicable optional parameters available through fluent API:

- `IfMatch(string revision)` - Conditionally operate on document with specified revision.
- `IfMatch(string revision, AUpdatePolicy updatePolicy)` - Conditionally operate on document with specified revision and update policy.
- `KeepNull(bool value)` - Determines whether to keep any attributes from existing document that are contained in the patch document which contains null value. Default value: true.
- `MergeArrays(bool value)` - Determines whether the value in the patch document will overwrite the existing document's value. Default value: true.

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
- `IfMatch(string revision, AUpdatePolicy updatePolicy)` - Conditionally operate on document with specified revision and update policy.

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

## Delete document

Deletes specified document.

- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.
- `IfMatch(string revision)` - Conditionally operate on document with specified revision.
- `IfMatch(string revision, AUpdatePolicy updatePolicy)` - Conditionally operate on document with specified revision and update policy.

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

## Static methods

`ADocument` class provides following static methods:

- `ADocument.IsID(string id)` - Determines if specified value has valid document `_id` format. 
- `ADocument.IsKey(string key)` - Determines if specified value has valid document `_key` format.
- `ADocument.IsRev(string id)` - Determines if specified value has valid document `_rev` format.
- `ADocument.Identify(string collection, long key)` - Constructs document ID from specified collection and key values.
- `ADocument.Identify(string collection, string key)` - Constructs document ID from specified collection and key values. If key format is invalid null value is returned.

## More examples

More examples regarding document operations can be found in [unit tests](../src/Arango/Arango.Tests/DocumentOperations/DocumentOperationsTests.cs).