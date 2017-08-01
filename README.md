Dapper.AmbientContext
=======

[![Build status](https://ci.appveyor.com/api/projects/status/omt8ahl09xbnp67t?svg=true)](https://ci.appveyor.com/project/sakopov/dapper-ambientcontext)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Dapper.AmbientContext.svg)](https://www.nuget.org/packages/Dapper.AmbientContext)

*Ambient context implementation for Dapper.*

Ambient Context is a little-known pattern that is frequently used by the .NET framework to address cross-cutting concerns such as security (Thread.CurrentPrincipal), logging (Trace & TraceListeners) and `HttpContext` and etc. The general idea is that the application sets up a context typically at an entry-point of an operation or a request, which becomes available to the rest of the system via a static property or method

Dapper.AmbientContext uses a similar approach to treat the database connection and transaction as resource which is seamlessly shared between various data access components. This pattern works really well with the service layer which consumes multiple data access repositories, commands and queries to perform a variety of database tasks within the same transaction.

More information in the [wiki](https://github.com/sakopov/Dapper.AmbientContext/wiki).

## Installation

Install [Dapper.AmbientContext with NuGet](https://www.nuget.org/packages/Dapper.AmbientContext)

```
Install-Package Dapper.AmbientContext
```
