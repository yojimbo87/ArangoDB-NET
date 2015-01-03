# Basic usage

- [Connection management](#connection-management)
- [Database context](#database-context)
- [ArangoResult object](#arangoresult-object)
- [ArangoError object](#arangoerror-object)
- [JSON representation](#json-representation)
- [Fluent API](#fluent-api)

## Connection management

Driver's public API with its core functionality is exposed through `Arango.Client` namespace. Before the connection is initiated, driver needs to know database address, name and credentials (if needed) which will be stored under specified alias. `ArangoSettings` static class provides several static methods which are used to manage connection data about remote database backends.

```csharp
// adds new connection data to database manager
ArangoSettings.AddConnection(
    "myDatabaseAlias",
    "127.0.0.1",
    8529,
    false,
    "myDatabase",
    "usr",
    "pwd"
);

// checks if the specific connection data exists
if (ArangoSettings.HasConnection("someDatabaseAlias"))
{
    // removes specified connection data
    ArangoSettings.RemoveConnection("someDatabaseAlias");
}
```

## Database context

Previously set database alias is used for the purpose of retrieving data needed for connecting to specific database and performing desired operations. There is no need to dispose `ArangoDatabase` instance in order to free database connection because operations are performed through HTTP calls.

```csharp
// initialize new database context
var db = new ArangoDatabase("myDatabaseAlias");

// retrieve specified document
var getResult = db.Document.Get("myCollection/123");
```

## ArangoResult object

Once the operation is executed, returned data are contained within `ArangoResult` object which consists of following properties:

- `Success` - boolean value indicating if the operation ended with success
- `StatusCode` - integer value of the operation response HTTP status code 
- `Value` - generic object which type and value depends on performed operation
- `Error` - if operation ended with failure, this property would contain instance of `ArangoError` object which contains further information about the error
- `Extra` - `Dictionary<string, object>` which might contain additional information on performed operation

## ArangoError object

In case of operation failure driver doesn't throw exceptions explicitely, but `ArangoResult` object `Error` property would contain instance of `ArangoError` object with following properties:

- `StatusCode` - integer value of the operation response HTTP status code 
- `Number` - integer value indicating [ArangoDB internal error code](https://docs.arangodb.com/ErrorCodes/README.html)
- `Message` - string value containing error description
- `Exception` - exception object with further information about failure

## JSON representation

JSON objects are by default represented as `Dictionary<string, object>`. In order to simplify usage of dictionaries, driver comes equipped with [dictator library](https://github.com/yojimbo87/dictator) which provide helpful set of methods to provide easier way to handle data stored in these objects.

## Fluent API

Driver is heavily using [fluent API](http://en.wikipedia.org/wiki/Fluent_interface) which provides extensive flexiblity to the way how various operations can be executed. Instead of having multiple overloaded methods with bloated set of parameters which can be assigned to given operation, fluent API gives developers ability to apply specific parameter only when needed without the need to obey method signature.

```csharp
// initialize new database context
var db = new ArangoDatabase("myDatabaseAlias");
// operation core
var queryOperation = db.Query
    .Aql("FOR item IN myCollection RETURN item");

// add optional parameter
if (... condition whether to use count query parameter ...)
{
    queryOperation.Count(true);
}

// add another optional parameter
if (... condition whether to use batch size query parameter ...)
{
    queryOperation.BatchSize(1);
}

// execute query operation
var queryResult1 = queryOperation.ToList();

// more concise example of query operation
var queryResult2 = db.Query
    .Count(true)
    .BatchSize(1)
    .Aql("FOR item IN myCollection RETURN item")
    .ToList();
```