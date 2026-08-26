# G5-3 Widgets product cutover


## Product boundary

- `Doroti.Framework.Widgets` is the reviewed product package for the full G5-3 Widgets selection.
- Generated application projects bind to reviewed framework packages through the framework project graph. They do not compile repository-private framework candidates into the application assembly.
- Each attached Flutter view receives typed capabilities. Missing capabilities fail with capability ID, view ID, target identity, and the calling Flutter symbol.
- The Windows host owns one native window, one Flutter surface, one frame request owner, and one terminal frame acknowledgement path.

## Compiler and promotion contract

- The frozen `g5-3-current68` baseline remains 185 generated files, 1,428 compiler diagnostics, and 211 unique C# errors. It is historical input, not current product success evidence.
- The current full selection contains 186 libraries and 1,715 declarations. Clean and incremental generation must produce the same output digest and compile with zero warnings and zero errors.
- Promotion validates all 1,715 dispositions and the `widgets.dart` public surface before copying reviewed sources into the product package.
- The current public API manifest records 169 exported Widgets libraries and 952 public declaration occurrences, with zero missing and zero extra entries.
- Handwritten compatibility projects may remain as non-product historical fixtures, but product Widget/Element lifecycle ownership must remain zero outside the reviewed framework package.

## Vertical behavior gates

W0–W7 are cumulative diagnostic slices, not separate product implementations:

- W0–W2 verify binding/root attach, stateless build/update/unmount, and stateful lifecycle/dirty coalescing.
- W3–W5 verify inherited dependencies, key reconciliation, focus/shortcut/action dispatch, overlay order, and route result/disposal.
- W6 verifies 1,000-item delegate behavior, scroll metrics/physics, and asynchronous image completion/lifetime.
- W7 verifies text controller revision, selection, composition, caret, action, and detach behavior.

Every slice requires clean/incremental identity and a zero-warning/zero-error candidate build. Managed product behavior is recorded separately from native-host and physical evidence.

## Application and package gate

A generated application exercises the promoted Widgets surface. The gate performs project-reference build first, then packs the 12 runtime/framework dependencies and restores, builds, and runs the generated application runner outside the repository. Repository-private compiler or candidate fallback count must remain zero.

## Evidence and deferred proof

The aggregate index is `migration/flutter-framework/g5-3-evidence.json`. It links predecessor, W0–W7, platform, text-input, API, disposition, behavior, and external application evidence.

Automated current-machine Windows evidence includes actual HWND bootstrap/attach/frame/GPU present/ACK/shutdown and typed capability round trips. Physical Windows IME, external physical accessibility, sustained physical GPU, and cross-monitor DPI remain `notVerified` until G5-8 `DorotiDemoApp` target-machine execution.
