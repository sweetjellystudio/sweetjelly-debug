#   Migrating to Debug 2.0

Requires Unity 6.3 LTS (`6000.3`), Bootstrapper 2.0.0 and uGUI 2.0.0. Read the dependency's migration guide when upgrading a project that still uses Bootstrapper 1.x profiles. This package does not edit game assets or upgrade them automatically.

Public logging/Trace APIs, enum values, assembly names, script GUIDs and serialized output/maxLines fields remain. There is no required rename or conversion to TMP/UI Toolkit. Runtime logging remains available in ordinary release Players; configure Enabled/MinimumLevel explicitly. Trace labels continue to expose caller file base name, member and line, not a full file path.

Behavioral changes:

*   Mutating target registrations during formatting/delivery affects the next entry, not the current snapshot. Existing callback exceptions still propagate immediately and may stop later targets.
*   Accepted recursive logging to the same service throws InvalidOperationException. Forward to an independent service or handle the recursion at the extension boundary. A filtered nested entry is ignored by the normal filter.
*   SWJDebug.Set(IDebugger) retains an injected custom IDebugService and its configuration, replacing targets through its ClearDebuggers/AddDebugger contract. With no configured service it creates the default implementation.
*   Bootstrap no longer overwrites another registered service/facade. Duplicate active composition fails explicitly. Registration failure preserves the prior owner; Shutdown releases only matching state.
*   UnityUIDebugger.SetMaxLines(int) clamps the existing log-entry bound and trims immediately. SetOutput renders retained entries immediately. A multiline message counts as one retained entry.
*   The sample retains the default Console and adds/removes an optional UI output. Disabling it releases only owned registrations/UI. External targets, borrowed Text and Canvas are retained.

Use one serialized caller context for a service; Unity targets require the Unity main thread. No thread queue is supplied. Formatters, ToString/enumerators and output callbacks may throw. Callers logging while handling another exception should decide how to preserve the original exception; automatic suppression is not introduced by this upgrade.
