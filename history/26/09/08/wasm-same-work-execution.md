# Same-target .NET WASM execution experiment — 2026-09-08

User requested implementation of `work.md` section 9. Baseline HEAD:
`4e853fb96ffdb349f0024cbb8655481c3527ec28`, initially clean. The old document-only
request is historical, not a prohibition on this authorized implementation.
Sections 1–8 and their FAIL/PARTIAL results remain unchanged.

## Implementation and decision

Implemented a generic typed bitmap HAMT behind the build property
`DorotiExperimentalHamt=true`. The public `PersistentHashMap<TKey,TValue>` API,
inherited lookup/notification rules, Widget/Element/RenderObject work, State
lifetime and execution timing are preserved by construction; the bounded fixture
below provides concrete evidence for its tested subtree. Nodes and arrays are
immutable, only the edited path is copied, full-hash collisions use typed leaves,
and removal collapses a branch only when hash-depth semantics remain valid.
Values are always replaced; equality does not suppress reference updates.
No current product call site enumerates these maps for ordering.

**Decision: retain as experimental / componentOnly; do not promote the default.**
The map has a demonstrated allocation reduction, so this is not a candidate with
no measured benefit. However it does not pass the full acceptance gates. Normal
builds retain Dictionary copying and exclude the trie implementation. Eager,
`worker-direct-webgl`, Jiterpreter/AOT/threads settings and sample ownership remain
unchanged. No C2/C3/C4 rewrite was opened without stronger hot-path evidence.

Added common Ui contract tracing, component cost counters, numeric section
materialization evidence, C#/Dart fixtures and strict comparison tooling. Runtime
trace IDs use weak object keys and bounded numeric events; names are exported
after capture. Disabled trace hooks guard receiver evaluation, avoiding additional
virtual getter calls in ordinary execution. Unsupported non-Box constraint
payloads are marked explicitly. The trace is a Q1 foundation, not complete coverage
of dirty-sort order, all cache calls, sliver constraints, hit testing or semantics.

## Q0 and provenance

- `.doroti/wasm-same-work/before-head.txt`, `start.diff`, `properties.json` fix the
  starting source and evaluated build properties. net10.0/browser-wasm, Release,
  WasmBuildNative=true, PublishTrimmed=true, Optimize/SIMD=true; RunAOTCompilation,
  explicit Jiterpreter and threads properties unset. Native relinking is not AOT.
- Frozen `baseline-wwwroot` (initial diagnosis), `before-wwwroot`, `hamt-wwwroot`
  each contain 562 endpoints with adjacent SHA-256 manifests. The common native
  runtime assets are the same in the before/HAMT pair. Served `dotnet.runtime.js`
  identifies Release 10.0.11. Active Jiterpreter options/statistics are notMeasured.
- `candidate-source.json` identifies the measured source. Subsequent trace guards,
  the explicit unsupported-constraint flag and default trie exclusion are final
  integration changes; their final contract is retested, but their performance
  is not a new measured corpus.
- Local `reference/flutter-master` is exactly lock revision
  `56b8e1a851a594b1a154f8ea93270807dab22b9a`, with cached 3.48.0-0.3.pre / Dart
  3.14 dev metadata. The installed SDK identified in the research is separate.
- Framework map, Widgets and Rendering files use the existing reviewed-source
  ownership marker. This experiment changes no generator dispatch lowering;
  regeneration of generated dispatch was not required or claimed.

## Bounded correctness evidence

- Map oracle checks pass for 4/16/64/29/290/4096 entries: snapshot immutability,
  collisions, shared low hash bits/high-bit divergence, replacement, removal,
  missing keys, nullable values, enumeration membership, original equal-key
  identity, replacement value identity, and removal reference release.
- Five deterministic ticks mount/update/reparent left→right→left/remove one
  GlobalKey subtree. Same State is retained for ticks 0–3 and disposed at tick 4;
  nearest inherited values are 10→20→10. C# A/B has **155 identical ordered events
  over 20 nodes**, including the wrapper mapping. Overflow is rejected.
- The repository-pinned Flutter fixture has **17 identical observable
  lifecycle/build callbacks**. It is a debug Flutter test versus Release Doroti;
  full Flutter internal wrapper/target tracing remains notVerified.
- Final guarded trace again matches the original 155 events and Flutter callbacks.
- Native Skia sample regression passes wheel input, initial scroll extent,
  first traversal, all 29 retained section States, widths 600/800/1280, text scale
  1/1.25 and one/two-column reflow. This is automated native rendering evidence,
  not physical Windows/IME/scan-out acceptance.
- Existing FrameworkWork contracts pass in the final normal-map run with
  `DOROTI_STAGE_TRACE=1`, `DOTNET_TieredCompilation=0`. The earlier run with added
  map-test warm-up reported 128 bytes instead of its required zero. That FAIL
  remains in its original log; the zero-allocation assertion was not relaxed.

## WASM attribution and paired results

Initial diagnosis already materialized IDs 0–28 on eager. It is **not** a partial
visit baseline. During its eight breakpoint inputs, inheritance put had 2,060
calls, 54,379 input entries, 1,400,800 allocated bytes and 20.8998ms against
4,488.8ms total callback time (~0.47%). This deprioritized HAMT as a latency
solution; a limited allocation experiment was still justified. C2 timings were
inclusive and do not establish its self-time rank. Aggregate Text/DefaultTextStyle
cost alone did not justify another speculative dynamic-dispatch rewrite.

Three paired independent Chromium contexts: AB, BA, AB; 1280×900, DPR1, eager,
worker-direct-webgl. Each run performs 24 preparatory forward wheel inputs plus
two return inputs, checks exact materialized IDs 0–28, then requests widths
980/900/800/1100/1400/900/1100/1280 at nominal 45ms gaps. The applied generations
are retained in each raw trace. Four framework callbacks were observed per run;
there are no hidden extra resize loops. Runtime errors are zero in all six runs,
and every final generation has an `exact-rendered` front commit.

| Pair | Callback total, before → HAMT | Callback max, before → HAMT | Final settle, before → HAMT |
| --- | --- | --- | --- |
| 1 | 5103.5 → 4401.6ms | 2386.4 → 2073.7ms | 4526.5 → 3815.8ms |
| 2 | 4729.0 → 4612.6ms | 2220.4 → 2295.7ms | 4161.0 → 4028.0ms |
| 3 | 5271.3 → 4723.9ms | 2482.1 → 2231.3ms | 4679.2 → 4106.7ms |

All six runs have exactly 2,060 map puts and 54,379 input entries. Map allocation
is **1,400,800 → 396,992 bytes (-71.7%)** in each pair. Map times are
14.7000/13.4022/13.4003ms before versus 6.8/8.4/8.3ms HAMT. Total diagnostic-window
managed allocation is 45,107,720/45,407,360/45,510,240 bytes before versus
43,188,880/44,610,248/44,563,424 bytes HAMT. This total includes diagnostic export
overhead and is not an isolated action-allocation measurement.

Callback total decreases in all three pairs, but max worsens in pair 2. With
only four callbacks per run, the probe's reported p95 is just the maximum.
These detailed runs do not establish minimally instrumented latency, statistical
confidence or causation for gains larger than the measured map time. The original
30% overall and 16.7/33.3/50/100ms gates remain **FAIL/PARTIAL**.

Raw evidence: `.doroti/wasm-same-work/pairs.json`, paired `.log` files,
`Doroti/validation/web-playwright/artifacts/state-resize/same-work-*.json`, and
`same-work-summary.json` (including raw heap estimates and GC collection deltas).
GC pauses, live/transient memory, process/GPU/WASM capacity and Jiterpreter
statistics remain notMeasured. Full sample logical target trace, minimal/control
workloads, DPR/geometry matrix, physical input/IME/accessibility and other physical
hosts remain notVerified. Stock Flutter sample performance remains notComparable.

## Execution budget and original failures

Expanded from 10 to **20 total executions** to include seven initial correctness/
setup attempts, baseline and pinned-Flutter fixtures, the required six paired Web
runs, native sample regression and final checks. Stop at 20: no further performance
or runtime retries. Every runtime/benchmark has an external 20-minute timeout;
browser retry is zero and builds/GPU workloads were sequential. Compilation and
publish alone are recorded separately, not counted as extra runtime executions.

1. Browser setup FAIL: relative endpoint manifest froze no assets; browser opened
   a directory listing. Aborted owned browser, preserved failure JSON, then used
   the absolute evaluated build manifest.
2. Web attribution PASS (all 29 eager sections already materialized).
3. Map correctness PASS; existing numeric-aspect zero-allocation check FAIL (128B).
4. C# fixture compile FAIL: ambiguous `Path`; qualified System.IO.Path.
5. Fixture setup FAIL: direct RenderPositionedBox required explicit textDirection.
6. Fixture setup FAIL: missing WidgetsBinding; added dispatcher/binding.
7. Fixture setup FAIL: binding required platform messaging; added a registered
   deterministic host with view/frame/messaging capabilities.
8. Fixture compile FAIL: ambiguous `Action<T>`; qualified System.Action.
9. Candidate map + fixture PASS.
10. Baseline map + fixture PASS.
11. Pinned Flutter fixture PASS.
12–17. Three Web pairs PASS for workload completion; adoption remains unqualified.
18. Candidate native sample regression PASS.
19. Final existing FrameworkWork contracts PASS, tiered compilation disabled.
20. Final candidate map + guarded trace fixture PASS.

The setup failures are not relabeled as passing tests. Logs are under
`Doroti/artifacts/direct-default/same-work-*.log`. No user-owned 5088 server was
changed. Only the task-owned 5188/5189 comparison servers were started.

## Integration status

Q0 source/artifact identity is captured; active runtime-option introspection is
incomplete. Q1 has a useful, tested bounded trace and C1 attribution, with the
missing coverage above. Q2 candidate implementation and local contracts pass.
Q3 paired experiment is complete within budget; broad adoption is withheld.
Q4 reviewed-source ownership and final tests are recorded; publish status is
recorded below after completion. This is **PARTIAL / componentOnly**, not all-platform
acceptance or completion of every Q0–Q4 gate.


Trimmed candidate Web publish: **PASS** (`dotnet publish`, Release,
`DorotiExperimentalHamt=true`, `PublishTrimmed=true`, external timeout 20 minutes).
The 70 WebAssembly/manifest files are hash-recorded in
`.doroti/wasm-same-work/trimmed-candidate-manifest.json`. This published artifact was
not launched as another runtime trial after the 20-run budget. The task-owned
5188/5189 servers were stopped; the existing 5088 listener was retained.

Final normal Web build (without the experiment property): **PASS**, 0 warnings /
0 errors. The working build outputs are restored to the default map path. Final
`git diff --check` passes. No physical or post-publish browser acceptance is added
by these build results.
