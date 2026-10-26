# Web Material sample: retained rendering and cache invalidation

Date: 2026-09-07. Baseline: Doroti `207010a48eb7040bed4475ace53c5363d819de6f`, clean working tree.
User scenario: `doroti.ps1 run -App ./DorotiTestbedApp -Platform web -Configuration Release`, `/?dorotiTestbedMode=sample`.

## Flutter comparison

The installed Flutter SDK is `C:/Users/parti/flutter`, revision
`6b182d2c7585eba26d4edce0f97630effd256c33`, also recorded in the reference app's `.metadata`.
This task compared source contracts; it did **not** measure Flutter's debug FPS or establish Flutter/Doroti display-FPS equivalence.

| Contract | Flutter | Doroti finding and repair |
| --- | --- | --- |
| Repaint invalidation | `RenderObject.markNeedsPaint` propagates to a repaint boundary; only dirty boundaries record a new picture. | Framework dirty propagation already exists. The measured steady scroll usually rebuilds no widgets and paints only a few boundaries. No replacement dirty-tree mechanism was needed. |
| Layer reuse | `_addToSceneWithRetainedRendering` calls `addRetained` when the subtree is unchanged. Property changes set `markNeedsAddToScene`; composing clears the flag afterwards. | This framework contract already exists. The Web mapper still flattened retained commands and repeatedly rebuilt path/text recipes and instruction bytes. Reuse now continues through those CPU stages. |
| Draw-time state | CanvasKit's recording canvas creates a `SkPicture`; path/text drawing records the state at that draw. | Path, clip/shadow geometry, transform arrays and paragraph layout were partly live references. They now capture draw-time snapshots, including line layout and text paints/styles. Mutating, relaying out, or disposing the source paragraph does not rewrite a retained picture. A repaint creates new snapshots. |
| Font changes | Recorded drawing and font/layout resources have explicit lifetimes; new layout/recording replaces previous work. | Mapping checks the current font binding and fallback-font table. Changes invalidate text recipes. Each scene still validates live resource descriptors; cached text cannot conceal a missing font. |
| Serialized text indices | Flutter's local `SkPicture` is not Doroti's cross-worker wire format. | Doroti additionally invalidates cached instructions when canonical strings or resource descriptors change. This prevents old string indices or font references from leaking into another scene. Golden wire bytes, checksum and malformed-input rejection are preserved. |
| Equal transforms | Recorded transforms are values in the drawing stream. | Fresh equal matrices previously missed the cache because equality was object identity. Immutable `DisplayMatrix` now has value equality; bounded translation reuse avoids rebuilding common matrices. |
| Clip antialiasing | `clipPath` / `clipRRect` forward the caller's antialiasing choice. | Doroti dropped this argument. Both CanvasKit mapping and native Skia replay now preserve it. |
| Pointer scrolling | Flutter's `pointerScroll` can send start/update/end synchronously within one input event. | Doroti's accessibility deferral only saw the active-position set, so every wheel event looked idle by the next frame. A 100ms arrival-clock quiet interval now spans the wheel stream and expires after input stops. Flutter's notification order stays unchanged. |

Source anchors: installed Flutter `packages/flutter/lib/src/rendering/object.dart:3326`,
`layer.dart:696`, `widgets/scroll_position_with_single_context.dart`;
repository reference `engine/src/flutter/lib/web_ui/lib/src/engine/canvaskit/canvas.dart:228`,
`picture_recorder.dart`, and `picture.dart` under `reference/flutter-master`.
Public contract: [Layer.engineLayer](https://api.flutter.dev/flutter/rendering/Layer/engineLayer.html),
[Layer.markNeedsAddToScene](https://api.flutter.dev/flutter/rendering/Layer/markNeedsAddToScene.html).

## Changes and limits

- The default Web producer uses bounded path/paragraph/translation mapping and encoded-instruction caches. `dorotiEncodingCache=0` still provides the uncached encoder path.
- The encoder caches whole validated instructions, including envelopes. Shader/image/filter resource paths remain validated through their ordinary encoder path. Output buffers remain independently owned.
- Mapping has separate 512-entry limits and caps on variable geometry/text/style sizes. Instruction storage is limited to 8192 entries / 8 MiB of charged storage, with separately bounded canonical tables. Text is charged even when equal strings do not share storage. Disposal clears producer caches.
- UI diagnostics are coalesced at the end of the current task and include completed frame counters. This avoids reporting the last expensive startup frame only after the first scrolling frame.
- Worker ownership, image resource transfer, raster recovery, default renderer, live-resize 30fps policy and AOT settings were not replaced. The resize experiments from earlier work are not reclassified as successful.
- Flutter's retained GPU pictures and scene culling are broader than the CPU reuse added here. This patch does not implement a new retained-picture wire resource or claim complete engine parity. New lazy sections can still incur substantial managed layout/build cost.

## Measurement

Hardware Chromium, AMD Radeon 780M via ANGLE D3D11, hardware WebGL2, 1280×900 at DPR 1.
120 alternating wheel samples (60 down, 60 up), 12px each, in the first component column.
The browser driver paces `mouse.wheel`; submission counts divided by wall time are **not display FPS**.
Trace-on phase measurements use actual managed recording times and matching UI callbacks, not the causally clamped frame timestamps.

| UI phase, milliseconds | Baseline median / p95 | Final median / p95 |
| --- | --- | --- |
| Full UI callback | 69.1 / 115.8 | 23.8 / 61.9 |
| Mapping | 14.4 / 17.4 | 5.3 / 7.7 |
| Encoding | 23.1 / 30.0 | 11.8 / 17.2 |
| Paint | 0.3 / 2.7 | 0.3 / 2.9 |
| Raster replay to submit notification | 3.3 / 9.7 | 3.2 / 5.4 |

This identifies managed scene work, rather than GPU raster, as the dominant steady-scroll bottleneck.
The original simple experimental picture/encoding flags alone did not help this sample (`existing-cache`: UI median 77.4ms); they were not used as promotion evidence.
The final trace still contains a 456.6ms UI callback when a new lazy section is built (393ms layout/compositing, up to 446 layout work entries). Steady improvement does not establish a 60fps worst-case guarantee. These first-construction costs and Flutter-style retained GPU picture replay/culling remain outstanding performance work.

Final trace-on run: 134 complete UI callbacks within the 6.03-second input window, dispatch mean 30.74ms, 119 submits, zero failed scenes/runtime errors. Detailed phase aggregation can include the last callback that ends after the window; the full-callback percentile calculation only includes completed callbacks. Final trace-off run after ten-second warmup: 137 callbacks, dispatch mean 30.35ms, raster mean 3.08ms, 120 submits in 6.01 seconds, zero failed scenes/runtime errors. Trace-off counters do not provide per-frame percentiles. Resize diagnostics remain enabled in both runs.

Raw evidence: `Doroti/validation/web-playwright/artifacts/sample-perf/`:
`baseline.json`, `baseline-phases.json`, `baseline-off.json`, `existing-cache.json`,
`fix1-restarted.json`, `fix2.json`, `fix3.json`, `final.json`, `final-phases.json`, `final-off.json`, and their phase summaries.
Original counter-only averages can straddle the expensive last startup callback; use the bounded trace window for the comparison above.
The reusable `measure-material-sample.mjs` now warms up for ten seconds and reports bounded UI callback percentiles. Final trace/off measurements are recorded separately.

## Validation and retained failures

- Release Web build: zero warnings/errors.
- Scheduler, browser mapping/snapshot regressions, DisplayList golden/malformed/fuzz/cache regressions: PASS.
- Full FCR-7 Material/widget runtime contracts: PASS. This is native runtime coverage, not native-window visual acceptance.
- TypeScript `tsc --noEmit`: PASS.
- Initial browser suite: 9 PASS, 1 FAIL, 1 SKIP. The failed inventory test expected an automatic palette despite D33 and the reference both requiring **Extract colors**. The test now checks that scrolling does not extract, then clicks Extract colors and requires a palette. The original failure and trace remain in `sample-perf-regression`.
- Initial lifecycle skip was its explicit renderer-mode guard. The final run sets `DOROTI_WEB_RENDERER_MODE=worker-canvaskit-webgl` to execute it and bounded raster replacement/resource replay.
- Full browser run: 11 PASS / 2 FAIL. The inventory helper imposed an unreachable 750px cutoff on the visible bottom action (at y818); the lifecycle helper did not recognize fingerprinted Release worker filenames. Both test setup defects were corrected while preserving the palette and stall-recovery assertions. Targeted recheck: 2 PASS. All 13 selected behaviors are now covered by passing runs; the original full-run failures remain in `sample-perf-final`, and the recheck in `sample-perf-recheck`. Wheel dispatch p95 was 28.8ms, maximum 51.6ms for 60 samples. Raster ownership, buffer terminals and three replacements with resource replay passed.
- Earlier probe setup failures are distinct from measurements: the wide navigation does not expose a `tab` named Components; rebuilding fingerprinted assets under a running DevServer required restarting that owned server. Those failed starts did not contribute performance samples. An initial build's ambiguous `Path` name was corrected before validation.
- Cleanup of the generated `validation/web-playwright/__pycache__/analyze-resize-phases.cpython-313.pyc` was rejected by automatic tool approval policy. The untracked analysis cache remains; it is not part of the product change.

Every test command has a 20-minute outer timeout. Automated browser screenshots/input and GPU-submit notifications do not prove physical scan-out, trackpad feel, or Flutter-equivalent smoothness. Those remain user-observed acceptance.

## Reproduce

Start the app with the user's original command. Restart a running server after rebuilding its fingerprinted assets.
From `Doroti/validation/web-playwright`, run:

```powershell
node measure-material-sample.mjs measured
node measure-material-sample.mjs measured-off --no-trace
```

Use the normal sample URL for user testing; no new experiment query is required.

## Follow-up: first entry into a new scroll section and CanvasKit boundaries

The user clarified that the remaining hitch occurs when scrolling into a new
section. The earlier results above remain the record of the first change; they
are not a claim that first-construction latency was solved.

### Findings and repairs

- The installed Flutter CanvasKit `CkParagraphBuilder.build` creates a paragraph
  without laying it out; `layout` supplies the actual width separately
  (`engine/src/flutter/lib/web_ui/lib/src/engine/canvaskit/text.dart`, around
  lines 930 and 1155). Doroti performed an extra unconstrained measurement in
  `ParagraphBuilder.build`, then measured the same paragraph at its real width.
  Web paragraphs now defer that first measurement. A two-width, per-paragraph
  cache invalidates on font generation changes and bounds retained glyph tables.
- Every measured paragraph returned JSON containing all code-unit advances,
  lines and grapheme boxes. The UI Worker parsed it into managed objects.
  This boundary now returns a versioned numeric array with strict validation.
  Float64 geometry and the exact existing UI/Raster metrics hash are preserved;
  the cross-worker DisplayList protocol is unchanged.
- Retained pictures now reuse the whole mapped command body, including immutable
  paths and paragraphs, within 256 pictures / 16 MiB charged storage. Each hit
  validates live font dependencies. Font binding changes invalidate the blocks;
  images, shaders and filters use their ordinary resource-aware mapping path.
- A newly visible label changed canonical string indices and invalidated the
  entire instruction cache. Only text instructions are now invalidated when
  those tables change; independent geometry remains reusable. Golden bytes and
  malformed-resource validation still apply.
- The sample mounted whole component groups as one Sliver child. It now mounts
  individual sections while preserving continuous card styling. Its finite
  inventory has 29 sections; only visited sections are retained, preserving
  local controls on reverse scroll. This deliberately retains more widget state
  than disposing every offscreen section; it is not an unbounded list policy.
- The 680,944-byte embedded WebP took a long UI microtask before raster upload.
  `ImmutableBuffer` converted the typed byte view through per-element managed
  LINQ, and resource registration calculated SHA-256 twice in .NET WASM.
  Conversion now copies the exact byte slice in bulk. Async image/font
  registration computes SHA-256 once via browser Web Crypto, owns the input
  before awaiting, and reuses the digest for the descriptor and fingerprint.
  Synchronous font registration also computes its hash only once. Resource
  journals, acknowledgements, cancellation checks and replay remain in place.
- A long layout could outlast the 100ms wheel quiet window while the UI Worker
  was unable to receive the next wheel event, incorrectly flushing semantics
  during the stream. The frame now retains its entry-time scroll-active state.

Temporary layout and microtask probes were removed from product code after
isolating these costs. The measurement script retains a parser for historical
probe evidence and adds `--cold` and `--sweep` scenarios. Cold-start, short-scroll
and full-inventory sweep percentiles are separate workloads.

### Diagnostic sequence

All values below are UI callback CPU time, not display FPS. The full sweep uses
360 wheel samples of 100px across both columns, down then up in each, following
a ten-second warmup. It includes new section construction and the image demo.

| Full sweep stage | Median / p95 / maximum (ms) | Long image microtask (ms) |
| --- | --- | --- |
| Initial follow-up candidate (`refresh-sweep`) | 22.1 / 144.9 / 969.4 | isolated in next probe |
| Targeted encoder invalidation (`refresh-drain`) | 24.0 / 173.2 / 961.7 | 658.0 |
| Bulk byte conversion (`refresh-buffer`) | 21.6 / 84.1 / 701.5 | 404.4 |
| Async single hash (`refresh-crypto`) | 23.9 / 109.9 / 454.7 | none above 25ms probe threshold |

These are sequential diagnostic runs, not a claim of monotonic p95 improvement.
The remaining maximum includes large managed control construction/layout. The
raw traces preserve that residual hitch. Startup probes separately measured
root layout from 2175.9ms to 1375.4ms after the text boundary fix and 817.1ms after
section splitting; these include probe overhead and are not first-content
acceptance measurements.

### Follow-up validation record

- Numeric paragraph transport, invalid tables, exact hashes, width/font cache
  invalidation, retained mapping and resource lifetime regressions: PASS.
- Independent geometry reuse across new text tables, golden DisplayList and
  encoder regressions: PASS. An initial test constructor omitted `DisplayPaint`
  color and failed compilation; `refresh-encoder.log` is preserved, with the
  correction passing in `refresh-encoder-fixed.log`.
- Async `JSImport` initially rejected `Span<byte>` (`SYSLIB1072` / generated
  `CS0029`). It uses an owned `byte[]`; the original `refresh-crypto-build.log`
  failure remains separate from subsequent successful Release builds.
- Release Web build, scheduler, TypeScript, and full FCR-7 Material/widget
  contracts passed. The new native byte-view regression verifies both slice
  boundaries and ownership after the original typed list is mutated.
- First follow-up browser suite: **13 PASS / 3 FAIL**, preserved under
  `sample-refresh-final`. Exact async hash bytes, DisplayList v2, buffer/lease
  terminals, worker ownership and three recoveries, navigation, sheets/search,
  image extraction, local state after a full scroll, wheel continuity, bounded
  stalls and stationary wrapping passed. The three failures exposed kept-alive
  subtree participation in focus traversal and overlay hit testing; they are
  product failures, not relabeled test setup problems.
- The common Widgets layer now excludes cached Sliver subtrees from traversal,
  including nested traversal groups. OverlayPortal paint/hit/layout enumeration
  follows its layout anchor's kept-alive visibility. This avoids reading geometry
  of replacements not laid out in an offscreen bucket. Returning to the active
  Sliver walk restores eligibility. Native regression coverage checks hidden
  traversal and portal-anchor visibility, then restoration.
- Recheck after that fix: **4 PASS**, including all three failed behaviors and
  the full-scroll local-state regression (`sample-refresh-visibility`). Across
  these passing runs, all 16 selected browser behaviors are covered. The initial
  failures remain preserved. This is automated browser coverage; physical input
  and Flutter-equivalent smoothness remain unverified.

### Final code measurements and remaining work

Final Release build: zero warnings/errors (`refresh-visibility-build.log`).
Full native Material/widget contracts, including kept-alive focus/portal
regressions, passed (`refresh-visibility-native.log`). `git diff --check` passed.
Browser tests and performance measurements ran sequentially. Temporary probes
were absent from the final product build.

| Short scroll, same 120 × 12px workload | Original baseline | Previous fix | Final follow-up |
| --- | --- | --- | --- |
| UI callback median (ms) | 69.1 | 23.8 | 16.8 |
| UI callback p95 (ms) | 115.8 | 61.9 | 53.3 |
| UI callback maximum (ms) | 451.4 | 456.6 | 135.1 |
| Mapping median (ms) | 14.4 | 5.3 | 1.9 |

`refresh-verified`: 151 completed UI callbacks, 130 submits, dispatch mean
22.18ms, raster replay mean 2.82ms, zero failed scenes/runtime errors. This is
CPU/submit evidence, not measured display FPS. Phase aggregation contains 152
callbacks because it can include a callback ending beyond the stimulus window.

The broader final `refresh-verified-sweep` is less favorable than the intermediate
`refresh-crypto` diagnostic and must not be replaced by that earlier result:
193 completed UI callbacks, median **25.9ms**, p95 **128.0ms**, maximum **723.8ms**;
153 submits, zero failed scenes/runtime errors. The maximum's phase breakdown
is **668.5ms layout/compositing**, 824 rebuild work entries, 563 layout work
entries, 45.4ms scene construction/encoding, and **11.0ms raster replay**. Other
first-construction callbacks still took 548.9ms and 527.3ms. Hash microtask
blocking was removed, but large managed control construction/layout remains a
material first-entry bottleneck. The changes do **not** meet a smooth 60fps or
maximum-frame-time acceptance gate across the whole inventory.

Remaining performance scope is reducing or subdividing that managed first
construction and qualifying broader retained GPU picture/culling work. Do not
infer that replacing CanvasKit itself, changing the resize-only 30fps policy,
or enabling AOT without qualification fixes it. No Flutter debug performance
run was performed. Existing resize/AOT failure history remains unchanged.

Final raw evidence and phase summaries:
`artifacts/sample-perf/refresh-verified.json`, `refresh-verified-phases.json`,
`refresh-verified-sweep.json`, `refresh-verified-sweep-phases.json`, relative to
`Doroti/validation/web-playwright`. Run the wider scenario with
`node measure-material-sample.mjs measured-sweep --sweep`. All tests and
measurements use a 20-minute outer timeout.

## Follow-up: continuous animation on an already mounted screen

The user reported severe animation stutter on the normal sample URL after the
preceding scroll fixes. A separate `--progress` workload now starts the sample's
indeterminate circular and linear indicators, moves the pointer away, lets
construction settle for three seconds, and measures six seconds with no wheel
or resize input. This isolates repeating animation work from section mounting.
Hardware/browser/viewport remain Chromium / AMD 780M / 1280×900 DPR 1.

`animation-before`: UI median 20.1ms, p95 23.0ms, maximum 40.2ms, 150 Raster
submits in 6.02s. The UI performed two rebuilds, zero layout work and no paragraph
measurement per typical frame. Encoding alone took 9.4ms median; GPU replay to
submit was 3.3ms median. Raster terminal notifications waited 36.3ms median for
the busy UI Worker. These submits per wall time are not display FPS.

Additional opt-in encoder stage diagnostics (`animation-stages`) isolated
canonical tables at 1.8ms, per-command encoding/copy at 4.7ms, buffer assembly at
0.2ms and CRC at 3.1ms. The normal trace-disabled path does not sample these
stage clocks. This was repeated scene processing despite retained drawing,
rather than fresh text measurement during every animation tick.

Changes:

- A reference-identity index avoids structural hashing when a retained command
  instance is reused; value equality still supports fresh equal commands.
  Both indexes evict/reset together and retain the same canonical keys.
- Sixteen-command blocks copy previously validated byte sequences in bulk.
  Every command must compare equal; changes re-encode the affected block.
  Canonical table changes clear all blocks. Storage is independently bounded
  to 512 slots / 4 MiB charged storage, in addition to the existing instruction
  cache's 8 MiB. Images/shaders/filters remain in their normal validated path.
- Unchanged immutable paragraph recipes reuse their strictly validated UTF-8
  string table and IDs. Admission caps recipes at 512 / 2 MiB charged recipe
  storage and 2 MiB charged table storage. Changed text/run strings require
  rebuilding the table; fresh recipes with equal strings reuse it even when
  geometry or paint changes. Per-scene resource validation still runs. Reset drops
  all these retained references. These explicit memory budgets trade bounded
  retention for fewer repeated allocations and writes; lower WASM RAM is not
  claimed.
- Web encoding uses the same IEEE CRC32 polynomial and zeroed checksum field
  through a synchronous JS implementation. UI and Raster validation use that
  same table implementation instead of eight bit iterations per byte. The
  managed view is copied into JS-owned bytes before reading; no WASM buffer is
  retained or detached. Native/default encoding retains its managed CRC path.
  The cross-worker schema, checksum requirement and exact canonical bytes stay
  unchanged.
- The checksum JS import initially used unsupported `uint` (`SYSLIB1072` and
  generated compile errors, `animation-fast-build.log`). It now transports the
  exact bit pattern as supported `int`, then reinterprets it as `uint`.
  `animation-fast-build-fixed.log` records the successful correction.

Intermediate evidence is preserved: `animation-fast` (CRC + identity index)
reduced CRC median to 0.3ms, but command processing stayed at 4.6ms. The identity
index alone was not sufficient. `animation-blocks` reduced UI median to 13.8ms
and increased Raster submissions to 293 in 6.01s, but its p95 remained 17.4ms
and maximum 39.6ms. Final string-table reuse is evaluated separately below.

The first string-table key used paragraph identity too strictly. Repaint created
fresh recipes and repeatedly missed; `animation-final` measured tables at 2.7ms
and UI median 14.5ms / p95 18.0ms / maximum 39.2ms, 276 submits. Its trace-off run
had mean UI dispatch 14.17ms and 312 submits. These intermediate results remain
preserved, not substituted for the final key's measurement. The corrected key
compares exactly the string-bearing paragraph/run fields, including fallback
family, feature and variation names. It ignores geometry and colors, which
still participate in command encoding and validation. It does not replace the
bounded retained recipe keys with uncharged new objects on a hit.

Validation:

- Native DisplayList golden (6330 bytes, unchanged SHA-256), malformed/fuzz,
  fresh-equal string recipes, single-command block invalidation, cache growth,
  reset and charged-memory bounds: PASS (`animation-string-value-contract.log`).
- Final Release Web build: zero warnings/errors (`animation-string-value-build.log`).
- TypeScript: PASS (`animation-tsc.log`).
- Browser suite: **12 PASS** (`sample-animation-final`), including changed
  animation pixels and stop/idle behavior, responsive navigation, selection,
  visited state, image themes, CRC/golden bytes, buffer/lease ownership, worker
  recovery, input continuity and 100ms stall recovery.
- After the final string-key change: **4 PASS** (`sample-animation-string-recheck`)
  for animation pixels/stop, image themes, CRC bit/slice/input ownership and the
  full managed golden. All tests have a 20-minute outer timeout. Browser tests
  and performance runs are sequential.

Final animation evidence (`animation-verified`, trace on):

| Metric | Before | Final |
| --- | --- | --- |
| UI callback median / p95 / max (ms) | 20.1 / 23.0 / 40.2 | 12.4 / 15.6 / 35.0 |
| Encoding median (ms) | 9.4 | 3.0 |
| UI validation/copy median (ms) | 1.95 | 0.5 |
| Raster terminal-to-UI median (ms) | 36.3 | 11.5 |
| GPU-submit interval median / p95 (ms) | 42.0 / 47.0 | 16.6 / 22.2 |
| Submits over approximately 6 seconds | 150 | 346 |

Final encoder substage medians were tables 0.4ms, commands 1.9ms, final buffer
copy 0.2ms and CRC 0.3ms. All 359 phase-aggregated callbacks hit the string table;
the completed-callback percentile window contains 358 callbacks. The stage
summary can include a callback ending just after the window. The measured
GPU-submit interval maximum was still 40.7ms. There were zero failed scenes or
browser runtime errors.

`animation-verified-off`, with detailed stage tracing disabled: **360 submits
in 6.016 seconds**, mean UI dispatch **10.63ms**, mean raster replay **2.23ms**,
zero failed scenes/runtime errors. This run has no per-frame percentile data.
Do not compare its count to the trace-on baseline as a controlled FPS ratio.
GPU-submit notifications are not physical scan-out or a guarantee of steady
60fps in every animation. Large first-section construction from the preceding
investigation remains separately outstanding; it was excluded from this warm
continuous-animation workload. No Flutter performance run was performed.

Run `node measure-material-sample.mjs animation --progress` or append
`--no-trace` for counter-only evidence. Raw JSON, stage summaries and logs are
under `Doroti/validation/web-playwright/artifacts/sample-perf/`; build/managed
contract logs are under `Doroti/artifacts/sample-perf/`. The normal sample URL
uses the fixes by default. The owned development server was restarted with the
final Release build; an already open app must reload to load the new runtime.

## SkiaSharp direct Worker comparison

The user subsequently proposed `worker-direct-webgl`. Current source already
implements this backend: `BrowserSkiaCapabilities` calls `SkiaSceneRenderer`
inside the persistent .NET Worker, which also owns framework layout and paint.
`DorotiWebWorkerSurface` owns the Skia GPU context and persistent surface.
This avoids the CanvasKit DisplayList mapping/encoding/transfer path, but puts
framework and raster work on one Worker event loop. The large first-section
layout costs above therefore remain relevant. This comparison does not qualify
all sample interactions or change the default backend.

Two issues were fixed to make this comparison usable:

- The legacy Skia Worker bootstrap ignored `dorotiTestbedMode=sample`. Forward
  the mode both on initial startup and runtime replacement, and configure
  `DOROTI_TESTBED_MODE` before creating the .NET runtime. The first comparison
  failed to find the progress control because it had loaded diagnostics.
- With resize diagnostics enabled, every Worker message serialized the entire
  trace into DOM attributes/script text. Coalesce that publication using the
  existing 100ms timer. Direct diagnostic capture still reads the live trace;
  the timer is disabled when diagnostics are off. Before this change a
  screenshot timed out and a nominal 6s capture spanned 6.893s with notification
  backlog. Those attempts are not a clean comparative benchmark.

Same Release sample, 1280x900, headless Chromium hardware ANGLE/AMD Radeon 780M,
warm progress animation, detailed CanvasKit stage tracing disabled:

| Measurement | CanvasKit | SkiaSharp direct |
| --- | ---: | ---: |
| Observation window | 6.011s | 6.002s |
| GPU submission/commit count | 359 | 360 |
| Worker commit interval median | 16.8ms | 16.7ms |
| Worker commit interval p95 | 19.9ms | 18.5ms |
| Worker commit interval max | 33.9ms | 23.9ms |

CanvasKit mean UI dispatch was 9.46ms and raster replay 2.05ms. Direct managed
surface work was median 5.5ms/p95 6.4ms/max 13.8ms; this excludes framework UI
work and MUST NOT be compared directly to CanvasKit UI dispatch as a speedup.
Commit intervals use Worker timestamps, including `commitEpochMilliseconds`
from CanvasKit trace details. Both runs had zero browser runtime errors. One
run per backend establishes a useful comparison, not a repeatable performance
win, physical FPS, Flutter parity, or visual/font parity. The direct screenshot
was inspected and the progress scene was present. New-section scrolling was
not remeasured with direct in this turn.

Artifacts: `sample-perf/canvaskit-comparison-progress.json`,
`sample-perf/direct-comparison-progress-final.json`, and
`artifacts/direct-comparison-progress-final.png` under web-playwright.
The measurement script now supports direct diagnostics, saves raw evidence
before screenshot capture, and accepts `--no-screenshot`.

Validation: Release build 0 warnings/errors; TypeScript check PASS. Direct
progress pixel-change/stop test PASS. Initial restart test failed because its
tab selector assumed narrow navigation at a wide viewport; the sample heading
and runtime recovery had succeeded. Fixing the test viewport to 800x900 and
asserting the tab before and after replacement passed. Preserve the original
1 PASS/1 FAIL log `direct-sample-browser.log` and corrected 1 PASS log
`direct-sample-restart-recheck.log`. Tests/builds used a 20-minute outer timeout.

The owned server was restarted with this Release build. Compare interactively
at `http://127.0.0.1:5088/?dorotiTestbedMode=sample&dorotiRenderer=worker-direct-webgl`.
The default remains `worker-canvaskit-webgl`; switching the engine binding alone
has not been shown to resolve the user's remaining stutter.
