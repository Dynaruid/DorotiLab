# Diagnostics Web resize scale repair

2026-09-08. Reported URL: http://127.0.0.1:5088/?dorotiTestbedMode=diagnostics

## Cause and implementation

The current default is `worker-direct-webgpu` (verified in the running server).
WebGPU resized its visible OffscreenCanvas to every exact frame size, while main
updated explicit CSS width/height upon a later `direct-commit` message. The two
updates are separate: a new bitmap could be displayed inside the previous CSS
box. The old `object-fit: cover` still scales when both dimensions change.

- WebGPU now retains its initial capacity and grows only when required.
- Graphite wraps that capacity but paints/clips using the exact physical viewport;
  unused texture pixels are initialized transparent. Layout dimensions are unchanged.
- Commit/resource messages report actual allocated dimensions.
- Both direct renderers use intrinsic canvas sizing (`auto`) and a constant
  physical-to-CSS DPR conversion. Neither viewport size nor capacity messages
  determine the displayed bitmap scale. This also covers simultaneous width/height
  growth while main has not processed the completion message.
- The capacity regression probe now measures the actual canvas bounds and
  intrinsic backing, rather than explicit inline width or asynchronous metadata.

The browser owns OffscreenCanvas-to-placeholder updates; see the
[HTML canvas specification](https://html.spec.whatwg.org/multipage/canvas.html).
Intrinsic sizing avoids a second application-controlled geometry handoff.

## Verification

Every build/test process used a 1,200-second outer timeout; Playwright tests also
retain the repository's 20-minute timeout. No automatic retries were enabled.

Final product build: Release, zero warnings/errors, 11.24 seconds.
Final Playwright TypeScript check and `git diff --check`: PASS.

Final acceptance items, run in separate groups on the same final product build:

| Item | Result |
| --- | --- |
| Diagnostics continuous viewport changes, WebGPU/WebGL × DPR 1/2 | 4 PASS |
| Visible pixels during delayed main commit, WebGPU/WebGL × DPR 1/2 | 4 PASS |
| Default WebGPU DPR2 sheet pointer open/close | 1 PASS |

The pixel test holds only main's direct-commit event. It shrinks, grows BOTH
dimensions beyond initial capacity, then returns to the original viewport.
All captured frames retain the diagnostics L marker's 102 physical pixels at
DPR1 / 408 at DPR2, and its exact bounds. It separately waits for right/bottom
edge pixels at the new target, without releasing the held commit. An old frame
or a Worker submission alone cannot satisfy this completion criterion.
Captured diagnostics images were also opened for visual inspection.

Evidence under `Doroti/validation/web-playwright/artifacts/`:

- `resize-scale-final-build.log`, `resize-scale-types-complete.log`.
- `resize-scale-verified.log` / `resize-scale-verified/report/`: the four geometry
  cases and DPR2 pointer PASS; two earlier WebGL pixel timing failures are retained.
- `resize-scale-pixels.log` / `resize-scale-pixels/report/`: final four pixel cases
  PASS, 27.8 seconds, including screenshots and every sampled marker geometry.

## Retained failures and limits

- Initial ad hoc probe used headless_shell and failed to load dxil.dll. Full
  Chromium (`channel=chromium`) initialized hardware AMD WebGPU successfully.
- `resize-scale-before`: original WebGPU fails the retained-capacity regression.
- `resize-scale-capacity`: four geometry cases PASS after the first source fix;
  two pixel cases FAIL due to assuming an F0 fixture query selected that fixture.
  They were corrected to use the actual diagnostics page's L marker.
- `resize-scale-grow-before`: corrected pixel probe exposes the remaining real
  capacity-growth defect: 22 solid marker pixels instead of 102. Intrinsic sizing
  repairs this second race.
- `resize-scale-final`: seven PASS; one geometry probe collected just one rAF
  sample. Sampling now also observes actual root ResizeObserver notifications.
- `resize-scale-verified`: seven PASS; the immediate WebGL edge-pixel assertions
  ran before the browser displayed the submitted frame. The final pixel probe
  waits for visible target edges while retaining scale checks for every capture.
  Earlier failures are not reclassified as PASS.

The rebuilt dev server remains on port 5088. Existing browser documents need a
reload to load the new runtime/assets.

Physical OS window-border dragging, scan-out/FPS, strict resize latency, monitor
DPR transitions, other browsers and devices remain **notVerified**. These checks
establish scale/geometry correctness in automated hardware Chromium captures;
they do not establish a latency or physical presentation performance claim.
