# Cross-platform modal navigation review

Date: 2026-10-21

Follow-up to [Android system Back](android-system-back.md). Existing Android
changes are retained. `work.md` remains the unrelated Qt packaging plan and was
not changed (SHA256 `496316a00f30978758d3d567d1d560224bdfb4905ca8c0a7319f0ac91ccc9c94`).

## Confirmed problems and changes

- **All widget platforms:** `NavigatorState.build` returned only its Overlay,
  leaving `NavigatorState.focusNode` unattached. Routes could not find its
  enclosing focus scope, so a modal without an autofocus control kept keyboard
  action lookup in the application's root/page. Restore Flutter's
  `FocusTraversalGroup`, attached Navigator `Focus` and `UnmanagedRestorationScope`
  around the Overlay. Escape now resolves the top modal's DismissIntent, follows
  `barrierDismissible`/`PopScope`, and restores the underlying page's focus.
- **Web:** history selection requests were skipped whenever the typed navigation
  capability existed. The browser bridge always used Router URL activation,
  including imperative Navigator dialogs. Forward the history-selection methods
  and implement Navigator's origin/guard entry versus Router's ordinary history.
  Restore the guard before dispatching `popRoute`, allowing asynchronous decisions
  and vetoes to stay in the document. Only an unhandled root Back requests
  `SystemNavigator.pop`, which removes the guard and exits to the previous document.
  Mode changes queue route reports while removing the old guard; URL/state
  Back/Forward, manually edited hashes, reload and bfcache return are covered.
  Late route reports during root exit are discarded so they cannot overwrite
  the destination history entry.
- **Web without restoration options:** the first view now gets browser navigation
  ownership without requiring a restoration namespace. Worker startup captures the
  initial route even when Navigation options are omitted. One document still has
  one navigation owner; auxiliary views do not claim the same history.
- **Web focused input:** the DOM keyboard filter discarded Escape whenever the
  native text endpoint had focus. Forward non-composing Escape to framework
  navigation while retaining native ownership during composition.
- **MAUI Windows focused input:** the legacy SKGL native keyboard adapter similarly
  discarded every TextBox key. Forward Escape outside native composition. Track
  composition from Start through End, including an initially empty range, and
  clear it on client replacement, clear, suspend, detach and focus loss.

WindowsAppSDK and Qt already translate Escape to the expected logical/physical
identity. AppKit's render surface and UIKit's hardware keyboard view also map the
key; the common Navigator fix applies to their framework event path. Desktop
window close and iOS native navigation retain their existing host policies.
This change does not introduce an Android-style global Back on Apple platforms.

Primary references used for the comparison:

- [Flutter Navigator build and focus ownership](https://github.com/flutter/flutter/blob/master/packages/flutter/lib/src/widgets/navigator.dart)
- [Flutter browser single/multiple entry history](https://github.com/flutter/flutter/blob/master/engine/src/flutter/lib/web_ui/lib/src/engine/navigation/history.dart)
- [Flutter SystemNavigator history selection](https://api.flutter.dev/flutter/services/SystemNavigator-class.html)
- [ModalRoute dismissible barrier and Escape policy](https://api.flutter.dev/flutter/widgets/ModalRoute/barrierDismissible.html)

## Verification

All test/build commands used `Doroti/eng/run-with-timeout.py --timeout 1200`.

| Check | Result and boundary |
| --- | --- |
| `Doroti.Tests --modal-navigation` | PASS: Windows/Linux/macOS/iOS flags, stacked dialogs, Escape barrier policy, PopScope, root, focus restoration; Android Back channel/lifecycle and navigation/restoration regression. CPU/synthetic. |
| Material regression executable | PASS: existing text focus/composition/selection, menus, tooltip/overlay and expansion lifetime. CPU/synthetic. |
| Cupertino regression executable | PASS: pointer-opened dialogs/blur/teardown, text input, gestures, semantics and concurrent dispatcher ownership. CPU/synthetic. |
| Node browser navigation/rendering/worker lifecycle | PASS: 29 tests, including seven new navigation tests. Synthetic browser history/DOM. |
| Qt host contracts | PASS: owner/lifecycle detachment and existing native ABI/activation contracts on Windows; no Linux Qt runtime claim. |
| Web Testbed Debug build | PASS: 0 warnings/errors, managed host + TypeScript + threaded browser runner. |
| WindowsAppSDK Testbed Debug build | PASS: 0 warnings/errors. |
| MAUI Windows Testbed Debug build | PASS: 0 warnings/errors. |
| Actual Chrome Worker WebGL and WebGPU | PASS in both: bottom sheet, alert dialog and fullscreen dialog close on browser Back in the same document; root exits to previous document; composing Escape is retained, subsequent non-composing Escape from the production DOM endpoint closes the dialog; Router Back/Forward URL/state remains usable; no page/console errors. Routes open through production DOM semantics actions; this does not qualify pointer geometry. Composition events were simulated, rather than physical IME input. |
| Actual WindowsAppSDK window | PASS: UIA-opened bottom sheet, alert dialog and fullscreen dialog close after posted HWND Escape; same PID 43044, `Responding=True`, title `Doroti Material Testbed`. Automated native messages, not physical keyboard input. |
| Actual MAUI Windows window | Starts with title `Doroti Material Testbed`, `Responding=True`. The window exposes one UIA element, so the modal UIA smoke could not proceed. Modal/IME native acceptance is not established for this host. |

Local logs, receipts, screenshots and the isolated Python Playwright environment
are under ignored `temp/testing/modal-navigation-20261021/`. Maintained tests and
reproduction commands are documented in [tests README](../../Doroti/tests/README.md).

Actual Linux Qt, macOS, Mac Catalyst and iOS execution was not performed in this
Windows workspace. Apple hardware keyboard/IME/native navigation, physical
Windows IME composition, Android predictive gestures and physical Android input
remain outside this follow-up's runtime evidence. The prior Android emulator
receipt predates the additional Navigator focus wrapper; Android's updated
framework path was rerun synthetically here.
