# Scroll regressions

Run from the repository root with the required 20-minute timeout:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
```

The suite checks queued release velocity, actual pauses, ballistic continuation,
Android MotionEvent clock conversion, semantic snapshot change detection, and
Windows native inertia completion versus recontact. Windows status callbacks use
a small fake COM viewport. A second Windows fixture creates two hidden HWNDs,
checks registration and message delivery through one real DirectManipulation
manager, and verifies both subclasses retire on disposal. It injects no physical
input and does not show a window.
These checks do not establish physical trackpad behavior or display frame pacing.

It also drives the production scheduler/ticker/animation controller at 60/120 Hz
and with missed pulses, checks fractional offsets and final ticker retirement,
unchanged dimensions and recontact, and tests all four semantic scroll boundaries.
Projection checks cover alias-safe allocation-free matrix composition, perspective
and legacy origins, DPR, layout text geometry and obscured text. Android vsync
conversion retains callback age instead of shifting time by the first delayed pulse.

Caret-drag checks reuse immutable semantics text geometry while glyph layout is
unchanged. They verify grapheme ranges and cache invalidation after text edits,
wrapping, text scaling, scroll offsets, RTL/center alignment, obscuring and disposal.
These CPU checks do not qualify physical finger-to-display latency.
[Galaxy cursor-handle drag evidence](../../../history/26/10/31/galaxy-caret-drag.md)
separates the first-drag fix, native magnifier lifetime/IME checks and remaining
sustained-drag performance limits.

Galaxy ADB and continuous single-contact profiling commands and evidence limits:
[2026-10-31 implementation record](../../../history/26/10/31/galaxy-scroll-implementation.md).

For a Windows physical-input investigation, launch with
`DOROTI_WINDOWS_FRAME_TIMING=1`, `DOROTI_WINDOWS_APPSDK_DIAGNOSTICS=1` and
`DOROTI_WINDOWS_APPSDK_REPORT` pointing to a local JSON file. Closing the app writes
the bounded native packet history and the existing framework scroll trace.
Replay its release estimates after building the regression console:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet Doroti/tests/Doroti.Scroll.Tests/bin/Release/net10.0/Doroti.Scroll.Tests.dll temp/scroll-fix/windows-trackpad-report.json
```
