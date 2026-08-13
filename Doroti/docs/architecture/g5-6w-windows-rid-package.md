# G5-6W Windows RID package boundary

G5-6W fixes the already validated G5-3B Windows source-port closure as the `Doroti.Target.Windows.win-x64` NuGet package. It does not move Flutter framework policy into the target. A selected framework package still talks only to `Doroti.Flutter.Hosting` and the typed `dart:ui` capability surface; the RID package composes that host with the Win32/WGL implementation.

## Package and release inputs

The package contains `Doroti.Host.Windows.dll` plus three review inputs:

- `doroti/doroti-target-manifest.json` identifies the RID, package, Flutter/Avalonia revisions, schemas and release-input hashes.
- `source-port/port-selection.json` is the reviewed source selection used by `Doroti.AvaloniaPort audit`.
- `source-port/provenance.json` binds the A1 Win32/IME/automation closure, A2 WGL/render/recovery closure and reviewed Skia adaptations to their source hashes.

The package graph contains no official `Avalonia` binary package. `Doroti.Vendor.Avalonia.*` remains a reviewed Doroti source-port dependency and is not an official Avalonia binary.

## Stable runtime contracts

`WindowsFlutterTarget` is the win-x64 composition entrypoint. Its target identity uses `doroti.target-identity/v1` and records runtime RID/OS/architecture, Win32 and WGL/OpenGL/Skia backend IDs, package version, Flutter/Avalonia revisions and source-port hashes.

`DesktopFlutterHost.GetTargetDiagnostics` exposes `doroti.desktop-flutter-target-diagnostics/v1` for `DorotiDemoApp` and package validation. One snapshot contains:

- frame submission/terminal ACK/mailbox/backend/fallback/recovery counters;
- native input capabilities and normalized pointer/key/focus/metrics counters;
- Flutter semantics generation and native automation node count;
- logical/physical coordinate state and native window/OpenGL resource counts.

The registered capability IDs are read from the actual attached Flutter view, not duplicated in the RID package. The shared required list is `FlutterCapabilityIds.RequiredDesktop`.

## Validation

Run from `Doroti`:

```powershell
./eng/validate-g5-6w.ps1
```

The validator reruns the G5-3B Windows predecessor, audits the selected Avalonia source port, builds and packs the product graph, validates packaged release-input hashes, and repacks the RID host without changing the Widgets framework package bytes. It then copies a package-only consumer outside the repository graph, restores into an isolated NuGet cache, builds, publishes for `win-x64`, and executes the published EXE against an actual HWND and strict WGL/OpenGL context.

The synthetic smoke verifies the 12 capability IDs, native message-queue pointer/key delivery, `WM_GETOBJECT`, semantics diagnostics, one injected GPU recovery, terminal ACK accounting, mailbox high-watermark, target identity and resource hooks. The publish graph and `project.assets.json` must contain zero official Avalonia binaries and zero repository-private fallback.

Machine-readable truth is written to:

- `migration/flutter-avalonia/bridge-validation/g5-6w.json`
- `migration/flutter-avalonia/target-capabilities/win-x64.json`
- `artifacts/g5-6w/windows-package/`
- `artifacts/g5-6w/windows-publish/win-x64/`

Physical mouse, precision touchpad, touch, Korean IME, cross-monitor DPI, external assistive-technology behavior and sustained GPU use are deliberately not claimed here. They remain `notVerified` until the final G5-8 `DorotiDemoApp` run.
