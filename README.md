# ArangoDB-NET

ArangoDB-NET is a C# driver for [ArangoDB](https://www.arangodb.com/) NoSQL multi-model database. Driver implements and communicates with database backend through its [HTTP API](https://docs.arangodb.com/HttpApi/README.html) interface and runs on Microsoft .NET and mono framework.

## Docs contents

- [Installation](#installation)
- [Connection management](#connection-management)
- [Basic usage](#basic-usage)
  - [ArangoResult object](#arangoresult-object)
  - [ArangoError object](#arangoerror-object)
  - [JSON representation](#json-representation)
- [Database operations](#database-operations)
  - [Create database](#create-database)
  - [Retrieve current database](#retrieve-current-database)
  - [Retrieve accessible databases](#retrieve-accessible-databases)
  - [Retrieve all databases](#retrieve-all-databases)
  - [Delete database](#delete-database)
- [Collection operations]()
- [Document operations]()
- [Edge operations]()
- [AQL query cursors execution]()
- [AQL user functions management]()

## Installation

There are following ways to install the driver:

- download and install [nuget package]() which contains latest stable version
- clone ArangoDB-NET [repository](https://github.com/yojimbo87/ArangoDB-NET) and build [master branch](https://github.com/yojimbo87/ArangoDB-NET/tree/master) to have latest stable version or [devel branch](https://github.com/yojimbo87/ArangoDB-NET/tree/devel) to have latest experimental version

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

## Basic usage

Previously set database alias is used for the purpose of retrieving data needed for connecting to specific database and performing desired operations.

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var getResult = db.Document.Get("myCollection/123");
```

### ArangoResult object

Once the operation is executed, returned data are contained within `ArangoResult` object which consists of following properties:

- `Success` - boolean value indicating if the operation ended with success
- `StatusCode` - integer value of the operation response HTTP status code 
- `Value` - generic object which type and value depends on performed operation
- `Error` - if operation ended with failure, this property would contain instance of `ArangoError` object which contains further information about the error
- `Extra` - `Dictionary<string, object>` which might contain additional information on performed operation

### ArangoError object

In case of operation failure driver doesn't throw exceptions explicitely, but `ArangoResult` object `Error` property would contain instance of `ArangoError` object with following properties:

- `StatusCode` - integer value of the operation response HTTP status code 
- `Number` - integer value indicating [ArangoDB internal error code](https://docs.arangodb.com/ErrorCodes/README.html)
- `Message` - string value containing error description
- `Exception` - exception object with further information about failure

### JSON representation

JSON objects are by default represented as `Dictionary<string, object>`. In order to simplify usage of dictionaries, driver comes equipped with [dictator library](https://github.com/yojimbo87/dictator) which provide helpful set of methods to provide easier way to handle data stored in these objects.

## Database operations

Database management operations can only be accessed via the default `_system` database.

### Create database

```csharp
var db = new ArangoDatabase("systemDatabaseAlias");

// creates new database
var createDatabaseResult1 = db.Create("myDatabase1");

// creates another new database with specified users
var users = new List<ArangoUser>()
{
    new ArangoUser { Username = "admin", Password = "secret", Active = true },
    new ArangoUser { Username = "tester001", Password = "test001", Active = false } 
};

var createDatabaseResult2 = db.Create("myDatabase2", users), 
```

### Retrieve current database

```csharp
var db = new ArangoDatabase("systemDatabaseAlias");

var currentDatabaseResult = db.GetCurrent();

if (currentDatabaseResult.Success)
{
    var name = currentDatabaseResult.Value.String("name");
    var id = currentDatabaseResult.Value.String("id");
    var path = currentDatabaseResult.Value.String("path");
    var isSystem = currentDatabaseResult.Value.Bool("isSystem");
}
```

### Retrieve accessible databases

```csharp
var db = new ArangoDatabase("systemDatabaseAlias");

var accessibleDatabasesResult = db.GetAccessibleDatabases();

if (accessibleDatabasesResult.Success)
{
    foreach (var database in accessibleDatabasesResult.Value)
    {
        var name = database;
    }
}
```

### Retrieve all databases

```csharp
var db = new ArangoDatabase("systemDatabaseAlias");

var allDatabasesResult = db.GetAllDatabases();

if (allDatabasesResult.Success)
{
    foreach (var database in allDatabasesResult.Value)
    {
        var name = database;
    }
}
```

### Delete database

```csharp
var db = new ArangoDatabase("systemDatabaseAlias");

var deleteDatabaseResult = db.Drop("myDatabase1");

if (deleteDatabaseResult.Success)
{
    var isDeleted = deleteDatabaseResult.Value;
}
```