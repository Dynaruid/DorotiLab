# WASM section structure execution — 2026-09-08

Request: `work.md의 전체작업해줘`. Baseline HEAD: `0cc805a4`, initially clean.
The earlier document-only scope and references to uncommitted changes were stale.
The original latency gates and failures remain in `work.md`.

## Implementation and adoption

The normal gallery now retains the body/Scaffold across navigation animation ticks;
only rail/bar animation wrappers change. Section descriptors directly invoke one
typed section builder instead of recreating a group's factory list. Directionality
participates in the cached home identity. These changes apply to the default eager
path. Widget/Element diagnostic string conversion also no longer throws; the
Widget failure was observed when constructing a Snackbar Hero tag.

The optional `indexed` path adds shared Rendering `SectionExtentIndex` and
`RenderSectionList`, Widgets `SectionList` and `SectionFocusCoordinator`. Actual
visible RenderBoxes still perform ordinary layout. A Fenwick index seeks estimated
offscreen heights; each index retains two exact width/revision configurations,
bounded to 4096 items. The sample has 29 sections, zero speculative cache extent,
stable GlobalKey state, and three bounded partition indices. All visited sections
remain pinned until screen disposal. KeepAlive release notifications permit explicit
cached-child disposal without a full retained-child scan on every frame.

Section anchors survive width and column changes. The left column is the primary
anchor; if its section moves into the right partition it is restored there. Target
requests cancel old ballistic activity, then materialize the requested section.
Boundary Tab/Shift+Tab materializes the next section, preserves queued input, and
uses ordinary focus traversal inside sections. Pointer input cancels stale requests.
Font generation, exact width, text scale, direction and conservative ThemeData
revision invalidate height estimates. Visible dirty content always receives real
layout. The image demo has an explicit 240px image viewport.

The eager path remains default. Screen-reader exploration of unmounted descendants,
automatic item-set reconciliation, physical IME/overlay migration and color-only
metric reuse remain incomplete/unqualified. A target-request API alone does not
prove these contracts. This is a substantial implemented and tested candidate,
not completion of every P3 acceptance requirement.

P4 removes group factory allocations and uses typed bounded prefix/measurement
operations in the measured section path. No unproven global dispatch rewrite,
object pool or generic layout cache was added. P5 limits offscreen preparation to
zero and never yields within build/layout. Managed UI remains the dominant resize
cost, so a new raster Worker/AOT/SIMD/threads experiment is not adopted. Prior AOT
failures remain failures; native relinking is not managed AOT.

## Evidence ownership

- `.doroti/wasm-structure/before-head.txt`, `before.diff`, `dotnet-info.txt`,
  `build-properties.json`: source/runtime identity. net10.0/browser-wasm,
  WasmBuildNative=true, PublishTrimmed=true; RunAOTCompilation unset.
- Immutable roots `before-wwwroot`, `p1-p2-wwwroot`, `indexed-wwwroot`,
  `indexed-v2-wwwroot`, `indexed-v3-wwwroot`, `final-wwwroot` and adjacent SHA-256
  manifests. `freeze-build.py` captures 562 uncompressed evaluated endpoints,
  including `_content` references/fingerprinted aliases.
- `Doroti/validation/web-playwright/artifacts/state-resize/wasm-*.json`:
  independent per-run traces, allocation/collection counts and failure records.
  `summarize-wasm-structure.mjs` preserves all observed targets, superseded targets,
  boundary-inclusive gaps and per-run distributions before grouping medians.
- `artifacts/section-lifetime/`, `artifacts/section-startup/`,
  `artifacts/sample-perf/`: memory, conservative startup availability, progress.
- `Doroti/artifacts/direct-default/wasm-*.log`: 20-minute native/build checks.
  Playwright uses one worker, retry 0, 20-minute test/global limits. Performance
  runs do not overlap builds or other GPU workloads.

## Measured v3 checkpoint (before final focus/release additions)

Five independent baseline and five v3 runs per resize/selection workload. The
following numbers belong specifically to the frozen v3 source, not later edits.

| Workload / metric | Before | Indexed v3 | Result |
| --- | ---: | ---: | --- |
| Same-column resize callback p95, run median | 341ms | 162.6ms | -52.3%; strict FAIL |
| Breakpoint resize callback p95, run median | 1699.8ms | 350.7ms | -79.4%; strict FAIL |
| Allocated bytes / 8 same-column steps, median | 12,312,400 | 9,030,392 | -26.7% |
| Allocated bytes / 8 breakpoint steps, median | 34,066,616 | 17,970,448 | -47.2% |
| Selection input→causal exact commit, median | 77.7ms | 77.7ms | No gain; 50ms gate FAIL |
| 20-cycle managed heap estimate peak | 67,144,664 | 67,998,944 | +1.27%; not forced-GC live bytes |
| Last-five-cycle picture resources | 24 | 24 | Stable; filter surfaces 0 |

Same-column v3 boundary gaps were 129.4–153.3ms, tracking p95 147.3–182.9ms,
and final settling 45.5–64.3ms. Breakpoint gaps were 275–322ms and settling
406–653ms. Original resize limits therefore remain FAIL. Lower allocation does
not establish GC pause, process memory, WASM capacity or physical display latency;
those measurements are notMeasured/notVerified.

Minimal progress runs (three paired runs, diagnostics/trace exports disabled)
had median causal response 69.9ms before and 71.2ms v3 (+1.9%); idle restart
medians were both 35ms. These are exploratory three-run comparisons, not the
five-run adoption corpus. Detailed onset exports originally occurred before the
causal front and interfered with the measurement; the harness now waits for that
front first. The old instrumented samples are retained and excluded from minimal
response claims.

Five cold contexts at each width checked actual sample semantics, a committed
front and nonempty PNG content. Before availability ranges were 4980–6033ms at
390px and 5480–5512ms at 1280px; v3 ranges were 2922–2972ms and 2975–3511ms.
These include readiness polling/readback overhead and are conservative content
availability upper bounds, not exact first-content timestamps or scan-out proof.

## Final frozen-source measurements

`final-wwwroot` includes the focus coordinator, queued traversal, bounded keep-alive
release notifications and RTL cache identity. Chromium 151.0.7922.34, direct GPU
backend `doroti-owned-canvas-webgl2-skia-gpu`, 1280×900 CSS px, DPR1; independent
five-run comparisons use the same immutable baseline. UI source SHA-256 identities
are in `.doroti/wasm-structure/final-source.json` and served asset identities in
`final-wwwroot-manifest.json`. Later validation/document edits do not change these
served binaries.

| Metric | Before | Final indexed | Qualification |
| --- | ---: | ---: | --- |
| Same-column callback p95, run median | 341ms | 160.3ms | -53.0%; strict FAIL |
| Breakpoint callback p95, run median | 1699.8ms | 379.5ms | -77.7%; strict FAIL |
| Selection causal response, median | 77.7ms | 75.0ms | No >10% regression; strict FAIL |
| Selection causal response, p95/max of five | 86.0ms | 85.1ms | 50ms p95 gate FAIL |
| First progress causal response, median / p95 | 69.9 / 73.5ms | 68.9 / 75.2ms | p95 +2.3%; strict FAIL |
| Idle restart causal response, median / p95 | 34.6 / 36.6ms | 29.7 / 32.1ms | This bounded marker workload meets 50/100ms |
| Allocated bytes / 8 same-column steps, median | 12,312,400 | 9,024,168 | -26.7% |
| Allocated bytes / 8 breakpoint steps, median | 34,066,616 | 17,856,152 | -47.6% |
| Sampled heap estimate peak over 20 cycles | 67,144,664B | 67,912,016B | +1.14%; live/transient peak notMeasured |
| Last-five-cycle heap estimate mean | 66,768,571B | 62,688,803B | No monotonic growth in observed window |
| Last-five-cycle picture entries | 24 | 24 | Stable; filter surfaces 0, failed scenes 0 |

Final per-run callback p95 values are wide `[175.4,171.1,158.8,155.7,160.3]`
and breakpoint `[403.3,416.2,379.5,375.2,353.2]` ms. These are not pooled samples.
All five selection actions selected Week and reported no runtime errors. The
quantitative structural improvement is reproducible, while original latency and
the complete live-memory/resource acceptance gates remain unqualified.

Final same-column caught-up p95 values are 173.7/165.2/156.2/147.9/148.9ms,
boundary gaps 134.7/133.1/125.4/124.3/132.5ms and settling
54.1/51.3/58.3/41.1/51.9ms. Each run superseded one of eight targets and
eventually caught up with every target. Breakpoint runs superseded 4/5/4/4/4
targets, with caught-up p95 585.4/595.4/585.2/544.6/554.1ms and boundary gaps
315.6/330.1/312.2/298.4/284.5ms. Time-weighted width-error means remain
50.1–56.6px (same column) and 237.1–254.0px (breakpoint). Arrival at the final
target does not erase the intermediate latency or geometry error.

Five final cold contexts at each width gave content-availability upper-bound
p95 2981.9ms at 390px and 3499.4ms at 1280px, versus baseline 6033/5511.5ms.
These remain semantic-readiness plus PNG-readback upper bounds. The first-progress
and restart comparisons above now each have five independent runs on both sides;
the earlier three-run v3 checkpoint remains separate.

The **default eager** final-source comparison also has five runs each: callback
p95 medians 336.8ms (wide) and 1666.9ms (breakpoint), only 1.2% and 1.9% below
baseline. Allocations remain 12,308,544 / 34,054,368 bytes per eight steps. Thus
P1/P2 are retained as local ownership/build refactoring, without claiming a 30%
performance adoption result. The large reduction belongs to the optional indexed
candidate. The eager comparison does not qualify the remaining layout bottleneck.

## Automatic contracts and failure preservation

- Index oracle: 1000 randomized updates/lookups each at 29, 290 and 4096 items;
  exact-width/revision invalidation and two-configuration storage bound PASS.
- Mounted variable-height 29/290-item fixtures both materialize 9 initial and
  22 visited children through End/middle seek and width changes. Anchors 14/7 and
  145/7 survive; offscreen unpin and balanced disposal PASS.
- Rapid Tab/Shift+Tab crosses an initially unmounted section and retains queued
  input: PASS (`wasm-section-focus-contract.log`).
- Latest deep sample reparent test: 1040 intermediate frames; existing State
  identity, anchor mapping and no redundant section builds PASS.
- Latest GPU app-bar contract: 420 intermediate scroll frames under cache
  pressure, CPU/GPU raster parity and reuse PASS. Scheduler contract PASS.
- Frozen v3 Web functional suite: 13 PASS (controls, picker/text, theme/images,
  all sections, navigation reversal, DPR1/2 columns). Latest-source results below
  supersede only their corresponding source scope.

Preserved failures include an incomplete static copy missing `_content`, wrong
Node working directory, rejected SliverList State loss (section 12 at 800px),
KeepAlive initially placed below wrappers, diagnostic ToString exceptions,
Snackbar Hero construction, and anchor drift from an uncancelled ballistic
activity. Initial stale progress hit rectangles caused timeouts; setup now waits
for scrolling to settle and refreshes bounds. Local C# naming/shadowing and
scheduler-interface compile errors were corrected; stale `--no-build` passes
after failed compilation are not final evidence. Logs were retained, not counted
as zero-time observations or rewritten as PASS.

## Latest source after the shared reorder repair

The repair was followed by a new non-AOT Release build, immutable
`reorder-wwwroot` (562 endpoints), `reorder-source.json`, and another five runs
each. These are the latest resize/selection results; the preceding `final` and
v3 corpora retain their own source identity. Browser hardware inventory and driver
details are in `.doroti/wasm-structure/browser-gpu.json`.

| Metric | Before | Latest indexed | Result |
| --- | ---: | ---: | --- |
| Same-column callback p95, run median | 341ms | 159.1ms | -53.3%; strict FAIL |
| Breakpoint callback p95, run median | 1699.8ms | 351.0ms | -79.4%; strict FAIL |
| Same-column allocation / 8 steps, median | 12,312,400B | 9,028,376B | -26.7% |
| Breakpoint allocation / 8 steps, median | 34,066,616B | 21,927,320B | -35.6%; varies with executed callback count |
| Selection response, median / p95 | 77.7 / 86.0ms | 81.2 / 84.4ms | No >10% non-target regression; 50ms gate FAIL |

Latest callback p95 lists are wide `[164.2,161.6,151.2,159.1,154.9]` and
breakpoint `[368.7,436.1,343.8,341.0,351.0]` ms. Same-column caught-up p95
ranges 144.7–158.7ms, boundary gaps 119.3–127.2ms and settling 47.7–58.6ms.
Breakpoint caught-up p95 ranges 544.9–721.2ms, gaps 272.9–347.6ms and settling
435.5–674.0ms. All runs have runtime errors 0. The earlier lower settling or
allocation numbers are not substituted for these latest values. Memory, startup
and progress corpora above predate this final generic reorder repair and are
reported as checkpoints, not rerun evidence for the repaired binary.

Latest endpoint bytes including route aliases are 129,454,356 vs baseline
129,432,248. This is an asset inventory bound, not compressed network payload.
The real trimmed publish passed five Web tests (`wasm-publish-functional`):
column restoration, progress pixels/stop, navigation reversal, pickers/menus/text
and selection controls. Successful compilation alone was not counted as startup
or UI evidence.

## Platform and build matrix after the shared reorder repair

| Scope | Result / evidence |
| --- | --- |
| Section index/focus/dynamic contracts | PASS, `wasm-final-section-dynamic.log` |
| Resize and Linux Qt ABI/keyboard/clipboard/disposal contracts | PASS, `wasm-final-resize-contract.log`, `wasm-final-linux-contract.log` |
| Complete native Material/widget contracts | PASS, `wasm-final-native-all.log` |
| Playwright TypeScript | PASS, `wasm-final-typescript.log` |
| Windows App SDK Release | PASS, `wasm-final-windows-build.log` |
| Linux managed Release linux-x64 | PASS, `wasm-final-linux-build.log` |
| MAUI Windows Release | PASS, `wasm-final-maui-build.log` |
| Android Release android-arm64 | PASS, `wasm-final-android-build.log`; adb has no device, runtime notVerified |
| Web Release trimmed publish, AOT=false | PASS, `wasm-final-publish.log`; isolated `.doroti/wasm-structure/final-publish` |
| WSLg Qt xcb + explicit Mesa D3D12 | PASS bounded run: 60 resize cycles, 156 rasterized / 129 presented / 27 replayed, failed=0, 102 semantics nodes, no software fallback |
| WSLg Qt Wayland + explicit Mesa D3D12 | FAIL exit 255: `xdg_surface has never been configured`; reproduced using prior publish, preserved in `wasm-linux-wayland-prior-publish.log` |
| Apple hosts | notVerified; macOS/Xcode/device execution environment unavailable |
| Physical Windows/Linux/mobile input, IME/accessibility, user perception | notVerified |

Qt execution used current managed output with the existing native library from
`/home/parti/doroti-boot-20260907/publish`; its C++/header/CMake source hashes match
the current unchanged native sources. WSLg is not a physical native-Linux desktop.
The explicit D3D12 renderer was AMD Radeon 780M, Mesa 26.0.3; Wayland failure is
retained and xcb success does not replace it. The xcb input counter is not proof
of deliberate touch/IME/keyboard interaction. Published Web runtime checks are
reported separately from successful compilation.

## Remaining acceptance

The additional dynamic fixture initially failed despite preserving State and
disposing the removed item: its new index retained total extent 10,000 but the
scroll position received max extent 0 and lost its 7px anchor. The shared
`RenderSliverMultiBoxAdaptor.move` returned early when a child's physical
predecessor did not change, omitting `didAdoptChild` for its changed slot. The
fix updates the manager-owned index even for an unchanged predecessor.
`wasm-dynamic-reorder-adoption-fixed.log` passes the entire section suite,
including surviving State/anchor, removed-child disposal and lazy new-item
materialization. Initial dynamic failures and diagnostics are preserved.
The first `wasm-final-section-dynamic` label was subsequently rerun; its original
failure is retained as `wasm-final-section-dynamic.initial-transcript.txt`, clearly
marked as recovered tool stdout. The three separately named diagnostic failure
logs remain original files. The successful rerun does not reclassify those failures.
Automatic item-set policy is still owned by the caller; the sample's inventory
is fixed. This generic reorder repair was added after the first frozen performance
corpus; its new build/function checks and five-run corpus are reported separately
in the latest-source section above.

Latest direct source functional run (`wasm-final-functional`) passed 17 tests,
including all four DPRs (1/1.25/1.5/2), two starting widths and ten width steps.
Its original column-return assertion failed because it scrolled downward from
the correctly restored end of the list. The PNG showed the final image section.
The corrected assertion scrolls upward and polls rendered content; separate
`wasm-final-column-return` runs passed at DPR1 and DPR2. This preserves the original
17 PASS / 1 FAIL report instead of overwriting it with the two subsequent passes.

CanvasKit remains a separate comparison corpus. `wasm-final-canvaskit` recorded
2 FAIL (column-return scroll response, progress activation) and 1 PASS
(navigation reversal). The same column-return assertion also failed on the
immutable baseline in `wasm-before-canvaskit`, which also failed progress and passed
navigation; this is not evidence of a new indexed regression. Follow-up checks use
the center of the right content instead of the card's outside margin, and refresh
the progress hit rectangle after it settles. The original failures remain in
their reports. `wasm-final-eager` passed selection/pickers/navigation but failed
the stale progress click; `wasm-final-eager-motion-fixed` passed both motion tests
after the geometry-aware setup correction. No latency limit was relaxed.
`wasm-final-canvaskit-target-fixed` subsequently passed all three comparison tests
(right-column content scrolling, actual progress pixel animation/stop, navigation
reversal). This qualifies that bounded automatic functional scope, not CanvasKit
performance or scrolling over outside-card margins. Direct and CanvasKit results
remain separate.

The strict sustained callback, input, resize and first-new-section latency goals
are not qualified. Large individual sections and visited-state reparenting remain
synchronous costs. Tenfold fixture counts prove bounded visible materialization,
not tenfold production WASM latency scaling. Physical window dragging, Korean
IME, assistive technology, touch/lifecycle, full DPI/GPU/OS coverage and user
perception remain notVerified. No user-observed acceptance has been collected in
this execution. The plan stays open for these remaining requirements.

| 구현 | 자동 기능 | 정량 성능 | 물리 표시/입력 | 사용자 체감 |
| --- | --- | --- | --- | --- |
| P1/P2 반영, 공용 indexed 후보 및 sliver 재정렬 수정. P3 전체 수용/기본 승격 보류 | native contracts, Web direct/CanvasKit의 명시된 범위, trimmed publish 5개 통과. 초기 실패 별도 보존 | 최신 indexed callback 53.3%/79.4% 개선. eager 약 1–2%. strict latency FAIL; live/transient memory 등 미측정 | notVerified. WSLg xcb 제한 실행 PASS, Wayland protocol FAIL | notVerified; 이번 실행 후 사용자 관찰 없음 |
