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

For a Windows physical-input investigation, launch with
`DOROTI_WINDOWS_FRAME_TIMING=1`, `DOROTI_WINDOWS_APPSDK_DIAGNOSTICS=1` and
`DOROTI_WINDOWS_APPSDK_REPORT` pointing to a local JSON file. Closing the app writes
the bounded native packet history and the existing framework scroll trace.
Replay its release estimates after building the regression console:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet Doroti/tests/Doroti.Scroll.Tests/bin/Release/net10.0/Doroti.Scroll.Tests.dll temp/scroll-fix/windows-trackpad-report.json
```
