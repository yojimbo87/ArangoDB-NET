# Foxx operations

- [Get request](#get-request)
- [Post request](#post-request)
- [Put request](#put-request)
- [Patch request](#patch-request)
- [Delete request](#delete-request)

Foxx operations are focused on management of [Foxx](https://docs.arangodb.com/latest/Manual/Foxx/index.html) services which are accessible through `Foxx` property in database context object.

## Get request

Sends GET request to specified foxx service location.

```csharp
var db = new ADatabase("myDatabaseAlias");

var getRequestResult = db.Foxx.Get<Dictionary<string, object>>("/getting-started/hello-world");
    
if (getRequestResult.Success)
{
    var deserializedBodyDocument = getRequestResult.Value;
}
```

## Post request

Sends POST request to specified foxx service location.

Applicable optional parameters available through fluent API:

- `Body(object value)` - Serializes specified value as JSON object into request body.

```csharp
var db = new ADatabase("myDatabaseAlias");

var body = Dictator.New()
    .String("foo", "some string");

var postRequestResult = db.Foxx
    .Body(body)
    .Post<Dictionary<string, object>>("/getting-started/hello-world");
    
if (postRequestResult.Success)
{
    var deserializedBodyDocument = postRequestResult.Value;
}
```

## Put request

Sends PUT request to specified foxx service location.

Applicable optional parameters available through fluent API:

- `Body(object value)` - Serializes specified value as JSON object into request body.

```csharp
var db = new ADatabase("myDatabaseAlias");

var body = Dictator.New()
    .String("foo", "some string");

var putRequestResult = db.Foxx
    .Body(body)
    .Put<Dictionary<string, object>>("/getting-started/hello-world");
    
if (putRequestResult.Success)
{
    var deserializedBodyDocument = putRequestResult.Value;
}
```

## Patch request

Sends PATCH request to specified foxx service location.

Applicable optional parameters available through fluent API:

- `Body(object value)` - Serializes specified value as JSON object into request body.

```csharp
var db = new ADatabase("myDatabaseAlias");

var body = Dictator.New()
    .String("foo", "some string");

var patchRequestResult = db.Foxx
    .Body(body)
    .Patch<Dictionary<string, object>>("/getting-started/hello-world");
    
if (patchRequestResult.Success)
{
    var deserializedBodyDocument = patchRequestResult.Value;
}
```

## Delete request

Sends DELETE request to specified foxx service location.

Applicable optional parameters available through fluent API:

- `Body(object value)` - Serializes specified value as JSON object into request body.

```csharp
var db = new ADatabase("myDatabaseAlias");

var body = Dictator.New()
    .String("foo", "some string");

var deleteRequestResult = db.Foxx
    .Body(body)
    .Delete<Dictionary<string, object>>("/getting-started/hello-world");
    
if (deleteRequestResult.Success)
{
    var deserializedBodyDocument = deleteRequestResult.Value;
}
```