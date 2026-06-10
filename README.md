![ChainSharp](ChainSharp/icon.png)

A functional chaining library allowing you to chain both synchronous and asynchronous functions together. Enables writing idiomatic lambdas and closures without deeply nested functions or unnecessary bindings.

## Usage

```
Func<HttpListenerContext, User?> GetUserFromRequest = Chain.Init<HttpListenerContext>()
    .Then(x => x.Request)
    .Then(ValidateRequest) // throws if failed auth, otherwise returns value
    .Then(x => x.QueryString)
    .Then(x => x["userId"])
    .Then(ValidateNotNull) // throws if null, otherwise returns value
    .Then(int.Parse)
    .Then(GetUser); // null if no user found
```

Asynchronous functions / tasks can be chained together similarly, using `.WaitThen()`, which will evaluate the return value of the task once completed. e.g.

```
Func<int, Task<string>> GetEmployeeName = Chain.Init<int>
    .Then(DB.GetEmployeeById)
    .WaitThen(employee => employee.Name);

await GetEmployeeName(2310); => David
```

### Null Handling

Chaining can be combined with monads or wrappers for graceful handling of errors or null values mid-chain. C\#hainSharp provides out of the box `Nullable<T>` monad support through `.ThenOrNull()`, which provides graceful null coalescence. e.g.

```
int? GetUserIdForEmail(Email email) { ... }
string GetNameForUser(int userId) { ... }

Func<Email, string?> GetUserNameForEmail = Chain<Email>()
    .Then(GetUserIdForEmail)
        // no need to write a custom version of GetNameForUser to handle null values
    .ThenOrNull(GetNameForUser); 
```

### Error Handling

Similarly, using a custom monad can be useful for handling errors mid-chain. For example, in middleware or API routing, you could define a monad e.g. `ValueOrHttpError<T>` that can be used to catch specific exceptions and pass them through to the end of the chain for handling by the server loop.
