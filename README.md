# ILogger Timings [![NuGet Release](https://img.shields.io/nuget/v/ILoggerTimings.svg)](https://nuget.org/packages/ILoggerTimings)

#Forked from SerilogTimings

.Net Core ILogger used instead of Serilog Logger. And parameters changed in order to allow the use of a IDictionary<string,object> to record properties. At least for now, Operation methods were disabled.

Serilog's support for structured data makes it a great way to collect timing information. It's easy 
to get started with in development, because the timings are printed to the same output as other
log messages (the console, files, etc.) so a metrics server doesn't have to be available all the time.

Serilog Timings is built with some specific requirements in mind:

 * One operation produces exactly one log event (events are raised at the completion of an operation)
 * Natural and fully-templated messages
 * Events for a single operation have a single event type, across both success and failure cases (only the logging level and `Outcome` properties change)

This keeps noise in the log to a minimum, and makes it easy to extract and manipulate timing 
information on a per-operation basis.

### Installation

The library is published as _ILoggerTimings_ on NuGet.

```powershell
Install-Package _ILoggerTimings_ -DependencyVersion Highest
```

.NET Core 2 supported.

### Getting started

Before your timings will go anywhere, [install and configure Serilog](http://serilog.net).

Types are in the `ILoggerTimings` namespace.

```csharp
using ILoggerTimings;
```

The simplest use case is to time an operation, without explicitly recording success/failure:

### Use with `ILogger`

If a contextual `ILogger` is available, the extensions methods `TimeOperation()` and
`BeginOperation()` can be used to write operation timings through it:

```csharp
using (logger.TimeOperation("Submitting payment for {OrderId}", order.Id))
{
    // Timed block of code goes here
}
```

These otherwise behave identically to `Operation.Time()` and `Operation.Begin()`.

### `LogContext` support

If your application enables the Serilog `LogContext` feature using `Enrich.FromLogContext()` on
the `LoggerConfiguration`, _SerilogTimings_ will add an `OperationId` property to all events inside
timing blocks automatically.

This is **highly recommended**, because it makes it much easier to trace from a timing result back 
through the operation that raised it.

### Caveats

One important usage note: because the event is not written until the completion of the `using` block
(or call to `Complete()`), arguments to `Begin()` or `Time()` are not captured until then; don't
pass parameters to these methods that mutate during the operation.

### How does this relate to SerilogMetrics?

[SerilogMetrics](https://github.com/serilog-metrics/serilog-metrics) is a mature metrics solution
for Serilog that includes timings as well as counters, gauges and more. Serilog Timings is an 
alternative implementation of timings only, designed with some different stylistic preferences and
goals. You should definitely check out SerilogMetrics as well, to see if it's more to your tastes!
