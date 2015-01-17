# Transaction operations

- [Execute transaction](#execute-transaction)

Transaction operations are focused on executing transactions. These operations are accessible through `Transaction` property in database context object.

## Execute transaction

Executes specified transaction.

Applicable optional parameters available through fluent API:

- `ReadCollection(string collectionName)` - Maps read collection to current transaction.
- `WriteCollection(string collectionName)` - Maps write collection to current transaction.
- `WaitForSync(bool value)` - Determines whether to wait until data are synchronised to disk. Default value: false.
- `LockTimeout(int value)` - Determines a numeric value that can be used to set a timeout for waiting on collection locks. Setting value to 0 will make ArangoDB not time out waiting for a lock.
- `Param(string key, object value)` - Maps key/value parameter to current transaction.

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var transactionResult = db.Transaction
    .WriteCollection("products")
    .Execute<int>(@"
    function () { 
        var db = require('internal').db; 

        db.products.save({ });

        return db.products.count(); 
    }
    ");
    
if (transactionResult.Success)
{
    var documentsCount = transactionResult.Value;
}
```