# Framework work attribution

The direct Worker enables `DOROTI_STAGE_TRACE` before runtime creation when
`dorotiResizeDiagnostics=1`. The counters and profiler belong to that managed
thread. Capture them using the existing Worker `__dorotiDirectDiagnostics()` hook.

`managed.work` contains cumulative numeric counters at frame trace sequence
boundaries. Subtract the build boundary from its following layout boundary for
build work. The 8,192-entry ring reports `Dropped`. `Names` defines the counter
column order; do not hardcode enum ordinals in an offline consumer.

`managed.profile` contains cumulative type/kind calls, inclusive microseconds,
self microseconds, allocation/managed heap/GC observations, thread ID and overflow
counts. Type names are materialized only on capture. Kinds are:

| Kind | Scope |
| --- | --- |
| 0 | Element rebuild, grouped by widget type |
| 1 | implicit animation update/tween check, grouped by owner State type |
| 2 | Web semantics bridge update, including serialization and posting |
| 3 | node payload preparation/cache and serialization, includes kind 4 |
| 4 | JSON serialization of uncached node payloads |
| 5 | synchronous bridge posting; excludes main-thread DOM application |
| 6 | animation restart count, grouped by owner State type |

`frames` is a bounded chronological ring. Each numeric row is
`[buildTraceSequence, layoutTraceSequence, ...five groups of
(typeId, calls, inclusiveMicroseconds, selfMicroseconds)]`, ranked by self time.
Zero-call groups are empty. Names come from `entries` in the same capture.
Inclusive values may exceed wall time because equal-type scopes can nest.
Never sum inclusive values or add raster to a callback that already contains it.
Self time excludes only instrumented children; it is not a complete managed
method CPU profile. Missing symbols or dropped boundaries remain unassigned.

`measure-material-sample.mjs` preserves `profileBefore`, `onsetDiagnostics`, and
the later `directDiagnostics` separately. Early capture prevents six seconds of
animation from evicting the initial frame. Diagnostic capture can perturb later
cadence; the minimal marker corpus is independent. `--cpu-profile` records a short
Chrome timeline in a separate file. Main LongTask/LoAF entries are independent of
Worker callbacks. `--sweep` records new/revisit and entry windows by column.

With detailed diagnostics OFF, `dorotiInputMarkers=1` retains direct commit
notifications and the Worker-dispatched input sequence captured when requesting
that scene. Only `exact-rendered` (not `replay-rendered`) is eligible for onset.
This does not measure visible pixels, browser compositor presentation or scan-out.
OFF runs do not measure managed callback percentiles or allocation.

Run processes with the repository's 20-minute deadline, for example:

```powershell
$env:DOROTI_WEB_BASE_URL = 'http://127.0.0.1:5088'
./Doroti/eng/invoke-work2-check.ps1 -Label local-onset -Directory Doroti/validation/web-playwright -Command @('node','measure-material-sample.mjs','local-onset','--progress','--onset','--no-diagnostics','--no-trace')
python Doroti/validation/web-playwright/analyze-framework-work.py Doroti/validation/web-playwright/artifacts/sample-perf/local-onset.json
```

The analyzer reports absent managed attribution in minimal-marker runs. It uses
`RecordedAtMicroseconds` for CPU spans and joins the exact request ID for causality.
It stops at the next build boundary so an absent semantics flush cannot consume
a later frame's span. The source/publish fingerprint is stored with each corpus.
