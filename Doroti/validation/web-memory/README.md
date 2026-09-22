# Web memory and iOS backend stability

Run commands from the repository root. Use `python3` on macOS (`python` on Windows).
Each browser invocation uses a fresh Chrome profile, a 20-minute process-tree
timeout, zero retries, and sequential GPU execution. `runs.json` counts failed
launches toward the 30-run M0–M5 budget. An exclusive lock prevents overlapping
runners. A process-tree timeout may leave a lock file; inspect the terminated
run before removing that stale lock manually. No retry is automatic. Historical P runs are separate.

```sh
python3 Doroti/validation/run-with-timeout.py dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -o Doroti/artifacts/web-memory/final/product
python3 Doroti/validation/run-with-timeout.py node Doroti/validation/web-memory/contracts.mjs
python3 Doroti/validation/run-with-timeout.py dotnet run --project Doroti/validation/web-memory/CacheContracts/CacheContracts.csproj -c Release
python3 Doroti/validation/run-with-timeout.py node Doroti/validation/web-memory/run.mjs Doroti/artifacts/web-memory/run-example Doroti/artifacts/web-memory/final/product/wwwroot auto memory 1
```

Driver arguments: fresh output directory, published wwwroot, backend (`auto`,
`worker-direct-webgl`, `worker-direct-webgpu`), mode, mobile policy emulation (`1`
or `0`). Modes `memory`, `memory-off`, `minimal`, and `smoke` reuse the frame-cost
driver. `textures` and `webview` use the existing product regression drivers
via `run-contract.mjs`; pass an already-running local published-product server
URL in place of wwwroot. The adapter selects host Chrome and, when mobile=1,
the same iPhone UA (viewport remains that of each contract). These runs are
headless functional contracts, not foreground performance observations.
The WebView controller driver needs a separate publish with
`-p:DorotiWebPlatformValidation=true`; its `WebPlatformExport` is intentionally
absent from a normal product publish. Keep that validation output separate from
normal benchmark/foreground assets. macOS and Windows installed Chrome are supported; `DOROTI_CHROME` overrides
the executable. A mobile run uses an iPhone UA, 390×900, DPR 3. It is **desktop
Chromium emulation, not iOS WebKit evidence**. Desktop uses 1280×900, DPR 1.

`memory-soak` (minimal diagnostics) or `memory-soak-off` (diagnostics OFF)
repeats visits for at least 10 foreground minutes, then runs the
resize sequence; both are still desktop observations. The 20-minute wrapper also applies.

The memory workload repeats full column traversal, image demo, theme changes and
Color/Components return three times, then landscape, portrait, shortened viewport
and restored viewport. Screenshots preserve actual image rendering. The shortened
viewport is a resize contract, not a real software-keyboard/IME test. Inputs and
segment start/completion markers are written outside the tab so a crashed page
cannot erase the last completed step. `memory-off` disables the numeric ring and
managed snapshots as well as detailed tracing. `memory` takes explicit boundary
snapshots; no per-frame serialization or forced GC is added.

Initial budget allocation: 2 backend baseline runs; 12 same-backend comparison
runs; 6 functional/diagnostics-off regressions; 4 physical-device observations;
6 investigation slots. Unused slots do not justify automatic retries. Every
manual device observation also needs an entry in `runs.json`, actual start/end,
OS/build/Chrome/URL, orientation/DPR, selected backend, and outcome. Stop at 20
minutes; final stability observation targets 10 minutes or twice the known failure
time, capped at 20 minutes. Physical device observation requires a reachable
secure isolated origin and actual browser interaction.

The native contract stresses 1,024 styles across four passes, Latin/Hangul/emoji,
font fallback, size/weight/spacing, exact pixel/measurement recreation, retained
paragraph/SKPicture replay after eviction and font registration, and repeated
Dispose. It renders with native macOS Skia at fractional offset and scale.
`RasterBudget/RasterBudget.csproj` additionally uses native Metal to verify
preallocation reservation, active-frame retention and exact cached/direct pixels.
Run GPU contracts sequentially with browser/device work.

Policy fixed for this experiment:

- iOS/iPad desktop-UA automatic selection: WebGL. Explicit overrides remain exact;
  initialization failures are errors, with no backend retry. Restore automatic
  WebGPU only after cause/OS-specific stability evidence exists.
- Text cache: 256 LRU entries. Native resource counts and wrapper/map byte
  estimates are available through `CaptureCacheMemory`; estimates exclude font
  files, shared typefaces and Skia glyph caches. The observed sample working set
  was 80 entries; the stress fixture deliberately exceeds the limit.
- Mobile WebGL native budget: 64MiB; raster cache: 24 entries and
  4M pixels (16MiB RGBA8). WebGPU and desktop keep 256MiB native
  context/recorder budgets and 16M raster pixels. The smaller WebGPU trial
  had inconsistent latency and usage above its soft budget, so was not adopted. Raster reservation
  happens before surface allocation; entries used in the current frame are
  protected. One picture can occupy at most one quarter of the pixel budget
  (mobile: 1M pixels). Oversized/deferred pictures use normal rendering. Native references held by draws/recordings
  may outlive cache eviction; the configured values are not total resident limits.
- Unused native resources: cleanup after at least five seconds without raster
  activity, between owner frames. No forced
  queue synchronization, disposal of active uploads, or whole-cache flush each
  frame. The installed Graphite upload cache already has a 256-entry LRU.
- Mobile backing: exact needed pixels initially/on growth. Shrink when excess
  area is at least 25%, after 1s of stable dimensions and at least 2s since the
  last allocation. Smaller changes retain less than 25% extra area. Shrink axes
  before growing the other axis to avoid a square rotation intermediate. DPR is
  unchanged. Only the transferred canvas owner writes width/height.

Diagnostics distinguish non-forcing managed heap estimate, interval allocations,
GC counts, WASM capacity, owned text/raster cache accounting, Ganesh cache usage,
Graphite context budgeted bytes, canvas RGBA8 estimates and old+new transition
estimates, queue/map counts, and texture retirement. Canvas estimates exclude
depth/stencil, multisampling, browser buffers, driver copies and delayed release.
The installed managed API does not expose Graphite recorder usage; it is labeled
`notMeasured`. Process/GPU resident memory and physical presentation remain
unmeasured. Never sum overlapping budget/accounting values as total GPU memory.

Use separate backend reports. WebGL mitigation is not a fix for WebGPU termination.
Keep source/asset manifests, run failures and adopted/rejected candidates in the
dated report; raw publishes, profiles and device logs in `Doroti/artifacts` are
disposable and are not committed.
