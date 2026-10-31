# Galaxy text cursor handle drag

2026-10-31 (Asia/Seoul). This follows the [SkiaSharp update](galaxy-scroll-implementation.md)
and addresses the user's clarification: stutter while **dragging the text cursor**.
Keyboard cursor movement is not used as a substitute for that gesture.
Status: **magnifier rendering fault and first-drag stall corrected in automated
Galaxy checks; sustained-drag performance remains PARTIAL**.

## Cause and implementation

Two problems are addressed:

- Android Material's framework magnifier emits a matrix backdrop filter. With
  a native editor on the same page, the native composition planner rejects this
  unsupported effect repeatedly. The baseline has 24–25 unsubmitted draws out
  of 30–31 during the gesture, and 1104–1150 renderer error log lines per trial.
  Those lines include repeated stack traces, not that many independent stalls.
- Without native views, the first drag frame still takes 133.9–167.0 ms;
  Graphite Snap accounts for 102.7–118.7 ms. Caret-only semantics updates also
  rebuild identical per-grapheme layout geometry.

The Android host now implements the existing `ITextMagnifierSession` contract
with the OS magnifier on API 28+. It anchors to the visible Doroti SurfaceView,
converts logical coordinates to that surface's pixel coordinates and samples
the caret's rendered line. It avoids the transparent IME proxy. Android owns
the popup and asynchronous source copy. Material uses the native session when
available, retaining its existing framework fallback on other backends.
No native composition capability guard is weakened.

The session dismisses and releases on drag-widget disposal, input deactivation,
host disposal and surface detachment. Updates queued for an old session are
ignored. Opt-in ring counters record magnifier create/update/dismiss counts;
there is no per-move diagnostic logging.

`TextPainter.layoutGeometryRevision` changes after real layout or changed
content width. A weak, one-entry-per-painter cache reuses immutable semantics
text geometry while layout and paint offset are unchanged. Edits, wrapping,
scale/alignment, scroll offsets, obscuring and disposal retain fresh/absent
geometry as appropriate. The cache does not retain painters after disposal/GC.

## Reproduction and evidence limits

Galaxy `SM-S931N`, USB `R3CY30KZA4B`, Android 17/API 37, 1080×2340/density 480,
SkiaSharp 4.154.0, Release/android-arm64, Mono profiled AOT, interpreter disabled.
The baseline is the previously installed stable APK:
`f689faaa38253b44c60aedbf336198c974abe3a90b209d61386b8115e39a0fd1`.
The new APK is `2217482289ffdff307d2a5722be015fb7c7d1f7e0656f6b2e1a5b9ba5f8c192c`.
Its official Android Skia native hash remains
`02c4cf50f41d50750d4e83f3e52c6e80dc7ac2ae43f50fefa381b45e18b81e9b`.

The existing `input` Testbed sample is used both with its native editor and with
native views removed. It has a three-line framework field and a live selection
status label. Each fresh-process trial inserts the same 80 ASCII characters,
taps the first line to expose the collapsed cursor handle, then injects one
continuous DOWN/MOVE/UP path at 120 Hz:
`(348,625,0) → (950,625,800) → (150,695,1600) → (850,625,2400)`.
This drags across a line, reverses and changes lines. There are three trials per
variant, with and without the native editor. Final text and selection are queried
after motion. No hierarchy query, build or screen recording runs during the
main gestures. First-drag costs are retained, not discarded as warmup.

The selection label itself rebuilds this diagnostic sample on every selection
change. Its costs are not generalized to every application TextField.
Owner/draw and callback metrics are CPU/submit evidence, not display FPS.
Reported display refresh rate is not a scanout receipt. Human finger-to-display
acceptance remains `notVerified`. Baseline battery/thermal state was not sampled
per trial; candidate state files preserve the later condition. These small samples
do not establish a statistically isolated effect for either change alone.

| Three fresh-process trials | Owner p95 range ms | Owner maximum range ms |
| --- | ---: | ---: |
| Baseline with native editor | 90.475–118.061 | 124.086–136.551 |
| Candidate with native editor | 36.940–41.985 | 41.257–46.114 |
| Baseline without native views | 16.039–18.133 | 133.852–166.990 |
| Candidate without native views | 33.816–38.183 | 38.655–40.051 |

The repeated render errors disappear in all six candidate trials. Every trial
ends with the original text intact and selection `31:31`. Each candidate creates
one native magnifier, updates it 72–155 times and dismisses it once. The separate
visual check confirms magnified source text, no magnifier layer after UP or
background/resume, and two balanced create/dismiss lifetimes across those checks.

The large first-drag tail is reduced, but the plain-field owner p95 **increases**.
The magnifier is now an OS popup rather than an animated framework overlay, so
draw counts/mixes differ (baseline 207–233 vs candidate 87–91). This is not proof
of smoother sustained motion. The measured input-age p95 also rises from 13 to
14 ms in the plain-field runs. Neither the per-draw distribution nor the lower
maximum qualifies the full drag response/120 Hz budget. Further work must reduce
framework callback cost and measure input-to-new-scene/display latency under a
matched motion window. No outlier or first-contact cost is discarded.

## Validation

All build/test commands use the repository's 1200-second timeout. Scroll/caret
regressions verify actual production text geometry with graphemes, zero-allocation
repeat capture, immutable prior snapshots, same-length text edits, wrapping,
text scaling, offset, RTL/center alignment, obscuring and disposal. Android Release
build succeeds with zero warnings/errors.
Windows Debug Testbed also builds with zero warnings/errors; this follow-up does
not claim a new Windows physical-input runtime acceptance.

In a separate real Samsung keyboard automation check, two keyboard touches
insert composed `사` at offset 12, with selection `13:13` and composing range
`12:13`. A subsequent cursor-handle drag retains that text and ends at selection
`21:21`, composing cleared. This bounded native IME check does not qualify all
Korean typing sequences or human typing latency.

The installed APK is pulled back and matches the candidate hash above. Normal
startup confirms native SkiaSharp 4.154.0, with no fatal exception. The final app
is restarted without diagnostic/sample extras on Components (46 provider nodes);
permanent accessibility services remain null and low power remains 0. Its final
screenshot is inspected. The earlier scroll plan remains PARTIAL independently.

Raw rings, APKs, XML, screenshots and logs in `temp/galaxy-caret` are disposable.
Trial aggregates, source and raw-file hashes are retained in the companion
[measurement record](galaxy-caret-drag-results.json).

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build ./samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -m:1 -nr:false -v:minimal
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/measure-android-caret.py --serial R3CY30KZA4B --label caret
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/measure-android-caret.py --serial R3CY30KZA4B --label caret-plain --without-native-views
```

Deploy the shell-only injector first using the commands in the linked scroll
implementation record. The app is not packaged with that injector.

Android's [Magnifier API](https://developer.android.com/reference/android/widget/Magnifier)
documents SurfaceView source sampling, view-relative coordinates and dismissal.
API 24–27 retain the previous fallback and are not qualified by this device run.
