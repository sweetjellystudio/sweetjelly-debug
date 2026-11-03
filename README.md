#   SWeetJelly Debug

Synchronous diagnostics with typed severity, filtering, custom formatters, a convenient Trace facade, Unity Console and uGUI Text output. Requires Unity 6.3 LTS.

##  Installation

After the version tag is published, add these exact public Git dependencies to the consuming project's Packages/manifest.json:

```json
{
  "dependencies": {
    "com.sweetjelly.debug": "https://github.com/sweetjellystudio/sweetjelly-debug.git#v2.0.0",
    "com.sweetjelly.bootstrapper": "https://github.com/sweetjellystudio/sweetjelly-bootstrapper.git#v2.0.0"
  }
}
```

The package manifest declares uGUI 2.0.0 and Bootstrapper 2.0.0. Custom Git dependency URLs belong to the project manifest; package dependencies contain exact versions. This example does not claim an unpublished tag is available. License: [MIT][license].

##  Usage

Add DebugPackageBootstrapperSo to a Bootstrapper profile, or compose manually:

```csharp
var service = new DebugService();
service.AddDebugger(new UnityConsoleDebugger());
service.MinimumLevel = DebugLevel.Trace;
SWJDebug.SetService(service);
SWJDebug.Log("PlayerHp", 100);
SWJDebug.TraceWarning("Unexpected state");
// At the owner's shutdown:
SWJDebug.SetService(null);
service.ClearDebuggers();
```

Types are in SWeetJelly.Debug.App, .Contract and .Framework.Unity. Log, LogWarning, LogError and LogException accept grouping labels; TraceLog/TraceWarning/TraceError/TraceException derive a caller label. With no service configured, the facade does not deliver entries. Enabled and MinimumLevel control filtering in Editor and ordinary release Players. Severity order is Trace, Info, Warning, Error, Exception.

Implement IDebugger.Publish to add an output or IDebugFormatter.Format to customize text, then use AddDebugger/RemoveDebugger/SetFormatter. SWJDebug.Set(IDebugger) retains the current service and replaces its targets. New subscriptions affect later entries if changed during a callback. Formatter/output exceptions propagate immediately; same-service recursive accepted logs are rejected. See [architecture][architecture] for the complete callback/thread/ownership contract.

UnityUIDebugger writes to the serialized legacy uGUI Text, with SetOutput, SetMaxLines and Clear for explicit runtime control. maxLines bounds retained entries, including multiline entries as one item. Its registration is owned by the caller. Use Unity targets from the main thread and serialize access to service configuration/calls.

##  Sample and upgrading

Import Debug Sample through Package Manager, open Sample Scene and play. Use the buttons for severity, Trace, exception and filtering. Console remains active while UI output is added/removed. The sample's UI lifetime is separate from the package output implementation.

See [migration][migration], [architecture][architecture], [decision][adr] and [tests][tests]. Package correctness tests belong here; environment-specific reports belong to the package-development host. Git publication and public-tag consumption verification are separate tasks.

[license]: LICENSE.md
[migration]: Documentation~/migration-2.0.md
[architecture]: ARCHITECTURE.md
[adr]: docs/adr/0001-synchronous-routing-and-ownership.md
[tests]: Tests/README.md
