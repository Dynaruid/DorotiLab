# Indexed gallery: carousel and scroll follow-up

## Scope and observation

The user reported that `http://127.0.0.1:5088/?dorotiTestbedMode=sample&dorotiSectionViewport=indexed`
felt roughly twice as fast as before, while carousel and scrolling still stuttered.
This is user-observed improvement of the earlier indexed candidate, not a measured
2x FPS result or acceptance of every gate in `work.md`.

## Implemented

The shared `Doroti.Skia.Rendering` renderer now retains native Skia picture
commands for repeated immutable framework pictures. A first use draws normally;
a second use records commands; subsequent uses replay the native picture. This
removes repeated managed-to-native drawing conversion while the destination
still applies its current fractional translation, scale, clipping and font policy.
The existing pixel raster cache and fractional-phase signature remain in place.

`willChange` pictures, device-space shadows, unbounded paint/color operations,
invalid bounds and oversized command lists use the ordinary path. Bounds changes
discard the old recording. Font registration, GPU invalidation and renderer disposal
release recordings. Retention is limited to 128 entries, 32,768 framework commands
and 4 MiB approximate native command bytes. Referenced image pixels are **not**
included in Skia's command-byte estimate; these are not a whole-process memory cap.
Temporary native paths used for clipping and drawing are now disposed immediately.

This repairs the shared renderer; the sample carousel geometry, snapping physics,
scroll physics, effects and indexed/eager selection have not been altered. The
implementation follows Skia's immutable recorded-picture semantics and respects
the recording cull bounds: [SkPictureRecorder reference](https://api.skia.org/classSkPictureRecorder.html).

## Reproduction and evidence

Use `Doroti/validation/web-playwright/measure-gallery-motion.mjs LABEL SCENARIO`
where SCENARIO is `carousel`, `snapping`, or `scroll`; `--fling` selects a shorter
touch drag followed by an input-free coast, and `--minimal` disables detailed
framework diagnostics. `DOROTI_WEB_BASE_URL` selects the isolated server.
Every invocation has a 20-minute timeout. Runs are sequential, without concurrent
builds or GPU tests. Measurements include CDP touch events, callback/raster trace,
commit notifications, allocated-byte deltas and a post-gesture screenshot.

- Before: previous turn's `.doroti/wasm-structure/reorder-wwwroot`, port 5090,
  `reorder-wwwroot-manifest.json` (562 endpoint hashes).
- Initial candidate: `.doroti/gallery-motion/command-wwwroot`, port 5091.
- Fling comparison: `.doroti/gallery-motion/final-wwwroot`, port 5092,
  `final-wwwroot-manifest.json` (562 endpoint hashes).
- Final served build: `.doroti/gallery-motion/served-wwwroot`, port 5088,
  `served-wwwroot-manifest.json` and `served-source.json`. It adds only the
  finite-float cull-bounds fallback to the fling comparison source.
- Raw data/screenshots: `Doroti/validation/web-playwright/artifacts/gallery-motion/`.
- `summarize-gallery-motion.mjs` produces `summary.json` from the 12 fling runs.
- Build/contract logs: `Doroti/artifacts/direct-default/motion-*.log`.

Budget: 4 initial slow-drag profiles, 12 interleaved fling comparisons (2 runs per
revision for each of 3 scenarios), and 4 minimal-diagnostics comparisons for the
ordinary carousel and vertical scroll, at most 20 performance invocations total.
The extended budget separates input pacing and diagnostic overhead from rendering.
Correctness fixture cases are separate from these performance invocations.

CDP input pacing is workload-dependent. A commit notification is not physical
scan-out. Coast interval percentiles can contain idle boundaries, and two runs
per condition do not establish confidence intervals. Report execution-level
medians, individual raw results and remaining long frames, not physical FPS.

## Results

Hardware browser: Chromium 151.0.7922.34, AMD Radeon 780M via ANGLE D3D11,
1280x900 at DPR 1. Each cell is the median of the two independent run-level
statistics; smaller times are better.

| Scenario | Callback p50 before -> after | Callback p95 before -> after | Raster p50 before -> after | Allocated bytes/submitted reduction |
|---|---:|---:|---:|---:|
| Carousel | 11.70 -> 7.60 ms (-35.0%) | 19.30 -> 15.90 ms (-17.6%) | 7.85 -> 3.60 ms (-54.1%) | 27.5% |
| Snapping carousel | 11.60 -> 6.80 ms (-41.4%) | 19.20 -> 14.15 ms (-26.3%) | 7.90 -> 3.30 ms (-58.2%) | 24.8% |
| Vertical scroll | 10.15 -> 6.45 ms (-36.5%) | 35.45 -> 18.55 ms (-47.7%) | 8.00 -> 3.83 ms (-52.2%) | 29.2% |

Input-free coast commit intervals remain around 16.5-17 ms at the median.
Coast p95 is 24.4 -> 24.2 ms for ordinary carousel (essentially unchanged),
22.4 -> 19.8 ms for snapping, and 42.3 -> 30.15 ms for vertical scrolling.
These are commit intervals, not display FPS. Rendering costs improved much more
than ordinary carousel commit cadence; this is not a 2x carousel FPS claim.

All 12 fling runs changed rendered pixels and reported zero page errors and zero
renderer failed frames. After snapshots retained 128 command entries, around
916-956 framework commands and 25-43 KiB native command bytes. Source-backed
checks cover the configured bounds; a complete live/transient memory acceptance
is still notVerified.

Long frames remain: the two final scroll runs peaked at 247.8 and 158.6 ms,
versus 122.0 and 99.8 ms before. At the 247.8 ms callback, raster took 57.1 ms;
the majority remained outside raster. Small-n, workload-dependent CDP gestures
do not establish the cause of the different maxima. New-section/first-input
worst latency is unresolved and must not be declared improved by the p95 result.

## Final validation

Final served build, detailed diagnostics disabled (one independent run per
revision/scenario, no callback timings collected):

| Scenario | Commit interval p50 before -> after | Commit interval p95 before -> after | Coast p95 before -> after |
|---|---:|---:|---:|
| Carousel | 16.8 -> 16.9 ms | 33.3 -> 31.1 ms | 24.4 -> 23.0 ms |
| Vertical scroll | 17.1 -> 16.6 ms | 57.6 -> 35.6 ms | 57.6 -> 34.5 ms |

All four minimal runs changed pixels and had zero page errors. Total performance
invocations: 20, with no discarded runs or benchmark retries. This is consistent
with lower rendering cost and fewer long scroll intervals; ordinary carousel
cadence remains close to the previous baseline.

| Check | Result | Evidence |
|---|---|---|
| Final Web Release build | PASS, 0 warnings/errors | `motion-served-web-build.log` |
| Native command cache | PASS: phase/DPR/blend/clip pixels, bounds change, excluded paths, budgets, font/context release | `motion-final-command-contract.log` |
| Existing text/platform surface regression | PASS: 180 fixture frames, coverage delta <= 2/255, ink center delta < 0.002 device pixels | `motion-final-text-platforms.log` |
| Existing raster promotion budget and lifecycle | PASS | `motion-final-raster-budget.log` |
| All blend modes against direct drawing | PASS | `motion-final-blends.log` |
| App bars through scroll/cache pressure | PASS, appbar pixel differences 0 | `motion-final-appbar.log`, `.doroti/gallery-motion/appbar/` |
| Final Web UI regression | PASS, 7 tests: column return, progress/navigation state, hover at DPR 1/1.25/1.5/2 | `motion-final-web-regressions.log`, `artifacts/gallery-motion-final/` |

The native checks use offscreen GPU fixtures, not physical display/IME/accessibility
acceptance. Prior work.md strict resize/startup/accessibility/platform failures and
notVerified gates are unchanged. Indexed remains opt-in. The 5088 server serves
the final immutable build; owned comparison servers 5090/5091/5092 were stopped.

Final delivery check: all 562 served endpoint SHA-256 values match the frozen manifest; renderer/probe source hashes match `served-source.json`; the exact user URL returns HTTP 200. The 5088 server remains running (PID recorded in `.doroti/gallery-motion/served-server.pid`).
