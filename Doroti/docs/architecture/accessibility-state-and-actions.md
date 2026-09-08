# Accessibility state and action boundaries

Reviewed 2026-09-09 after the Galaxy SegmentedButton selection reversal.

## Contract and Flutter comparison

The framework owns widget state. Publishing that state to the operating system
must not produce a new framework input action. An actual assistive-technology
request must remain able to activate, edit, select, focus, or adjust the widget.
Disabling accessibility controls wholesale would remove that input path.

Flutter implements this separation through platform accessibility adapters:

| Platform | Flutter source and boundary |
| --- | --- |
| Android | [AccessibilityBridge.java](https://github.com/flutter/flutter/blob/master/engine/src/flutter/shell/platform/android/io/flutter/view/AccessibilityBridge.java): `AccessibilityNodeProvider` exposes virtual nodes; `updateSemantics` publishes state and `performAction` handles accessibility requests. |
| iOS | [SemanticsObject.mm](https://github.com/flutter/flutter/blob/master/engine/src/flutter/shell/platform/darwin/ios/framework/Source/SemanticsObject.mm): node updates are separate from `accessibilityActivate` and increment/decrement callbacks. |
| macOS | [FlutterPlatformNodeDelegateMac.mm](https://github.com/flutter/flutter/blob/master/engine/src/flutter/shell/platform/darwin/macos/framework/Source/FlutterPlatformNodeDelegateMac.mm): platform accessibility actions dispatch through the delegate rather than control property-change handlers. |
| Windows | [accessibility_bridge_windows.cc](https://github.com/flutter/flutter/blob/master/engine/src/flutter/shell/platform/windows/accessibility_bridge_windows.cc): semantics updates and platform accessibility action forwarding have separate paths. |
| Linux | [fl_accessible_node.cc](https://github.com/flutter/flutter/blob/master/engine/src/flutter/shell/platform/linux/fl_accessible_node.cc): accessible node state and assistive action implementations are separate. Flutter uses ATK here; Doroti uses Qt. |
| Web | [text_field.dart](https://github.com/flutter/flutter/blob/master/engine/src/flutter/lib/web_ui/lib/src/engine/semantics/text_field.dart): DOM/native editing remains necessary; editing-state projection and input/selection event processing have separate responsibilities. |

These are source comparisons, not a claim that every Flutter callback or every
Doroti platform has identical behavior. Embedded platform views also have their
own accessibility implementations.

## Doroti changes

| Host / targets | Finding and change |
| --- | --- |
| MAUI: Android, iOS, Mac Catalyst, macOS, Windows | `MauiSemanticsBridge` uses real invisible controls. Its earlier per-control guard did not cover notifications from other controls during an update. Added a whole-tree projection boundary covering removal, reparenting, property writes, and batch commit. Every native action callback now checks that boundary and the current binding/action/enabled/read-only state. |
| Web | Added a whole-update boundary and current-node/action checks. Text and selection baselines use normalized DOM values so queued unchanged events do not echo the projection. Disabled fields use native `disabled`; real supported DOM editing/actions remain available. Geometry-only updates retain action metadata. |
| Windows App SDK native | Already uses virtual UIA providers; state publication does not invoke their action methods. Fixed radio selection to prioritize explicit checked state over independent row selection/highlighting, including property-change notifications. |
| Linux Qt | Already uses virtual `QAccessible` interfaces. Added current-node checks at action dispatch for removed, disabled, hidden, read-only adjustment, and unsupported actions. Applied the same change to the app and template native hosts. |

MAUI and Web track observed native values, advancing the baseline on native
changes as well as projection. Comparing every event only with the last framework
snapshot would incorrectly drop a rapid edit/toggle back to its original value
before the next snapshot. Regression tests cover this case. MAUI Entry now
projects normalized selection ranges; Web preserves selection direction.

The prior unique MAUI radio groups and activation-only radio handler remain in
place. Explicit radio checked state now takes priority over tile highlighting in
MAUI as well. The framework continues to own mutually exclusive selection.

## Validation

All test processes use a 1,200-second timeout.

| Validation | Result and scope |
| --- | --- |
| `validation/app-bootstrap/semantics-radio` | PASS, Debug/Release production MAUI bridge with real managed controls: cross-node projection, text/selection, checkbox/switch/radio/slider, repeated notifications, rapid reverse changes, and action lifecycle guards. No native platform handlers in this harness. |
| `validation/web-playwright/tests/semantics-projection.spec.ts` | PASS in Chromium using compiled production JS. Only managed callbacks and GPU startup are replaced. Covers queued selection events, normalized values, whole-tree suppression, genuine DOM actions, rapid reverse edits, backward selection, and stale/unsupported/disabled/read-only actions. |
| `validation/accessibility-projection-native` on Windows | PASS against production UIA bridge in a hidden window: repeated snapshots emit zero actions; actual Invoke/Toggle/Select/RangeValue interfaces each emit the expected action. |
| Same native validation under WSL Ubuntu / Qt | PASS with the production `QAccessible` implementation and offscreen Qt: snapshots emit zero actions; press/increase dispatch; invalid requests are rejected. |
| `validation/fcr6-semantics` | PASS framework semantics contracts. |
| Builds | Web, Android arm64 Release signed package, MAUI Windows Release, and production Linux Qt native build pass. Web test TypeScript check passes. |

Example commands from the repository root (each build/test subprocess should
also be run with `subprocess.run(..., timeout=1200, check=True)`):

```text
dotnet run --project Doroti/validation/app-bootstrap/semantics-radio -c Release
dotnet build Doroti/src/Doroti.Host.Web
# From Doroti/validation/web-playwright:
node node_modules/@playwright/test/cli.js test semantics-projection.spec.ts --project=chromium-hardware

cmake -S Doroti/validation/accessibility-projection-native -B <windows-build> -A x64
cmake --build <windows-build> --config Release
ctest --test-dir <windows-build> -C Release --output-on-failure

# From Linux/WSL; requires Qt 6.5+, wayland-client and wayland-scanner:
cmake -S Doroti/validation/accessibility-projection-native -B <qt-build> -G Ninja
cmake --build <qt-build>
ctest --test-dir <qt-build> --output-on-failure
```

## Remaining verification and architectural limit

The Galaxy was no longer attached during this broader change; only an x86_64
Android emulator was available. The arm64 package could not be installed there
(`INSTALL_FAILED_NO_MATCHING_ABIS`). This change has no new Galaxy device result.
The earlier radio-specific Galaxy touch/mouse results are historical evidence
for that earlier fix, not acceptance of this broader change.

iOS/macOS/Mac Catalyst builds and physical devices, and actual TalkBack,
VoiceOver, Narrator and Orca sessions, remain unverified. Native UIA/Qt calls and
managed/DOM event tests are not equivalent to a complete screen-reader session.

MAUI still exposes real invisible controls. The new boundary suppresses
projection-time callbacks and unchanged late notifications; it does not replace
MAUI with Flutter-style virtual providers or prove that every asynchronous OS
handler behavior is covered. A future native-provider migration should preserve
text editing, selection, focus, traversal and actions and be accepted separately
on each platform. The present changes address the demonstrated feedback path
and enforce a shared action boundary without that larger migration.
