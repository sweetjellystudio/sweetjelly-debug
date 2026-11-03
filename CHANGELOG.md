#   Changelog

##  2.0.0

*   Require Unity 6.3 LTS, Bootstrapper 2.0.0 and uGUI 2.0.0; preserve ordinary Player logging, public APIs, enum values and asset GUIDs.
*   Snapshot output targets before formatting, reject accepted same-service recursion and retain immediate callback exception propagation.
*   Preserve custom service identity when selecting an output through SWJDebug.Set.
*   Protect bootstrap registration/facade ownership and roll back initialization failures.
*   Apply UI history bounds explicitly and clean up only sample-owned outputs and generated UI while retaining Console output.
*   Add correctness/lifecycle tests, maintained architecture/decision documents and migration guidance.

## v1.0.0

- [Added] `SWJDebug` static facade for convenient log calls.
- [Added] `IDebugService`, `IDebugger`, and `IDebugFormatter` contracts.
- [Added] `DebugService` for routing, filtering, log level management, and formatter management.
- [Added] `DebugFormat` default message formatter.
- [Added] `UnityConsoleDebugger` output target.
- [Added] `UnityUIDebugger` output target.
- [Added] `DebugPackageBootstrapperSo` for SWeetJelly Bootstrapper integration.
