# Platform renderer preference and pending-frame admission

Automatic selection now prefers WebGPU on Android and WebGL2 elsewhere,
including desktop-UA iPadOS. Omitted, `auto`, and unknown renderer values share
this policy. Explicit URL/host overrides retain their requested backend.

Before transferring the visible canvas, Android auto checks the main-runtime,
secure/isolated-origin, WebGPU API, and adapter conditions. An unavailable or
rejected adapter probe selects WebGL2 with a diagnostic fallback reason.
Device/context initialization failures after preflight remain errors; there is
no live backend retry. The existing WebGPU limit of two pending submissions is
unchanged. A request replaced while awaiting GPU capacity is superseded before
raster so the latest request gets the next available slot.

Validation on 2026-09-28 (all commands wrapped by the 20-minute
`Doroti/validation/run-with-timeout.py` deadline):

- Release `Doroti.Host.Web` build: PASS, zero warnings/errors.
- Release Testbed publish: PASS, output under
  `Doroti/artifacts/web-platform-policy/product`.
- `web-memory/contracts.mjs`: PASS. Android/iPhone/iPad/macOS/Windows/Linux,
  auto/unknown/explicit selection, unavailable/null/rejected adapter probes,
  runtime/origin prerequisites, existing canvas-capacity contracts.
- `web-memory/frame-admission.mjs`: PASS. Executes compiled Worker queue/render
  functions with a controlled completion gate. An unchanged request renders;
  three queued requests during a stall render only the newest request, with one
  terminal outcome per request and one submission. GPU execution is stubbed.
- Windows Chrome 153.0.8010.54, auto: selected `worker-direct-webgl` /
  `default-webgl2`; startup and Material rendering observed in captured images.
- Same desktop Chrome, Android UA, 390x900/DPR 3: auto selected
  `worker-direct-webgpu` / `android-prefer-webgpu`; startup and Material rendering
  observed, no renderer error or unhandled page exception in the inspect run.
- Same desktop Chrome, iPhone UA, 390x900/DPR 3: auto selected
  `worker-direct-webgl` / `ios-webkit-stability`; inspect run completed without
  renderer error or unhandled page exception.
- The broader Windows `smoke` run FAILED at `Semantics text input did not retain
  text`. The selected input was `Search colors`; focus remained on the canvas
  before/after `Input.insertText`. This run is not a full interaction PASS, and
  the cause of the focus failure was not established by this renderer task.

Local browser artifacts are in `Doroti/artifacts/web-platform-policy/` under
`windows-auto`, `android-auto`, and `ios-auto`; they are disposable build/test
outputs. UA emulation uses the Windows GPU/browser, not physical Android or
iOS. Physical input/display latency, FPS improvement, and mobile-device
stability remain **notMeasured/notVerified**. The admission contract proves
obsolete work is skipped; it does not establish the cause of the previously
reported platform performance differences.
