# HAMT + indexed performance comparison — 2026-09-08

User explicitly requested performance comparison of
`http://127.0.0.1:5189/?dorotiTestbedMode=sample&dorotiSectionViewport=indexed`.
This is a new authorized six-run corpus, separate from the previous 20-run eager
experiment. No product source or build defaults were changed during this comparison.

**Result: no demonstrated indexed latency benefit from adding HAMT.** Same-column
callback total and final settling were slower in all three pairs. Breakpoint
results varied in direction. Keep the existing Dictionary + indexed path as the
comparison baseline; do not promote HAMT based on these results. The previous
eager map-allocation reduction does not establish indexed responsiveness.

## Controlled inputs and artifact identity

- Before: Dictionary + indexed, frozen `before-wwwroot`, port 5188.
- Candidate: HAMT + indexed, the requested frozen `hamt-wwwroot`, port 5189.
- Foundation WebAssembly SHA-256 values were verified against the preserved
  manifests: before `e854c3d86d8e13e030a0730d105e785a51fdc6eb17d764ab381a43a68dcd4e8f`,
  HAMT `b983a89f104c2e47f088e752730233cf092fb3947269f3f402215f1fe68d49e3`.
- Chromium 151.0.7922.34, 1280×900 CSS px, DPR1, worker-direct-webgl. All six runs
  used hardware WebGL2: AMD Radeon 780M / ANGLE D3D11; no software fallback.
- Order AB, BA, AB; six independent browser contexts, sequential, zero automatic
  retries, external 20-minute timeout per run. No extra warm-up-only execution,
  builds or other agent-owned GPU workloads ran concurrently.
- Each context starts with exactly IDs `[0,1,2,3,12,13,14]` materialized. Each
  column receives 12 forward wheel inputs of 600px, separated by 750ms, then
  -20000px to return to the beginning. Prepared IDs are exactly 0–28 in every run.
  This is actual indexed materialization evidence, unlike eager's initial 29 IDs.
- Each context then measures two explicitly separated sequences: same-column
  1240/1200/1160/1120/1160/1200/1240/1280, followed by breakpoint
  980/900/800/1100/1400/900/1100/1280. Nominal interval 45ms. That is exactly 16
  resize inputs per context, not 16 independent trials. The breakpoint test follows
  the same-column test in both modes; it is not a cold first-computation result.
- Raw requested timestamps, observed epochs, generation traces and exact final
  commits are retained. All 12 measured segments reach an exact-rendered final
  generation. Runtime errors: zero in all six contexts.

## Results

Each cell below is the median of the three per-run values. Lower is better.
"Callback max" means the maximum callback in each short sequence, not a stable p95
or measured display FPS. Final settling is the time from the end of the requested
input sequence to its first exact final-generation front commit.

| Metric | Dictionary + indexed | HAMT + indexed | Change |
| --- | ---: | ---: | ---: |
| Same-column callback max | 323.3ms | 333.7ms | +3.2% |
| Same-column callback total | 884.6ms | 915.6ms | +3.5% |
| Same-column final settling | 350.3ms | 396.4ms | +13.2% |
| Breakpoint callback max | 423.0ms | 419.5ms | -0.8% |
| Breakpoint callback total | 1386.2ms | 1382.8ms | -0.2% |
| Breakpoint final settling | 780.3ms | 769.6ms | -1.4% |

Raw per-run times, before → HAMT:

| Pair | Same-column max / total / settling | Breakpoint max / total / settling |
| --- | --- | --- |
| 1 | 306.1/848.7/324.7 → 304.9/854.1/367.5ms | 413.6/1386.2/787.8 → 419.5/1382.8/769.6ms |
| 2 | 323.3/884.6/350.3 → 352.8/954.6/438.9ms | 427.8/1399.8/780.3 → 430.3/1448.7/832.5ms |
| 3 | 325.7/888.6/375.9 → 333.7/915.6/396.4ms | 423.0/1371.7/755.0 → 404.1/1364.6/768.0ms |

Same-column settling worsened in all three pairs (+13.2%, +25.3%, +5.5%).
Breakpoint settling changed -2.3%, +6.7%, +1.7%; its small median reduction is not
a repeatable improvement. The original 16.7/33.3/50/100ms acceptance gates remain
unmet; do not infer physical window-drag smoothness from these callbacks.

## Work and allocation attribution

Aggregate work counts are identical across all before/HAMT runs:

| Sequence | Framework callbacks | Rebuild | Layout entry / actual work | SetState | Map put |
| --- | ---: | ---: | ---: | ---: | ---: |
| Same-column | 3 | 2100 | 975 / 870 | 17 | 0 |
| Breakpoint | 5 | 3681 | 1450 / 1295 | 46 | 30 |

These aggregates support workload consistency but do not replace the full logical
node trace required for universal same-target qualification.

Map allocation is zero in both modes for same-column resize. In each breakpoint
run it is **20,400 → 5,760 bytes**, a 14,640-byte reduction (-71.8%). Map put time
is approximately 0.2ms, so copying this map is a very small portion of this indexed
workload. This is consistent with the lack of overall latency improvement.
Lookup self time was not isolated, so no specific cause is assigned to slower
same-column results.

Whole diagnostic-window allocation varies substantially: same-column before
24,093,232/35,212,712/23,741,160 bytes versus HAMT
31,985,264/24,371,904/31,980,960; breakpoint before
40,156,936/40,793,000/40,888,168 versus HAMT
28,857,592/28,918,352/28,824,736. These deltas include diagnostic snapshot/JSON
allocation and variable-length prior frame histories. The breakpoint difference
is much larger than the isolated map saving; it is not attributed to HAMT or
reported as actual UI allocation improvement. GC collection deltas and heap
estimates are retained in summary.json. GC pauses, process/GPU memory, WASM
capacity and physical display remain notMeasured/notVerified.

## Reproduction and evidence

Probe: `Doroti/validation/web-playwright/measure-indexed-hamt.mjs LABEL`.
Set `DOROTI_WEB_BASE_URL` to either frozen origin; the probe explicitly adds indexed
and diagnostic query parameters. Summary:
`Doroti/validation/web-playwright/summarize-indexed-hamt.py`.

Raw six runs and `summary.json`:
`Doroti/validation/web-playwright/artifacts/indexed-hamt/`.
Runner, logs, SHA identities and six-entry ledger:
`.doroti/wasm-indexed-comparison/`.

Both comparison URLs remain available for the user's manual inspection. The
existing 5088 server was untouched. These detailed measurements do not qualify
minimally instrumented performance, physical input, IME or accessibility.
