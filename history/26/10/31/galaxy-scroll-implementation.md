# Galaxy scroll implementation and SkiaSharp update

2026-10-31 (Asia/Seoul). Source baseline: `bc486409`.
Status: **SkiaSharp 4.154.0 stable adopted; scroll performance remains PARTIAL**.

The [work plan](../../../../work.md) remains the acceptance contract.
The [retained measurements](galaxy-scroll-results.json) include source hashes,
APK hashes, trial aggregates and the Perfetto attribution below. Raw APKs,
traces, rings and screenshots are disposable local files in `temp/galaxy-scroll`.

## Implemented changes

- Common engine: `Matrix4.multiply` computes into stack memory, including when
  both operands are the same matrix. Semantics projection removes redundant
  matrix copies and per-rectangle corner arrays. Identical/absent text geometry
  retains the original snapshot; obscured text still removes its geometry.
- Common scroll: restore the missing available scroll actions at both ends and
  in the middle, for all four axis directions. Reuse the last action set while
  the available directions remain unchanged.
- Android: map Choreographer timestamps using their age in `System.nanoTime`'s
  domain. The previous mapping anchored a delayed first callback to its arrival,
  shifting subsequent animation timestamps. Existing monotonic dispatch guards
  and callback/invalidation ownership remain in place.
- Boundary diagnostics: opt-in preallocated rings connect native pulse/input,
  framework trace, scene/submission IDs, allocation/collection counts and Vulkan
  stages. No per-frame log/file/JSON export is needed. Export only after motion.
  Graphite snap/insert/submit timings are available separately.
- Continuous input: a shell-only Java driver injects one DOWN, 120 Hz MOVE
  samples with one downTime, two direction reversals and one final UP. It is
  never installed or packaged with the application. This is injected touch,
  not human finger acceptance.

No friction, simulation duration or stopping tolerance was changed. The current
two-frame Vulkan limit, native insertion/readback synchronization and retirement
contracts were reviewed and retained. No worker ownership migration or extra
in-flight buffering was introduced.

## Device and comparison

USB `R3CY30KZA4B`, Galaxy S25 `SM-S931N`, Android 17/API 37, 1080×2340,
density 480, Adreno 830, reported variable-refresh range 10–120 Hz. Reported
120 Hz is not measured display FPS. Low power was 0; enabled accessibility
services were null. Charging stayed enabled. Baseline trials were 62–63% battery,
26.7–27.0°C; first candidate trials were 67%, 27.9–28.5°C. No outlier is discarded
because of that temperature difference.

Original installed APK was backed up before replacement. It contains Mono and
AOT libraries, but its exact source revision was not recoverable. A reproducible
source baseline and an instrumented baseline were built separately. All main
comparisons use Release/android-arm64, Mono, profiled AOT, interpreter disabled,
optimization enabled. These runtime settings were not changed for the comparison.

| APK | SHA-256 |
| --- | --- |
| Original installed | `3342af069f581e1b8dcdeb91640c2caac831627426b462a988f368c06d27a20d` |
| Source baseline | `383a83bde2f8b25a05437efe7414314817af6add7ef56fbbd278ed03433448f1` |
| Instrumented baseline | `cd2a0871f45c8d99eba88f992ea1e3e4dd42c15526b2783d2309514b0925a0b9` |
| Engine/clock candidate, old Skia preview | `2c1d76330a70114b155eb0dfe3c23fd4376c2ee54f4fcfa4b4bcc65c36df6193` |

The old-Skia candidate is an intermediate result, not the final upgraded APK.

Each main trial starts a fresh process, waits 12 seconds, restores a middle
position outside the main window and performs 350/120/650 ms swipes at x=950.
The fast reverse gesture reaches the top boundary; its rest/overscroll interval
must not be classified as unexplained ballistic starvation. Baseline/candidate
have three trials each, nine measured gestures each. No build, screen recording
or UI hierarchy traversal runs during measured motion.

| Variant/trial | Draws | Owner p95 ms | Owner p99 ms | Owner max ms | Allocation p50 bytes/draw | Gen0 collections |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Baseline 1 | 291 | 10.999 | 19.483 | 42.566 | 591424 | 44 |
| Baseline 2 | 282 | 10.515 | 29.503 | 50.967 | 602688 | 44 |
| Baseline 3 | 287 | 11.819 | 21.691 | 71.715 | 596840 | 43 |
| First candidate 1 | 289 | 9.264 | 20.703 | 40.961 | 539104 | 40 |
| First candidate 2 | 278 | 10.551 | 22.769 | 57.405 | 535600 | 39 |
| First candidate 3 | 287 | 9.659 | 19.475 | 55.298 | 537336 | 39 |

Median trial p95 changes 10.999→9.659 ms. Median trial allocation p50 changes
596840→537336 bytes/draw. This is a combined engine/clock result. It does not
attribute CPU improvements to the clock change alone. Neither variant meets
the 120 Hz owner budget of approximately 6.67 ms at p95.

Cold startup owner maxima remain 633–661 ms. First setup contact maxima remain
53–62 ms. They are separated from warm main windows, not omitted from acceptance.
All main trials retain three repeated final scene submissions and zero backward
submitted scene IDs. Final replays are not proof of a visible position reversal.

### Continuous drag and allocation diagnosis

The continuous path is `(950,1800)→(950,700)→(950,1750)→(950,900)→(950,700)`
over 2650 ms. The injector generated 317 MOVE samples and one DOWN/UP pair.
The Android rings received 305 baseline / 303 candidate MOVE events plus native
history; batching means event count is not hardware sample count.

In separate allocation-profile runs, mean managed bytes/draw decreased
543176→487261. `PipelineOwner` semantics work changed from 122815576 bytes over
450 calls to 95007040 over 445 calls, approximately 272923→213499 bytes/call.
This isolates the changed common semantics path more narrowly than total owner
allocation. Nested layout/allocation instrumentation is diagnostic overhead and
is not mixed into the main trial table. Continuous owner maxima are 42.551 and
58.070 ms, so the continuous test also remains below acceptance.

The 30-action starting budget was expanded for the user's continuous-drag request,
pipeline attribution after a remaining failure, and the later SkiaSharp upgrade.
The first Perfetto configuration could not be opened under Android SELinux from
`/data/local/tmp`; the driver now passes the configuration on stdin. That failed
capture is not used as Perfetto evidence.

### Remaining stall attribution

A successful separate continuous Perfetto capture records a 50.922 ms owner
frame: callback 1.409 ms, Graphite Snap 46.281 ms. On the same UI thread,
`vkCreateGraphicsPipelines` consumes 44.414 ms. Scheduling accounts for 50.646 ms
Running and 0.275 ms Runnable in the owner interval. This identifies actual CPU
pipeline creation on the UI thread for this captured stall, rather than assuming
a GPU fence wait. Another captured frame contains a 17.732 ms pipeline creation.
Other long frames include layout work and ART GC; they are not all the same cause.

The trace has **zero actual FrameTimeline rows for the app**. SurfaceFlinger
`--latency` does expose 127 samples for the exact Doroti SurfaceView BLAST layer,
but these are a rolling tail and are not joined to every scene or expected
deadline. Thus `presentObserved=false` for the ring and full deadline/display
qualification remains open. An idle gap in that rolling layer history is not
counted as active-scroll jank.

The installed preview's public Graphite API exposes no precompile context or
pipeline-cache option. Upstream Recorder Snap prepares resources; a package
upgrade is a justified candidate, not evidence that compilation moved off-thread.

## Regression and runtime checks

- The required Scroll console passes with the 1200-second timeout. It exercises
  production scheduler/ticker/controller at 60/120 Hz and with missed pulses,
  fractional offsets, ticker retirement, unchanged dimensions and recontact.
- The new boundary regression failed against the original empty action set and
  passes with the fix. Projection tests cover self-aliasing, zero-allocation
  composition, perspective/zero-w, DPR, legacy origins, text reveal and obscuring.
- Before the Skia upgrade, the normal Windows command launched the Debug Testbed.
  Computer Use verified OS wheel movement, Platform views, framework text input
  `scroll-check`, Components return and normal close. The report has one mouse
  wheel packet, no pan/zoom packets, no failed terminals and `pending=0` on drain.
  This does not requalify physical trackpad motion.
- Android provider queries showed 45 nodes on the scrolled Components page and
  32 on Color; Primary was present and the old Elevated action was absent on Color.
  The Platform views page exposes actual native controls. Framework text input
  accepted `scroll-check` through its native IME bridge. A later background/resume
  retained PID 28742, returned a rendered Components page and 46 app nodes.
  These are bounded automation/query checks, not Korean composition or TalkBack
  focus/action acceptance. UiAutomation service participation is temporary;
  permanent enabled accessibility services remain null.

## SkiaSharp update

The user subsequently requested adoption of the latest release. Official NuGet
and release metadata identify `4.154.0` as the latest stable release (2026-10-28).
`4.156.0-preview.1.26512.10` is a separate prerelease (2026-10-15).

The update keeps all platform packages on one official stable version. Central
pins, runner defaults, Android native overrides, target manifests and managed/
native provenance hashes are updated together. Desktop and both Android ABI
hashes come from the resolved official archives; no hash guard is removed.
Retained license and native notices are refreshed from the official Win32 archive.

Release arm64 build and Scroll regressions pass. The Android build finishes with
zero warnings/errors. Windows launches through the normal command, renders the
Components page, accepts an OS wheel, renders native interleaved views, accepts
`stable-skia` through the native text bridge and closes normally. Failed terminals
are zero and shutdown drains with `pending=0`.

The deployed Windows managed/native SHA-256 values exactly match the official
stable archive entries: `c3719ba116d106b97d65665cc83cf9cfa1cbb8e58fbda0580d26a69340c8a82e`
and `82f9b2ee2b2dc91f738ae4baa54cd21fdd1181883c0fa207ab7066c708f07108`.
Android arm64 native SHA-256 is
`02c4cf50f41d50750d4e83f3e52c6e80dc7ac2ae43f50fefa381b45e18b81e9b`.
The final APK, also pulled back from the Galaxy, is
`f689faaa38253b44c60aedbf336198c974abe3a90b209d61386b8115e39a0fd1`.

| Stable 4.154.0 / trial | Draws | Owner p95 ms | Owner p99 ms | Owner max ms | Snap max ms |
| --- | ---: | ---: | ---: | ---: | ---: |
| 1 | 282 | 10.715 | 20.942 | 40.716 | 38.342 |
| 2 | 287 | 9.264 | 25.595 | 55.539 | 50.823 |
| 3 | 277 | 11.307 | 23.797 | 56.821 | 49.508 |

The latest stable package does **not** establish a performance fix. Its median
trial p95 is 10.715 ms versus 9.659 ms for the first candidate on the preview;
the small three-trial samples do not establish a statistically reliable regression
or improvement from the version alone. Remaining >50 ms frames still fail the
plan's acceptance criterion. Stable trials were 75–76% battery, 27.5–28.1°C, with
the same charging, refresh policy and accessibility configuration.

All active platform manifest version fields now say `4.154.0`. The remaining
preview version in `VulkanObserver.Synchronization.cs` is the historical source
reference for the depth/stencil barrier correction, not a package selector.
The stable public Graphite API still has no exposed precompile context/option.
Apple, Linux and Web runtime acceptance remains unverified on this Windows host.

### Stable continuous capture and final Android smoke

The stable package was also tested with one continuous 2650 ms reversing drag.
The injector generated 317 MOVE events; Android received 301 MOVE events plus
one DOWN/UP pair and native history. This separate Perfetto/ring run is not pooled
with the three main trials. It captures two distinct remaining costs:

- Owner 65.115 ms, callback 60.584 ms. Framework recording-clock events place
  56.437 ms inside layout; one managed Gen0 collection occurred between the
  adjacent draw samples. Scheduling reports 60.409 ms Running and 4.476 ms Runnable
  over the owner interval. The collection count does not isolate GC's share of
  layout time, so this is not attributed entirely to GC.
- Owner 45.307 ms, Snap 40.626 ms, including `vkCreateGraphicsPipelines` 38.910 ms
  on the same UI thread. It spends 44.923 ms Running and 0.301 ms Runnable.

The stable trace has two actual FrameTimeline rows for transaction/window layers
(`TX - Bounds` and `TX - VRI`), both on time. It has no actual FrameTimeline rows
for the Doroti SurfaceView drawing layer. Two transaction rows do not qualify
full scroll presentation or the deadline target.

With the final stable APK and a fresh process without diagnostic extras, the
Android smoke exposes 46 Components nodes and 32 Color nodes. Primary is present
and the old Elevated action is absent on Color. Platform views exposes the native
editor; the framework text field accepts `stable-android` through the native IME
bridge. Background/resume retains the same PID and exposes 43 app nodes.
No fatal exception is logged. The app is finally restarted normally on Components;
the screenshot is inspected, and the pulled installed APK matches the final hash
above. Startup also confirms the loaded official Android native version `4.154.0`
and its exact arm64 hash. Permanent accessibility services remain null and low
power remains 0. Physical input, Korean composition and active TalkBack remain
outside these automation checks.

## Remaining acceptance work

- Remove or move the confirmed expensive graphics-pipeline preparation off the
  critical input/display path, with explicit recorder/device ownership and native
  view/texture lifetime tests. Merely increasing queue depth or shortening the
  simulation does not address the captured cause.
- Retain first-contact and cold resource preparation costs in the result. Neither
  the main warm trial table nor the version upgrade closes those gates.
- Join full layer/buffer timing to scene IDs and expected deadlines, or keep the
  display/1%-miss criterion unqualified. Current 127-sample layer tails cannot do so.
- Physical finger/trackpad, Korean composition and active TalkBack action/focus
  acceptance remain `notVerified`. Successful provider queries are narrower evidence.

The implementation, regression checks and package upgrade are recorded here;
the overall smooth-scroll objective is not marked complete while these failures remain.

## Reproduction

From repository-root PowerShell, all tests/builds use the required 20-minute limit:

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build ./samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -m:1 -nr:false -v:minimal
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/measure-android-scroll.py --serial R3CY30KZA4B --label comparison
```

Build and deploy the shell-only continuous injector once (JDK 21 and Android SDK 37):

```powershell
$sdkPath = Join-Path $env:LOCALAPPDATA 'Android/Sdk'
New-Item -ItemType Directory -Force temp/galaxy-scroll/gesture-classes,temp/galaxy-scroll/gesture-dex | Out-Null
javac --release 8 -cp "$sdkPath/platforms/android-37.0/android.jar" -d temp/galaxy-scroll/gesture-classes Doroti/eng/android/ScrollGesture.java
& "$sdkPath/build-tools/37.0.0/d8.bat" --min-api 34 --output temp/galaxy-scroll/gesture-dex temp/galaxy-scroll/gesture-classes/ScrollGesture.class
jar cf temp/galaxy-scroll/doroti-scroll.jar -C temp/galaxy-scroll/gesture-dex classes.dex
& "$sdkPath/platform-tools/adb.exe" -s R3CY30KZA4B push temp/galaxy-scroll/doroti-scroll.jar /data/local/tmp/doroti-scroll.jar
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/measure-android-scroll.py --serial R3CY30KZA4B --label continuous --trials 1 --continuous
```

Use `--profile allocation` for a separate allocation investigation or `--perfetto`
for the separate scheduling capture. `--profile log` measures the existing verbose
logger path; `--profile off` disables these diagnostic flags. Logging and ring runs
must not be pooled as if their instrumentation were identical.

`summarize-android-scroll.py` summarizes exported rings. Raw paths in this record
are disposable, not promises that the files will remain after artifact cleanup.

## References

- [SkiaSharp 4.154.0 release](https://github.com/mono/SkiaSharp/releases/tag/v4.154.0)
- [NuGet version index](https://api.nuget.org/v3-flatcontainer/skiasharp/index.json)
- [Choreographer FrameCallback](https://developer.android.com/reference/android/view/Choreographer.FrameCallback)
- [Perfetto FrameTimeline and SurfaceView limitations](https://perfetto.dev/docs/data-sources/frametimeline)
- [Upstream Graphite Recorder Snap](https://github.com/google/skia/blob/main/src/gpu/graphite/Recorder.cpp)
- [Flutter semantic scroll actions](https://github.com/flutter/flutter/blob/master/packages/flutter/lib/src/widgets/scroll_position.dart)
