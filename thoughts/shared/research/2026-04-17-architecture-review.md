---
date: 2026-04-17T16:30:00-05:00
researcher: Claude (Opus 4.6)
git_commit: 89aa2d802e916e421444e53412b2e5fc5a5a9700
branch: master
repository: Dapper.AmbientContext
topic: "Architecture review and optimization recommendations"
tags: [research, codebase, architecture, optimization, refactoring]
status: complete
last_updated: 2026-04-17
last_updated_by: Claude (Opus 4.6)
---

# Research: Architecture Review and Optimization Recommendations

**Date**: 2026-04-17T16:30:00-05:00
**Researcher**: Claude (Opus 4.6)
**Git Commit**: 89aa2d8
**Branch**: master
**Repository**: Dapper.AmbientContext

## Research Question

What do you think about the overall architecture in this library? Do you think it's structured in an optimized way? What changes would you make, if any?

## Summary

The library implements the Ambient Context pattern for Dapper — a sound architectural choice for managing shared database connections and transactions across service/repository boundaries. The core lifecycle logic (`AmbientDbContext.cs` at 270 lines) is well-designed: stack-based nesting, lazy connection opening, and parent-child transaction inheritance are all solid.

However, **~80% of the codebase (5,540 of 6,896 lines) is hand-written boilerplate** in the Dapper proxy layer. This proxy layer is the single biggest architectural issue — it creates a massive maintenance surface, an enormous interface contract (`IAmbientDbContextQueryProxy` at 2,414 lines), and locks the library to a specific Dapper API surface that breaks on every Dapper update. The storage layer adds unnecessary indirection for modern .NET targets. The static singleton pattern for storage configuration makes testing fragile.

## Detailed Findings

### 1. The Proxy Boilerplate Problem (Highest Impact)

The most consequential architectural decision is wrapping every Dapper extension method with a proxy that calls `PrepareConnectionAndTransaction()` before delegating.

**Scale of the problem:**
- `IAmbientDbContextQueryProxy.cs` — 2,414 lines (interface definition)
- `AmbientDbContext.Proxy.cs` — 1,374 lines (sync implementations)
- `AmbientDbContext.ProxyAsync.cs` — 1,752 lines (async implementations)
- **Total: 5,540 lines** — 80% of all source code

Every proxy method follows the exact same pattern:

```csharp
public T QueryFoo<T>(string sql, object param = null, ...)
{
    PrepareConnectionAndTransaction();
    return Connection.QueryFoo<T>(sql, param, Transaction, ...);
}
```

**Problems this creates:**
- Every Dapper version update requires manually mirroring new methods
- The `IAmbientDbContextQueryProxy` interface is so large that consumers can't reasonably mock it in tests
- Multi-mapping queries are duplicated 6 times each (2-7 type parameters), both sync and async
- The `CommandDefinition` overloads exist alongside the parameter-list overloads, doubling the surface

**Alternative approach:** Instead of proxying every Dapper method, expose a method that returns the prepared connection and transaction as a tuple, and let consumers call Dapper extension methods directly:

```csharp
public interface IAmbientDbContext : IDisposable
{
    (IDbConnection Connection, IDbTransaction Transaction) GetConnectionAndTransaction();
    // ... commit/rollback/etc
}
```

This reduces 5,540 lines to roughly 20. The locator returns the context, the consumer destructures, and calls Dapper directly. The tradeoff is that callers must pass the transaction explicitly, but this is exactly what Dapper already expects.

### 2. Storage Layer Over-Engineering

The `ContextualStorageHelper` uses a two-level indirection:

```
AsyncLocal/CallContext → ContextualStorageItem (GUID key) → ConditionalWeakTable → IImmutableStack
```

This exists because `LogicalCallContext` (NET 4.5.1) requires serializable values. Since `IImmutableStack<IAmbientDbContext>` isn't serializable, the helper stores a cross-reference key in the call context and the actual stack in a `ConditionalWeakTable`.

**For `AsyncLocal<T>` (netstandard1.3+)**, this indirection is unnecessary. `AsyncLocal<T>` has no serialization constraint — the stack could be stored directly:

```csharp
private static readonly AsyncLocal<IImmutableStack<IAmbientDbContext>> _stack = new();
```

This eliminates: `ContextualStorageHelper`, `ContextualStorageItem`, `AmbientDbContextStorageKey`, `IContextualStorage`, the `ConditionalWeakTable`, and the GUID-based cross-referencing. The entire storage layer collapses to ~10 lines for modern targets.

The `ConditionalWeakTable` was specifically chosen to prevent leaks when contexts aren't disposed. But `ConditionalWeakTable` keys are `ContextualStorageItem` objects held strongly by `AsyncLocal`, so the weak reference doesn't actually help — the entry lives as long as the `AsyncLocal` value does (i.e., the async flow's lifetime). The weak-reference rationale in the code comments doesn't apply to this usage.

### 3. Static Singleton for Storage Configuration

`AmbientDbContextStorageProvider` is a static mutable singleton:

```csharp
public static class AmbientDbContextStorageProvider
{
    private static IContextualStorage _storage;
    public static void SetStorage(IContextualStorage storage) { _storage = storage; }
}
```

**Problems:**
- No thread safety on `SetStorage` (could corrupt state if called concurrently)
- Tests must manually `SetStorage(null)` in cleanup — every single test does this
- Only one storage strategy per AppDomain — can't have different strategies in parallel tests
- The factory and locator reach for `AmbientDbContextStorageProvider.Storage` internally, creating hidden coupling

**Alternative:** Pass the storage (or eliminate it entirely) through constructor injection on the factory and locator. For modern .NET, just use `AsyncLocal` directly without the abstraction.

### 4. Factory Creates Connections It Doesn't Need

`AmbientDbContextFactory.Create()` unconditionally calls `_connectionFactory.Create()` even when `join=true` and a parent exists. We patched this in the constructor to dispose the orphan, but architecturally the factory should own this decision:

```csharp
public IAmbientDbContext Create(bool join = true, ...)
{
    // Check if joining is possible before allocating
    if (join && ParentExists())
    {
        return new AmbientDbContext(connection: null, join: true, ...);
    }

    var connection = _connectionFactory.Create();
    return new AmbientDbContext(connection, join, ...);
}
```

This prevents the allocation entirely rather than allocating-then-disposing.

### 5. `PrepareConnectionAndTransaction` Has Race Conditions

Both the sync and async versions of `PrepareConnectionAndTransaction` check `Connection.State` and then mutate state (open connection, begin transaction) without synchronization. If two threads call query methods on the same context concurrently:

```csharp
// Thread 1                          // Thread 2
if (Connection.State != Open)        if (Connection.State != Open)
    Connection.Open();                   Connection.Open(); // Double-open!
    Transaction = BeginTransaction();    Transaction = BeginTransaction(); // Double-begin!
```

This could cause double-open or double-transaction-begin, depending on the ADO.NET provider. The `AmbientDbContext` is not documented as thread-safe, but `AsyncLocal` sharing across continuations makes this plausible.

### 6. `IImmutableStack` Allocation Overhead

Every push/pop creates a new `IImmutableStack` instance (immutability invariant). Then `SaveStack` removes and re-adds the entry in `ConditionalWeakTable`. So every context creation/disposal does:

1. Allocate new `IImmutableStack`
2. Look up `ContextualStorageItem` from storage
3. `TryGetValue` on `ConditionalWeakTable`
4. `Remove` from `ConditionalWeakTable`
5. `Add` to `ConditionalWeakTable`

For a library whose core job is to share a connection reference, this is significant overhead per operation. A mutable stack behind a lock (or stored directly in `AsyncLocal`) would be simpler and faster.

### 7. `IAmbientDbContext.Transaction` Has a Public Setter

```csharp
IDbTransaction Transaction { get; set; }
```

This allows any consumer to replace the transaction from outside the context, which could corrupt the parent-child relationship. The setter is only used internally by `PrepareConnectionAndTransaction()` and `Commit()`/`Rollback()`. It should not be on the public interface.

## What Works Well

- **The ambient context pattern itself** is the right abstraction for this problem
- **Lazy connection opening** (`PrepareConnectionAndTransaction`) means connections aren't held open until needed
- **Parent-child join semantics** correctly share connections and transactions through a stack
- **LIFO disposal enforcement** catches out-of-order disposal bugs early
- **The `IDbConnectionFactory` abstraction** gives consumers clean control over connection creation

## Recommended Changes (Priority Order)

### High Priority
1. **Eliminate the proxy layer** — Replace `IAmbientDbContextQueryProxy` with a method that exposes the prepared connection + transaction. Delete `Proxy.cs`, `ProxyAsync.cs`, and the 2,414-line interface. This removes ~5,500 lines and decouples the library from Dapper's API surface.

2. **Make `Transaction` setter internal** — Remove from `IAmbientDbContext` public interface to prevent external mutation.

### Medium Priority
3. **Simplify storage for modern targets** — For netstandard2.0+, store `IImmutableStack` directly in `AsyncLocal`. Keep `ConditionalWeakTable` indirection only for NET451 via `#if` compile target.

4. **Avoid unnecessary connection allocation** — Move the join-check into the factory so connections are never allocated when joining a parent.

### Low Priority
5. **Replace static storage provider with constructor injection** — Pass storage through factory/locator constructors.

6. **Add thread safety to `PrepareConnectionAndTransaction`** — Use `Interlocked` or a lightweight lock for the check-then-act on connection state.

7. **Consider dropping NET451 target** — NET 4.5.1 is long out of support. Dropping it eliminates the need for `LogicalCallContextStorage`, `MarshalByRefObject`, and the `ConditionalWeakTable` indirection entirely.

## Code References

- `src/Dapper.AmbientContext/IAmbientDbContextQueryProxy.cs` — 2,414-line proxy interface
- `src/Dapper.AmbientContext/AmbientDbContext.Proxy.cs:1341-1373` — `PrepareConnectionAndTransaction()` sync
- `src/Dapper.AmbientContext/AmbientDbContext.ProxyAsync.cs:1720-1751` — `PrepareConnectionAndTransactionAsync()`
- `src/Dapper.AmbientContext/AmbientDbContext.cs:63-94` — Constructor with join logic
- `src/Dapper.AmbientContext/AmbientDbContext.cs:122-168` — Dispose method
- `src/Dapper.AmbientContext/AmbientDbContextFactory.cs:74-86` — Factory.Create()
- `src/Dapper.AmbientContext/Storage/ContextualStorageHelper.cs:42-175` — Storage indirection
- `src/Dapper.AmbientContext/AmbientDbContextStorageProvider.cs:36-75` — Static singleton

## Related Research
- `thoughts/shared/research/2026-04-17-memory-leak-connection-pool-exhaustion.md` — Root cause analysis of the connection leak bug
