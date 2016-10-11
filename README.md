Dapper.AmbientContext
=======

[![Build status](https://ci.appveyor.com/api/projects/status/omt8ahl09xbnp67t?svg=true)](https://ci.appveyor.com/project/sakopov/dapper-ambientcontext)
[![Test status](http://teststatusbadge.azurewebsites.net/api/status/sakopov/dapper-ambientcontext)]
(https://ci.appveyor.com/project/sakopov/dapper-ambientcontext)

Ambient context implementation for Dapper.NET.

Ambient Context is a little-known pattern that is frequently used by the .NET framework to address cross-cutting concerns such as security (Thread.CurrentPrincipal) and Logging (Trance & TraceListeners). The general idea is that your application sets up a context at start-up of an operation or a request which becomes available to the rest of the system via a static property or method of some sort.

Dapper.AmbientContext uses the same approach to treat the database connection and transaction as resource which is seemlessly shared between various data access components of a system. This pattern works really well with the service layer which consumes multiple data access repositories, commands and queries to perform various database tasks.

##Installation

Install Dapper.AmbientContext with NuGet

```
Install-Package Dapper.AmbientContext
```

This project is still a work in progress and i'm working hard on making the first release :)

##Configuration

Create a database connection factory for your particular database engine. Below is an example for SQL Server.

```csharp
public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Create()
    {
        return new SqlConnection(_connectionString);
    }
}
```

Configure ambient storage strategy (typically at the start-up of your application). The default is `LogicalCallContextStorage` which uses logical `CallContext` to persist ambient database context. You can create a custom storage strategy by implementing `IContextualStorage` interface.

```csharp
AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
```

Create an abstract repository which will take `IAmbientDbContextLocator` as the dependency to resolve an active ambient database context that can be used to run queries against your database engine. The same pattern can apply to commands or queries in your data access layer.

```csharp
public abstract class AbstractAmbientRepository
{
    private readonly IAmbientDbContextLocator _ambientDbContextLocator;

    protected AmbientRepository(IAmbientDbContextLocator ambientDbContextLocator)
    {
        _ambientDbContextLocator = ambientDbContextLocator;
    }

    protected IAmbientDbContextQueryProxy Context
    {
        get { return _ambientDbContextLocator.Get(); }
    }
}
```

Implement your custom repository type which inherits from previously created `AbstractAmbientRepository` and use `Context` to execute queries. You have full access to Dapper query interface both for synchronous and asynchronous queries. 

```csharp
public class MyRepository : AbstractAmbientRepository
{
    public PostLogsRepository(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
    {
    }

    public void DoSomething()
    {
        Context.Execute("SELECT ...");
    }
}
```

Create a service class and execute your repository. Note, you could easily use your favorite flavor or DI container to inject all dependencies here. 

```csharp
public class MyService 
{
  public void DoSomeWork()
  {
    var connectionFactory = new SqlConnectionFactory("connection-string");
    
    using (var context = new AmbientDbContextFactory(connectionFactory).Create())
    {
      new MyRepository(new AmbientDbContextLocator()).DoSomething();

      context.Commit();
    }
  }
}
```

If you're nesting ambient database context within another, you will join the parent database context by default. This will automatically inherit the database connection and all transaction properties of the parent database context. 

```csharp
public class MyService 
{
  public void DoSomeWork()
  {
    var connectionFactory = new SqlConnectionFactory("connection-string");
    
    using (var context = new AmbientDbContextFactory(connectionFactory).Create())
    {
      // context2 inherits context
      using (var context2 = new AmbientDbContextFactory(connectionFactory).Create())
      {
        new MyRepository(new AmbientDbContextLocator()).DoSomething();
        
        context2.Commit(); // does nothing until parent scope is committed
      }
      
      context.Commit(); // commits everything within its scope
    }
  }
}
```

Note, while this isn't recommended you could nest an ambient database context within another one and opt out of joining the parent context. You can do so by calling `AmbientDbContextFactory.Create(join: false)` which will force the context to spawn its own database connection and transaction (if needed). The default for this option is `true`, which will always join the parent or create the parent if there isn't one already.

```csharp
public class MyService 
{
  public void DoSomeWork()
  {
    var connectionFactory = new SqlConnectionFactory("connection-string");
    
    using (var context = new AmbientDbContextFactory(connectionFactory).Create())
    {
      // context2 is now separate from context
      using (var context2 = new AmbientDbContextFactory(connectionFactory).Create(join: false))
      {
        new MyRepository(new AmbientDbContextLocator()).DoSomething();
        
        context2.Commit(); // commits everything within its scope
      }
      
      context.Commit(); // does nothing
    }
  }
}
```

You can also specify whether you want transactional support via the `suppress` parameter and the transaction isolation level via `isolationLevel`. The defaults are `false` and `IsolationLevel.ReadCommitted` respectively.
