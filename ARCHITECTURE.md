#   Debug architecture

Debug provides synchronous filtering, message formatting and pluggable diagnostic output. DDD responsibility boundaries and Clean Architecture dependency direction guide this design. The package is a distribution unit; its folders do not define separate bounded contexts. A separate Domain assembly would add no independent model here.

##  Responsibilities and references

Contract owns `DebugEntry`, severity/label values, `IDebugService`, `IDebugFormatter` and `IDebugger`. App owns `DebugService`, the default CLR formatter and the convenience `SWJDebug` facade. Both asmdefs have `noEngineReferences=true`; App references Contract. Formatting does not need a Unity adapter or an additional layer because it operates on CLR values and these contracts.

Framework.Unity owns Console formatting, Unity object presentation and uGUI Text output. Framework.Bootstrapper composes the concrete service and Console target through the current Bootstrapper contracts. The sample owns its buttons and optional UI target registration. The UI target itself is a supported package feature. Manifest dependencies are Bootstrapper 2.0.0 and uGUI 2.0.0; the actual asmdef/compiler references must remain consistent with those ports and Unity UI types.

##  Routing contract

The caller serializes access to a service and its registrations/configuration. Calls are synchronous; Unity targets require Unity's main thread. There is no worker queue, automatic thread marshalling or multithreaded guarantee. `Enabled` and `MinimumLevel` filter entries before formatting/delivery. Disabled calls still follow normal C# argument evaluation at the call site.

Each accepted entry snapshots output targets before formatting. Add/Remove/Clear in a formatter or target applies to later entries; targets in the snapshot still receive the current entry unless an earlier callback throws. Formatter, value conversion/enumeration and target exceptions propagate immediately; later targets are not guaranteed delivery. A same-service accepted recursive log throws `InvalidOperationException`. A finally block releases that guard after failures. An independent service may be called, provided it does not cycle back into an active service.

`SWJDebug` delegates to a configured service. `Set(IDebugger)` replaces its targets through the injected service's public contract and preserves custom service identity, formatter and filtering; a default service is created only when absent. Consumers that inject a facade manually own its lifetime and eventual `SetService(null)`. The facade is runtime global state, not a global project asset.

##  Ownership and Unity lifetime

A bootstrap asset is reusable configuration. Only one active module may publish a facade. Existing registry/facade registrations and repeated Bootstrap calls are rejected. Registry registration completes before facade publication. Failure rolls back only a matching newly registered service. Shutdown uses the registry captured at Bootstrap and identity checks, clears its own targets and remains idempotent. Replacement services/facades survive another owner's Shutdown.

`UnityUIDebugger` bounds retained log entries by the serialized `maxLines` setting, clamped to at least one. A message containing newlines is still one entry. SetMaxLines applies the bound immediately, SetOutput renders retained entries on the selected Text, and Clear clears history/display. The caller that registers a target owns its unregistration; the target does not search a global registry. The sample removes only its own target and destroys only generated output/Canvas/components while retaining borrowed objects.

Player functionality is intentional in ordinary release builds. Consumers choose filtering and sensitive values; this package does not infer a development-only policy or remove call sites. The approved trade-offs and compatibility constraints are recorded in [ADR 0001][adr].

[adr]: docs/adr/0001-synchronous-routing-and-ownership.md
