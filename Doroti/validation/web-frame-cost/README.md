# Web frame cost

Run from the repository root. Every build, browser runner, and test uses the
20-minute process-tree wrapper. Browsers run sequentially, in a fresh foreground
Chrome profile; no retries. The runner owns and closes its browser and local HTTP
server. Existing development servers are not required.

```powershell
python Doroti/validation/run-with-timeout.py dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -o Doroti/artifacts/web-frame-cost/product
python Doroti/validation/run-with-timeout.py node Doroti/validation/web-frame-cost/run.mjs Doroti/artifacts/web-frame-cost/run-01 Doroti/artifacts/web-frame-cost/product/wwwroot minimal
python Doroti/validation/run-with-timeout.py python Doroti/validation/web-frame-cost/analyze.py Doroti/artifacts/web-frame-cost/run-01
python Doroti/validation/run-with-timeout.py python Doroti/validation/web-frame-cost/test_analyze.py
python Doroti/validation/run-with-timeout.py node Doroti/validation/web-frame-cost/contracts.mjs Doroti/artifacts/web-frame-cost/product/wwwroot/_content/Doroti.Host.Web/doroti.frame-cost.js
```

Arguments: output directory, publish `wwwroot` (or an existing server URL), mode,
backend, width, DPR. Default geometry is 1280×900/DPR 1. Use a **new output
directory** for each invocation. Chrome's installed path is currently the Windows
default. The driver does not install or update browsers.

Modes:

- `off`: no numeric buffer, stage, layout, or allocation profiling. CDP renderer
  process CPU counters and external observations remain available.
- `minimal`: `dorotiFrameCost=1`; bounded numeric Worker buffer, no detailed
  profiling. Explicit export/reset only at segment boundaries.
- `diagnosis`: also enables resize/stage, layout and allocation profiles before
  runtime creation. It is for attribution, not A/B latency acceptance.
- `smoke`: short tab/theme/text-focus regression, including narrow geometry.
- `inspect`: initial scene only, detailed profile enabled.

The Worker API is accessed through a bounded request/response mailbox on the
existing port. It does not attach a debugger to idle runtime pthreads. Main
ingress and Worker timestamps use `performance.timeOrigin + performance.now()`.
Managed clock capture is bracketed by Worker `started`/`ended`; it provides an
offset interval, not an exact zero-error calibration.

Numeric rows have six values: `[kind, id, inputWatermark, startEpochMs,
endEpochMs, detail]`. Kind 1 is synchronous Framework dispatch (id = callback;
detail = Worker rAF timestamp), kind 2 is input dispatch (detail = main bridge
ingress epoch), kind 3 is synchronous raster/submit (id = present request;
detail = 1 new scene, 0 replay, -1 rejected WebGL attempt). New scene means
`exact-rendered`, not physical scanout. WebGPU rejected attempts are not currently
in this ring. Lifecycle/terminal information is captured separately.

The ring retains at most 16,384 rows and reports overwrite count. Export copies
rows; subsequent writes/reset cannot mutate old results. The analyzer unions
overlapping intervals, so a raster nested in Framework dispatch is counted only
once. `ownerActiveMs` means **instrumented synchronous intervals**, not an OS
thread CPU counter or every activity on the owner. Renderer process CPU includes
main JS and other runtime threads. Neither measures GPU/display time.

`inputWatermarkToSubmitProxy` joins an input to the first subsequent new-scene
submission with a sufficient dispatch watermark. This is **not proof that the
scene reflects that input**; exact scene/input causal correlation remains open.
Do not label it physical input latency or use it alone to accept an optimization.

Managed interval allocation starts after the previous diagnostic export and is
read before the next export allocates JSON/arrays. Owner-thread and process-total
deltas are separate. Heap is a non-forcing live estimate. GC counts are not pause
durations. Summary percentiles use the lower empirical order statistic. Only
continuous S3 submit gaps are counted; idle intervals are not jank.
The final `finish` endpoint reads the managed endpoint and numeric ring in one
Worker turn so an animation callback cannot fall between the two snapshots.

Current reproducible corpus: S0 initial static Components screen; S1 first left
column descent; S2 repeat up/down; prepare the progress section outside timing;
S3 visible progress for 10 seconds; S4 return and press Filled; switch Color and
back; S5 settle for 10 seconds. Inputs, semantics rectangles/labels, actual scroll
offsets, screenshots, environment and served asset SHA-256 values are retained.
This is a **subset** of the full work.md corpus: a small independent animation,
complete right-column traversal, deterministic animation/GlobalKey contract, and
simultaneous focus/scroll interaction are not supplied by these performance runs.
The smoke mode covers text focus separately.

Raw browser profiles, traces and publishes under `Doroti/artifacts` are disposable.
The dated report is the tracked evidence summary and preserves failures and
unverified boundaries after raw cleanup.
