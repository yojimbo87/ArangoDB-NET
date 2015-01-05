# AQL user functions management

- [Register function](#register-function)
- [Retrieve function list](#retrieve-function-list)
- [Unregister function](#unregister-function)

Function operations are focused on AQL user functions management operations. These operations are accessible through `Function` property in database context object.

## Register function

Creates new or replaces existing AQL user function with specified name and code.

Applicable optional parameters available through fluent API:

- `IsDeterministic(bool value)` - Determines whether function return value solely depends on the input value and return value is the same for repeated calls with same input. This parameter is currently not applicable and may be used in the future for optimisation purpose.

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var registerFunctionResult = db.Function.Register(
    "myfunctions::temperature::celsiustofahrenheit", 
    "function (celsius) { return celsius * 1.8 + 32; }"
);
    
if (registerFunctionResult.Success)
{
    var isFunctionRegistered = registerFunctionResult.Value;
}
```

## Retrieve function list

Retrieves list of registered AQL user functions.

Applicable optional parameters available through fluent API:

- `Namespace(string value)` - Determines optional namespace from which to return all registered AQL user functions.

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var functionListResult = db.Function.List();
    
if (functionListResult.Success)
{
    foreach (var functionData in functionListResult.Value)
    {
        var functionName = functionData.String("name");
        var functionCode = functionData.String("code");
    }
}
```

## Unregister function

Unregisters specified AQL user function.

Applicable optional parameters available through fluent API:

- `Group(bool value)` - Determines whether the function name is treated as a namespace prefix, and all functions in the specified namespace will be deleted. If set to false, the function name provided in name must be fully qualified, including any namespaces. Default value: false.

```csharp
var db = new ArangoDatabase("myDatabaseAlias");

var unregisterFunctionResult = db.Function.Unregister("myfunctions::temperature::celsiustofahrenheit");
    
if (unregisterFunctionResult.Success)
{
    var isFunctionUnregistered = unregisterFunctionResult.Value;
}
```