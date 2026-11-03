#   Debug tests

Add com.sweetjelly.debug to the consumer project's testables and install Unity Test Framework. Run EditMode and PlayMode separately with filter SWeetJelly.Debug.Tests. The package owns correctness tests; the host owns cross-package and platform verification.

Test seams are the approved public service/formatter/facade contracts, bootstrap composition with ServiceRegistry and Unity UI Text output. EditMode covers formatting/filtering, callback registration changes/failures, recursion, custom service preservation and lifecycle ownership. PlayMode covers the real Unity UI history, bound, clear and borrowed output lifetime. Tests restore global facade/configuration state and release generated Unity objects.

Release validation additionally imports/exercises the actual sample and Inspector, checks an independent consumer with declared dependencies, and runs the targeted ordinary Players. Record exact Editor patch, target and fresh executed/passed/failed/skipped counts. Zero tests or a successful dispatch is not a passing result.
