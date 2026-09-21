# Web work1/work2 execution — 2026-09-21

**PARTIAL**. Functional product integration is implemented; performance, default Debug startup, two product owners, broader browser/physical coverage and NativeAOT are not approved. Artifacts: `Doroti/artifacts/webview/2026-09-21/web/`. Contract: [Web iframe](../../docs/platform-views/web-webview.md).

## Implementation and evidence boundary

- The common coordinator, controller/widget, typed command/events and planner now drive stable main-DOM iframes through the existing managed Worker mailbox. No second browser/controller registry or compositor is added inside WebView. Existing DOM registry is reused by the product composition service.
- Mixed content uses bounded CPU Skia RGBA canvas upload, with equivalent-content reuse. Both actual Worker WebGPU and Worker WebGL products run this composition. This is not a shared-GPU multi-canvas implementation; zero-native rendering retains its existing GPU path.
- Explicit BrowserDefault profile; native private profiles and global browser-data deletion rejected. Same-origin navigation/HTML/JS/reload/stop, document invalidation, bounded waits/queues and opted-in origin/source/nonce message bridge are implemented. Native app resource schemes, iframe history and full navigation/popup/permission policies remain unsupported.
- A complete frame is validated and applied before the ACK releases plan references. Resize-stale ACK is superseded instead of killing the Worker. Retiring instances cannot be reactivated. Canvas backing size tracks the tested DPR + viewport changes. iframe identity survives movement/effect/resize.

## Executed gates

| Gate | Evidence / result |
|---|---|
| Source/build | Web host and Release product build: zero warnings/errors; TypeScript strict compilation included |
| Common contracts | `../common-platform-final.log`: 13 common contracts (including ACK-driven Attached/Hidden state and late-disposal protection); `../common-webview-final.log`: owner, async command, cancellation, typed factory failure and stale-generation checks passed |
| Product APIs/composition | `release-product/`, `release-product-gl/`: public controller JSON/undefined/errors, HTML/navigation/reload, pending JS invalidation, cross-origin rejection; real two-WebView interleaving, native/shield input and Ctrl-wheel default cancellation, Doroti text field → iframe text input → Doroti focus return (including CDP Korean text insertion, not physical IME), editing identity, ten remove/recreate cycles, DPR 1/1.25/1.5/2 **with viewport resize and checked physical backing dimensions** |
| Message/profile/close | `release-contract/`, `final-contract-gl-2/`: private profile rejection, trusted message, replay/wrong nonce/stale/oversized rejection, command origin rejection, caller cancellation, pending JS close; device loss terminated and removed DOM resources. Final WebGPU run includes asynchronous retirement/control-reply drainage before Worker shutdown |
| DOM lifetime | `dom-lifetime/`: two independent DOM owners, foreign owner rejection, stale/resize/retiring/closed frame rejection and late factory reclamation. This does not prove two full Doroti products in one runtime |
| Pixels | `published-calibration-gpu/`, `final-calibration-gl-3/`: native/raster sigma 4 = 3.999, sigma 16 = 15.996; reset max difference 0; saturation-zero red = [54,54,54]; half-blue tint on black = [0,0,128]; live red/blue updates. Cross-origin iframe sigma 4 also measures 3.999 |
| Publish | `publish-release.log` + `publish-release/wwwroot/`: Release trimmed interpreter publish completed without warnings; actual published pages ran through a separate static HTTP server with COOP/COEP |
| NativeAOT | `nativeaot.log`: actual publish attempt rejected with DOROTIAOT002 (runner's iOS-arm64-only profile). Mono interpreter publish is not NativeAOT |
| Template | `pack-results.json`, `package-release.log`, `release-package/` (final WebView app), `template-zero/` (earlier default app): 21 locally packed NuGet packages (unique verification version), fresh generated app with no source project references, default zero-view and WebView JS/effect/resize passed. Initial missing public feed and inherited repository central-package settings were isolated in the artifact workspace. A real Release PDB asset bug was fixed in shared Runner SDK props/targets. This is not a clean-machine test |
| Physical | Headless Chrome 153.0.8010.48, Windows host, reported AMD RDNA-3 hardware WebGPU. Browser CDP input and screenshots are not physical input, monitor scanout, Korean IME or screen-reader approval |

## Performance observations

`workloads-gpu/` and `workloads-gl/`: 0/1/4 views × idle/animation/scroll/modal; each settled sample lasts 2.5 seconds. These are managed raster duration samples, not end-to-end frame/scanout/input latency. Browser JS heap is only an observation; Chromium process/GPU memory and WebView-internal metrics were not measured. No before-change product implementation existed for an equivalent mixed scene, so these are not before/after budget approvals.

| Renderer | 1-view animation p95 | 4-view animation p95 | 1/4-view active upload | Zero-view resources/upload |
|---|---:|---:|---|---|
| WebGPU | 31.76 ms | 30.90 ms | roughly 331–366 MB / 2.5 s | 0 extra raster/effect nodes, 0 bytes |
| WebGL | 29.15 ms | 30.69 ms | roughly 336–371 MB / 2.5 s | 0 extra raster/effect nodes, 0 bytes |

Static 1/4-view idle has no extra raster work or upload after settling; browser-driven iframe motion itself remains live. Active Doroti animation causes broad raster repaint/upload. **Performance fails a 16.7 ms raster budget**. GPU surface transport and/or finer independent raster slices remain necessary follow-up work; do not label the current bounded upload path as performance-qualified.

## Failures and remaining gates

The final zero-view test (`final-zero/`) also confirms that no composition/iframe modules are fetched until needed.

The default Debug native runtime asserts in `mono_threads_platform_get_stack_bounds` before managed startup. A fresh native intermediate rebuild reproduced it, so the initial stale-cache hypothesis was not established. `gpu-1` also recorded a dev-server stale static-asset manifest during concurrent rebuild; the server was restarted. `gpu-3`, `gpu-4` and `debug-o2` preserve runtime failures. Release startup is independently verified. An attempted command-line optimization setting did not change the observed Debug response-file `-O0`; it is not a validated workaround and no runtime safety guard was bypassed.

Product development exposed and fixed initial shield-only scene lowering, owner ID versus host ID confusion, unpremultiplied canvas transport, resize-stale ACK handling, ACK-driven attachment state, native focus feedback, and draining asynchronous disposal/control replies before Worker-role shutdown. Early validation-only export lookup errors are preserved separately from product failures. A publish attempted while the TypeScript source changed failed with duplicate static-asset fingerprint metadata; the stable-source retry is recorded separately. Running three startup tests concurrently through the small Python development server caused proxy 502/SRI download errors; sequential final runs passed. A GL calibration harness initially sampled bounds before the first visible placement; it now waits for nonzero iframe geometry, and the corrected pixel analysis passes.

Pure CDP DPR change without a viewport change did not advance the raster after the first transition in this environment; the accepted DPR gate explicitly combines viewport resize and checks physical dimensions. Real cross-monitor/zoom-only transitions remain open. Other remaining gates: two complete product owners, physical keyboard/IME/selection/accessibility, protected/video sampling, Firefox/WebKit, broader GPU/OS, full C1–C6/E1–E3, clean-machine deployment and NativeAOT. No user skip was requested.

## Reproduction

All children use the repository's 1200-second timeout. Run from repository root:

```powershell
python Doroti/validation/run-with-timeout.py dotnet publish DorotiTestbedApp/web/DorotiTestbedApp.Web.csproj -c Release -p:DorotiWebPlatformValidation=true -p:RunAOTCompilation=false -o <output>
python Doroti/validation/run-with-timeout.py python Doroti/eng/serve-isolated-web.py <output>/wwwroot --port 5192
python Doroti/validation/run-with-timeout.py node Doroti/validation/webview/verify-web-product.mjs <new-evidence-directory> http://127.0.0.1:5192 worker-direct-webgpu
python Doroti/validation/run-with-timeout.py node Doroti/validation/webview/verify-web-product.mjs <new-evidence-directory> http://127.0.0.1:5192 worker-direct-webgl
python Doroti/validation/run-with-timeout.py node Doroti/validation/webview/verify-web-contract.mjs <new-evidence-directory> http://127.0.0.1:5192 worker-direct-webgpu
python Doroti/validation/run-with-timeout.py node Doroti/validation/webview/verify-web-dom-lifetime.mjs <new-evidence-directory> http://127.0.0.1:5192
python Doroti/validation/run-with-timeout.py node Doroti/validation/webview/verify-web-calibration.mjs <new-evidence-directory> http://127.0.0.1:5192 worker-direct-webgpu
python Doroti/validation/run-with-timeout.py python Doroti/validation/webview/analyze-web-calibration.py <calibration-evidence-directory>
python Doroti/validation/run-with-timeout.py node Doroti/validation/webview/measure-web-workloads.mjs <new-evidence-directory> http://127.0.0.1:5192 worker-direct-webgpu
```

`DorotiWebPlatformValidation` only includes a Testbed export that dispatches onto the live product owner; it is omitted by default. Product pages also expose the WebView sample through Material sample navigation. `BrowserDefault` and iframe limitations apply in that sample as in the API.

## Source and package provenance

`source-provenance.json` records the final dirty worktree hashes, HEAD, published asset hashes and package hashes. The 0/1/4-view measurements precede the final lazy-module/unknown-state/typed-transport refinements; no raster drawing/upload algorithm changed after those measurements. Final product and package tests validate the latest runtime sources, including additive DTO properties that preserve existing constructor/deconstruction ABI and native text-focus handoff. Pixel and message/loss results are reused for unchanged composition and command implementations. The SDK symbol-policy fix is qualified by the newly generated package app.

The final local package gate uses `0.2.0-webverify.20260921.8` for all 21 packages; earlier revisions preserve the initial PDB failure/fix evidence; these were not published to a remote feed. `pack-template-release.py` and its individual pack logs are retained with the artifacts. The generated test workspace contains local NuGet.Config and empty parent-build/central-package overrides to reproduce a standalone template rather than accidentally inherit this repository's build rules.
