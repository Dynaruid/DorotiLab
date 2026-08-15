# R9 product UI foundations

> Historical roadmap evidence. Ownership rows naming `Doroti.Widgets` describe the pre-G5 implementation; current ownership is in the reviewed `Doroti.Flutter.Framework.*` graph.

R9 adds the design-system-neutral services that Material and Cupertino must consume. It does not add either design system, and it does not move backend, HWND, Skia, or Dart types into the public widget/render contracts.

## Ownership and flow

| Capability | Owner | Runtime flow |
|---|---|---|
| notification and UI dispatch | `Doroti.Core` | `ChangeNotifier` / `ValueNotifier<T>` and queued owner-thread dispatch |
| scroll and animation | `Doroti.Widgets` | lazy fixed-extent viewport, controller/physics/drag arena, fake-time animation/tween/ticker |
| focus and command | `Doroti.Widgets` + `Doroti.Platform` | focus tree/traversal/shortcut/action to explicit clipboard port |
| text editing | `Doroti.Rendering` + `Doroti.Widgets` + `Doroti.Platform` | Unicode paragraph snapshot, grapheme editing, composition reducer and caret geometry |
| image | `Doroti.Composition` + backend decoder | encoded provider to CPU BGRA cache/lease/resource registry and immutable display command |
| semantics | `Doroti.Rendering` + `Doroti.Platform` | RenderObject descriptions to immutable snapshot/action bridge after layout |
| overlay and route | `Doroti.Widgets` + `Doroti.FlutterCompat` | opaque entry pruning, modal barrier, route observer/result/disposal/root protection |

`InteractiveApplication` drains queued UI completions before build and publishes the post-layout semantics snapshot to an optional window bridge. `RasterInteractiveFrameSink` owns the image cache with the same resource registry used by frame ACK processing, so eviction cannot remove an active-frame resource.

## Automated acceptance

`tests/Doroti.R9.Tests` is part of `Doroti.RuntimeTests.slnx` and the `runtime`/`full` engineering suites. Its seven scenarios cover notification and fake-time ordering, a 1,000-item lazy list and drag competition, focus/actions, editable composition/caret, encoded image/cache/cancellation, semantics actions, and overlay/navigation lifetime.

`DorotiDemoApp` is the target smoke path. It renders 1,000 logical items through `VirtualListView` backed by `RenderSliverFixedExtentViewport`; only the visible/cache range is mounted. The current A2 path uses the source-ported Win32 shell and strict Skia GPU frame pipeline. F2's 30-second target measures continuous scroll and animation frame cadence; the separate four-scale DPI/input report remains an explicit target gate.

## Deliberate release boundary

The former H6 bounded Material/Cupertino vertical slice is not an active Goal3 completion input. Its test and package evidence were removed in G3-0; generated/reviewed framework convergence must be established again from the pinned Flutter source through the new validation graph.

`DOT-0041` therefore remains open and `migration/releases/r9-foundations.json` remains authoritative. Unicode grapheme-aware measurement is still not a HarfBuzz-grade shaping/font-fallback implementation, real Korean IME candidate placement and Narrator need human validation, device-loss recreation and target performance/minimized-window behavior remain unmeasured, and physical Linux/macOS runs are absent. The H6 slice must not be read as complete Material or Cupertino catalog compatibility.
