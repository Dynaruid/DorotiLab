# MediaQuery and SafeArea host contract

The framework reference is Flutter `56b8e1a851a594b1a154f8ea93270807dab22b9a`. This is a pinned comparison, not a claim about the latest Flutter release.

Use `MediaQuery.fromView` at a view boundary (the default framework view already supplies it), then `SafeArea` or `SliverSafeArea` where content needs protection. Do not add platform-specific status-bar or keyboard heights to app widgets. `Scaffold.resizeToAvoidBottomInset` owns keyboard avoidance for Scaffold slots.

`ViewMetrics` carries physical drawable size, physical insets, physical gesture slop and corner radii, and immutable **logical** display-feature bounds. It validates finite, nonnegative geometry and positive DPR, including fractional DPR below one. `padding` is derived per edge as `max(0, viewPadding - viewInsets)`. `MediaQueryData.CreateFromView` captures one metrics record and divides physical measurements by DPR once. Display-feature bounds are already logical.

Geometry, environment changes and GPU surface replacement have separate identities. An inset-only update must not publish a new native resize target. Windows native ABI and Qt adapter fixtures explicitly exercise keyboard show/hide without a resize, including Qt's subsequent `BeginFrame`. MAUI defers geometry-inconsistent environment observations until the matching drawable epoch arrives. The Windows surface captures XAML environment data on the UI thread before releasing its native metrics target to the existing dedicated metrics worker.

Platform settings have an explicit dispatcher snapshot and per-view environment scope. Creating a second view does not reset settings to defaults. Locale, text scale, brightness and accessibility callbacks use actual changes; other settings such as 24-hour format also update mounted `fromView` widgets. Native font scaling is captured with the text scaler rather than read from a changing dispatcher later. `LocalizationsResolver` initializes its locale and registers its observer, matching the pinned constructor. `ScrollPosition.moveTo` normalizes the base-type optional `clamp` argument so keyboard-driven caret reveal has Dart-equivalent behavior.

## Platform suppliers

| Host | Geometry and settings |
|---|---|
| Android MAUI | API 30 typed system-bar/IME/gesture insets and animation callbacks; API 24–29 visible-frame fallback; cutouts, API 31 corners and bold text; API 34 nonlinear SP conversion and contrast; WindowManager fold/hinge callbacks; settings observers and locale list. |
| UIKit: iOS, iPadOS, Catalyst | Actual UIView safe area, scene/window coordinate conversion for keyboard frames, temporary display-link interpolation during reported keyboard animations, Dynamic Type and accessibility notifications. Reduce Motion changes `reduceMotion`; the pinned UIKit policy leaves `disableAnimations` false. |
| AppKit MAUI | NSView safe-area edges, owning NSWindow backing scale, screen/fullscreen notifications, NSWorkspace accessibility/VoiceOver observation and current locale. |
| Windows MAUI | Actual render element and XamlRoot scale, per-HWND InputPane, UISettings text/animation and Win32 contrast, window settings/time messages, user locale preferences. |
| Windows App SDK | Native InputPane interop, UISettings events and Win32 contrast/settings messages, client-to-child intersection, versioned native metrics payload and managed resize admission. Non-client title bars/taskbars are not assumed to be content insets. |
| Linux Qt | QInputMethod keyboard rectangles, QWindow scale, Qt 6.9 safe areas and Qt 6.10 contrast preference behind version guards. Qt 6.5 remains the minimum. Empty compositor keyboard rectangles remain zero with an explicit support limitation. |
| Web | CSS safe-area probe, root-local occlusion intersection, VirtualKeyboard geometry when available, conservative focused full-page visualViewport fallback, dynamic media queries and feature-detected viewport segments. Both worker renderers use this provider. |

The complete field-by-host inventory, minimum versions, initial values, events and fallback reasons is [coverage.json](../validation/evidence/media-query-safe-area/coverage.json). `unsupported` is a documented API/service policy, never a successful device observation. Native failures and unexecuted scenarios remain `notVerified`. A supported supplier returning zero is different from a missing supplier.

## MAUI ownership

The standalone ContentPage, Doroti Grid and semantics AbsoluteLayout explicitly use `SafeAreaEdges.None`. Android standalone windows request edge-to-edge content with `adjustResize`; actual drawable bounds decide whether an IME rectangle still overlaps the view. No extra keyboard height is subtracted after native viewport resize. The iOS process-wide keyboard auto-scroll manager is disconnected only when the process owns a `DorotiMauiApplication`; arbitrary embedding applications are not globally reconfigured.

Native observers belong to the render element and are removed on unload, handler replacement and disposal. Keyboard and safe-area observation continues when MAUI layout consumption is disabled. Diagnostic snapshots include both the native environment view size and drawable size, plus raw physical insets.

## Compatibility and limits

- Windows host ABI numeric version is **2**, with a 192-byte metrics payload. Export/file names retain the `v1` family names; the version/size/offset handshake rejects incompatible binaries. Rebuild/repackage managed host and native target together.
- Qt ABI numeric version is **3**, with a 160-byte metrics payload and the extended configuration callback. Header/export family filenames remain `v2`; rebuild the consumer's native host from the updated template. An old native library is rejected.
- Web worker protocol is **4**. Deploy the managed runtime and all generated worker modules together; older envelopes are rejected. Insets and environment generation are validated before managed delivery. Display features use explicit wire DTOs, avoiding computed `Ui.Rect` aliases in JSON metadata.
- Flutter's pinned `MediaQueryData` equality uses display-feature **content** equality, while the display-feature aspect uses list identity. Mounted `fromView` reuses the unchanged list. `removeDisplayFeatures` retains the original `size` and feature coordinates, matching this pin.
- Floating/split keyboard rectangles that do not span an edge cannot be represented by four insets. They are not inflated into a full-width bottom keyboard. UIKit animation interpolation is an approximation of reported timing; physical interactive dismissal, spring curves and multiple simultaneously active scenes remain device gates.
- Web visualViewport fallback is heuristic. It requires editing, scale near one, a substantial height decrease and a full-page root. Embedded roots use explicit VirtualKeyboard geometry when available and otherwise retain the pinned zero-keyboard fallback. Address-bar/pinch fixtures pass; actual Safari/Chromium mobile behavior still needs device evidence.
- Browser font scaling and AT-active state are not guessed from DPR or semantics enablement. Hosts without a native announcement service report `supportsAnnounce=false`; Doroti overlay menus do not imply `supportsShowingSystemContextMenu=true`.
- AppKit has no implemented public onscreen keyboard-occlusion supplier. Qt 6.5–6.8 has no QWindow safe-area API; Qt before 6.10 has no contrast-preference API. Unsupported native text-scale/accessibility fields retain documented defaults and parent overrides.
- RID-less MAUI restore and compilation now share `obj/project.assets.json`; framework-specific output folders remain distinct. This prevents fresh consumers from compiling against stale per-framework package assets.

## Verification and reproduction

From the repository root:

```powershell
python Doroti/validation/media-query-safe-area/run-validation.py builds
python Doroti/validation/media-query-safe-area/run-validation.py contracts
python Doroti/validation/media-query-safe-area/run-validation.py products
```

Each invocation inside the runners has a 1200-second timeout. Native Windows is built by `Doroti/eng/build-hwnd-exact-cpp-native.ps1`; Qt is built from the template CMake project with the installed Qt SDK. The browser fixture uses Playwright from `validation/web-playwright` and generated host modules, so build the Web host first.

Set `DOROTI_TESTBED_MODE=media-query` to open the diagnostic testbed. On Android, launch the testbed Activity with `--es doroti_testbed_mode media-query`. On Web use the existing `dorotiTestbedMode=media-query` query option. The screen exposes raw/logical/consumed metrics, SafeArea and Scaffold avoidance toggles, SliverSafeArea, bottom and multiline inputs, a dialog and a modal bottom sheet. `MQ-FIXTURE` log lines contain scalar diagnostic snapshots.

Evidence is indexed under [media-query-safe-area](../validation/evidence/media-query-safe-area/README.md). Compiling iOS/Catalyst/AppKit on Windows verifies managed sources against the platform reference SDK; it is not Apple native execution or signing validation. Physical Android, Apple devices, notched/foldable hardware, touch keyboards, Wayland/X11 and mobile browser coverage retain their individual evidence status. The work was closed at the user's request; decisions, results and the recorded verification scope are preserved in the [completion summary](../../history/26-09-10/media-query-safe-area-summary.md).
