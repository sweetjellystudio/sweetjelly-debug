---
status: accepted
date: 2026-11-03
---

#   Preserve synchronous diagnostics while making routing and ownership explicit

The real consumer uses Trace APIs in normal game flow and exception cleanup, and Demo/Prototype profiles share a bootstrap configuration asset. Debug 2.0 retains public APIs, script GUIDs, ordinary Player functionality and uGUI Text support while requiring Unity 6.3 and dependency 2.0 versions. A UI Toolkit rewrite, a new Domain layer and automatic development-only stripping would impose unrelated migration costs and were rejected.

An entry snapshots targets before formatting, so target-list mutations affect later entries. Formatter/target failures continue to propagate immediately. We rejected silently swallowing errors or continuing other targets because the caller-visible failure contract would change. Accepted same-service recursion throws rather than risking unbounded recursion; no asynchronous queue or thread-marshalling policy is introduced. Callers must avoid allowing a logging failure to mask their original game exception where that distinction matters.

Bootstrap registers before publishing the facade and only releases matching owned state. Conflicting owners are rejected rather than overwritten/restored as a hidden stack. SWJDebug.Set retains custom service identity. The sample retains Console output and manages only its optional UI target; clearing unrelated targets to simulate exclusive switching was rejected. These are approved Q1–Q7 decisions with explicit compatibility effects documented in the migration guide.
