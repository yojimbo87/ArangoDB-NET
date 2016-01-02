# Changelog

## Current stable release

### 0.9.3

Tested against ArangoDB 2.6.12, 2.7.3, 2.8.0-beta3

- Faster deserialization of retrieved documents.

### 0.9.2

Tested against ArangoDB 2.5.1

- Implementation of [index operations](docs/IndexOperations.md) API [#15](https://github.com/yojimbo87/ArangoDB-NET/issues/20).
- Enforce enum format for internal operation parameters to prevent potential format override from global settings.

## Previous releases

### 0.9.1

Tested against ArangoDB 2.5.0

- Don't throw exception when accessing single object from AQL query which returns empty list.
- Fixed issue [#15](https://github.com/yojimbo87/ArangoDB-NET/issues/15).
- Fixed issue [#16](https://github.com/yojimbo87/ArangoDB-NET/issues/16).
- Updated dictator library.

### 0.9.0

Tested against ArangoDB 2.4.0

- Complete rewrite.
