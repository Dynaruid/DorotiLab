# Windows trackpad inertia and Android scroll review

Date: 2026-10-31 (Asia/Seoul). Baseline: `0f2964c4618d6cb9599dd537743ae12bd4078239`.

## Changes

- Windows DirectManipulation registers the logical input HWND and the raster
  child's input surface with one manager/viewport. Both can be input targets as
  the composition host changes its visible presentation surface.
- Windows DirectManipulation `INERTIA -> READY` no longer cancels the framework's
  independent ballistic scroll. An interruption/recontact still sends an inertia
  cancel, including when the last native delta was zero.
- Drag recognizers estimate release velocity against the input's release timestamp.
  The existing parameterless velocity API and the 40 ms real-pause threshold remain.
  The baseline reproduced `-1000 px/s -> 0 px/s` solely after a 65 ms dispatch delay;
  the native status probe also reproduced cancellation on natural completion.
- Android translates `MotionEvent.EventTime` from the uptime clock into the
  framework clock while retaining queue age. Both Graphite and the MAUI input
  subscription use this conversion for touch, trackpad and wheel input.
- Android virtual accessibility retains the latest snapshot without indexing,
  filtering, diffing or notifying the native root on every update when no service
  or virtual focus needs it. Provider reads/actions materialize the latest snapshot
  before using it. Active accessibility services retain immediate updates.
- The snapshot boolean comparison avoids allocating a full delta and content hashes
  when Android only needs to know whether the tree changed.

## Verification

All test/build commands use `Doroti/eng/run-with-timeout.py --timeout 1200`.
The focused suite lives in `Doroti/tests/Doroti.Scroll.Tests`.

Windows baseline was launched with the user's exact command:

```powershell
dotnet run --project ./samples/DorotiTestbedApp/windowsappsdk/DorotiTestbedApp.WindowsAppSdk.csproj
```

The baseline window rendered, accepted an injected OS wheel scroll, and closed
cleanly. The changed Debug window also launched with that command and rendered.
Delivered `Doroti.Hosting`, `Doroti.Ui` and `Doroti.Framework.Gestures` DLL SHA-256
values matched their current Debug source-build outputs. The user physically
confirmed inertia exists, but initially reported it remains shorter than Flutter
even while content remains below the current position. That feedback prompted
the native input captures below; the first changed build was still **PARTIAL**.

A follow-up physical trackpad capture recorded **627 mouse wheel packets and zero
pan/zoom packets** (`trackpadWheelPackets=0`, device scale 2). All 360 retained scroll
trace entries used `IdleScrollActivity`/`pointerScroll`, including motion in the
middle of content (`maxExtent=4264.2`). Thus the initial changes did not repair the
actual input route; short motion was not a clamping-physics or content-edge result.
A hit-test-only experiment also retained wheel fallback (522 mouse wheel packets,
zero pan/zoom, native hit-tests=0, status=READY). It was reverted. The next build
registers the raster child with the existing DirectManipulation manager as well
as the logical input HWND.

The subsequent physical capture recorded **90 pan/zoom updates and zero wheel
packets**. Native hit-tests/accepted contacts were 10/10, and the recorded contact
HWND was the raster child (`8784688`), rather than the logical input HWND
(`11078466`). The same manager owned both windows. The retained 150 packets include
26 starts/ends and six inertia-cancel signals. The retained framework trace has
194 `BallisticScrollActivity` and nine `DragScrollActivity` records, replacing the
previous wheel-only `IdleScrollActivity` path. `failedTerminals=0`; shutdown drained
with `pending=0` and exited 0. The user then confirmed **"관성이 길어짐"**.
This verifies repaired physical trackpad ingress and improved inertia for the
reported Testbed route. Exact physical parity with Flutter is not asserted.

The final Debug build preserves the original three-argument public constructor.
Its delivered Hosting/AppSDK DLLs match their corresponding source-build SHA-256
values (`67911819cbe6effbb967ea2c73933b159b5193cfedfc526a8c96fe16c8ab3f07` /
`42b7a762a1ca4a4d60e924471be0dd36fc307387fa0b4426eee1127ed5907451`).
An injected OS wheel still moves the right-hand list. The Platform views page
loads native controls; a native editor accepts `scroll-check`, and clicking
Components returns focus and rendering to the framework. These are interaction
smokes, separate from the physical trackpad capture above. That final run records
one mouse wheel packet / zero pan-zoom packets, zero failed terminals, and exits
0 after draining all pending work.

The opt-in Windows report records at most 256 native precision-trackpad packets
alongside the existing scroll-position/activity trace, plus native hit-test,
contact and update counts. It is disabled in normal runs and performs no per-packet
file I/O. A temporary continuous-update-pump hypothesis was removed before the
routing build; idle polling is not part of the final change.
The regression console can replay a report to calculate release velocity and
predicted duration/distance with the production tracker and physics.

An actual Flutter SDK test passed the same synthetic constant-velocity gesture.
Both implementations produce approximately 458.231 ms / 194.314 logical pixels
at 1000 logical pixels/s. Flutter's other reference values were 140.114 ms / 11.882
pixels at 200 px/s, 275.078 ms / 58.322 pixels at 500 px/s, and 763.350 ms / 647.399
pixels at 2000 px/s. This rules out the tested clamping formula as the difference;
it does not establish parity for native input timing or displayed motion.

Android baseline and changed APK use the same command:

```powershell
dotnet build ./samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-x64 -m:1 -nr:false -v:minimal
```

Emulator: `19_12T`, `emulator-5554`, Android 13 / API 33, x86_64,
1920 x 1200, density 240, Graphite/Vulkan, NVIDIA RTX 4060 Laptop GPU.
No accessibility service was enabled during the scroll comparison.
The baseline APK was rebuilt from HEAD, installed and visually checked, rather
than using the old installed Gallery build.

Each variant runs three fresh-process trials, waits 12 seconds for startup, then
performs four alternating 350 ms swipes at x=600, y=950->300 and y=350->1000,
with 2 seconds between gestures. Diagnostic intent extras are identical:
`DOROTI_PLATFORM_FRAME_PROFILE=1`, `DOROTI_INPUT_TIMING=1`.
No builds run concurrently with the measured windows.

Baseline APK SHA-256:
`0655d27b50f5197bb87527d41715e8c4ff070141912e0c06caccf1672516d279`.

Changed APK SHA-256 (also verified against the installed `base.apk`):
`c831b675609b7cae72b6a8b720713b4bb8234c3750412eaee481365244974bfb`.

| Variant / trial | Frames | Owner p95 ms | Contact owner p95 ms | Owner max ms | Input queue max ms |
| --- | ---: | ---: | ---: | ---: | ---: |
| Baseline 1 | 197 | 17.929 | 18.585 | 54.941 | 56 |
| Baseline 2 | 185 | 19.639 | 21.858 | 121.129 | 114 |
| Baseline 3 | 179 | 17.569 | 20.701 | 57.245 | 58 |
| Changed 1 | 196 | 17.671 | 18.686 | 60.131 | 53 |
| Changed 2 | 203 | 15.456 | 15.074 | 54.594 | 47 |
| Changed 3 | 204 | 14.545 | 14.751 | 58.916 | 52 |

The median of the three contact p95 values changed from 20.701 to 15.074 ms.
Contact windows contain 221 baseline / 222 changed frames, and are separated
from ballistic tails because release timing changes can alter the tail's length.
Across the entire scroll windows, owner work over 16.667 ms changed from 39/561
to 29/603 frames. This is a combined-change diagnostic; the individual contribution
of accessibility and timestamp fixes was not isolated.

Residual stalls remain: changed owner maxima are 54.6-60.1 ms, and first-touch
dispatch maxima remain 94.2-103.2 ms. This does not establish a jank-free result.
Owner/Vulkan timings measure processing/submission, not display FPS or scanout.
Physical Android acceptance and TalkBack interaction remain unverified.

The focused regression suite passed after the final input-clock and multi-window
registration changes, including real hidden HWND message delivery and disposal.
The changed Android Release build finished with 0 warnings / 0 errors; baseline
had one Gradle SDK XML-version warning and 0 errors.
UIAutomator queried 71 nodes on Components, then 67 on Color after native touch
navigation. Color contained Primary/Secondary roles and no stale Elevated action.
This verifies query freshness across page changes, not a full screen-reader audit.
Raw APKs/logs/captures are disposable local files under `temp/scroll-fix`; the
commands, hashes and measured aggregates above are the retained evidence.

## References

- [Android MotionEvent timing](https://developer.android.com/reference/android/view/MotionEvent#getEventTime())
- [DirectManipulation configuration](https://learn.microsoft.com/en-us/windows/win32/api/directmanipulation/ne-directmanipulation-directmanipulation_configuration)
- [DirectManipulation input flow](https://learn.microsoft.com/en-us/previous-versions/windows/desktop/directmanipulation/directmanipulation-processing-input-with-directmanipulation)
- [Flutter Windows DirectManipulation implementation](https://github.com/flutter/flutter/blob/master/engine/src/flutter/shell/platform/windows/direct_manipulation.cc)
