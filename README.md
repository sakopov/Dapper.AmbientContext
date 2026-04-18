# Dapper.AmbientContext

[![CI/CD](https://github.com/sakopov/Dapper.AmbientContext/actions/workflows/ci.yml/badge.svg)](https://github.com/sakopov/Dapper.AmbientContext/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Dapper.AmbientContext.svg)](https://www.nuget.org/packages/Dapper.AmbientContext)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

*Ambient context implementation for Dapper with automatic transaction management.*

## Overview

Dapper.AmbientContext implements the ambient context pattern to seamlessly manage database connections and transactions across multiple data access components. It allows service layers to compose multiple repositories, commands, and queries that automatically participate in the same transaction without explicit connection or transaction passing.

### Key Features

- ✅ **Automatic transaction management** - Share connections and transactions across components
- ✅ **Thread-safe** - Safe for concurrent operations
- ✅ **Nested contexts** - Support for parent-child context hierarchies
- ✅ **Cross-platform** - Works on .NET Framework, .NET Core, and .NET 5+

## Installation

Install via NuGet:

```bash
dotnet add package Dapper.AmbientContext
```

Or using Package Manager Console:

```powershell
Install-Package Dapper.AmbientContext
```

## Requirements

- **.NET Framework** 4.6.2 or higher
- **.NET Core** 2.0 or higher
- **.NET** 5.0 or higher

**Note:** For .NET Framework 4.5.1 - 4.6.1 support, use version 1.8.1.

## Quick Start

### 1. Set up your connection factory

```csharp
public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Create()
    {
        return new SqlConnection(_connectionString);
    }
}
```

### 2. Register services in your DI container

```csharp
using Dapper.AmbientContext.Extensions;

// With connection factory instance
services.AddDapperAmbientContext(new SqlConnectionFactory(connectionString));

// OR with connection factory type
services.AddDapperAmbientContext<SqlConnectionFactory>(connectionString);

// Register your repositories
services.AddTransient<IUserRepository, UserRepository>();
services.AddTransient<IOrderRepository, OrderRepository>();
```

### 3. Create a repository

```csharp
public class UserRepository : IUserRepository
{
    private readonly IAmbientDbContextLocator _locator;

    public UserRepository(IAmbientDbContextLocator locator)
    {
        _locator = locator;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        var context = _locator.Get();
        var prepared = await context.PrepareAsync();
        
        return await prepared.Connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id",
            new { Id = id },
            transaction: prepared.Transaction);
    }

    public async Task CreateAsync(User user)
    {
        var context = _locator.Get();
        var prepared = await context.PrepareAsync();
        
        await prepared.Connection.ExecuteAsync(
            "INSERT INTO Users (Name, Email) VALUES (@Name, @Email)",
            user,
            transaction: prepared.Transaction);
    }
}
```

### 4. Use in your service layer

```csharp
public class UserService
{
    private readonly IAmbientDbContextFactory _contextFactory;
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;

    public UserService(
        IAmbientDbContextFactory contextFactory,
        IUserRepository userRepository,
        IOrderRepository orderRepository)
    {
        _contextFactory = contextFactory;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
    }

    public async Task<User> CreateUserWithOrderAsync(User user, Order order)
    {
        // Create ambient context - both repositories will share this transaction
        using (var context = _contextFactory.Create())
        {
            await _userRepository.CreateAsync(user);
            await _orderRepository.CreateAsync(order);
            
            // Transaction is automatically committed on Dispose if no exceptions
            return user;
        }
    }
}
```

## Advanced Usage

### Nested Contexts

```csharp
using (var parentContext = _contextFactory.Create())
{
    await _userRepository.CreateAsync(user);
    
    // Child context joins parent's transaction
    using (var childContext = _contextFactory.Create(join: true))
    {
        await _orderRepository.CreateAsync(order);
        // Child dispose doesn't commit - parent owns the transaction
    }
    
    // Transaction committed here when parent is disposed
}
```

### Transaction Suppression

```csharp
// Create context without a transaction
using (var context = _contextFactory.Create(suppress: true))
{
    // Queries run without transaction
    var users = await _userRepository.GetAllAsync();
}
```

### Custom Isolation Level

```csharp
using (var context = _contextFactory.Create(
    isolationLevel: IsolationLevel.Serializable))
{
    await _userRepository.CreateAsync(user);
}
```

### Manual Commit/Rollback

```csharp
using (var context = _contextFactory.Create())
{
    try
    {
        await _userRepository.CreateAsync(user);
        context.Commit(); // Explicit commit
    }
    catch
    {
        context.Rollback(); // Explicit rollback
        throw;
    }
}
```

## How It Works

The ambient context pattern uses `AsyncLocal` (.NET Core/5+) or `LogicalCallContext` (.NET Framework) to maintain a stack of database contexts. When you create a context:

1. A new connection is opened (or inherited from parent)
2. A transaction is started (unless suppressed)
3. Context is pushed onto the ambient stack
4. All data access components get the current context via `IAmbientDbContextLocator`
5. On dispose, the context is popped and transaction is committed/rolled back

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

Built on top of [Dapper](https://github.com/DapperLib/Dapper) - the simple object mapper for .NET.
