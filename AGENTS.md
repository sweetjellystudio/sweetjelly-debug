#   Debug package instructions

Use the installed `unity-package` coordinator. Read [architecture][architecture] and [ADR 0001][adr] before changing routing, callbacks, ownership, assembly dependencies or Player behavior.

*   Preserve the public facade/ports, enum values, script GUIDs and serialized `output`/`maxLines` fields. Keep UI output in the package and sample-only screen logic in Samples~.
*   App/Contract remain Unity-independent. The caller serializes access; Unity outputs run on the main thread. Keep the synchronous exception contract and reject accepted same-service recursive logs.
*   Snapshot output targets before formatting. Registration changes affect later entries. Bootstrap rollback/shutdown release only the registration and facade they own.
*   The sample adds/removes its own UI output without clearing other consumers' targets. Preserve borrowed Canvas/Text objects; release generated objects and subscriptions.
*   Run package EditMode/PlayMode tests, actual sample/Inspector checks, isolated dependency consumption and supported Player checks on Unity 6.3 LTS. Keep current execution reports in the development host.
*   Installation belongs in [README][readme], upgrade effects in [migration][migration]. Follow the host's shared publication policy; committing and publishing require separate requests.

[architecture]: ARCHITECTURE.md
[adr]: docs/adr/0001-synchronous-routing-and-ownership.md
[readme]: README.md
[migration]: Documentation~/migration-2.0.md
