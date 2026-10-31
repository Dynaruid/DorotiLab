# Galaxy caret drag and magnifier follow-up

Date: 2026-10-31 (Asia/Seoul). Baseline source: clean
`d608a30076c07e3396d76226f219d7bdcda27ae2`.
This follows [the first caret-drag fix](galaxy-caret-drag.md); its measurements
and limitations remain historical evidence, rather than results for this build.

The Android magnifier now uses a rounded capsule. Selection-only changes avoid
rebuilding Material decoration and reprojecting unchanged semantic glyph geometry.
The diagnostic Input lifetime sample also stops rebuilding the whole page for
every selection update. Accessibility remains enabled and current selections are
still published. Sustained motion/display acceptance remains **PARTIAL**.

## Findings and changes

- `TextField` listened to the full `TextEditingController` for both InputDecorator
  and the outer semantics length wrapper. Neither needs a rebuild when only
  selection/composition changes. A lazy text-only listenable now notifies these
  consumers on actual text changes, including same-length replacements. The full
  controller still publishes selection/composition synchronously to EditableText,
  IME and other listeners. Focus and text changes still refresh decoration.
- Source glyph geometry was already retained by the first fix. Projection into
  view coordinates still recreated all glyph runs on each selection update.
  A weak cache now reuses the latest projection of an immutable geometry snapshot
  only when all 16 cumulative transform values and the clip match. DPR, transform,
  clip and source changes invalidate it; obscured text never publishes geometry.
  Entire semantic nodes are never cached, so selection/actions/flags stay current.
- The Input lifetime sample attached a page-wide `setState` listener to its first
  controller. Moving its caret rebuilt buttons, native-view wrappers and other
  fields. Only its selection/composition status label now rebuilds. The second
  field has no such listener even in the baseline; it is measured separately to
  avoid attributing this sample-specific saving to every application TextField.
- The Android bridge already retains the latest snapshot and lazily materializes
  virtual nodes when no accessibility consumer/focus exists. It creates no hidden
  native control per semantic node. This review does not disable that provider,
  delay actual selections, or change accessibility settings. Opt-in diagnostics
  now expose its actual apply/coalesce counters and work separately from common
  semantic generation and renderer projection.
- API 29+ native magnifiers now use 77.37 x 37.9 logical pixels, half-height corner
  radius, zoom 1.25 and a small elevation. At density 3 this is 232 x 114 pixels
  with radius 57. Preview position updates retain only the latest request for one
  `PostOnAnimation` callback; pointer samples and actual selection updates are
  never dropped. UP, cancellation, detach and disposal retire pending preview work.
  API 28 keeps its OS default; API 24-27 keep the existing framework fallback.

The first follow-up candidate (`97a5863c…`) failed a real drag: disposing its Java
Runnable while Choreographer already held a callback caused a late native
activation exception. It is **not accepted evidence**. The corrected session
removes queued work, keeps the callback peer alive for a harmless late invocation,
and checks its disposed/owner state before doing UI work. The JNI GC bridge owns
the eventual peer retirement.

## Reproduction and measurement limits

Same USB Galaxy SM-S931N, Android 17/API 37, arm64-v8a, 1080 x 2340, density 480,
Adreno 830; normal variable refresh policy (reported range 10-120 Hz).
Release Mono/profiled AOT, interpreter off, optimization on, official SkiaSharp
4.154.0. The starting installed APK's source identity was unknown; all comparisons
below use a fresh build of the clean baseline, rather than that initial APK.

| Package | SHA-256 |
| --- | --- |
| Clean-source baseline | `6397ef12cd1eaed2a619f3a3c5f99d3926ec689147dbb233ad967b1f13b4fc19` |
| Corrected candidate | `2d20c94ab6cd9300c9f6d809d19a55ce3fbf3b5c020d20a0bc3723b14fbe8cc1` |
| Official arm64 libSkiaSharp | `02c4cf50f41d50750d4e83f3e52c6e80dc7ac2ae43f50fefa381b45e18b81e9b` |

Each run starts a new process and types the same 80 UTF-16 characters. A shell-only
injector sends one DOWN, 287 timed MOVE samples at approximately 120 Hz, and one
UP over 2.4 seconds, reversing direction. Android may batch them into history;
all history remains on the selection path. No UI hierarchy query runs during the
main measured motion. The first-field path is `(348,625) -> (950,625) -> (150,695)
-> (850,625)` at 0/800/1600/2400 ms. The separate second-field path stays at y=1314.
These are injected contacts, not a human finger or physical display measurement.

Statistics use only the last gesture and the recorded DOWN-to-UP interval, with
floor-index percentiles. The eight or so trailing idle draws remain in raw rings
but do not dilute motion statistics. This differs from the earlier document's
whole-gesture/tail distribution. Both baseline and candidate use the same new
window rule. All source-baseline and candidate runs are retained, including
outliers. Profile/observer runs remain separate from ring-only comparisons.
First-field pre-run battery temperatures are 27.8/28.1/28.3 C for the baseline
and 27.7/28.0/28.2 C for the candidate; permanent services remain null and low
power remains 0. This small sequential comparison does not isolate every external
or thermal influence.

## Results

The companion [JSON record](galaxy-caret-accessibility-review-results.json)
preserves trial distributions, phase totals, package/source hashes and validation.

| Ring-only comparison | Trials per build | Baseline owner p95 ms | Candidate owner p95 ms | Baseline/candidate owner maximum ms |
| --- | ---: | ---: | ---: | ---: |
| First field, native views removed | 3 | 36.415-37.588 | 21.209-26.485 | 41.155 / 31.539 |
| Second field, no page selection listener | 3 | 25.709-27.082 | 22.386-24.989 | 31.130 / 35.183 |
| First field with native editor | 1 | 40.199 | 29.176 | 44.241 / 38.115 |

The first-field trial-median owner p95 falls 36.676 -> 26.131 ms (28.8%);
per-draw median allocation falls 1,672,256 -> 708,616 bytes (57.6%). Its build
phase median falls from about 7.9 to 1.4 ms. This large saving includes the
sample-specific page rebuild fix. Input age p95 is 14-15 -> 13 ms, with unchanged
text and latest selection `31:31` in every trial.

The independent second-field comparison shows the smaller common-path gain:
trial-median owner p95 27.063 -> 24.135 ms, callback median 4.505 -> 3.657 ms,
per-draw median allocation 740,536 -> 646,448 bytes (12.7%). Its candidate first
trial also contains a **worse** input dispatch maximum (41.160 vs the baseline
group maximum 23.763 ms), input age maximum (47 vs 32 ms), owner maximum
(35.183 vs 31.130 ms), and submit gap (82.731 vs 72.141 ms). These outliers are
retained. The other candidate second-field trials still have 61-64 ms maximum
submit gaps. Lower median/p95 costs do not qualify sustained smoothness or a
120 Hz budget; full input-to-new-scene/physical-display latency remains unresolved.

Separate allocation-profile runs show first-field build total 851.004 -> 202.850
ms and common semantics phase total 187.032 -> 135.799 ms over motion. The
baseline profile's build median/p95 are 8.177/26.812 ms, compared with semantics
1.072/13.138 ms. Build work dominates this sample; semantic generation also
contributes. Whole-session work aggregates include startup and instrumentation
differences and are not strict per-change performance isolation.

In the candidate profile with no consumer, Android receives 168 snapshots but
applies only one before export (166 coalesced). That one apply takes 3.520 ms;
the separately timed renderer projection/update path totals 25.147 ms across 168
whole-session calls. In the bounded UiAutomation consumer case, it receives 167
snapshots and applies 83, totaling 29.915 ms with a 14.851 ms maximum. This proves
that active consumption uses the real provider update path; it does not identify
the provider as the principal cause of the no-consumer stutter. Consumer-case
owner p95 is 36.987 -> 23.913 ms in one profiled trial per build. One observer
trial cannot establish TalkBack performance or the provider's isolated effect.

All nine accepted candidate motion trials retain the 80-character text, publish
valid collapsed selection and balance native magnifier creation/dismissal, with
zero fatal/render errors. Main runs coalesce only one preview request each, so
they do not establish a large latency saving from preview scheduling alone.

## Validation and remaining work

All builds/tests run with the repository's 1200-second timeout. CPU regressions
cover text-only notification gates, immediate selection/composition publication,
same-length edits, empty text, disposal and reentrant reads. They also verify
latest semantic selection with reused glyphs, selection-only deltas, transform,
clip, DPR, obscuring and concurrent projections. Earlier grapheme/layout cache
and scroll/inertia regressions remain part of the same suite.

The corrected Android Release build passes with zero warnings/errors. The actual
Galaxy screenshot shows magnified source text in the rounded capsule. Normal UP,
HOME during a held drag, resume and three short drags (26/67/125 ms measured by
the injector) leave no magnifier layer behind. The separate lifecycle run records
four creations/four dismissals, five coalesced requests and three cancelled pending
callbacks, with zero fatal/render errors. This covers the late-callback failure
that rejected the first follow-up candidate.

Windows App SDK Debug Testbed also builds with zero warnings/errors; no new
Windows physical input/runtime acceptance is claimed. Two ADB taps on the actual
Samsung keyboard insert composed `사` at offset 12 (selection `13:13`, composing `12:13`).
A subsequent handle drag retains the text and ends at `21:21`, composing cleared.
The installed APK is pulled back and matches the corrected candidate hash; its
arm64 native SkiaSharp library matches the hash above and startup confirms 4.154.0.
The final process runs normally on Components with no diagnostic/sample extras,
46 provider nodes, no fatal exception, permanent services `null` and low power `0`.
Both the during-drag capsule screenshot and final Components screenshot are
visually inspected. These bounded native checks do not qualify every IME sequence.

The residual owner/submit gaps require further timing work; none is explained away
by the lower median. Ring submission is not display FPS or scanout evidence.
Human finger-to-display acceptance and full active TalkBack action/focus behavior
remain `notVerified`. The original [scroll plan](../../../../work.md) stays PARTIAL.

```powershell
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet run --project Doroti/tests/Doroti.Scroll.Tests/Doroti.Scroll.Tests.csproj -c Release -m:1 -nr:false
python Doroti/eng/run-with-timeout.py --timeout 1200 dotnet build ./samples/DorotiTestbedApp/android/DorotiTestbedApp.Android.csproj -c Release -r android-arm64 -m:1 -nr:false -v:minimal
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/measure-android-caret.py --serial R3CY30KZA4B --label caret-review --without-native-views
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/measure-android-caret.py --serial R3CY30KZA4B --label caret-isolated --without-native-views --last-field
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/measure-android-caret.py --serial R3CY30KZA4B --label caret-observer --without-native-views --trials 1 --allocation --observe-accessibility
python Doroti/eng/run-with-timeout.py --timeout 1200 python Doroti/eng/summarize-android-caret.py temp/galaxy-caret/caret-review-1.json
```

Deploy the shell-only injector using the linked scroll implementation record.
The observer option starts a bounded 12-second UiAutomation event consumer,
discards global event payloads and never enables a permanent service. It is a
provider-update diagnostic, not a TalkBack qualification.

Android's [Magnifier.Builder documentation](https://developer.android.com/reference/android/widget/Magnifier.Builder)
specifies pixel units for size/corner radius and the near-rectangular OS defaults.
API 28 and earlier fallback appearance are not qualified by this API 37 run.
Raw APKs, rings, screenshots/XML and logs under `temp/galaxy-caret*` are disposable;
the companion JSON retains the durable aggregates and hashes.
