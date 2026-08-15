# A1 source-ported desktop shell

`Doroti.Shell.Core` is the backend-neutral contract for the desktop event loop, dispatcher, top-level/window lifecycle, native handle descriptor, screens, scale/surface generation and platform-service registry. Public contracts contain only BCL and Doroti geometry types.

`Doroti.Vendor.Avalonia.Win32` adapts the A0-pinned Avalonia Win32 `WindowImpl`, WndProc, dispatcher, screen, cursor, clipboard and IMM32 flow. It owns HWND creation, per-monitor-v2 DPI, input translation, capture cancellation, close ordering and native service calls. Its only friend is `Doroti.Host.Desktop`, and it has no Avalonia NuGet dependency. [`a1-source-port-provenance.json`](../../migration/avalonia-shell/a1-source-port-provenance.json) records upstream input hashes as provenance metadata and pins every local adaptation owner.

`Doroti.Host.Desktop` converts the internal source-port events to `Doroti.Platform` window, pointer, key, text, clipboard, cursor and semantics contracts. Its UI Automation provider maps the immutable Doroti semantics tree to `IRawElementProviderFragmentRoot`/fragment nodes and `InvokePattern`; the source-ported WndProc returns that provider from `WM_GETOBJECT`. `WindowMetrics.Generation` and `IWindowCoordinateDiagnostics` allow the A1 target test to prove that native client size, logical pointer position, physical pointer position and IME caret derive from the same metrics generation. A2/D0 may refine the surface-generation and rounding policy without exposing HWND or vendor types.

The default template now selects `Doroti.Host.Desktop`. It still selects `Doroti.Backends.Skia` explicitly as a temporary pre-A2 surface; A2 owns the direct GPU surface and asynchronous frame-pipeline cutover. The old `Doroti.Host.Avalonia` remains only in the full solution and `ShellHostComparison` A/B harness.

## Verification

`tests/Doroti.Shell.Win32.Tests` runs a real HWND through startup, resize, minimize/restore, monitor movement, native pointer/Unicode text, IME caret placement, clipboard, cursor, semantics action and ordered close. A separate `powershell.exe` process uses Windows `UIAutomationClient` to discover the HWND, navigate to `semantics-7`, validate name/control type/enabled state/bounds and invoke `InvokePattern` back into the Doroti action delegate. The suite also scans the public API and direct assembly references for Avalonia/vendor/Win32 leaks.

`samples/ShellHostComparison` runs the same lifecycle scenario with `--host source-port` and `--host package`, writing `doroti.a1-shell-comparison/v1` reports. On the 2026-08-03 Windows target, both paths passed and the connected physical monitors exposed 125% and 200% scale. The 96/120/144/192 DPI coordinate contract matrix passes deterministically; physical 96/144 DPI displays are not an A1 completion requirement. Native `WM_GETOBJECT` and external UI Automation navigation/invocation pass, so the A1 target ledger is `pass`.

## G7-3M successor

G7-3M lifts input, text, clipboard, cursor, graphics, focus, input-test, and accessibility services into typed `Doroti.Shell.Core` capabilities. `Doroti.Host.Desktop` now receives `IShellWindowingPlatform` instead of constructing Win32 types, while `Doroti.Target.Windows.win-x64` preserves Windows composition and `Doroti.Target.macOS.osx-arm64` adds the independent AppKit/NSOpenGL composition root. See [the G7-3M macOS RID package record](g7-3m-macos-rid-package.md) for the current cross-target boundary and evidence.
