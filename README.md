# SWeetJelly Debug

SWeetJelly Debug provides a small debug facade, a routable debug service, message formatting, and Unity output implementations.

The usual call site should stay simple:

```csharp
using SWeetJelly.Debug.App;

SWJDebug.Log("PlayerHp", hp);
SWJDebug.LogWarning("Inventory", itemCount);
SWJDebug.LogError("Save", saveResult);
SWJDebug.LogException("Save", exception);
```

## Structure

```text
Runtime
├── Contract
│   ├── IDebugger
│   ├── IDebugService
│   ├── IDebugFormatter
│   ├── DebugEntry
│   ├── DebugLabelType
│   └── DebugLevel
├── App
│   ├── SWJDebug
│   ├── DebugService
│   └── DebugFormat
├── Framework
│   ├── UnityConsoleDebugger
│   └── UnityUIDebugger
└── Framework/Bootstrapper
    └── DebugPackageBootstrapperSo
```

## Concepts

`SWJDebug` is the static facade for everyday use.

`IDebugService` owns routing, filtering, level checks, formatter selection, and debugger registration.

`IDebugger` is an output target. The package includes `UnityConsoleDebugger` and `UnityUIDebugger`.

`IDebugFormatter` converts a `DebugEntry` into a readable message. The default implementation is `DebugFormat`.

## Bootstrapper Integration

Add `DebugPackageBootstrapperSo` to the SWeetJelly Bootstrapper module list.

The module creates a `DebugService`, registers a `UnityConsoleDebugger`, sets it as the `SWJDebug` service, and registers `IDebugService` in the service registry.

```csharp
using SWeetJelly.Debug.App;

SWJDebug.Log("Debug", "Ready");
```

## Manual Setup

You can also configure the service manually.

```csharp
using SWeetJelly.Debug.App;
using SWeetJelly.Debug.Contract;
using SWeetJelly.Debug.Framework.Unity;

var service = new DebugService();
service.MinimumLevel = DebugLevel.Trace;
service.AddDebugger(new UnityConsoleDebugger());

SWJDebug.SetService(service);
```

For the previous simple setup style, `SWJDebug.Set(IDebugger)` is still available.

```csharp
using SWeetJelly.Debug.App;
using SWeetJelly.Debug.Framework.Unity;

SWJDebug.Set(new UnityConsoleDebugger());
```

## Trace Logs

Trace methods automatically include the caller file, member, and line number.

```csharp
SWJDebug.TraceLog(playerState);
SWJDebug.TraceWarning("Unexpected state");
SWJDebug.TraceError(errorCode);
SWJDebug.TraceException(exception);
```

## Filtering

Use `MinimumLevel` to skip lower priority entries.

```csharp
SWJDebug.Service.MinimumLevel = DebugLevel.Warning;
```

Levels are ordered as:

```text
Trace < Info < Warning < Error < Exception
```

## Custom Debugger

Implement `IDebugger` to add a new output target.

```csharp
using SWeetJelly.Debug.Contract;

public sealed class MyDebugger : IDebugger
{
    public void Publish(DebugEntry entry, string formattedMessage)
    {
        // Send formattedMessage somewhere.
    }
}
```

Then register it with the service.

```csharp
SWJDebug.Service.AddDebugger(new MyDebugger());
```

## Custom Formatter

Implement `IDebugFormatter` to change message formatting.

```csharp
using SWeetJelly.Debug.Contract;

public sealed class MyFormatter : IDebugFormatter
{
    public string Format(DebugEntry entry)
    {
        return $"{entry.Level}: {entry.Label} {entry.Value}";
    }
}
```

Then set it on the service.

```csharp
SWJDebug.Service.SetFormatter(new MyFormatter());
```

