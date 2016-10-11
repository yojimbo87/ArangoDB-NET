# Changelog

## Current stable release

### 0.10.2

Tested against ArangoDB 3.0.10

- Implement ExecuteNonQuery operation [#40](https://github.com/yojimbo87/ArangoDB-NET/issues/40).
- Support of serialization and deserialization for nullable types.
- Added option to create connection which enables to use web proxy.

## Previous releases

### 0.10.1

Tested against ArangoDB 3.0.3

- Fixed issue [#34](https://github.com/yojimbo87/ArangoDB-NET/issues/34).

### 0.10.0

Tested against ArangoDB 3.0.0, 3.0.1

- Implement [incompatible changes in ArangoDB 3.0](https://docs.arangodb.com/devel/Manual/ReleaseNotes/UpgradingChanges30.html).
- AEdge class was removed and its functionality is now part of ADocument class.
- This is release is not backwards compatible with ArangoDB 2.x versions.

### 0.9.4

Tested against ArangoDB 2.8.1

- ~50% performance increase when creating, updating and replacing documents/edges from generic object.

### 0.9.3

Tested against ArangoDB 2.6.12, 2.7.3, 2.8.0-beta3

- Faster deserialization of retrieved documents.

### 0.9.2

Tested against ArangoDB 2.5.1

- Implementation of [index operations](docs/IndexOperations.md) API [#15](https://github.com/yojimbo87/ArangoDB-NET/issues/20).
- Enforce enum format for internal operation parameters to prevent potential format override from global settings.

### 0.9.1

Tested against ArangoDB 2.5.0

- Don't throw exception when accessing single object from AQL query which returns empty list.
- Fixed issue [#15](https://github.com/yojimbo87/ArangoDB-NET/issues/15).
- Fixed issue [#16](https://github.com/yojimbo87/ArangoDB-NET/issues/16).
- Updated dictator library.

### 0.9.0

Tested against ArangoDB 2.4.0

- Complete rewrite.
