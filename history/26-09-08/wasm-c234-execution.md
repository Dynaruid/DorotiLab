# C2 / C3 / C4 implementation and bounded comparison — 2026-09-08

User request: `C2, C3, C4적용 바로 해보자`.
**Final decision: implemented and tested, not adopted; product source reverted to
the existing HAMT + indexed baseline under work.md 9.7's no-improvement rule.**
The [candidate patch](wasm-c234-candidate.patch), regression fixture and
[machine-readable comparison](wasm-c234-summary.json) remain reviewable.
Start HEAD: `a206a56e8794b691961bb2e65151bcd5aa85233b`. The only pre-existing
untracked file was `work3.md`; it is left untouched. Both comparison builds use
HAMT + indexed and the default worker-direct-webgl renderer.

## Implementation

- C2 (`Rendering/box.cs`): four shared stateless intrinsic adapters; existing
  DartMap TryGetValue/compute/assignment in intrinsic, dry and baseline caches.
  Remove nested closure allocation while preserving compute override dispatch,
  constraint/key equality, null baseline hits, invalidation, exception behavior
  and outer-last storage after reentrant computation. Request method delegates
  remain; no per-RenderBox delegate cache or pooling is added.
- C3 (`Widgets/sliver.cs`): statically typed RenderObject parent-data access and
  RenderSliverMultiBoxAdaptor calls in SliverMultiBoxAdaptorElement. Keep public
  signatures and virtual getters/insert/move/remove/indexOf; do not capture a
  receiver across callbacks or alter general Dart conversion semantics.
- C4 (`Widgets/framework.cs`, `Rendering/object.cs`, `Widgets/sliver.cs`): shared
  comparison delegates retain the original long-comparison sign, same-depth
  behavior and List.Sort algorithm. Copy the remaining dirty tail into the
  distinct swapped queue without Skip/ToList. Snapshot sliver keys in an array;
  never replace the snapshot with mutation-sensitive live enumeration.

These files retain their reviewed Framework source markers. They are product C#
sources, not compiler-owned generated-base/effective workspace artifacts. The
optional Dart importer and global dispatch lowering are unchanged. Future Flutter
source refreshes must review and preserve these adaptations explicitly.

## Functional evidence

The new `--execution-cost OUTPUT` contract ran before and after product changes:

- Four intrinsic kinds, NaN/infinity/signed zero, equal constraints, cached null
  baseline and separate baseline kinds.
- Dirty/equal-constraint invalidation, compute exception followed by retry, and
  same-key reentrant computation with the outer result stored last.
- Overridden sliver getter and insert/move/remove invoked in the same order;
  three operations still perform exactly three virtual receiver reads.
- A layout callback adds a shallower dirty node while a deeper tail remains.
  Both implementations process `first, added, tail`, once each, and release both
  processed queue lists.
- HAMT correctness and existing five-tick subtree trace: 20 nodes, 155 events
  strictly equal A/B, pinned Flutter observable callbacks equal. Full Flutter
  internal target/semantics trace remains notVerified.

CLR allocation: for each of four intrinsic kinds, eight pre-populated cache hits
allocate **2,272 → 1,312 bytes (-42.3%)**. This is a bounded CLR observation,
including remaining request delegates/key-related cost, not WASM/live-heap proof.

Existing FCR-7 modes passed: section-index (focus/queued Tab, reorder/add/remove,
surviving State/anchor and disposal), sample-columns (deep retained anchors over
800/1280/800/1001/1000/390/1501/800), and virtual-dispatch behavior. Both Release Web
builds passed with zero warnings and errors. Each has 562 frozen endpoints and a
SHA-256 manifest under `.doroti/c234/{before,after}-wwwroot-manifest.json`.

The compiled DLR contract passed, including zero generated call-site fields under
SliverMultiBoxAdaptorElement. Browser regression passed two tests: restored right
column pixels/upward scrolling, and selection identity/value through responsive
reparenting and theme changes. `returned-1.png` was also visually inspected.

## WASM comparison

Three independent browser contexts per build, order AB / BA / AB. Same browser,
viewport 1280×900, DPR1, hardware-enabled launch and renderer; sequential execution and
zero automatic retries. Every context visits both columns using 12 forward wheel
inputs of 600px and a -20000px return per column, then asserts materialized IDs
0–28. Eight same-column widths and eight breakpoint widths follow, as in the
previous indexed comparison. Initial readiness/preparation is inside each run;
there are no extra warm-up-only processes.

Chromium 151.0.7922.34, WebGL2 on AMD Radeon 780M / ANGLE D3D11; hardware true,
software fallback false. All six runs have runtime errors 0 and all 12 segments
reach an exact-rendered final-generation commit.

The following are descriptive medians of three runs, not a stable p95. The
breakpoint corpus includes unmatched layout work in pairs 2 and 3, so its timing
ratios are **notComparable for same-work causal attribution**.

| Metric | Before | C2/C3/C4 candidate | Observed change |
| --- | ---: | ---: | ---: |
| Same-column callback max | 320.8ms | 330.4ms | +3.0% |
| Same-column callback total | 871.2ms | 884.9ms | +1.6% |
| Same-column final settling | 356.5ms | 349.6ms | -1.9%; mixed paired direction |
| Breakpoint callback max | 402.0ms | 417.1ms | +3.8%; unmatched-work caveat |
| Breakpoint callback total | 1285.8ms | 1369.7ms | +6.5%; unmatched-work caveat |
| Breakpoint final settling | 697.1ms | 765.6ms | +9.8%; unmatched-work caveat |

Raw paired maximum / total / final settling, before → candidate:

| Pair | Same column | Breakpoint |
| --- | --- | --- |
| 1 | 314.1/871.2/393.6 → 323.7/865.8/342.6ms | 415.0/1371.9/788.4 → 426.3/1391.4/772.8ms |
| 2 | 327.5/885.8/356.5 → 334.4/889.2/373.1ms | 397.5/1280.4/697.1 → 417.1/1369.7/750.6ms |
| 3 | 320.8/868.8/353.9 → 330.4/884.9/349.6ms | 402.0/1285.8/685.7 → 400.3/1362.2/765.6ms |

Same-column selected aggregate work is identical in all six runs: Rebuild 2100,
LayoutEntry 975, LayoutWork 870, SetState 17, BuildSort 12, BuildResort 51. Maximum
callback increases in all three pairs; overall repeatable improvement is absent.

Breakpoint Rebuild 3681, SetState 46, DelegateRebuild 4, RetainedChildVisit 58,
BuildSort 16 and BuildResort 43 are identical. However, before runs 2 and 3 have
LayoutEntry/LayoutWork **1345/1208**, versus **1450/1295** for before run 1 and
all candidate runs. The source of this continuous-input scheduling/work difference
was not isolated. Do not ignore it, call it full target parity or attribute the
entire timing difference to the optimization. The fixed-tick 155-event fixture
still matches, but it is a separate and much narrower contract.

C2's intrinsic/dry/baseline profile calls are **zero** in both resize segments in
all six runs. Its CLR allocation saving therefore does not explain resize latency.
HAMT put remains 0 for same-column and 30 / 5,760 bytes for breakpoint. Diagnostic
window allocated bytes include JSON/snapshots; they do not measure whole live heap,
transient peak, process/GPU memory or GC pause improvements.

A minimal-instrumentation scroll control ran once per build: six touch flings,
each with six move events, no resize inputs. Both changed pixels and had errors 0.
Commit interval medians were 16.7ms in both; descriptive within-run p95 values were
49.0 → 33.4ms (232 → 247 commits). Coast p95 was 41.2 → 24.7ms. Detailed callbacks
are disabled (zero captured), so these are commit-notification observations, not
callback latency or physical FPS. One pair cannot establish repeatable improvement
or the non-target p95 regression gate.

Because the same-column workload shows no repeatable improvement and part of the
breakpoint comparison lacks equal work, the combined candidate fails the adoption
conditions. No individual C2/C3/C4 latency effect is isolated by this combined A/B.
The product edits and candidate-only DLR assertion were reversed using the saved
patch and verified byte-equivalent through Git to the starting source. The generic
new cache/dispatch/layout regression fixture and all original evidence are kept.
The final restore build is recorded below; no new AOT, Worker, pool or mode switch
was introduced. Earlier latency/memory/physical gates remain PARTIAL/notVerified.

Raw files: `Doroti/validation/web-playwright/artifacts/indexed-hamt/c234-*.json`.
Runner/ledger, logs, frozen assets and summary: `.doroti/c234/`.
The existing earlier HAMT corpus is not overwritten.

## Budget and preserved failures

Final budget: **20 test/build/browser processes**, timeout **20 minutes**
per process, one GPU workload at a time. Exceeding ten is justified by the six
paired browser contexts plus independent contract, native feature, build,
browser and minimally instrumented scroll checks. The two setup failures count.
The initially planned 19 commands were extended by one final build to restore the
normal build outputs after rejecting the candidate. Browser command 16 contains
two sequential functional tests; raw gesture/callback samples are not independent runs.

1. Baseline Web build PASS.
2. Baseline contract compilation FAIL: new test used ambiguous `Path` (System.IO
   versus Doroti.Ui). Fixed with an explicit alias.
3. Baseline contract runtime FAIL: new fixture reflected the private scratch
   **field** as a property. Corrected reflection; preserve original log.
4. Corrected baseline contracts PASS.
5. Candidate contracts PASS.
6. Candidate Web build PASS.
7–9. Native section-index, sample-columns and virtual-dispatch PASS.
10–15. Browser A/B corpus: see `paired-runs.json`.
16. Browser regression PASS: two tests, runtime errors 0.
17. Compiled DLR assertion and existing typed contracts PASS.
18–19. Minimal scroll before/after PASS (pixel change, runtime errors 0).
20. Restored baseline Web build PASS: zero warnings, zero errors.

The fresh restore has 562 endpoints but 12 endpoint hashes differ from the original
before manifest (Rendering/Widgets WASM/PDB and their fingerprint aliases, dotnet.js
and index.html aliases). This rebuilt artifact was not browser-tested after the
20-run limit; it is not claimed byte-identical. Product source Git blob hashes
match the starting revision. Preview `http://127.0.0.1:5189/?dorotiTestbedMode=sample`
serves the **original tested `before-wwwroot` snapshot**, not the rejected candidate
or the newly rebuilt restore. HTTP 200 was verified; the owned comparison server
on 5188 was stopped. Candidate assets remain frozen under `after-wwwroot`.

Initial failures are test setup failures, not product performance results. Logs
are retained at `Doroti/artifacts/framework-web-work2/c234-*` and `.doroti/c234/`.
No historical latency, memory, platform or physical acceptance FAIL/PARTIAL is
relabelled by these changes.

Reproduction: [execution-cost README](../../Doroti/validation/framework-work/README.execution-cost.md).
