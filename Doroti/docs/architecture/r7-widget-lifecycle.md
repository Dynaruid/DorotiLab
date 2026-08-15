# R7 Widget lifecycle and Compiler C2

> Historical roadmap evidence. The handwritten `Doroti.Widgets` implementation and FlutterCompat adapters described below were removed after G5-3.

R7 adds the backend-neutral Widget/Element tree in `Doroti.Widgets`. Widgets are immutable configuration, Elements own identity and reconciliation, State belongs to a `StatefulElement`, and RenderObjects remain the only layout/paint owners. `BuildOwner` pins the UI thread, processes dirty Elements by depth and stable schedule sequence, restores `Idle` after faults, and retains failed dirty work for an explicit retry.

Identity is `Widget.IdentityType + Key` within a parent. Unkeyed children prefer the current slot; local keyed children can reorder within their parent without replacing State. `RenderObjectElement` derives the direct RenderObject descendants from the reconciled Element order and applies that order through `RenderProxyBox` or `IRenderObjectChildContainer`. Widget configuration never enters `Doroti.Rendering`.

State is mounted before `InitState`, then observes `DidChangeDependencies`, build, update, deactivate/activate and dispose in trace order. `SetState` validates the BuildOwner thread and lifecycle before running the callback. Calls during build, after dispose, before mount, or with a Task-returning callback fail explicitly. An exception leaves the Element dirty and the build phase recoverable.

`InheritedElement` owns its dependent set. Dependencies are registered by `BuildContext`, notified only when `UpdateShouldNotify` returns true, and removed on deactivate. A `GlobalKey` remains registered while its subtree is inactive, may be retaken once in the same build/finalize cycle, and is rejected if it is duplicated or moved from an unsupported nested inactive root. `FinalizeTree` unmounts remaining inactive roots and clears every reservation.

`Doroti.FlutterCompat` defines separate public Widget, State and Key types. Internal adapters translate them to `Doroti.Widgets`; no native Element, RenderObject, platform, backend or vendor type appears in the public facade. `migration/behavior/r7-widget-lifecycle.json` runs through the pinned Dart reference model and the real FlutterCompat adapter. `Doroti.R7.Tests` additionally covers reorder, RenderObject order, dependency cleanup, misuse, exception recovery, GlobalKey movement and thread ownership.

Compiler C2 accepts the selected callback/lambda, `super` constructor/method call, mutable State field, override and Widget/State/Key inheritance forms in `c2_widgets_fixture`. The generated project references only `Doroti.FlutterCompat`, builds outside the product solution, and is compared with an equivalent hand-written fixture using the same normalized lifecycle trace. Clean/cache-off, incremental/cache-on and cache-hit artifacts must remain byte-identical.
