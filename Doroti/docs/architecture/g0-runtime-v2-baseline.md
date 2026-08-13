# G0 runtime-v2 baseline

G0 added an executable diagnostic scenario; it did not promote the old H1-H6
evidence to the new runtime standard.

The former `DorotiDemoApp --runtime-v2` diagnostic entrypoint was removed when
the product demo was consolidated into the Goal6 Material application. The
baseline below remains historical evidence rather than a current run command.

The scenario resizes the client, visits every connected monitor, captures every observed DPI, replays wheel up/down and a fractional wheel delta through the host raw-input boundary, performs a drag, then runs image plus animation with continuous fractional scrolling for 30 seconds. `--runtime-v2-duration-ms <positive integer>` exists only for shortened infrastructure checks.

Eight opaque markers cover all four client corners and all four 10-logical-pixel inset positions. Each capture compares the render readback with the native Windows client crop at a two-physical-pixel tolerance. Missing 100/125/150/200% monitor scales remain `not-verified`; the report gives the exact monitor condition needed for a rerun.

The correlated trace contract is `raw-platform-delta -> normalized-pointer-signal -> scroll-position -> build -> layout -> paint -> commit -> raster -> present -> ack`. UI and raster phase timings, managed allocations, GC collections, process counters, mailbox depth, coalescing, framebuffer allocation, pending-array allocation, both full-frame copies, `WriteableBitmap` creation, and UI-thread synchronous waits are recorded. EventPipe/ETW stacks are explicitly `not-verified` when the in-process run is used.

Committed machine-independent results live in `migration/baselines/runtime-v2/doroti-demo-baseline.json`. Machine screenshots and detailed traces live under `artifacts/runtime-v2/<environment>/g0-diagnostic/` and are intentionally ignored by Git. The pre-runtime-v2 H1-H6 JSON files remain historical records; the runtime-v2 ledger starts every H1-H6 gate at `not-verified`.
