---
date: 2026-04-17T15:56:00-05:00
researcher: Claude (Opus 4.6)
git_commit: 89aa2d802e916e421444e53412b2e5fc5a5a9700
branch: master
repository: Dapper.AmbientContext
topic: "Memory leak causing connection pool exhaustion"
tags: [research, codebase, memory-leak, connection-pool, AmbientDbContext, AmbientDbContextFactory, ContextualStorageHelper]
status: complete
last_updated: 2026-04-17
last_updated_by: Claude (Opus 4.6)
---

# Research: Memory Leak Causing Connection Pool Exhaustion

**Date**: 2026-04-17T15:56:00-05:00
**Researcher**: Claude (Opus 4.6)
**Git Commit**: 89aa2d8
**Branch**: master
**Repository**: Dapper.AmbientContext

## Research Question

This library has a memory leak somewhere which ends up exhausting the connection pool. What is the root cause?

## Summary

The root cause is an **orphaned connection leak** in the join scenario. Every time `AmbientDbContextFactory.Create(join: true)` is called and a parent context exists, a new `IDbConnection` is allocated by the factory but then immediately abandoned in the `AmbientDbContext` constructor when the `Connection` property is overwritten with the parent's connection. The original connection is never closed or disposed, causing it to leak. Under sustained load with nested/child contexts, this steadily exhausts the connection pool.

A secondary contributor is the `GC.Collect()` + `GC.WaitForPendingFinalizers()` call in `Dispose()`, which was added as a workaround (commit `1e5dd65`) but creates GC pressure rather than fixing the underlying leak.

## Detailed Findings

### 1. The Connection Leak Flow (Root Cause)

The leak occurs across two files in a two-step handoff:

**Step 1 — Factory always allocates a new connection** (`AmbientDbContextFactory.cs:76`):

```csharp
var connection = _connectionFactory.Create();  // NEW IDbConnection allocated
var ambientDbContext = new AmbientDbContext(connection, join, suppress, isolationLevel);
```

The factory unconditionally creates a new connection and passes it to the constructor. It does not check whether the context will join an existing parent.

**Step 2 — Constructor overwrites the connection when joining** (`AmbientDbContext.cs:63-88`):

```csharp
Connection = connection;  // Initially set to the new connection

if (join)
{
    if (!immutableStack.IsEmpty)
    {
        var parent = immutableStack.Peek();
        Parent = parent;
        Connection = parent.Connection;  // OVERWRITTEN — original connection is now orphaned
        Transaction = parent.Transaction;
        Suppress = parent.Suppress;
        IsolationLevel = parent.IsolationLevel;
    }
}
```

When `join=true` and a parent exists on the stack, `Connection` is reassigned to the parent's connection. The original `IDbConnection` created by the factory loses all references and is never disposed.

**Step 3 — Dispose correctly skips child cleanup** (`AmbientDbContext.cs:143-160`):

```csharp
if (Parent == null)
{
    // Only parent contexts close and dispose their connection
    if (Connection != null)
    {
        if (Connection.State == ConnectionState.Open)
            Connection.Close();
        Connection.Dispose();
        Connection = null;
    }
}
```

Child contexts (where `Parent != null`) correctly do NOT close the connection, since `Connection` now points to the shared parent connection. But the originally allocated connection was already lost in step 2.

### 2. Why This Exhausts the Connection Pool

With ADO.NET providers like `SqlConnection`:
- Each `_connectionFactory.Create()` allocates a new connection object
- Even closed/unopened connections may hold pool tracking metadata depending on the provider
- Abandoned connections are never returned to the pool via `Dispose()`
- The finalizer thread may or may not properly clean these up (provider-dependent, non-deterministic)
- Under load with frequent child context creation, orphaned connections accumulate faster than the GC finalizer can reclaim them

### 3. The `GC.Collect()` Workaround (`AmbientDbContext.cs:162-163`)

Commit `1e5dd65` ("second try for a fix for #11") added:

```csharp
GC.Collect();
GC.WaitForPendingFinalizers();
```

This forces a full garbage collection on every `Dispose()` call. The intent was to trigger finalizers on orphaned connections so they'd release pool slots. However:
- It does not fix the root cause (connections are still leaked)
- It introduces significant GC pressure and pauses under load
- `GC.WaitForPendingFinalizers()` blocks the calling thread

### 4. Prior Fix: String Interning Issue (Commit `3ae7197`)

An earlier fix ("remove empty stacks when disposing parent context") addressed a different memory issue in `ContextualStorageHelper`. Previously, non-.NET451 targets used `ConditionalWeakTable<string, ...>` with raw `string` keys. Since .NET interns strings, `ConditionalWeakTable`'s reference-equality check meant interned string keys were never collected, effectively making the "weak" reference strong. This prevented cleanup of old context stacks.

The fix unified all platforms to use `ContextualStorageItem` (a non-interned class) as the key, allowing proper GC of abandoned stacks. This fix was correct for that specific issue but did not address the connection leak.

### 5. `AmbientDbContext` Lifecycle Architecture

The library uses a stack-based ambient context pattern:

- **Factory** (`AmbientDbContextFactory`): Creates contexts, always allocating a fresh `IDbConnection`
- **Context** (`AmbientDbContext`): Holds connection/transaction, pushed onto an immutable stack in contextual storage
- **Storage** (`ContextualStorageHelper`): Manages the stack via `ConditionalWeakTable` + `AsyncLocal`/`LogicalCallContext`
- **Locator** (`AmbientDbContextLocator`): Retrieves the top-of-stack (active) context

Parent contexts own the connection lifecycle. Child contexts (joined) share the parent's connection and transaction. Disposal must happen in LIFO order (enforced by the `this != topItem` check in `Dispose()`).

## Code References

- `src/Dapper.AmbientContext/AmbientDbContextFactory.cs:76` — Connection allocated unconditionally
- `src/Dapper.AmbientContext/AmbientDbContext.cs:69` — Connection first assigned
- `src/Dapper.AmbientContext/AmbientDbContext.cs:79` — Connection overwritten with parent's (leak point)
- `src/Dapper.AmbientContext/AmbientDbContext.cs:143-160` — Dispose only cleans up parent connections
- `src/Dapper.AmbientContext/AmbientDbContext.cs:162-163` — GC.Collect workaround
- `src/Dapper.AmbientContext/Storage/ContextualStorageHelper.cs:53` — ConditionalWeakTable definition

## Architecture Documentation

The library implements an ambient context pattern for Dapper database access:

1. A `ContextualStorageHelper` maintains a `ConditionalWeakTable<ContextualStorageItem, IImmutableStack<IAmbientDbContext>>` keyed by a GUID-based `ContextualStorageItem` stored in `AsyncLocal` (or `LogicalCallContext` on .NET 4.5.1)
2. Each new `AmbientDbContext` pushes itself onto the stack; `Dispose()` pops it
3. When `join=true`, child contexts inherit the parent's connection and transaction
4. When `join=false`, each context maintains its own independent connection
5. Connection opening and transaction creation are deferred until the first query (`PrepareConnectionAndTransaction`)

## Historical Context

- Issue #11 on GitHub reported the runaway memory bug
- Commit `3ae7197` (PR #12) fixed the `ConditionalWeakTable` string-interning issue
- Commit `1e5dd65` (PR #13) added `GC.Collect()` as a second attempt to address the symptom

## Open Questions

- Whether the `GC.Collect()` workaround provides any measurable benefit given that it doesn't address the root cause
- Whether the factory should avoid creating a connection when `join=true` and a parent exists, or whether the constructor should dispose the orphaned connection
