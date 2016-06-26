# Collection operations

- [Create collection](#create-collection)
- [Retrieve collection](#retrieve-collection)
- [Retrieve collection properties](#retrieve-collection-properties)
- [Retrieve collection count](#retrieve-collection-count)
- [Retrieve collection figures](#retrieve-collection-figures)
- [Retrieve collection revision](#retrieve-collection-revision)
- [Retrieve collection checksum](#retrieve-collection-checksum)
- [Retrieve all indexes](#retrieve-all-indexes)
- [Truncate collection](#truncate-collection)
- [Load collection](#load-collection)
- [Unload collection](#unload-collection)
- [Change collection properties](#change-collection-properties)
- [Rename collection](#rename-collection)
- [Rotate collection journal](#rotate-collection-journal)
- [Delete collection](#delete-collection)
- [More examples](#more-examples)

Collection operations are focused on management of document and edge type collections which are accessible through `Collection` property in database context object.

## Create collection

Creates new collection in current database context.

Applicable optional parameters available through fluent API:

- `Type(ACollectionType value)` - Determines type of the collection. Default value: Document.
- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.
- `JournalSize(long value)` - Determines maximum size of a journal or datafile in bytes. Default value: server configured.
- `DoCompact(bool value)` - Determines whether the collection will be compacted. Default value: true.
- `IsSystem(bool value)` - Determines whether the collection is a system collection. Default value: false.
- `IsVolatile(bool value)` - Determines whether the collection data is kept in-memory only and not made persistent. Default value: false.
- `KeyGeneratorType(AKeyGeneratorType value)` - Determines the type of the key generator. Default value: Traditional.
- `AllowUserKeys(bool value)` - Determines whether it is allowed to supply custom key values in the _key attribute of a document. Default value: true.
- `KeyIncrement(long value)` - Determines increment value for autoincrement key generator.
- `KeyOffset(long value)` - Determines initial offset value for autoincrement key generator.
- `NumberOfShards(int value)` - Determines the number of shards to create for the collection in cluster environment. Default value: 1.
- `ShardKeys(List<string> value)` - Determines which document attributes are used to specify the target shard for documents in cluster environment. Default value: ["_key"].

```csharp
var db = new ADatabase("myDatabaseAlias");

// creates new document type collection
var createCollectionResult = db.Collection
    .KeyGeneratorType(AKeyGeneratorType.Autoincrement)
    .WaitForSync(true)
    .Create("MyDocumentCollection");
    
if (createCollectionResult.Success)
{
    var id = createCollectionResult.Value.String("id");
    var name = createCollectionResult.String("name");
    var waitForSync = createCollectionResult.Value.Bool("waitForSync");
    var isVolatile = createCollectionResult.Value.Bool("isVolatile");
    var isSystem = createCollectionResult.Value.Bool("isSystem");
    var status = createCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = createCollectionResult.Value.Enum<ACollectionType>("type");
}
```

## Retrieve collection

Retrieves basic information about specified collection.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getCollectionResult = db.Collection
    .Get("MyDocumentCollection");

if (getCollectionResult.Success)
{
    var id = getCollectionResult.Value.String("id");
    var name = getCollectionResult.Value.String("name");
    var isSystem = getCollectionResult.Value.Bool("isSystem");
    var status = getCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = getCollectionResult.Value.Enum<ACollectionType>("type");
}
```

## Retrieve collection properties

Retrieves basic information with additional properties about specified collection.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getCollectionResult = db.Collection
    .GetProperties("MyDocumentCollection");

if (getCollectionResult.Success)
{
    var id = getCollectionResult.Value.String("id");
    var name = getCollectionResult.Value.String("name");
    var waitForSync = getCollectionResult.Value.Bool("waitForSync");
    var isVolatile = getCollectionResult.Value.Bool("isVolatile");
    var isSystem = getCollectionResult.Value.Bool("isSystem");
    var status = getCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = getCollectionResult.Value.Enum<ACollectionType>("type");
    var doCompact = getCollectionResult.Value.Bool("doCompact");
    var journalSize = getCollectionResult.Value.Long("journalSize");
    var keyGeneratorType = getCollectionResult.Value.Enum<AKeyGeneratorType>("keyOptions.type");
    var allowUserKeys = getCollectionResult.Value.Bool("keyOptions.allowUserKeys");
}
```

## Retrieve collection count

Retrieves basic information with additional properties and document count in specified collection.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getCollectionResult = db.Collection
    .GetCount("MyDocumentCollection");

if (getCollectionResult.Success)
{
    var id = getCollectionResult.Value.String("id");
    var name = getCollectionResult.Value.String("name");
    var waitForSync = getCollectionResult.Value.Bool("waitForSync");
    var isVolatile = getCollectionResult.Value.Bool("isVolatile");
    var isSystem = getCollectionResult.Value.Bool("isSystem");
    var status = getCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = getCollectionResult.Value.Enum<ACollectionType>("type");
    var doCompact = getCollectionResult.Value.Bool("doCompact");
    var journalSize = getCollectionResult.Value.Long("journalSize");
    var keyGeneratorType = getCollectionResult.Value.Enum<AKeyGeneratorType>("keyOptions.type");
    var allowUserKeys = getCollectionResult.Value.Bool("keyOptions.allowUserKeys");
    var count = getCollectionResult.Value.Long("count");
}
```

## Retrieve collection figures

Retrieves basic information with additional properties, document count and figures in specified collection.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getCollectionResult = db.Collection
    .GetFigures("MyDocumentCollection");

if (getCollectionResult.Success)
{
    var id = getCollectionResult.Value.String("id");
    var name = getCollectionResult.Value.String("name");
    var waitForSync = getCollectionResult.Value.Bool("waitForSync");
    var isVolatile = getCollectionResult.Value.Bool("isVolatile");
    var isSystem = getCollectionResult.Value.Bool("isSystem");
    var status = getCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = getCollectionResult.Value.Enum<ACollectionType>("type");
    var doCompact = getCollectionResult.Value.Bool("doCompact");
    var journalSize = getCollectionResult.Value.Long("journalSize");
    var keyGeneratorType = getCollectionResult.Value.Enum<AKeyGeneratorType>("keyOptions.type");
    var allowUserKeys = getCollectionResult.Value.Bool("keyOptions.allowUserKeys");
    var count = getCollectionResult.Value.Long("count");
    var figures = getCollectionResult.Value.Document("figures");
}
```

## Retrieve collection revision

Retrieves basic information and revision ID of specified collection.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getCollectionResult = db.Collection
    .GetRevision("MyDocumentCollection");

if (getCollectionResult.Success)
{
    var id = getCollectionResult.Value.String("id");
    var name = getCollectionResult.Value.String("name");
    var isSystem = getCollectionResult.Value.Bool("isSystem");
    var status = getCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = getCollectionResult.Value.Enum<ACollectionType>("type");
    var revision = getCollectionResult.Value.String("revision");
}
```

## Retrieve collection checksum

Retrieves basic information, revision ID and checksum of specified collection.

Applicable optional parameters available through fluent API:

- `WithRevisions(bool value)` - Determines whether to include document revision ids in the checksum calculation. Default value: false.
- `WithData(bool value)` - Determines whether to include document body data in the checksum calculation. Default value: false.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getCollectionResult = db.Collection
    .GetChecksum("MyDocumentCollection");

if (getCollectionResult.Success)
{
    var id = getCollectionResult.Value.String("id");
    var name = getCollectionResult.Value.String("name");
    var isSystem = getCollectionResult.Value.Bool("isSystem");
    var status = getCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = getCollectionResult.Value.Enum<ACollectionType>("type");
    var revision = getCollectionResult.Value.String("revision");
    var checksum = getCollectionResult.Value.Long("checksum");
}
```

## Retrieve all indexes

Retrieves indexes in specified collection.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getIndexesResult = db.Collection
    .GetAllIndexes("MyEdgeCollection");

if (getIndexesResult.Success)
{
    foreach (var index in getIndexesResult.Value.List<Dictionary<string, object>>("indexes"))
    {
        var indexID = index.String("id");
    }
}
```

## Truncate collection

Removes all documents from specified collection.

```csharp
var db = new ADatabase("myDatabaseAlias");

var truncateCollectionResult = db.Collection
    .Truncate("MyDocumentCollection");

if (truncateCollectionResult.Success)
{
    var id = truncateCollectionResult.Value.String("id");
    var name = truncateCollectionResult.Value.String("name");
    var isSystem = truncateCollectionResult.Value.Bool("isSystem");
    var status = truncateCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = truncateCollectionResult.Value.Enum<ACollectionType>("type");
}
```

## Load collection

Loads specified collection into memory.

Applicable optional parameters available through fluent API:

- `Count(bool value)` - Determines whether the return value should include the number of documents in collection. Default value: true.

```csharp
var db = new ADatabase("myDatabaseAlias");

var loadCollectionResult = db.Collection
    .Count(false)
    .Load("MyDocumentCollection");

if (loadCollectionResult.Success)
{
    var id = loadCollectionResult.Value.String("id");
    var name = loadCollectionResult.Value.String("name");
    var isSystem = loadCollectionResult.Value.Bool("isSystem");
    var status = loadCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = loadCollectionResult.Value.Enum<ACollectionType>("type");
}
```

## Unload collection

Unloads specified collection from memory.

```csharp
var db = new ADatabase("myDatabaseAlias");

var unloadCollectionResult = db.Collection
    .Unload("MyDocumentCollection");

if (unloadCollectionResult.Success)
{
    var id = unloadCollectionResult.Value.String("id");
    var name = unloadCollectionResult.Value.String("name");
    var isSystem = unloadCollectionResult.Value.Bool("isSystem");
    var status = unloadCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = unloadCollectionResult.Value.Enum<ACollectionType>("type");
}
```

## Change collection properties

Changes properties of specified collection.

Applicable optional parameters available through fluent API:

- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.
- `JournalSize(long value)` - Determines maximum size of a journal or datafile in bytes. Default value: server configured.

```csharp
var db = new ADatabase("myDatabaseAlias");

var changeCollectionResult = db.Collection
    .WaitForSync(true)
    .JournalSize(1999999999)
    .ChangeProperties("MyDocumentCollection");

if (changeCollectionResult.Success)
{
    var id = changeCollectionResult.Value.String("id");
    var name = changeCollectionResult.Value.String("name");
    var waitForSync = changeCollectionResult.Value.Bool("waitForSync");
    var isVolatile = changeCollectionResult.Value.Bool("isVolatile");
    var isSystem = changeCollectionResult.Value.Bool("isSystem");
    var status = changeCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = changeCollectionResult.Value.Enum<ACollectionType>("type");
    var doCompact = changeCollectionResult.Value.Bool("doCompact");
    var journalSize = changeCollectionResult.Value.Long("journalSize");
    var keyGeneratorType = changeCollectionResult.Value.Enum<AKeyGeneratorType>("keyOptions.type");
    var allowUserKeys = changeCollectionResult.Value.Bool("keyOptions.allowUserKeys");
}
```

## Rename collection

Renames specified collection.

```csharp
var db = new ADatabase("myDatabaseAlias");

var renameCollectionResult = db.Collection
    .Rename("MyDocumentCollection", "MyFooCollection");

if (renameCollectionResult.Success)
{
    var id = renameCollectionResult.Value.String("id");
    var name = renameCollectionResult.Value.String("name");
    var isSystem = renameCollectionResult.Value.Bool("isSystem");
    var status = renameCollectionResult.Value.Enum<ACollectionStatus>("status");
    var type = renameCollectionResult.Value.Enum<ACollectionType>("type");
}
```

## Rotate collection journal

Rotates the journal of specified collection to make the data in the file available for compaction. Current journal of the collection will be closed and turned into read-only datafile. This operation is not available in cluster environment.

```csharp
var db = new ADatabase("myDatabaseAlias");

var rotateJournalResult = db.Collection
    .RotateJournal("MyDocumentCollection");
```

## Delete collection

Deletes specified collection.

```csharp
var db = new ADatabase("myDatabaseAlias");

var deleteCollectionResult = db.Collection
    .Delete("MyDocumentCollection");

if (deleteCollectionResult.Success)
{
    var id = deleteCollectionResult.Value.String("id");
}
```

## More examples

More examples regarding collection operations can be found in [unit tests](../src/Arango/Arango.Tests/CollectionOperations/CollectionOperationsTests.cs).
