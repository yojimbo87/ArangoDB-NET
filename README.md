# ArangoDB-NET

ArangoDB-NET is a C# driver for [ArangoDB](https://www.arangodb.com/) NoSQL multi-model database. Driver implements and communicates with database backend through its [HTTP API](https://docs.arangodb.com/HttpApi/README.html) interface and runs on Microsoft .NET and mono framework.

## Installation

There are following ways to install the driver:

- download and install [nuget package]() which contains latest stable version
- clone ArangoDB-NET [repository](https://github.com/yojimbo87/ArangoDB-NET) and build [master branch](https://github.com/yojimbo87/ArangoDB-NET/tree/master) to have latest stable version or [devel branch](https://github.com/yojimbo87/ArangoDB-NET/tree/devel) to have latest experimental version

## Docs contents

- [Basic usage](docs/BasicUsage.md)
  - [Connection management](docs/BasicUsage.md#connection-management)
  - [Database context](docs/BasicUsage.md#database-context)
  - [ArangoResult object](docs/BasicUsage.md#arangoresult-object)
  - [ArangoError object](docs/BasicUsage.md#arangoerror-object)
  - [JSON representation](docs/BasicUsage.md#json-representation)
  - [Fluent API](docs/BasicUsage.md#fluent-api)
- [Database operations](docs/DatabaseOperations.md)
  - [Create database](docs/DatabaseOperations.md#create-database)
  - [Retrieve current database](docs/DatabaseOperations.md#retrieve-current-database)
  - [Retrieve accessible databases](docs/DatabaseOperations.md#retrieve-accessible-databases)
  - [Retrieve all databases](docs/DatabaseOperations.md#retrieve-all-databases)
  - [Retrieve database collections](docs/DatabaseOperations.md#retrieve-database-collections)
  - [Delete database](docs/DatabaseOperations.md#delete-database)
  - [More examples](docs/DatabaseOperations.md#more-examples)
- [Collection operations](docs/CollectionOperations.md)
  - [Create collection](docs/CollectionOperations.md#create-collection)
  - [Retrieve collection](docs/CollectionOperations.md#retrieve-collection)
  - [Retrieve collection properties](docs/CollectionOperations.md#retrieve-collection-properties)
  - [Retrieve collection count](docs/CollectionOperations.md#retrieve-collection-count)
  - [Retrieve collection figures](docs/CollectionOperations.md#retrieve-collection-figures)
  - [Retrieve collection revision](docs/CollectionOperations.md#retrieve-collection-revision)
  - [Retrieve collection checksum](docs/CollectionOperations.md#retrieve-collection-checksum)
  - [Retrieve all documents](docs/CollectionOperations.md#retrieve-all-documents)
  - [Retrieve all edges](docs/CollectionOperations.md#retrieve-all-edges)
  - [Truncate collection](docs/CollectionOperations.md#truncate-collection)
  - [Load collection](docs/CollectionOperations.md#load-collection)
  - [Unload collection](docs/CollectionOperations.md#unload-collection)
  - [Change collection properties](docs/CollectionOperations.md#change-collection-properties)
  - [Rename collection](docs/CollectionOperations.md#rename-collection)
  - [Rotate collection journal](docs/CollectionOperations.md#rotate-collection-journal)
  - [Delete collection](docs/CollectionOperations.md#delete-collection)
  - [More examples](docs/CollectionOperations.md#more-examples)
- [Document operations](docs/DocumentOperations.md)
  - [Create document](docs/DocumentOperations.md#create-document)
  - [Check document existence](docs/DocumentOperations.md#check-document-existence)
  - [Retrieve document](docs/DocumentOperations.md#retrieve-document)
  - [Update document](docs/DocumentOperations.md#update-document)
  - [Replace document](docs/DocumentOperations.md#replace-document)
  - [Delete document](docs/DocumentOperations.md#delete-document)
  - [More examples](docs/DocumentOperations.md#more-examples)
- [Edge operations](docs/EdgeOperations.md)
  - [Create edge](docs/EdgeOperations.md#create-edge)
  - [Check edge existence](docs/EdgeOperations.md#check-edge-existence)
  - [Retrieve edge](docs/EdgeOperations.md#retrieve-edge)
  - [Retrieve vertex edges](docs/EdgeOperations.md#retrieve-vertex-edges)
  - [Update edge](docs/EdgeOperations.md#update-edge)
  - [Replace edge](docs/EdgeOperations.md#replace-edge)
  - [Delete edge](docs/EdgeOperations.md#delete-edge)
  - [More examples](docs/EdgeOperations.md#more-examples)
- [AQL query cursors execution]()
- [AQL user functions management](docs/FunctionOperations.md)
  - [Register function](docs/FunctionOperations.md#register-function)
  - [Retrieve function list](docs/FunctionOperations.md#retrieve-function-list)
  - [Unregister function](docs/FunctionOperations.md#unregister-function)
  - [More examples](docs/FunctionOperations.md#more-examples)
