# Debug Dev Sample

This sample shows the consumer-side flow for `com.sweetjelly.debug`.

- `Debug Package Bootstrapper.asset` installs the package `DebugPackageBootstrapperSo`.
- `Sample Package Bootstrapper Profile.asset` is assigned to `BootstrapperBehaviour` in the sample scene.
- `DebugSampleConsumerBehaviour` resolves `IDebugService` and emits logs through `SWJDebug`.
- `DebugUIDebuggerInstallerBehaviour` lets the sample switch between `UnityConsoleDebugger` and `UnityUIDebugger`.

## Setup

Open `Sample Scene` and enter Play Mode.

Use the on-screen buttons to emit info, warning, error, trace, and exception logs. The Debug package starts with its default console debugger, and the sample GUI can switch the registered debugger between Unity Console output and Unity UI text output at runtime.
